namespace CinemaApp.Models;

public class MovieSession
{
    private Movie _movie;
    private Hall _hall;
    private DateTime _startTime;
    private decimal _ticketPrice;

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
            if (value == default)
                throw new ArgumentException("Необхідно вказати коректний час початку сеансу.", nameof(StartTime));
            _startTime = value;
        }
    }

    public decimal TicketPrice
    {
        get => _ticketPrice;
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(TicketPrice), "Ціна квитка повинна бути більшою за 0 грн.");
            _ticketPrice = value;
        }
    }

    public MovieSession(Movie movie, Hall hall, DateTime startTime, decimal ticketPrice)
    {
        _movie = movie ?? throw new ArgumentNullException(nameof(movie), "Фільм для сеансу не може бути null.");
        _hall = hall ?? throw new ArgumentNullException(nameof(hall), "Кінозал для сеансу не може бути null.");
        StartTime = startTime;
        TicketPrice = ticketPrice;
    }

    public void SellTickets(int count)
    {
        Hall.BookSeat(count);
    }

    public decimal CalculateTotalRevenue()
    {
        return Hall.OccupiedSeats * TicketPrice;
    }

    public string GetSessionInfo()
    {
        return $"Сеанс: {StartTime:dd.MM.yyyy HH:mm} | Зал №{Hall.Number} | Ціна квитка: {TicketPrice:F2} грн.\n" +
               $"   Фільм: {Movie.GetDescription()}\n" +
               $"   Продано квитків: {Hall.OccupiedSeats}/{Hall.Seats} (вільно: {Hall.AvailableSeats}) | Виручка: {CalculateTotalRevenue():F2} грн.";
    }
}