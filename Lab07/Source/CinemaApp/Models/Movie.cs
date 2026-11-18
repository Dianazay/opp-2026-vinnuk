using CinemaApp.Interfaces;

namespace CinemaApp.Models;

public class Movie : IPlayable
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

    public bool IsPlaying { get; protected set; }

    public Movie(string title, string genre, int duration)
    {
        Title = title;
        Genre = genre;
        Duration = duration;
        IsPlaying = false;
    }

    public virtual string GetDescription()
    {
        return $"«{Title}» (жанр: {Genre}, тривалість: {Duration} хв.)";
    }

    public virtual void Play()
    {
        if (IsPlaying)
            throw new InvalidOperationException($"Фільм «{Title}» уже відтворюється.");

        IsPlaying = true;
        Console.WriteLine($"▶ Розпочато відтворення фільму «{Title}» ({Duration} хв.).");
    }

    public virtual void Stop()
    {
        if (!IsPlaying)
            throw new InvalidOperationException($"Фільм «{Title}» зараз не відтворюється.");

        IsPlaying = false;
        Console.WriteLine($"⏹ Завершено відтворення фільму «{Title}».");
    }
}