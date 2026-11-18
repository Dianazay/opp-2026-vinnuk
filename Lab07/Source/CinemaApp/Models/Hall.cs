namespace CinemaApp.Models;

public class Hall
{
    private int _number;
    private int _seats;

    public int Number
    {
        get => _number;
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(Number), "Номер залу повинен бути більшим за 0.");
            _number = value;
        }
    }

    public int Seats
    {
        get => _seats;
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(Seats), "Кількість місць у залі повинна бути більшою за 0.");
            if (value < OccupiedSeats)
                throw new InvalidOperationException("Кількість місць не може бути меншою за кількість вже заброньованих.");
            _seats = value;
        }
    }

    public int OccupiedSeats { get; private set; }

    public int AvailableSeats => Seats - OccupiedSeats;

    public Hall(int number, int seats)
    {
        Number = number;
        Seats = seats;
        OccupiedSeats = 0;
    }

    public void BookSeat(int count = 1)
    {
        if (count <= 0)
            throw new ArgumentOutOfRangeException(nameof(count), "Кількість місць для бронювання повинна бути більшою за 0.");

        if (OccupiedSeats + count > Seats)
            throw new InvalidOperationException($"Недостатньо місць у залі №{Number}. Вільно: {AvailableSeats}, запитано: {count}.");

        OccupiedSeats += count;
    }

    public void CancelSeat(int count = 1)
    {
        if (count <= 0)
            throw new ArgumentOutOfRangeException(nameof(count), "Кількість місць для скасування повинна бути більшою за 0.");

        if (OccupiedSeats - count < 0)
            throw new InvalidOperationException($"Неможливо скасувати {count} місць. У залі №{Number} заброньовано лише {OccupiedSeats}.");

        OccupiedSeats -= count;
    }
}