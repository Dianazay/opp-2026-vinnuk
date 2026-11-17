using CinemaApp.Models;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// 1. Формування поліморфного каталогу фільмів (List<Movie>)
List<Movie> movieCatalog = new()
{
    new FeatureFilm("Дюна: Частина друга", "Наукова фантастика", 166, "Дені Вільнев", "12+"),
    new Documentary("Наша планета", "Природа та екологія", 95, "Зміна клімату та дика природа", 2023),
    new AnimatedMovie("Думками навиворіт 2", "Сімейна комедія", 96, "Pixar", "3D-анімація"),
    new FeatureFilm("Оппенгеймер", "Біографічна драма", 180, "Крістофер Нолан", "16+"),
    new AnimatedMovie("Мавка. Лісова пісня", "Фентезі", 99, "Animagrad", "3D-анімація")
};

// 2. Поліморфна обробка каталогу через базовий тип Movie
DisplayCatalog(movieCatalog);

// 3. Зв'язок із функціональністю попередніх робіт (робота залу)
Console.WriteLine("\n=== Стан кінозалу для показу каталогу ===");
Hall mainHall = new(1, 120);
mainHall.BookSeat(35);
Console.WriteLine($"Зал №{mainHall.Number}: заброньовано {mainHall.OccupiedSeats} із {mainHall.Seats} місць (вільно: {mainHall.AvailableSeats}).");

// 4. Перевірка валідації у похідних класах
Console.WriteLine("\n=== Перевірка валідації похідних класів ===");
try
{
    Movie invalidDoc = new Documentary("Космос", "Наука", 60, "", 2024);
}
catch (Exception ex)
{
    Console.WriteLine($"[Помилка створення документального фільму]: {ex.Message}");
}

static void DisplayCatalog(List<Movie> movies)
{
    Console.WriteLine("=== Каталог фільмів кінотеатру (поліморфний вивід) ===");
    int totalDuration = 0;

    for (int i = 0; i < movies.Count; i++)
    {
        // Змінна має базовий тип Movie, але викликається перевизначений метод похідного класу
        Movie currentMovie = movies[i];
        Console.WriteLine($"{i + 1}. {currentMovie.GetDescription()}");
        totalDuration += currentMovie.Duration;
    }

    Console.WriteLine($"\nВсього фільмів у каталозі: {movies.Count}");
    Console.WriteLine($"Загальна тривалість усіх сеансів: {totalDuration} хв.");
}