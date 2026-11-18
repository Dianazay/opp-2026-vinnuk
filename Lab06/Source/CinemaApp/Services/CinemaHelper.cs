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

    public static IEnumerable<T> Find<T>(IEnumerable<T> items, Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(predicate);

        foreach (T item in items)
        {
            if (predicate(item))
                yield return item;
        }
    }

    public static IEnumerable<T> FindMovies<T>(IEnumerable<T> movies, Func<T, bool> predicate)
        where T : Movie
    {
        ArgumentNullException.ThrowIfNull(movies);
        ArgumentNullException.ThrowIfNull(predicate);

        foreach (T movie in movies)
        {
            if (predicate(movie))
                yield return movie;
        }
    }
}