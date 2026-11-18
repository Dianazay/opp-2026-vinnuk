using CinemaApp.Models;

namespace CinemaApp.Services;

public static class CinemaHelper
{
    public static T ChooseBy<T, TKey>(T first, T second, Func<T, TKey> selector)
        where TKey : IComparable<TKey>
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);
        ArgumentNullException.ThrowIfNull(selector);

        return selector(first).CompareTo(selector(second)) >= 0
            ? first
            : second;
    }

    public static string DescribeMovie<T>(T movie)
        where T : Movie
    {
        ArgumentNullException.ThrowIfNull(movie);
        return $"Фільм: «{movie.Title}» | Жанр: {movie.Genre} | Тривалість: {movie.Duration} хв. | Тип: {movie.GetType().Name}";
    }
}