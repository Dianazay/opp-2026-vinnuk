namespace CinemaApp.Models;

public sealed class SessionCancelledEventArgs : EventArgs
{
    public Movie Movie { get; }
    public DateTime StartTime { get; }
    public int HallNumber { get; }
    public int RefundedTicketsCount { get; }

    public SessionCancelledEventArgs(
        Movie movie,
        DateTime startTime,
        int hallNumber,
        int refundedTicketsCount)
    {
        Movie = movie ?? throw new ArgumentNullException(nameof(movie));
        StartTime = startTime;
        HallNumber = hallNumber;
        RefundedTicketsCount = refundedTicketsCount;
    }
}