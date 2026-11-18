using CinemaApp.Models;
using CinemaApp.Services;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// Підготовка наявних сутностей предметної області
FeatureFilm dune = new("Dune: Part Two", "Наукова фантастика", 166, "Дені Вільнев", "12+");
FeatureFilm oppenheimer = new("Oppenheimer", "Біографічна драма", 180, "Крістофер Нолан", "16+");
AnimatedMovie insideOut = new("Inside Out 2", "Сімейна комедія", 96, "Pixar", "3D-анімація");
Documentary planet = new("Our Planet", "Природа та екологія", 95, "Дика природа", 2023);

MovieSession s1 = new(dune, new Hall(1, 100), new DateTime(2026, 11, 25, 14, 0, 0), 190m);
MovieSession s2 = new(insideOut, new Hall(2, 50), new DateTime(2026, 11, 25, 16, 30, 0), 150m);
MovieSession s3 = new(oppenheimer, new Hall(3, 40), new DateTime(2026, 11, 25, 19, 30, 0), 220m);
MovieSession s4 = new(planet, new Hall(2, 50), new DateTime(2026, 11, 25, 21, 15, 0), 140m);

// 1. Перетворення у record MovieSessionSummary та record struct SessionDuration
Console.WriteLine("=== 1. Моделі record (MovieSessionSummary) та record struct (SessionDuration) ===");

MovieSessionSummary sum1 = MovieSessionSummary.FromSession(1, s1, localizedTitle: "Дюна: Частина друга");
MovieSessionSummary sum2 = MovieSessionSummary.FromSession(2, s2, localizedTitle: "Думками навиворіт 2", ageRatingOverride: "0+");
MovieSessionSummary sum3 = MovieSessionSummary.FromSession(3, s3, localizedTitle: null); // локалізована назва відсутня
MovieSessionSummary sum4 = MovieSessionSummary.FromSession(4, s4); // відсутні і локалізована назва, і віковий рейтинг

// Перевірка семантики значень record та копіювання через вираз with
MovieSessionSummary sum1Clone = sum1 with { };
MovieSessionSummary sum1EveningPromo = sum1 with
{
    StartTime = new DateTime(2026, 11, 25, 22, 0, 0),
    TicketPrice = 160m
};

Console.WriteLine($"Оригінал: {sum1.LocalizedTitle} | Тривалість: {sum1.Duration} | Ціна: {sum1.TicketPrice:F2} грн.");
Console.WriteLine($"Рівність за значенням (sum1 == sum1Clone): {sum1 == sum1Clone}");
Console.WriteLine($"Копія через with (нічний сеанс): {sum1EveningPromo.StartTime:HH:mm} | Нова ціна: {sum1EveningPromo.TicketPrice:F2} грн. (оригінал не змінився: {sum1.TicketPrice:F2} грн.)");

MovieSessionSummary[] summaries = [sum1, sum2, sum3, sum4];

// 2. Nullable reference types та Pattern Matching (switch-вираз)
Console.WriteLine("\n=== 2. Nullable-дані та класифікація вікових категорій (Pattern Matching) ===");
foreach (MovieSessionSummary item in summaries)
{
    string displayTitle = CinemaDataProcessor.GetDisplayTitle(item);
    string ratingLabel = CinemaDataProcessor.GetAgeRatingLabel(item);
    string category = CinemaDataProcessor.ClassifyAgeCategory(item);

    Console.WriteLine($" - Сеанс #{item.SessionId}: {displayTitle}");
    Console.WriteLine($"   Тривалість: {item.Duration} | Рейтинг: {ratingLabel} -> {category}");
}

// 3. Індекси (^) та діапазони (..) + перевірка граничних випадків
Console.WriteLine("\n=== 3. Індекси (^) та діапазони (..) і граничні випадки ===");

Console.WriteLine("Скорочення назв фільмів через діапазон [..10] та індекс [^1]:");
Console.WriteLine($" - Довга назва:  {CinemaDataProcessor.GetShortTitleWithEndMarker("Дюна: Частина друга")}");
Console.WriteLine($" - Коротка назва: {CinemaDataProcessor.GetShortTitleWithEndMarker("Дюна")}");
Console.WriteLine($" - Null-рядок:    {CinemaDataProcessor.GetShortTitleWithEndMarker(null)}");

MovieSessionSummary[] middleSlice = CinemaDataProcessor.GetMiddleSessionsSlice(summaries);
Console.WriteLine($"\nСередній зріз масиву сеансів [1..^1] (елементів: {middleSlice.Length}):");
foreach (MovieSessionSummary item in middleSlice)
{
    Console.WriteLine($" - Сеанс #{item.SessionId}: {CinemaDataProcessor.GetDisplayTitle(item)} ({item.StartTime:HH:mm})");
}

MovieSessionSummary? lastSession = CinemaDataProcessor.GetLastSession(summaries);
Console.WriteLine($"Останній сеанс у масиві [^1]: {(lastSession is not null ? $"#{lastSession.SessionId} {lastSession.OriginalTitle}" : "відсутній")}");

// Перевірка граничного випадку: порожній масив
MovieSessionSummary[] emptyArray = [];
MovieSessionSummary[] emptySlice = CinemaDataProcessor.GetMiddleSessionsSlice(emptyArray);
MovieSessionSummary? lastInEmpty = CinemaDataProcessor.GetLastSession(emptyArray);
Console.WriteLine($"Граничний випадок (порожній масив): зріз [1..^1] має {emptySlice.Length} елементів; [^1] = {(lastInEmpty is null ? "null (безпечно опрацьовано)" : lastInEmpty.OriginalTitle)}");