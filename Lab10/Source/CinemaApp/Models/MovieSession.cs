using CinemaApp.Common;

namespace CinemaApp.Models;

public class MovieSession
{
    private static readonly DateTime MinAllowedSessionDate = new(2020, 1, 1);

    private Movie _movie;
    private Hall _hall;
    private DateTime _startTime;
    private decimal _ticketPrice;

    public event EventHandler<SessionCancelledEventArgs>? SessionCancelled;

    public Movie Movie
    {
        get => _movie;
        set => _movie = value ?? throw new ArgumentNullException(nameof(Movie), "Фільм для сеансу не може бути null.");
    }

    public Hall Hall
    {
        get => _hall;
        set => _hall = value ?? throw new ArgumentNullException(nameof(Hall), "Кінозал для сеансу не може бути null.");
    }

    public DateTime StartTime
    {
        get => _startTime;
        set
        {
            if (value == default || value < MinAllowedSessionDate)
                throw new ArgumentOutOfRangeException(nameof(StartTime), value, $"Час початку сеансу не може бути порожнім або ранішим за {MinAllowedSessionDate:dd.MM.yyyy}.");

            _startTime = value;
        }
    }

    public DateTime EndTime => StartTime.AddMinutes(Movie.Duration);

    public decimal TicketPrice
    {
        get => _ticketPrice;
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(TicketPrice), value, "Ціна квитка повинна бути більшою за 0 грн.");

            _ticketPrice = value;
        }
    }

    public bool IsCancelled { get; private set; }

    public int AvailableTickets => IsCancelled ? 0 : Hall.AvailableSeats;

    public MovieSession(Movie movie, Hall hall, DateTime startTime, decimal ticketPrice)
    {
        _movie = movie ?? throw new ArgumentNullException(nameof(movie), "Фільм для сеансу не може бути null.");
        _hall = hall ?? throw new ArgumentNullException(nameof(hall), "Кінозал для сеансу не може бути null.");

        if (startTime == default || startTime < MinAllowedSessionDate)
            throw new ArgumentOutOfRangeException(nameof(startTime), startTime, $"Час початку сеансу не може бути порожнім або ранішим за {MinAllowedSessionDate:dd.MM.yyyy}.");

        if (ticketPrice <= 0)
            throw new ArgumentOutOfRangeException(nameof(ticketPrice), ticketPrice, "Ціна квитка повинна бути більшою за 0 грн.");

        _startTime = startTime;
        _ticketPrice = ticketPrice;
    }

    public static OperationResult<MovieSession> TryCreate(Movie movie, Hall hall, DateTime startTime, decimal ticketPrice)
    {
        try
        {
            MovieSession session = new(movie, hall, startTime, ticketPrice);
            return OperationResult<MovieSession>.Success(session, $"Сеанс на фільм «{movie.Title}» успішно створено.");
        }
        catch (Exception ex)
        {
            return OperationResult<MovieSession>.Failure($"Не вдалося створити сеанс: {ex.Message}");
        }
    }

    public void SellTickets(int count)
    {
        if (IsCancelled)
            throw new InvalidOperationException($"Неможливо продати квитки: сеанс на фільм «{Movie.Title}» скасовано.");

        Hall.BookSeat(count);
    }

    public OperationResult<int> TrySellTickets(int count)
    {
        try
        {
            SellTickets(count);
            return OperationResult<int>.Success(count, $"Успішно продано квитків: {count} шт. (вільно місць у залі №{Hall.Number}: {AvailableTickets}).");
        }
        catch (Exception ex)
        {
            return OperationResult<int>.Failure($"Помилка продажу квитків: {ex.Message}");
        }
    }

    public void Cancel()
    {
        if (IsCancelled)
            throw new InvalidOperationException($"Сеанс на фільм «{Movie.Title}» ({StartTime:dd.MM.yyyy HH:mm}) уже скасовано.");

        int soldTickets = Hall.OccupiedSeats;
        IsCancelled = true;

        SessionCancelled?.Invoke(
            this,
            new SessionCancelledEventArgs(Movie, StartTime, Hall.Number, soldTickets));
    }

    public decimal CalculateTotalRevenue()
    {
        return IsCancelled ? 0m : Hall.OccupiedSeats * TicketPrice;
    }

    public string GetSessionInfo()
    {
        string statusText = IsCancelled ? " [СКАСОВАНО]" : string.Empty;
        return $"Сеанс: {StartTime:dd.MM.yyyy HH:mm}–{EndTime:HH:mm}{statusText} | Зал №{Hall.Number} | Ціна квитка: {TicketPrice:F2} грн.\n" +
               $"   Фільм: {Movie.GetDescription()}\n" +
               $"   Продано квитків: {Hall.OccupiedSeats}/{Hall.Seats} (доступно: {AvailableTickets}) | Виручка: {CalculateTotalRevenue():F2} грн.";
    }
}