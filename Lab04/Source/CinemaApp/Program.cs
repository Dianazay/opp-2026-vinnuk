using CinemaApp.Interfaces;
using CinemaApp.Models;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// 1. Створення об'єктів фільмів (різних похідних класів) та кінозалів
FeatureFilm dune = new("Дюна: Частина друга", "Наукова фантастика", 166, "Дені Вільнев", "12+");
Documentary planet = new("Наша планета", "Природа та екологія", 95, "Зміна клімату та дика природа", 2023);
AnimatedMovie insideOut = new("Думками навиворіт 2", "Сімейна комедія", 96, "Pixar", "3D-анімація");

Hall imaxHall = new(1, 100);
Hall cozyHall = new(2, 40);

// 2. Демонстрація композиції: створення кіносеансів (MovieSession об'єднує Movie та Hall)
Console.WriteLine("=== 1. Композиція об'єктів: формування сеансів та продаж квитків ===");
MovieSession eveningSession = new(dune, imaxHall, new DateTime(2026, 11, 18, 19, 30, 0), 220m);
MovieSession familySession = new(insideOut, cozyHall, new DateTime(2026, 11, 18, 16, 0, 0), 150m);

eveningSession.SellTickets(45);
familySession.SellTickets(28);

Console.WriteLine(eveningSession.GetSessionInfo());
Console.WriteLine();
Console.WriteLine(familySession.GetSessionInfo());

// 3. Поліморфна система відтворення через інтерфейс IPlayable
Console.WriteLine("\n=== 2. Поліморфна робота через інтерфейс IPlayable ===");
List<IPlayable> playbackQueue = new()
{
    dune,
    planet,
    insideOut
};

StartAllPlaybacks(playbackQueue);
Console.WriteLine();
StopAllPlaybacks(playbackQueue);

// 4. Перевірка валідації композиції та стану відтворення
Console.WriteLine("\n=== 3. Перевірка валідації та обробка винятків ===");
try
{
    MovieSession invalidSession = new(planet, cozyHall, DateTime.Now, -50m);
}
catch (Exception ex)
{
    Console.WriteLine($"[Помилка створення сеансу]: {ex.Message}");
}

try
{
    // Спроба зупинити фільм, який зараз не відтворюється:
    dune.Stop();
}
catch (Exception ex)
{
    Console.WriteLine($"[Помилка керування відтворенням]: {ex.Message}");
}

static void StartAllPlaybacks(List<IPlayable> items)
{
    Console.WriteLine("--- Запуск черги відтворення (через інтерфейс IPlayable) ---");
    foreach (IPlayable item in items)
    {
        item.Play();
    }
}

static void StopAllPlaybacks(List<IPlayable> items)
{
    Console.WriteLine("--- Зупинка черги відтворення (через інтерфейс IPlayable) ---");
    foreach (IPlayable item in items)
    {
        item.Stop();
    }
}