namespace CinemaApp.Models;

public class AnimatedMovie : Movie
{
    private string _studio = string.Empty;
    private string _animationTechnique = string.Empty;

    public string Studio
    {
        get => _studio;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Назва студії не може бути порожньою.", nameof(Studio));
            _studio = value.Trim();
        }
    }

    public string AnimationTechnique
    {
        get => _animationTechnique;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Техніка анімації не може бути порожньою.", nameof(AnimationTechnique));
            _animationTechnique = value.Trim();
        }
    }

    public AnimatedMovie(string title, string genre, int duration, string studio, string animationTechnique)
        : base(title, genre, duration)
    {
        Studio = studio;
        AnimationTechnique = animationTechnique;
    }

    public override string GetDescription()
    {
        return $"[Анімаційний фільм] {base.GetDescription()} | Студія: {Studio} | Техніка: {AnimationTechnique}";
    }

    public override void Play()
    {
        if (IsPlaying)
            throw new InvalidOperationException($"Анімаційний фільм «{Title}» уже відтворюється.");

        IsPlaying = true;
        Console.WriteLine($"▶ [Анімаційний фільм] Запуск мультфільму «{Title}» від студії {Studio} ({AnimationTechnique}).");
    }

    public override void Stop()
    {
        if (!IsPlaying)
            throw new InvalidOperationException($"Анімаційний фільм «{Title}» зараз не відтворюється.");

        IsPlaying = false;
        Console.WriteLine($"⏹ [Анімаційний фільм] Показ мультфільму «{Title}» завершено.");
    }
}