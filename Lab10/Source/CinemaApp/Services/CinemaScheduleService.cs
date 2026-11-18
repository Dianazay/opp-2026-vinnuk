using CinemaApp.Common;
using CinemaApp.Exceptions;
using CinemaApp.Models;

namespace CinemaApp.Services;

public class CinemaScheduleService
{
    private readonly Dictionary<int, Movie> _movieCatalog = [];
    private readonly List<MovieSession> _schedule = [];

    public int CatalogCount => _movieCatalog.Count;
    public int ScheduleCount => _schedule.Count;
    public bool IsSaleInProgress { get; private set; }

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

    // Lab 10 (Підзадача 2): перевірка конфлікту розкладу залу перед додаванням сеансу
    public void AddSession(MovieSession session)
    {
        ArgumentNullException.ThrowIfNull(session);

        if (!session.IsCancelled)
        {
            MovieSession? conflict = _schedule.FirstOrDefault(existing =>
                !existing.IsCancelled &&
                existing.Hall.Number == session.Hall.Number &&
                session.StartTime < existing.EndTime &&
                existing.StartTime < session.EndTime);

            if (conflict is not null)
            {
                throw new HallScheduleConflictException(
                    hallNumber: session.Hall.Number,
                    existingMovieTitle: conflict.Movie.Title,
                    existingStart: conflict.StartTime,
                    existingEnd: conflict.EndTime,
                    requestedStart: session.StartTime);
            }
        }

        _schedule.Add(session);
    }

    // Lab 10 (Підзадача 3): межа оброблення помилок під час продажу квитків
    public OperationResult<int> ProcessTicketSale(MovieSession session, int ticketCount)
    {
        ArgumentNullException.ThrowIfNull(session);

        IsSaleInProgress = true;
        try
        {
            session.SellTickets(ticketCount);
            return OperationResult<int>.Success(
                ticketCount,
                $"Продаж підтверджено: {ticketCount} квит. на «{session.Movie.Title}» (залишилось місць: {session.AvailableTickets}).");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return OperationResult<int>.Failure($"[Некоректні вхідні дані]: {ex.Message}");
        }
        catch (InsufficientSeatsException ex)
        {
            return OperationResult<int>.Failure($"[Відсутні вільні місця]: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            return OperationResult<int>.Failure($"[Недопустимий стан сеансу]: {ex.Message}");
        }
        finally
        {
            IsSaleInProgress = false;
        }
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