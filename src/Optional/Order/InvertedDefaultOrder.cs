namespace Optional.Order;

internal sealed class InvertedDefaultOrder<T> : IComparer<T>
{
    public static InvertedDefaultOrder<T> Singleton { get; } = new();

    public int Compare(T? x, T? y) => Comparer<T>.Default.Compare(y, x);
}