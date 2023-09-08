using Optional.Collections;

namespace Optional.Tests.Collections;

public sealed class AsyncStreamExtensions_Should
{
    [Test]
    public async Task METHOD()
    {
        using var cts = new CancellationTokenSource();
        await foreach (var item in new[] { 1.Some() }.ToAsyncEnumerable().Values().WithCancellation(cts.Token).ConfigureAwait(false))
        {
            Console.WriteLine(item);
        }
    }
}