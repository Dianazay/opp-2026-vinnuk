using CinemaApp.Models;

namespace CinemaApp.Services;

public sealed class TicketAvailabilityTracker
{
    private readonly Dictionary<MovieSession, int> _availableTickets = [];

    public void TrackSession(MovieSession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        _availableTickets[session] = session.AvailableTickets;
        session.SessionCancelled += HandleSessionCancelled;
    }

    public void Unsubscribe(MovieSession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        session.SessionCancelled -= HandleSessionCancelled;
    }

    public void RefreshAvailability(MovieSession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        _availableTickets[session] = session.AvailableTickets;
    }

    public int GetTrackedAvailableTickets(MovieSession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        return _availableTickets.TryGetValue(session, out int count) ? count : 0;
    }

    private void HandleSessionCancelled(object? sender, SessionCancelledEventArgs e)
    {
        if (sender is MovieSession session)
        {
            _availableTickets[session] = 0;
        }

        Console.WriteLine(
            $"[Сповіщення підписника]: Сеанс «{e.Movie.Title}» ({e.StartTime:dd.MM.yyyy HH:mm}, Зал №{e.HallNumber}) скасовано. " +
            $"Доступність квитків оновлено до 0 (повернено квитків: {e.RefundedTicketsCount} шт.).");
    }
}