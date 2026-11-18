namespace CinemaApp.Models;

public readonly record struct SessionDuration
{
    public int Minutes { get; }

    public SessionDuration(int minutes)
    {
        if (minutes <= 0)
            throw new ArgumentOutOfRangeException(nameof(minutes), "Тривалість сеансу повинна бути більшою за 0 хвилин.");

        Minutes = minutes;
    }

    public int Hours => Minutes / 60;
    public int RemainingMinutes => Minutes % 60;

    public override string ToString() =>
        Hours > 0 ? $"{Hours} год {RemainingMinutes} хв ({Minutes} хв)" : $"{Minutes} хв";
}

public sealed record MovieSessionSummary(
    int SessionId,
    string OriginalTitle,
    string? LocalizedTitle,
    string? AgeRating,
    SessionDuration Duration,
    int HallNumber,
    DateTime StartTime,
    decimal TicketPrice,
    bool IsCancelled)
{
    public static MovieSessionSummary FromSession(
        int sessionId,
        MovieSession session,
        string? localizedTitle = null,
        string? ageRatingOverride = null)
    {
        ArgumentNullException.ThrowIfNull(session);

        string? resolvedAgeRating = ageRatingOverride ?? session.Movie switch
        {
            FeatureFilm featureFilm => featureFilm.AgeRating,
            _ => null
        };

        return new MovieSessionSummary(
            SessionId: sessionId,
            OriginalTitle: session.Movie.Title,
            LocalizedTitle: localizedTitle,
            AgeRating: resolvedAgeRating,
            Duration: new SessionDuration(session.Movie.Duration),
            HallNumber: session.Hall.Number,
            StartTime: session.StartTime,
            TicketPrice: session.TicketPrice,
            IsCancelled: session.IsCancelled);
    }
}