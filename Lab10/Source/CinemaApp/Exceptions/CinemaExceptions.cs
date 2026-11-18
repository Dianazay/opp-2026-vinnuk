namespace CinemaApp.Exceptions;

public sealed class HallScheduleConflictException : Exception
{
    public int HallNumber { get; }
    public string ExistingMovieTitle { get; }
    public DateTime ExistingStart { get; }
    public DateTime ExistingEnd { get; }
    public DateTime RequestedStart { get; }

    public HallScheduleConflictException(
        int hallNumber,
        string existingMovieTitle,
        DateTime existingStart,
        DateTime existingEnd,
        DateTime requestedStart)
        : base($"Конфлікт розкладу в залі №{hallNumber}: сеанс о {requestedStart:dd.MM.yyyy HH:mm} перетинається з фільмом «{existingMovieTitle}» ({existingStart:HH:mm}–{existingEnd:HH:mm}).")
    {
        HallNumber = hallNumber;
        ExistingMovieTitle = existingMovieTitle;
        ExistingStart = existingStart;
        ExistingEnd = existingEnd;
        RequestedStart = requestedStart;
    }
}

public sealed class InsufficientSeatsException : Exception
{
    public int HallNumber { get; }
    public int RequestedSeats { get; }
    public int AvailableSeats { get; }

    public InsufficientSeatsException(int hallNumber, int requestedSeats, int availableSeats)
        : base($"Недостатньо вільних місць у залі №{hallNumber}: запитано {requestedSeats} шт., доступно лише {availableSeats} шт.")
    {
        HallNumber = hallNumber;
        RequestedSeats = requestedSeats;
        AvailableSeats = availableSeats;
    }
}