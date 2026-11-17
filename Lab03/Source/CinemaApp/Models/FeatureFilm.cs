namespace CinemaApp.Models;

public class FeatureFilm : Movie
{
    private string _director = string.Empty;
    private string _ageRating = string.Empty;

    public string Director
    {
        get => _director;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Ім'я режисера не може бути порожнім.", nameof(Director));
            _director = value.Trim();
        }
    }

    public string AgeRating
    {
        get => _ageRating;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Віковий рейтинг не може бути порожнім.", nameof(AgeRating));
            _ageRating = value.Trim();
        }
    }

    public FeatureFilm(string title, string genre, int duration, string director, string ageRating)
        : base(title, genre, duration)
    {
        Director = director;
        AgeRating = ageRating;
    }

    public override string GetDescription()
    {
        return $"[Художній фільм] {base.GetDescription()} | Режисер: {Director} | Віковий рейтинг: {AgeRating}";
    }
}