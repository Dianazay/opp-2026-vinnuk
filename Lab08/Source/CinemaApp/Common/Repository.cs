namespace CinemaApp.Common;

public class Repository<T> where T : class
{
    private readonly List<T> _items = [];

    public int Count => _items.Count;

    public void Add(T item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _items.Add(item);
    }

    public bool Remove(T item)
    {
        ArgumentNullException.ThrowIfNull(item);
        return _items.Remove(item);
    }

    public IReadOnlyList<T> GetAll() => _items.AsReadOnly();

    public IEnumerable<T> Find(Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        foreach (T item in _items)
        {
            if (predicate(item))
                yield return item;
        }
    }
}