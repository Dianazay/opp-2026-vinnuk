namespace CinemaApp.Models;

public class Movie
{
    private string _title = string.Empty;
    private string _genre = string.Empty;
    private int _duration;

    public string Title
    {
        get => _title;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Назва фільму не може бути порожньою.", nameof(Title));
            _title = value.Trim();
        }
    }

    public string Genre
    {
        get => _genre;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Жанр фільму не може бути порожнім.", nameof(Genre));
            _genre = value.Trim();
        }
    }

    public int Duration
    {
        get => _duration;
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(Duration), "Тривалість фільму повинна бути більшою за 0 хвилин.");
            _duration = value;
        }
    }

    public Movie(string title, string genre, int duration)
    {
        Title = title;
        Genre = genre;
        Duration = duration;
    }
}