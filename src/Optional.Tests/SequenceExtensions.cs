using System.Runtime.CompilerServices;
using Optional.Tests.Collections;

namespace Optional.Tests;

public static class SequenceExtensions
{
    public static async IAsyncEnumerable<T> ToAsync<T>(
        this IEnumerable<T> sequence,
        [EnumeratorCancellation] CancellationToken token = default)
    {
        await Task.Yield();
        token.ThrowIfCancellationRequested();
        foreach (var item in sequence)
        {
            yield return item;
            await Task.Yield();
            token.ThrowIfCancellationRequested();
        }
    }

    public static MockAsyncEnumerable<T> Mock<T>(
        this IEnumerable<T> sequence,
        params MockAsyncEnumerable.Stage[] expectedStages) => new(sequence.ToAsync(), expectedStages);
}