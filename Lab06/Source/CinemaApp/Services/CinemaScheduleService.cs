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
}