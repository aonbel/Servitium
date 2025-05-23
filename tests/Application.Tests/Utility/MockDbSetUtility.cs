using Microsoft.EntityFrameworkCore;
using Moq;

namespace Application.Tests.Utility;

internal static class MockDbSetUtility
{
    public static DbSet<T> MockDbSet<T>(IEnumerable<T> elements) where T : class
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