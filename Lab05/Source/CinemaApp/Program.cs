using CinemaApp.Common;
using CinemaApp.Models;
using CinemaApp.Services;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// Створення окремих об'єктів предметної області
FeatureFilm dune = new("Дюна: Частина друга", "Наукова фантастика", 166, "Дені Вільнев", "12+");
FeatureFilm oppenheimer = new("Оппенгеймер", "Біографічна драма", 180, "Крістофер Нолан", "16+");
Documentary doc2023 = new("Наша планета", "Природа та екологія", 95, "Дика природа", 2023);
Documentary doc2025 = new("Таємниці океану", "Науково-популярний", 88, "Глибоководні екосистеми", 2025);
AnimatedMovie insideOut = new("Думками навиворіт 2", "Сімейна комедія", 96, "Pixar", "3D-анімація");

Hall hall1 = new(1, 50);

// 1. Узагальнений результат операції OperationResult<T>
Console.WriteLine("=== 1. Узагальнений результат операції OperationResult<T> ===");

OperationResult<MovieSession> createOk = MovieSession.TryCreate(
    dune, hall1, new DateTime(2026, 11, 20, 19, 0, 0), 200m);
Console.WriteLine($"[Створення сеансу 1]: IsSuccess = {createOk.IsSuccess} | {createOk.Message}");

OperationResult<MovieSession> createFail = MovieSession.TryCreate(
    oppenheimer, hall1, new DateTime(2026, 11, 20, 22, 0, 0), -100m);
Console.WriteLine($"[Створення сеансу 2]: IsSuccess = {createFail.IsSuccess} | {createFail.Message}");

if (createOk.IsSuccess && createOk.Value is not null)
{
    OperationResult<int> sellOk = createOk.Value.TrySellTickets(15);
    Console.WriteLine($"[Продаж квитків 1]: IsSuccess = {sellOk.IsSuccess} | Продано: {sellOk.Value} | {sellOk.Message}");

    OperationResult<int> sellFail = createOk.Value.TrySellTickets(45);
    Console.WriteLine($"[Продаж квитків 2]: IsSuccess = {sellFail.IsSuccess} | Продано: {sellFail.Value} | {sellFail.Message}");
}

// 2. Узагальнений метод вибору ChooseBy<T, TKey> (виведення аргументів типу)
Console.WriteLine("\n=== 2. Узагальнений вибір ChooseBy<T, TKey> ===");

FeatureFilm longerFilm = CinemaHelper.ChooseBy(dune, oppenheimer, m => m.Duration);
Console.WriteLine($"Довший художній фільм: «{longerFilm.Title}» ({longerFilm.Duration} хв.)");

Documentary newerDoc = CinemaHelper.ChooseBy(doc2023, doc2025, d => d.ReleaseYear);
Console.WriteLine($"Новіший документальний фільм: «{newerDoc.Title}» ({newerDoc.ReleaseYear} р.)");

FeatureFilm higherAgeRating = CinemaHelper.ChooseBy(dune, oppenheimer, f => f.AgeRating);
Console.WriteLine($"Фільм із вищим віковим рейтингом: «{higherAgeRating.Title}» ({higherAgeRating.AgeRating})");

// 3. Узагальнений метод із обмеженням where T : Movie
Console.WriteLine("\n=== 3. Узагальнений опис DescribeMovie<T> where T : Movie ===");
Console.WriteLine(CinemaHelper.DescribeMovie(dune));
Console.WriteLine(CinemaHelper.DescribeMovie(doc2025));
Console.WriteLine(CinemaHelper.DescribeMovie(insideOut));
// Примітка: виклик CinemaHelper.DescribeMovie(hall1) не скомпілюється через обмеження where T : Movie.