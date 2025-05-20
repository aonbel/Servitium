using Application.Features.Appointments.Handlers;
using Application.Features.Appointments.Queries;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.Core;
using Domain.Entities.People;
using Domain.Entities.Services;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Application.Tests.FeaturesTests.Appointments
{
    public class GetAllPossibleAppointmentsQueryHandlerTests
    {
        [Fact]
        public async Task Test_Handle_ReturnsAllSlots_WhenNoExistingAppointments()
        {
            // Arrange
            var serviceId = 1;
            var serviceDuration = TimeSpan.FromHours(1);
            var fromDate = new DateTime(2024, 6, 1, 8, 0, 0);
            var timeSpan = TimeSpan.FromDays(1);
            var distance = 10.0;
            var fromCoordinates = new Coordinates { Latitude = 0, Longitude = 0 };

            var service = new Service
            {
                Id = serviceId,
                Name = "Test Service",
                ShortName = "TS",
                Description = "Test Service Description",
                RequiredHealthCertificateTemplateIds = new List<int>(),
                ResultHealthCertificateTemplateIds = new List<int>(),
                PricePerHourForMaterials = 0,
                PricePerHourForEquipment = 0,
                Duration = serviceDuration
            };
            var serviceProvider = new ServiceProvider
            {
                Id = 10,
                Name = "Test Provider",
                ShortName = "TP",
                Address = "Test Address",
                Coordinates = new Coordinates { Latitude = 0, Longitude = 0 },
                WorkTime = new TimeOnlySegment(new TimeOnly(8, 0), new TimeOnly(12, 0)),
                WorkDays = new List<DayOfWeek> { fromDate.DayOfWeek },
                Contacts = new List<string>()
            };
            var specialist = new Specialist
            {
                Id = 20,
                PersonId = 100,
                PricePerHour = 100m,
                ServiceProviderId = serviceProvider.Id ?? 0,
                ServiceIds = new List<int> { serviceId },
                WorkTime = new TimeOnlySegment(new TimeOnly(8, 0), new TimeOnly(12, 0)),
                WorkDays = new List<DayOfWeek> { fromDate.DayOfWeek },
                Contacts = new List<string>(),
                Location = "TestLocation"
            };

            var mockDbContext = new Mock<IApplicationDbContext>();
            mockDbContext.Setup(db => db.Services.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(service);
            mockDbContext.Setup(db => db.ServiceProviders)
                .Returns(MockDbSet(new List<ServiceProvider> { serviceProvider }));
            mockDbContext.Setup(db => db.Specialists)
                .Returns(MockDbSet(new List<Specialist> { specialist }));
            mockDbContext.Setup(db => db.Appointments)
                .Returns(MockDbSet(new List<Appointment>()));

            var handler = new GetAllPossibleAppointmentsQueryHandler(mockDbContext.Object);

            var request = new GetAllPossibleAppointmentTimesQuery(
                serviceId,
                fromCoordinates,
                distance,
                fromDate,
                timeSpan
            );

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            var response = result.Value;
            Assert.NotNull(response);
            Assert.NotEmpty(response.PossibleTimes);
            foreach (var slot in response.PossibleTimes)
            {
                Assert.Equal(serviceProvider.Id, slot.ServiceProviderId);
                Assert.Equal(specialist.Id, slot.SpecialistId);
                Assert.True(slot.Time >= specialist.WorkTime.Begin && slot.Time < specialist.WorkTime.End);
            }
        }

        [Fact]
        public async Task Test_Handle_ReturnsNotFound_WhenServiceDoesNotExist()
        {
            // Arrange
            var serviceId = 99;
            var fromDate = DateTime.Now;
            var timeSpan = TimeSpan.FromDays(1);
            var distance = 10.0;
            var fromCoordinates = new Coordinates { Latitude = 0, Longitude = 0 };

            var mockDbContext = new Mock<IApplicationDbContext>();
            mockDbContext.Setup(db => db.Services.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Service?)null);

            var handler = new GetAllPossibleAppointmentsQueryHandler(mockDbContext.Object);

            var request = new GetAllPossibleAppointmentTimesQuery(
                serviceId,
                fromCoordinates,
                distance,
                fromDate,
                timeSpan
            );

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ServiceErrors.NotFoundById(serviceId), result.Error);
        }

        [Fact]
        public async Task Test_Handle_ExcludesSlots_WithExistingAppointments()
        {
            // Arrange
            var serviceId = 1;
            var serviceDuration = TimeSpan.FromHours(1);
            var fromDate = new DateTime(2024, 6, 1, 8, 0, 0);
            var timeSpan = TimeSpan.FromDays(1);
            var distance = 10.0;
            var fromCoordinates = new Coordinates { Latitude = 0, Longitude = 0 };

            var service = new Service
            {
                Id = serviceId,
                Name = "Test Service",
                ShortName = "TS",
                Description = "Test Service Description",
                RequiredHealthCertificateTemplateIds = new List<int>(),
                ResultHealthCertificateTemplateIds = new List<int>(),
                PricePerHourForMaterials = 0,
                PricePerHourForEquipment = 0,
                Duration = serviceDuration
            };
            var serviceProvider = new ServiceProvider
            {
                Id = 10,
                Name = "Test Provider",
                ShortName = "TP",
                Address = "Test Address",
                Coordinates = new Coordinates { Latitude = 0, Longitude = 0 },
                WorkTime = new TimeOnlySegment(new TimeOnly(8, 0), new TimeOnly(12, 0)),
                WorkDays = new List<DayOfWeek> { fromDate.DayOfWeek },
                Contacts = new List<string>()
            };
            var specialist = new Specialist
            {
                Id = 20,
                PersonId = 100,
                PricePerHour = 100m,
                ServiceProviderId = serviceProvider.Id ?? 0,
                ServiceIds = new List<int> { serviceId },
                WorkTime = new TimeOnlySegment(new TimeOnly(8, 0), new TimeOnly(12, 0)),
                WorkDays = new List<DayOfWeek> { fromDate.DayOfWeek },
                Contacts = new List<string>(),
                Location = "TestLocation"
            };

            // Existing appointment from 9:00 to 10:00
            var appointment = new Appointment
            {
                ClientId = 0,
                ServiceId = 0,
                SpecialistId = specialist.Id ?? 0,
                Date = new DateOnly(2024, 6, 1),
                TimeSegment = new TimeOnlySegment(new TimeOnly(9, 0), new TimeOnly(10, 0))
            };

            var mockDbContext = new Mock<IApplicationDbContext>();
            mockDbContext.Setup(db => db.Services.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(service);
            mockDbContext.Setup(db => db.ServiceProviders)
                .Returns(MockDbSet(new List<ServiceProvider> { serviceProvider }));
            mockDbContext.Setup(db => db.Specialists)
                .Returns(MockDbSet(new List<Specialist> { specialist }));
            mockDbContext.Setup(db => db.Appointments)
                .Returns(MockDbSet(new List<Appointment> { appointment }));

            var handler = new GetAllPossibleAppointmentsQueryHandler(mockDbContext.Object);

            var request = new GetAllPossibleAppointmentTimesQuery(
                serviceId,
                fromCoordinates,
                distance,
                fromDate,
                timeSpan
            );

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            var response = result.Value;
            Assert.NotNull(response);

            // Should exclude 9:00 slot, only 8:00, 10:00, 11:00 should be available
            var expectedSlots = new List<TimeOnly>
            {
                new TimeOnly(8,0),
                new TimeOnly(10,0),
                new TimeOnly(11,0)
            };
            var actualSlots = response.PossibleTimes
                .Where(x => x.SpecialistId == specialist.Id && x.ServiceProviderId == serviceProvider.Id)
                .Select(x => x.Time)
                .ToList();

            Assert.Equal(expectedSlots, actualSlots);
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