using System.Linq.Expressions;

namespace DukkanDepo.Infrastructure.Persistence.Querying;

public static class IQueryableExtensions
{
    public static IQueryable<T> OrderByDynamic<T>(
        this IQueryable<T> query,
        string? propertyPath,
        bool descending)
    {
        if (string.IsNullOrWhiteSpace(propertyPath))
            propertyPath = "Id";

        var parameter = Expression.Parameter(typeof(T), "x");

        Expression body = parameter;

        foreach (var member in propertyPath.Split('.'))
        {
            var property = body.Type.GetProperty(member);

            if (property is null)
            {
                property = body.Type.GetProperty("Id")
                    ?? throw new InvalidOperationException($"'{propertyPath}' için üye bulunamadı.");

                body = Expression.Property(body, property);
                break;
            }

            body = Expression.Property(body, property);
        }

        var lambda = Expression.Lambda(body, parameter);

        var methodName = descending
            ? nameof(Queryable.OrderByDescending)
            : nameof(Queryable.OrderBy);

        var method = typeof(Queryable)
            .GetMethods()
            .First(methodInfo =>
                methodInfo.Name == methodName &&
                methodInfo.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), body.Type);

        return (IQueryable<T>)method.Invoke(null, [query, lambda])!;
    }
}