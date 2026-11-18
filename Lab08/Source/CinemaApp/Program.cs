using CinemaApp.Common;
using CinemaApp.Models;
using CinemaApp.Services;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// Підготовка фільмів
FeatureFilm dune = new("Дюна: Частина друга", "Наукова фантастика", 166, "Дені Вільнев", "12+");
FeatureFilm oppenheimer = new("Оппенгеймер", "Біографічна драма", 180, "Крістофер Нолан", "16+");
AnimatedMovie insideOut = new("Думками навиворіт 2", "Сімейна комедія", 96, "Pixar", "3D-анімація");

CinemaScheduleService cinemaService = new();
cinemaService.RegisterMovie(101, dune);
cinemaService.RegisterMovie(102, oppenheimer);
cinemaService.RegisterMovie(103, insideOut);

// Створення сеансів
MovieSession s1 = new(dune, new Hall(1, 100), new DateTime(2026, 11, 24, 13, 0, 0), 180m);
MovieSession s2 = new(insideOut, new Hall(2, 50), new DateTime(2026, 11, 24, 15, 30, 0), 140m);
MovieSession s3 = new(dune, new Hall(1, 100), new DateTime(2026, 11, 24, 19, 0, 0), 220m);
MovieSession s4 = new(oppenheimer, new Hall(3, 40), new DateTime(2026, 11, 24, 21, 15, 0), 200m);

s1.SellTickets(40);
s2.SellTickets(30);
s3.SellTickets(65);
s4.SellTickets(20);

cinemaService.AddSession(s1);
cinemaService.AddSession(s2);
cinemaService.AddSession(s3);
cinemaService.AddSession(s4);

// Підписка на події сеансів
TicketAvailabilityTracker availabilityTracker = new();
availabilityTracker.TrackSession(s1);
availabilityTracker.TrackSession(s2);
availabilityTracker.TrackSession(s3);
availabilityTracker.TrackSession(s4);

// 1. Добір сеансів через стандартний делегат Predicate<MovieSession>
Console.WriteLine("=== 1. Добір сеансів за допомогою Predicate<MovieSession> ===");

Predicate<MovieSession> byDuneMovie = s => s.Movie.Title.Contains("Дюна", StringComparison.OrdinalIgnoreCase);
Predicate<MovieSession> eveningSessions = s => s.StartTime.Hour >= 18;
Predicate<MovieSession> affordablePrice = s => s.TicketPrice <= 180m;

Console.WriteLine("Сеанси фільму «Дюна: Частина друга»:");
foreach (MovieSession s in cinemaService.SelectSessions(byDuneMovie))
{
    Console.WriteLine($" - {s.StartTime:dd.MM.yyyy HH:mm} | Зал №{s.Hall.Number} | {s.TicketPrice:F2} грн.");
}

Console.WriteLine("\nВечірні сеанси (з 18:00):");
foreach (MovieSession s in cinemaService.SelectSessions(eveningSessions))
{
    Console.WriteLine($" - «{s.Movie.Title}» | {s.StartTime:HH:mm} | Зал №{s.Hall.Number}");
}

Console.WriteLine("\nСеанси з ціною квитка до 180 грн:");
foreach (MovieSession s in cinemaService.SelectSessions(affordablePrice))
{
    Console.WriteLine($" - «{s.Movie.Title}» | {s.StartTime:HH:mm} | {s.TicketPrice:F2} грн.");
}

// 2. Обчислення очікуваної виручки через Func<MovieSession, decimal>
Console.WriteLine("\n=== 2. Розрахунок очікуваної виручки через Func<MovieSession, decimal> ===");

Func<MovieSession, decimal> currentSoldRevenue = s => s.CalculateTotalRevenue();
Func<MovieSession, decimal> maxPotentialRevenue = s => s.Hall.Seats * s.TicketPrice;

decimal soldTotalBefore = cinemaService.CalculateExpectedRevenue(cinemaService.GetAllSessions(), currentSoldRevenue);
decimal potentialTotalBefore = cinemaService.CalculateExpectedRevenue(cinemaService.GetAllSessions(), maxPotentialRevenue);

Console.WriteLine($"Поточна виручка за проданими квитками: {soldTotalBefore:F2} грн.");
Console.WriteLine($"Максимальна очікувана виручка (за повного заповнення залів): {potentialTotalBefore:F2} грн.");

// 3. Подія скасування сеансу SessionCancelled та реакція підписника
Console.WriteLine("\n=== 3. Подія скасування сеансу SessionCancelled та робота підписника ===");
Console.WriteLine($"До скасування s3 — доступних квитків у трекері: {availabilityTracker.GetTrackedAvailableTickets(s3)} шт.");

s3.Cancel();

Console.WriteLine($"Після скасування s3 — доступних квитків у трекері: {availabilityTracker.GetTrackedAvailableTickets(s3)} шт.");

OperationResult<int> sellAfterCancel = s3.TrySellTickets(5);
Console.WriteLine($"Спроба продажу квитків на скасований сеанс: IsSuccess = {sellAfterCancel.IsSuccess} | {sellAfterCancel.Message}");

// Демонстрація відписки від події (Unsubscribe)
Console.WriteLine("\n=== 4. Демонстрація відписки від події та перерахунок виручки ===");
availabilityTracker.Unsubscribe(s4);
s4.Cancel();
Console.WriteLine($"Сеанс s4 скасовано після відписки підписника (IsCancelled = {s4.IsCancelled}), сповіщення обробника не викликалося.");

decimal soldTotalAfter = cinemaService.CalculateExpectedRevenue(cinemaService.GetAllSessions(), currentSoldRevenue);
Console.WriteLine($"Оновлена поточна виручка активних сеансів: {soldTotalAfter:F2} грн.");