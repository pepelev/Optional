namespace Optional.Tests.Collections;

public static partial class AsyncStreamExtensions
{
    private static IComparer<int> ByLastDigit => Comparation.Order.Of<int>().By(number => number % 10);

    private static async Task AssertThrowsBeforeAsync<T>(this IAsyncEnumerable<T> sequence, int index)
    {
        using var tokenSource = new CancellationTokenSource();
        await using var enumerator = sequence.GetAsyncEnumerator(tokenSource.Token);
        var nextItemIndex = 0;
        while (true)
        {
            if (nextItemIndex == index)
            {
                tokenSource.Cancel();
                _ = Assert.CatchAsync<OperationCanceledException>(
                    async () => await enumerator.MoveNextAsync().ConfigureAwait(false)
                ) ?? throw new Exception();
                return;
            }

            if (!await enumerator.MoveNextAsync().ConfigureAwait(false))
            {
                throw new Exception($"Sequence ended before index {index} reached");
            }

            nextItemIndex++;
        }
    }
}