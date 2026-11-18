using CinemaApp.Common;
using CinemaApp.Models;
using CinemaApp.Services;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// Підготовка об'єктів предметної області
FeatureFilm dune = new("Дюна: Частина друга", "Наукова фантастика", 166, "Дені Вільнев", "12+");
FeatureFilm oppenheimer = new("Оппенгеймер", "Біографічна драма", 180, "Крістофер Нолан", "16+");
Documentary planet = new("Наша планета", "Природа та екологія", 95, "Дика природа", 2023);
AnimatedMovie insideOut = new("Думками навиворіт 2", "Сімейна комедія", 96, "Pixar", "3D-анімація");

// Окремі екземпляри стану залів для кожного сеансу
Hall imaxHallDay = new(1, 100);
Hall cozyHall = new(2, 45);
Hall imaxHallEvening = new(1, 100);
Hall vipHall = new(3, 30);

// 1. Робота з Generic-репозиторієм Repository<Movie>
Console.WriteLine("=== 1. Generic-репозиторій фільмів Repository<Movie> ===");
Repository<Movie> movieRepository = new();
movieRepository.Add(dune);
movieRepository.Add(oppenheimer);
movieRepository.Add(planet);
movieRepository.Add(insideOut);

Console.WriteLine($"Кількість фільмів у репозиторії після додавання: {movieRepository.Count}");

bool isRemoved = movieRepository.Remove(planet);
Console.WriteLine($"Видалення фільму «{planet.Title}»: {(isRemoved ? "успішно" : "не знайдено")}");
Console.WriteLine($"Залишилося фільмів у репозиторії: {movieRepository.Count}");

foreach (Movie movie in movieRepository.GetAll())
{
    Console.WriteLine($" - {movie.GetDescription()}");
}

// 2. Каталог Dictionary<int, Movie> та розклад List<MovieSession>
Console.WriteLine("\n=== 2. Каталог Dictionary<int, Movie> та розклад List<MovieSession> ===");
CinemaScheduleService cinemaService = new();

cinemaService.RegisterMovie(101, dune);
cinemaService.RegisterMovie(102, oppenheimer);
cinemaService.RegisterMovie(103, insideOut);

OperationResult<Movie> foundMovie = cinemaService.GetMovieById(102);
Console.WriteLine($"[Пошук у Dictionary за ID 102]: {foundMovie.Message}");

OperationResult<Movie> missingMovie = cinemaService.GetMovieById(999);
Console.WriteLine($"[Пошук у Dictionary за ID 999]: {missingMovie.Message}");

MovieSession session1 = new(dune, imaxHallDay, new DateTime(2026, 11, 21, 14, 0, 0), 190m);
MovieSession session2 = new(insideOut, cozyHall, new DateTime(2026, 11, 21, 16, 30, 0), 150m);
MovieSession session3 = new(dune, imaxHallEvening, new DateTime(2026, 11, 21, 19, 30, 0), 230m);
MovieSession session4 = new(oppenheimer, vipHall, new DateTime(2026, 11, 21, 21, 0, 0), 210m);

session1.SellTickets(40);
session3.SellTickets(75);

cinemaService.AddSession(session1);
cinemaService.AddSession(session2);
cinemaService.AddSession(session3);
cinemaService.AddSession(session4);

Console.WriteLine($"Всього сеансів у розкладі (List<MovieSession>): {cinemaService.ScheduleCount}");

// 3. Ітератори IEnumerable<MovieSession> з yield return
Console.WriteLine($"\n=== 3. Сеанси фільму «{dune.Title}» (ітератор yield return) ===");
foreach (MovieSession session in cinemaService.GetSessionsByMovie(dune))
{
    Console.WriteLine(session.GetSessionInfo());
}

DateTime eveningThreshold = new(2026, 11, 21, 18, 0, 0);
Console.WriteLine($"\n=== 4. Вечірні сеанси з {eveningThreshold:HH:mm} (ітератор yield return) ===");
foreach (MovieSession session in cinemaService.GetSessionsFromTime(eveningThreshold))
{
    Console.WriteLine($" - {session.StartTime:dd.MM.yyyy HH:mm} | Зал №{session.Hall.Number} | «{session.Movie.Title}» ({session.TicketPrice:F2} грн.)");
}

// 4. Узагальнений пошук з обмеженням where T : Movie
Console.WriteLine("\n=== 5. Узагальнений пошук FindMovies<T> where T : Movie (тривалість > 120 хв.) ===");
IEnumerable<Movie> longMovies = CinemaHelper.FindMovies(
    cinemaService.GetCatalog().Values,
    m => m.Duration > 120);

foreach (Movie movie in longMovies)
{
    Console.WriteLine($" - {CinemaHelper.DescribeMovie(movie)}");
}