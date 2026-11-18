using CinemaApp.Exceptions;

namespace CinemaApp.Models;

public class Hall
{
    public int Number { get; }
    public int Seats { get; }
    public int OccupiedSeats { get; private set; }
    public int AvailableSeats => Seats - OccupiedSeats;

    public Hall(int number, int seats)
    {
        if (number <= 0)
            throw new ArgumentOutOfRangeException(nameof(number), number, "Номер кінозалу повинен бути більшим за 0.");

        if (seats <= 0)
            throw new ArgumentOutOfRangeException(nameof(seats), seats, "Кількість місць у залі повинна бути більшою за 0.");

        Number = number;
        Seats = seats;
        OccupiedSeats = 0;
    }

    public void BookSeat(int count)
    {
        if (count <= 0)
            throw new ArgumentOutOfRangeException(nameof(count), count, "Кількість квитків для бронювання повинна бути більшою за 0.");

        if (count > AvailableSeats)
            throw new InsufficientSeatsException(Number, count, AvailableSeats);

        OccupiedSeats += count;
    }
}