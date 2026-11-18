using CinemaApp.Common;
using CinemaApp.Exceptions;
using CinemaApp.Models;
using CinemaApp.Services;

Console.OutputEncoding = System.Text.Encoding.UTF8;

FeatureFilm dune = new("Дюна: Частина друга", "Наукова фантастика", 166, "Дені Вільнев", "12+");
AnimatedMovie insideOut = new("Думками навиворіт 2", "Сімейна комедія", 96, "Pixar", "3D-анімація");

// 1. Перевірка вхідних даних та стандартних винятків (ArgumentOutOfRangeException)
Console.WriteLine("=== 1. Валідація вхідних даних та стандартні винятки ===");

try
{
    _ = new Hall(1, 0);
}
catch (ArgumentOutOfRangeException ex)
{
    Console.WriteLine($"[Очікуваний ArgumentOutOfRangeException | Параметр: {ex.ParamName}]: {ex.Message}");
}

try
{
    _ = new MovieSession(dune, new Hall(1, 50), default, 180m);
}
catch (ArgumentOutOfRangeException ex)
{
    Console.WriteLine($"[Очікуваний ArgumentOutOfRangeException | Параметр: {ex.ParamName}]: {ex.Message}");
}

try
{
    _ = new MovieSession(dune, new Hall(1, 50), new DateTime(2026, 11, 26, 14, 0, 0), -120m);
}
catch (ArgumentOutOfRangeException ex)
{
    Console.WriteLine($"[Очікуваний ArgumentOutOfRangeException | Параметр: {ex.ParamName}]: {ex.Message}");
}

// 2. Перевірка власного винятку HallScheduleConflictException та незмінності розкладу
Console.WriteLine("\n=== 2. Конфлікт розкладу залу (HallScheduleConflictException) ===");
CinemaScheduleService cinemaService = new();
cinemaService.RegisterMovie(101, dune);
cinemaService.RegisterMovie(102, insideOut);

// Сеанс 1: Зал №1, 14:00 - 16:46 (тривалість 166 хв)
MovieSession validSession1 = new(dune, new Hall(1, 25), new DateTime(2026, 11, 26, 14, 0, 0), 200m);
cinemaService.AddSession(validSession1);
Console.WriteLine($"Додано сеанс 1: Зал №1, {validSession1.StartTime:HH:mm}–{validSession1.EndTime:HH:mm}. Сеансів у розкладі: {cinemaService.ScheduleCount}");

// Спроба додати сеанс у той самий Зал №1 на 15:30 (перетинається з 14:00-16:46)
MovieSession conflictingSession = new(insideOut, new Hall(1, 25), new DateTime(2026, 11, 26, 15, 30, 0), 150m);
try
{
    cinemaService.AddSession(conflictingSession);
}
catch (HallScheduleConflictException ex)
{
    Console.WriteLine($"[Перехоплено власний виняток]: {ex.Message}");
}
Console.WriteLine($"Стан розкладу після відмови: сеансів у списку = {cinemaService.ScheduleCount} (не змінився).");

// Коректне додавання сеансу після завершення попереднього (17:00)
MovieSession validSession2 = new(insideOut, new Hall(1, 25), new DateTime(2026, 11, 26, 17, 0, 0), 150m);
cinemaService.AddSession(validSession2);
Console.WriteLine($"Додано сеанс 2 без перетину: Зал №1, {validSession2.StartTime:HH:mm}–{validSession2.EndTime:HH:mm}. Сеансів у розкладі: {cinemaService.ScheduleCount}");

// 3. Межа оброблення помилок під час продажу квитків (штатний, помилковий і граничний сценарії)
Console.WriteLine("\n=== 3. Надійність продажу квитків та збереження узгодженого стану ===");

// Штатний сценарій: купівля 20 із 25 місць
OperationResult<int> sale1 = cinemaService.ProcessTicketSale(validSession1, 20);
Console.WriteLine($"1) Штатний продаж (20 квит.): IsSuccess={sale1.IsSuccess} | {sale1.Message}");

// Помилкове введення: від'ємна або нульова кількість квитків
OperationResult<int> saleInvalid = cinemaService.ProcessTicketSale(validSession1, -3);
Console.WriteLine($"2) Некоректне введення (-3 квит.): IsSuccess={saleInvalid.IsSuccess} | {saleInvalid.Message}");
Console.WriteLine($"   Перевірка інваріанта залу: зайнято = {validSession1.Hall.OccupiedSeats}/25, вільно = {validSession1.AvailableTickets}");

// Відсутні місця: запит 10 місць, коли залишилося лише 5
OperationResult<int> saleOverflow = cinemaService.ProcessTicketSale(validSession1, 10);
Console.WriteLine($"3) Перевищення місткості (10 квит.): IsSuccess={saleOverflow.IsSuccess} | {saleOverflow.Message}");
Console.WriteLine($"   Перевірка інваріанта залу: зайнято = {validSession1.Hall.OccupiedSeats}/25, вільно = {validSession1.AvailableTickets}");

// Граничне значення: викуп рівно 5 останніх місць (залишок стає 0)
OperationResult<int> saleBoundary = cinemaService.ProcessTicketSale(validSession1, 5);
Console.WriteLine($"4) Граничний продаж (останні 5 квит.): IsSuccess={saleBoundary.IsSuccess} | {saleBoundary.Message}");
Console.WriteLine($"   Прапорець активної операції після finally: IsSaleInProgress = {cinemaService.IsSaleInProgress}");

// Спроба продажу на скасований сеанс
validSession2.Cancel();
OperationResult<int> saleCancelled = cinemaService.ProcessTicketSale(validSession2, 2);
Console.WriteLine($"5) Продаж на скасований сеанс: IsSuccess={saleCancelled.IsSuccess} | {saleCancelled.Message}");