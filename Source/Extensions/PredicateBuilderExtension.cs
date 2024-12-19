public static class PredicateBuilder
{
    public static Expression<Func<T, bool>> AndAlso<T>(
        this Expression<Func<T, bool>> expression1,
        Expression<Func<T, bool>> expression2)
    {
        var parameters = expression1.Parameters[0];
        var body = Expression.AndAlso(expression1.Body, Expression.Invoke(expression2, parameters));

        return Expression.Lambda<Func<T, bool>>(body, parameters);
    }
}
