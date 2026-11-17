using CinemaApp.Models;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("=== 1. Створення коректних об'єктів ===");
Movie movie1 = new("Дюна: Частина друга", "Наукова фантастика", 166);
Movie movie2 = new("Оппенгеймер", "Біографічна драма", 180);

Hall hall1 = new(1, 50);
Hall hall2 = new(2, 10);

Console.WriteLine($"Фільм 1: «{movie1.Title}» ({movie1.Genre}, {movie1.Duration} хв.)");
Console.WriteLine($"Фільм 2: «{movie2.Title}» ({movie2.Genre}, {movie2.Duration} хв.)");
Console.WriteLine($"Зал №{hall1.Number}: всього місць — {hall1.Seats}, зайнято — {hall1.OccupiedSeats}");
Console.WriteLine($"Зал №{hall2.Number}: всього місць — {hall2.Seats}, зайнято — {hall2.OccupiedSeats}");

Console.WriteLine("\n=== 2. Зміна властивостей та керування станом залу ===");
movie1.Duration = 170;
Console.WriteLine($"Оновлена тривалість фільму «{movie1.Title}»: {movie1.Duration} хв.");

hall2.BookSeat();      // бронюємо 1 місце
hall2.BookSeat(6);     // бронюємо ще 6 місць (разом 7 із 10)
Console.WriteLine($"Зал №{hall2.Number} після бронювання: зайнято {hall2.OccupiedSeats} із {hall2.Seats} (вільно: {hall2.AvailableSeats})");

hall2.CancelSeat(2);   // скасовуємо 2 місця (залишається 5)
Console.WriteLine($"Зал №{hall2.Number} після скасування 2 місць: зайнято {hall2.OccupiedSeats} із {hall2.Seats} (вільно: {hall2.AvailableSeats})");

// Пряма зміна стану заборонена компілятором через private set:
// hall2.OccupiedSeats = 100;

Console.WriteLine("\n=== 3. Перевірка валідації (спроба передати некоректні дані) ===");

try
{
    Movie invalidMovie = new("", "Комедія", 120);
}
catch (Exception ex)
{
    Console.WriteLine($"[Помилка створення фільму]: {ex.Message}");
}

try
{
    movie2.Duration = -15;
}
catch (Exception ex)
{
    Console.WriteLine($"[Помилка зміни тривалості]: {ex.Message}");
}

try
{
    Hall invalidHall = new(3, 0);
}
catch (Exception ex)
{
    Console.WriteLine($"[Помилка створення залу]: {ex.Message}");
}

try
{
    // У залі №2 зараз зайнято 5 із 10 місць, спробуємо забронювати ще 8:
    hall2.BookSeat(8);
}
catch (Exception ex)
{
    Console.WriteLine($"[Помилка бронювання]: {ex.Message}");
}

try
{
    // Спробуємо скасувати 10 місць, коли зайнято лише 5:
    hall2.CancelSeat(10);
}
catch (Exception ex)
{
    Console.WriteLine($"[Помилка скасування]: {ex.Message}");
}