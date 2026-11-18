namespace CinemaApp.Models;

public record ScheduleProjection(
    string MovieTitle,
    DateTime StartTime,
    decimal TicketPrice,
    int HallNumber);

public record MovieSessionStats(
    string MovieTitle,
    int SessionsCount,
    decimal AverageTicketPrice,
    decimal TotalRevenue);