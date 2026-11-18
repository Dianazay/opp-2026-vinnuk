using CinemaApp.Models;

namespace CinemaApp.Services;

public static class CinemaDataProcessor
{
    // Безпечне опрацювання nullable-властивості LocalizedTitle
    public static string GetDisplayTitle(MovieSessionSummary summary)
    {
        ArgumentNullException.ThrowIfNull(summary);

        return summary.LocalizedTitle is { Length: > 0 } localTitle
            ? $"{localTitle} ({summary.OriginalTitle})"
            : summary.OriginalTitle;
    }

    // Безпечне опрацювання nullable-властивості AgeRating
    public static string GetAgeRatingLabel(MovieSessionSummary summary)
    {
        ArgumentNullException.ThrowIfNull(summary);
        return summary.AgeRating is { Length: > 0 } rating ? rating : "Не вказано";
    }

    // Pattern matching (switch-вираз) для класифікації вікової категорії
    public static string ClassifyAgeCategory(MovieSessionSummary summary)
    {
        ArgumentNullException.ThrowIfNull(summary);

        return summary.AgeRating?.Trim() switch
        {
            null or "" => "Без вікових обмежень (рейтинг не задано)",
            "0+" or "6+" => "Дитяча та сімейна аудиторія",
            "12+" => "Підліткова аудиторія (від 12 років)",
            "16+" => "Юнацька аудиторія (від 16 років)",
            "18+" => "Лише для дорослих (18+)",
            { Length: > 0 } other => $"Спеціальна категорія ({other})"
        };
    }

    // Використання діапазону (..) та індексу (^) для рядка з перевіркою коротких назв
    public static string GetShortTitleWithEndMarker(string? title, int prefixLength = 10)
    {
        if (string.IsNullOrWhiteSpace(title))
            return "[Назва відсутня]";

        if (title.Length <= prefixLength)
            return $"{title} (повна назва, останній символ: '{title[^1]}')";

        string prefix = title[..prefixLength];
        char lastChar = title[^1];
        return $"{prefix}... (останній символ: '{lastChar}')";
    }

    // Використання діапазону [1..^1] для масиву зведень із перевіркою граничних випадків
    public static MovieSessionSummary[] GetMiddleSessionsSlice(MovieSessionSummary[] sessions)
    {
        ArgumentNullException.ThrowIfNull(sessions);

        if (sessions.Length <= 2)
            return [];

        return sessions[1..^1];
    }

    // Використання індексу з кінця [^1] із перевіркою порожнього масиву
    public static MovieSessionSummary? GetLastSession(MovieSessionSummary[] sessions)
    {
        ArgumentNullException.ThrowIfNull(sessions);
        return sessions.Length > 0 ? sessions[^1] : null;
    }
}