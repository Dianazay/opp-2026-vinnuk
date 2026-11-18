using CinemaApp.Models;
using CinemaApp.Services;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// Підготовка фільмів
FeatureFilm dune = new("Дюна: Частина друга", "Наукова фантастика", 166, "Дені Вільнев", "12+");
FeatureFilm oppenheimer = new("Оппенгеймер", "Біографічна драма", 180, "Крістофер Нолан", "16+");
AnimatedMovie insideOut = new("Думками навиворіт 2", "Сімейна комедія", 96, "Pixar", "3D-анімація");
Documentary planet = new("Наша планета", "Природа та екологія", 95, "Дика природа", 2023);

CinemaScheduleService cinemaService = new();
cinemaService.RegisterMovie(101, dune);
cinemaService.RegisterMovie(102, oppenheimer);
cinemaService.RegisterMovie(103, insideOut);
cinemaService.RegisterMovie(104, planet);

// Створення початкового розкладу (окремий стан залу для кожного сеансу)
MovieSession s1 = new(dune, new Hall(1, 100), new DateTime(2026, 11, 22, 12, 0, 0), 170m);
MovieSession s2 = new(insideOut, new Hall(2, 50), new DateTime(2026, 11, 22, 14, 30, 0), 140m);
MovieSession s3 = new(dune, new Hall(1, 100), new DateTime(2026, 11, 22, 18, 0, 0), 220m);
MovieSession s4 = new(oppenheimer, new Hall(3, 40), new DateTime(2026, 11, 22, 20, 30, 0), 210m);
MovieSession s5 = new(dune, new Hall(2, 50), new DateTime(2026, 11, 23, 19, 0, 0), 240m);

s1.SellTickets(45);
s2.SellTickets(30);
s3.SellTickets(80);
s4.SellTickets(25);
s5.SellTickets(35);

cinemaService.AddSession(s1);
cinemaService.AddSession(s2);
cinemaService.AddSession(s3);
cinemaService.AddSession(s4);
cinemaService.AddSession(s5);

// 1. Фільтрація та сортування (методи розширення та запитний синтаксис)
Console.WriteLine("=== 1. Фільтрація та сортування сеансів (LINQ Where + OrderBy) ===");
var duneSessions = cinemaService.FilterAndSortSessions(movieTitle: "Дюна");
foreach (MovieSession session in duneSessions)
{
    Console.WriteLine($" - {session.StartTime:dd.MM.yyyy HH:mm} | Зал №{session.Hall.Number} | «{session.Movie.Title}» | {session.TicketPrice:F2} грн.");
}

Console.WriteLine("\n=== 1б. Бюджетні сеанси до 180 грн (запитний синтаксис LINQ) ===");
var budgetSessions = cinemaService.GetSessionsByMaxPriceQuerySyntax(180m);
foreach (MovieSession session in budgetSessions)
{
    Console.WriteLine($" - «{session.Movie.Title}» | {session.StartTime:dd.MM.yyyy HH:mm} | {session.TicketPrice:F2} грн.");
}

// 2. Проєктування розкладу через Select (типізована модель та анонімний тип)
Console.WriteLine("\n=== 2. Проєктування розкладу (LINQ Select) ===");
foreach (ScheduleProjection item in cinemaService.GetScheduleProjections())
{
    Console.WriteLine($" - [{item.StartTime:dd.MM HH:mm}] Зал №{item.HallNumber}: «{item.MovieTitle}» — {item.TicketPrice:F2} грн.");
}

Console.WriteLine("\nПроєкція в анонімні типи (сеанси на 22.11.2026):");
var anonymousSchedule = cinemaService.GetAllSessions()
    .Where(s => s.StartTime.Date == new DateTime(2026, 11, 22))
    .Select(s => new
    {
        Film = s.Movie.Title,
        Time = s.StartTime.ToString("HH:mm"),
        Price = $"{s.TicketPrice:F2} грн."
    });

foreach (var row in anonymousSchedule)
{
    Console.WriteLine($" - {row.Time} | {row.Film} ({row.Price})");
}

// 3. Групування за фільмами та агрегування (GroupBy, Count, Average, Sum)
Console.WriteLine("\n=== 3. Статистика за фільмами (LINQ GroupBy + Count + Average + Sum) ===");
foreach (MovieSessionStats stat in cinemaService.GetMovieStatistics())
{
    Console.WriteLine($" - «{stat.MovieTitle}»: сеансів — {stat.SessionsCount} шт. | середня ціна — {stat.AverageTicketPrice:F2} грн. | виручка — {stat.TotalRevenue:F2} грн.");
}

// 4. Демонстрація відкладеного (Deferred) та негайного (Immediate) виконання LINQ
Console.WriteLine("\n=== 4. Відкладене виконання LINQ vs матеріалізація ToList() ===");

// Запит формується, але ще не виконується (відкладене виконання)
IEnumerable<MovieSession> deferredEveningQuery = cinemaService.GetAllSessions()
    .Where(s => s.StartTime.Hour >= 18)
    .OrderBy(s => s.StartTime);

// Негайна матеріалізація поточного знімка даних у список
List<MovieSession> immediateSnapshot = deferredEveningQuery.ToList();

Console.WriteLine($"До додавання нового сеансу: у знімку ToList() = {immediateSnapshot.Count} сеанси.");

// Додаємо ще один вечірній сеанс уже ПІСЛЯ створення LINQ-запиту
MovieSession s6 = new(planet, new Hall(3, 40), new DateTime(2026, 11, 23, 21, 0, 0), 160m);
s6.SellTickets(20);
cinemaService.AddSession(s6);

Console.WriteLine($"Після додавання нового сеансу:");
Console.WriteLine($" - Матеріалізований список ToList() (незмінний знімок): {immediateSnapshot.Count} сеанси.");
Console.WriteLine($" - Відкладений запит IEnumerable (обчислюється під час перебору): {deferredEveningQuery.Count()} сеанси:");

foreach (MovieSession session in deferredEveningQuery)
{
    Console.WriteLine($"   * {session.StartTime:dd.MM.yyyy HH:mm} — «{session.Movie.Title}» ({session.TicketPrice:F2} грн.)");
}