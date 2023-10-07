namespace Optional.Order;

internal sealed class InvertedOrder<T> : IComparer<T>
{
    private readonly IComparer<T> order;

    public InvertedOrder(IComparer<T> order)
    {
        this.order = order;
    }

    public int Compare(T? x, T? y) => order.Compare(y, x);
}