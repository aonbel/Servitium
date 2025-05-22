using Application.Features.Appointments.Handlers;
using Application.Features.Appointments.Queries;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.Core;
using Domain.Entities.People;
using Domain.Entities.Services;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Application.Tests.Features.Appointments
{
    public class GetAllPossibleAppointmentTimesQueryHandlerTests
    {
        private readonly int _serviceId = 1;
        private readonly int _nonExistingServiceId = 2;
        private readonly TimeSpan _serviceDuration = TimeSpan.FromHours(1);
        private readonly DateTime _fromDate = new (2024, 6, 1, 8, 0, 0);
        private readonly TimeSpan _timeSpan = TimeSpan.FromDays(1);
        private readonly double _distance = 10.0;
        private readonly CancellationToken _cancellationToken = CancellationToken.None;
        
        private readonly Mock<IApplicationDbContext> _mockDbContext = new();
        
        private readonly Service _service;
        private readonly ServiceProvider _serviceProvider;
        private readonly Specialist _specialist;
        private readonly Appointment _appointment;

        public GetAllPossibleAppointmentTimesQueryHandlerTests()
        {
            _service = new Service
            {
                Id = _serviceId,
                Name = "Test Service",
                ShortName = "TS",
                Description = "Test Service Description",
                RequiredHealthCertificateTemplateIds = new List<int>(),
                ResultHealthCertificateTemplateIds = new List<int>(),
                PricePerHourForMaterials = 0,
                PricePerHourForEquipment = 0,
                Duration = _serviceDuration
            };

            _serviceProvider = new ServiceProvider
            {
                Id = 10,
                Name = "Test Provider",
                ShortName = "TP",
                Address = "Test Address",
                WorkTime = new TimeOnlySegment(new TimeOnly(8, 0), new TimeOnly(12, 0)),
                WorkDays = new List<DayOfWeek> { _fromDate.DayOfWeek },
                Contacts = new List<string>()
            };

            _specialist = new Specialist
            {
                Id = 20,
                PersonId = 100,
                PricePerHour = 100m,
                ServiceProviderId = _serviceProvider.Id ?? 0,
                ServiceIds = new List<int> { _serviceId },
                WorkTime = new TimeOnlySegment(new TimeOnly(8, 0), new TimeOnly(12, 0)),
                WorkDays = new List<DayOfWeek> { _fromDate.DayOfWeek },
                Contacts = new List<string>(),
                Location = "TestLocation"
            };
            
            _appointment = new Appointment
            {
                ClientId = 0,
                ServiceId = 0,
                SpecialistId = _specialist.Id ?? 0,
                Date = new DateOnly(2024, 6, 1),
                TimeSegment = new TimeOnlySegment(new TimeOnly(9, 0), new TimeOnly(10, 0))
            };
        }
        
        [Fact]
        public async Task Test_Handle_ReturnsAllSlots_WhenNoExistingAppointments()
        {
            // Arrange
            
            _mockDbContext.Setup(db => db.Services.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(_service);

            _mockDbContext.Setup(db => db.ServiceProviders)
                .Returns(MockDbSet(new List<ServiceProvider> { _serviceProvider }));

            _mockDbContext.Setup(db => db.Specialists)
                .Returns(MockDbSet(new List<Specialist> { _specialist }));

            _mockDbContext.Setup(db => db.Appointments)
                .Returns(MockDbSet(new List<Appointment>()));

            var handler = new GetAllPossibleAppointmentTimesQueryHandler(_mockDbContext.Object);

            var request = new GetAllPossibleAppointmentTimesQuery(
                _serviceId,
                _fromCoordinates,
                _distance,
                _fromDate,
                _timeSpan
            );
            
            // Act

            var result = await handler.Handle(request, _cancellationToken);
            
            // Assert
            
            Assert.True(result.IsSuccess);
            var response = result.Value;
            Assert.NotNull(response);
            Assert.NotEmpty(response.PossibleTimes);
            foreach (var slot in response.PossibleTimes)
            {
                Assert.Equal(_serviceProvider.Id, slot.ServiceProviderId);
                Assert.Equal(_specialist.Id, slot.SpecialistId);
                Assert.True(slot.Time >= _specialist.WorkTime.Begin && slot.Time < _specialist.WorkTime.End);
            }
        }

        [Fact]
        public async Task Test_Handle_ReturnsNotFound_WhenServiceDoesNotExist()
        {
            // Arrange
            
            _mockDbContext.Setup(db => db.Services.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Service?)null);

            var handler = new GetAllPossibleAppointmentTimesQueryHandler(_mockDbContext.Object);

            var request = new GetAllPossibleAppointmentTimesQuery(
                _nonExistingServiceId,
                _fromCoordinates,
                _distance,
                _fromDate,
                _timeSpan
            );
            
            // Act
            
            var result = await handler.Handle(request, CancellationToken.None);
            
            // Assert
            
            Assert.False(result.IsSuccess);
            Assert.Equal(ServiceErrors.NotFoundById(_nonExistingServiceId), result.Error);
        }

        [Fact]
        public async Task Test_Handle_ExcludesSlots_WithExistingAppointments()
        {
            // Arrange

            _mockDbContext.Setup(db => db.Services.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_service);
            _mockDbContext.Setup(db => db.ServiceProviders)
                .Returns(MockDbSet(new List<ServiceProvider> { _serviceProvider }));
            _mockDbContext.Setup(db => db.Specialists)
                .Returns(MockDbSet(new List<Specialist> { _specialist }));
            _mockDbContext.Setup(db => db.Appointments)
                .Returns(MockDbSet(new List<Appointment> { _appointment }));

            var handler = new GetAllPossibleAppointmentTimesQueryHandler(_mockDbContext.Object);

            var request = new GetAllPossibleAppointmentTimesQuery(
                _serviceId,
                _fromCoordinates,
                _distance,
                _fromDate,
                _timeSpan
            );
            
            // Should exclude 9:00 slot, only 8:00, 10:00, 11:00 should be available
            
            var expectedSlots = new List<(int ServiceProviderId, int SpecialistId, DateOnly Date, TimeOnly Time)>
            {
                (_serviceProvider.Id ?? 0, _specialist.Id ?? 0, DateOnly.FromDateTime(_fromDate), new TimeOnly(8, 0)),
                (_serviceProvider.Id ?? 0, _specialist.Id ?? 0, DateOnly.FromDateTime(_fromDate), new TimeOnly(10, 0)),
                (_serviceProvider.Id ?? 0, _specialist.Id ?? 0, DateOnly.FromDateTime(_fromDate), new TimeOnly(11, 0)),
            };

            // Act
            
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            
            Assert.True(result.IsSuccess);
            var response = result.Value;
            Assert.NotNull(response);
            
            Assert.Equal(expectedSlots, expectedSlots);
        }

        private static DbSet<T> MockDbSet<T>(IEnumerable<T> elements) where T : class
        {
            var queryable = elements.AsQueryable();
            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            return dbSet.Object;
        }
    }
}