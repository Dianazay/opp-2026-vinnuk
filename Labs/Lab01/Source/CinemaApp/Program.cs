using CinemaApp.Models;
Console.OutputEncoding = System.Text.Encoding.UTF8;

Movie movie1 = new()
{
    Title = "Дюна: Частина друга",
    Genre = "Наукова фантастика",
    Duration = 166
};

Movie movie2 = new()
{
    Title = "Оппенгеймер",
    Genre = "Біографічна драма",
    Duration = 180
};

Hall hall1 = new()
{
    Number = 1,
    Seats = 120
};

Hall hall2 = new()
{
    Number = 2,
    Seats = 85
};

Console.WriteLine("Фільми в прокаті:");
Console.WriteLine($"«{movie1.Title}» — жанр: {movie1.Genre}, тривалість: {movie1.Duration} хв.");
Console.WriteLine($"«{movie2.Title}» — жанр: {movie2.Genre}, тривалість: {movie2.Duration} хв.");

Console.WriteLine("\nКінозали:");
Console.WriteLine($"Зал №{hall1.Number} — кількість місць: {hall1.Seats}");
Console.WriteLine($"Зал №{hall2.Number} — кількість місць: {hall2.Seats}");