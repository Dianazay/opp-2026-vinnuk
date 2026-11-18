namespace CinemaApp.Models;

public class Documentary : Movie
{
    private string _topic = string.Empty;
    private int _releaseYear;

    public string Topic
    {
        get => _topic;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Тема документального фільму не може бути порожньою.", nameof(Topic));
            _topic = value.Trim();
        }
    }

    public int ReleaseYear
    {
        get => _releaseYear;
        set
        {
            if (value < 1895 || value > DateTime.Now.Year + 1)
                throw new ArgumentOutOfRangeException(nameof(ReleaseYear), "Вказано некоректний рік виходу фільму.");
            _releaseYear = value;
        }
    }

    public Documentary(string title, string genre, int duration, string topic, int releaseYear)
        : base(title, genre, duration)
    {
        Topic = topic;
        ReleaseYear = releaseYear;
    }

    public override string GetDescription()
    {
        return $"[Документальний фільм] {base.GetDescription()} | Тема: {Topic} | Рік виходу: {ReleaseYear}";
    }

    public override void Play()
    {
        if (IsPlaying)
            throw new InvalidOperationException($"Документальний фільм «{Title}» уже відтворюється.");

        IsPlaying = true;
        Console.WriteLine($"▶ [Документальний фільм] Початок показу «{Title}» (тема: «{Topic}», {ReleaseYear} р.).");
    }

    public override void Stop()
    {
        if (!IsPlaying)
            throw new InvalidOperationException($"Документальний фільм «{Title}» зараз не відтворюється.");

        IsPlaying = false;
        Console.WriteLine($"⏹ [Документальний фільм] Демонстрацію «{Title}» завершено.");
    }
}