namespace TFGames.Common.Extensions
{
    public static class PagingUtil
    {
        public static IQueryable<T> Page<T>(this IQueryable<T> query, int page, int size)
        {
            return query.Skip((page - 1) * size).Take(size);
        }

        public static IEnumerable<T> Page<T>(this IEnumerable<T> enumerable, int page, int size)
        {
            return enumerable.Skip((page - 1) * size).Take(size);
        }
    }
}