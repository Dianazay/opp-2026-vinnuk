using CinemaApp.Common;
using CinemaApp.Models;

namespace CinemaApp.Services;

public class CinemaScheduleService
{
    private readonly Dictionary<int, Movie> _movieCatalog = [];
    private readonly List<MovieSession> _schedule = [];

    public int CatalogCount => _movieCatalog.Count;
    public int ScheduleCount => _schedule.Count;

    public OperationResult<Movie> RegisterMovie(int id, Movie movie)
    {
        if (id <= 0)
            return OperationResult<Movie>.Failure("Ідентифікатор фільму повинен бути більшим за 0.");

        if (movie is null)
            return OperationResult<Movie>.Failure("Об'єкт фільму не може бути null.");

        if (!_movieCatalog.TryAdd(id, movie))
            return OperationResult<Movie>.Failure($"Фільм з ID = {id} уже зареєстровано в каталозі.");

        return OperationResult<Movie>.Success(movie, $"Фільм «{movie.Title}» додано до каталогу під ID = {id}.");
    }

    public OperationResult<Movie> GetMovieById(int id)
    {
        if (_movieCatalog.TryGetValue(id, out Movie? movie))
            return OperationResult<Movie>.Success(movie, $"Знайдено фільм за ID = {id}: «{movie.Title}».");

        return OperationResult<Movie>.Failure($"Фільм з ID = {id} у каталозі не знайдено.");
    }

    public IReadOnlyDictionary<int, Movie> GetCatalog() => _movieCatalog.AsReadOnly();

    public void AddSession(MovieSession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        _schedule.Add(session);
    }

    public IReadOnlyList<MovieSession> GetAllSessions() => _schedule.AsReadOnly();

    public IEnumerable<MovieSession> GetSessionsByMovie(Movie movie)
    {
        ArgumentNullException.ThrowIfNull(movie);

        foreach (MovieSession session in _schedule)
        {
            if (ReferenceEquals(session.Movie, movie) ||
                session.Movie.Title.Equals(movie.Title, StringComparison.OrdinalIgnoreCase))
            {
                yield return session;
            }
        }
    }

    public IEnumerable<MovieSession> GetSessionsFromTime(DateTime fromTime)
    {
        foreach (MovieSession session in _schedule)
        {
            if (session.StartTime >= fromTime)
                yield return session;
        }
    }

    public IEnumerable<MovieSession> FilterAndSortSessions(
        string? movieTitle = null,
        DateOnly? date = null,
        decimal? maxPrice = null)
    {
        IEnumerable<MovieSession> query = _schedule;

        if (!string.IsNullOrWhiteSpace(movieTitle))
        {
            query = query.Where(s =>
                s.Movie.Title.Contains(movieTitle, StringComparison.OrdinalIgnoreCase));
        }

        if (date.HasValue)
        {
            query = query.Where(s => DateOnly.FromDateTime(s.StartTime) == date.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(s => s.TicketPrice <= maxPrice.Value);
        }

        return query
            .OrderBy(s => s.StartTime)
            .ThenBy(s => s.TicketPrice);
    }

    public IEnumerable<MovieSession> GetSessionsByMaxPriceQuerySyntax(decimal maxPrice)
    {
        var query =
            from session in _schedule
            where session.TicketPrice <= maxPrice
            orderby session.TicketPrice ascending, session.StartTime ascending
            select session;

        return query;
    }

    public IEnumerable<ScheduleProjection> GetScheduleProjections()
    {
        return _schedule
            .OrderBy(s => s.StartTime)
            .Select(s => new ScheduleProjection(
                s.Movie.Title,
                s.StartTime,
                s.TicketPrice,
                s.Hall.Number));
    }

    public IEnumerable<MovieSessionStats> GetMovieStatistics()
    {
        return _schedule
            .GroupBy(s => s.Movie.Title)
            .Select(group => new MovieSessionStats(
                MovieTitle: group.Key,
                SessionsCount: group.Count(),
                AverageTicketPrice: Math.Round(group.Average(s => s.TicketPrice), 2),
                TotalRevenue: group.Sum(s => s.CalculateTotalRevenue())))
            .OrderByDescending(stat => stat.SessionsCount)
            .ThenByDescending(stat => stat.AverageTicketPrice);
    }

    // Lab 08 (Підзадача 1): добір сеансів через стандартний делегат Predicate<MovieSession>
    public List<MovieSession> SelectSessions(Predicate<MovieSession> match)
    {
        ArgumentNullException.ThrowIfNull(match);

        List<MovieSession> selected = [];
        foreach (MovieSession session in _schedule)
        {
            if (match(session))
                selected.Add(session);
        }

        return selected;
    }

    // Lab 08 (Підзадача 3): обчислення очікуваної виручки через делегат Func<MovieSession, decimal>
    public decimal CalculateExpectedRevenue(
        IEnumerable<MovieSession> sessions,
        Func<MovieSession, decimal> revenueSelector)
    {
        ArgumentNullException.ThrowIfNull(sessions);
        ArgumentNullException.ThrowIfNull(revenueSelector);

        decimal total = 0m;
        foreach (MovieSession session in sessions)
        {
            if (!session.IsCancelled)
            {
                total += revenueSelector(session);
            }
        }

        return total;
    }
}