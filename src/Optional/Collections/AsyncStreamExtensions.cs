#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Optional.Collections;

public static class AsyncStreamExtensions
{
    /// <summary>
    ///     Flattens a sequence of optionals into a sequence containing all inner values.
    ///     Empty elements are discarded.
    /// </summary>
    /// <param name="source">The sequence of optionals.</param>
    /// <returns>A flattened sequence of values.</returns>
    public static IAsyncEnumerable<T> Values<T>(this IAsyncEnumerable<Option<T>> source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return Yield();

        async IAsyncEnumerable<T> Yield([EnumeratorCancellation] CancellationToken token = default)
        {
            await foreach (var option in source.WithCancellation(token).ConfigureAwait(false))
            {
                if (option.HasValue)
                {
                    yield return option.Value;
                }
            }
        }
    }

    /// <summary>
    ///     Flattens a sequence of optionals into a sequence containing all inner values.
    ///     Empty elements and their exceptional values are discarded.
    /// </summary>
    /// <param name="source">The sequence of optionals.</param>
    /// <returns>A flattened sequence of values.</returns>
    public static IAsyncEnumerable<T> Values<T, TException>(this IAsyncEnumerable<Option<T, TException>> source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return Yield();

        async IAsyncEnumerable<T> Yield()
        {
            await foreach (var option in source.ConfigureAwait(false))
            {
                if (option.HasValue)
                {
                    yield return option.Value;
                }
            }
        }
    }

    /// <summary>
    ///     Flattens a sequence of optionals into a sequence containing all exceptional values.
    ///     Non-empty elements and their values are discarded.
    /// </summary>
    /// <param name="source">The sequence of optionals.</param>
    /// <returns>A flattened sequence of exceptional values.</returns>
    public static IAsyncEnumerable<TException> Exceptions<T, TException>(this IAsyncEnumerable<Option<T, TException>> source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return Yield();

        async IAsyncEnumerable<TException> Yield()
        {
            await foreach (var option in source.ConfigureAwait(false))
            {
                if (!option.HasValue)
                {
                    yield return option.Exception;
                }
            }
        }
    }

    /// <summary>
    ///     Returns the first element of a sequence if such exists.
    /// </summary>
    /// <param name="source">The sequence to return the first element from.</param>
    /// <returns>An Option&lt;T&gt; instance containing the first element if present.</returns>
    public static async Task<Option<TSource>> FirstOrNone<TSource>(this IAsyncEnumerable<TSource> source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        await foreach (var item in source.ConfigureAwait(false))
        {
            return item.Some();
        }

        return Option.None<TSource>();
    }

    /// <summary>
    ///     Returns the last element of a sequence if such exists.
    /// </summary>
    /// <param name="source">The sequence to return the last element from.</param>
    /// <returns>An Option&lt;T&gt; instance containing the last element if present.</returns>
    public static async Task<Option<TSource>> LastOrNone<TSource>(this IAsyncEnumerable<TSource> source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        var result = Option.None<TSource>();
        await foreach (var item in source.ConfigureAwait(false))
        {
            result = item.Some();
        }

        return result;
    }

    /// <summary>
    ///     Returns a single element from a sequence, if it exists
    ///     and is the only element in the sequence.
    /// </summary>
    /// <param name="source">The sequence to return the element from.</param>
    /// <returns>An Option&lt;T&gt; instance containing the element if present.</returns>
    public static async Task<Option<TSource>> SingleOrNone<TSource>(this IAsyncEnumerable<TSource> source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        await using var enumerator = source.GetAsyncEnumerator();
        var hasOne = await enumerator.MoveNextAsync().ConfigureAwait(false);
        if (!hasOne)
        {
            return Option.None<TSource>();
        }

        var potentialResult = enumerator.Current;
        var hasTwo = await enumerator.MoveNextAsync().ConfigureAwait(false);
        return hasTwo
            ? Option.None<TSource>()
            : potentialResult.Some();
    }

    /// <summary>
    ///     Returns an element at a specified position in a sequence if such exists.
    /// </summary>
    /// <param name="source">The sequence to return the element from.</param>
    /// <param name="index">The index in the sequence.</param>
    /// <returns>An Option&lt;T&gt; instance containing the element if found.</returns>
    public static async Task<Option<TSource>> ElementAtOrNone<TSource>(this IAsyncEnumerable<TSource> source, Index index)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (index.IsFromEnd)
        {
            var capacity = index.Value + 1;
            var buffer = new Queue<TSource>(capacity);
            await foreach (var item in source.ConfigureAwait(false))
            {
                if (buffer.Count == capacity)
                {
                    buffer.Dequeue();
                }

                buffer.Enqueue(item);
            }

            return buffer.Count == capacity
                ? buffer.Peek().Some()
                : Option.None<TSource>();
        }

        var itemIndex = 0;
        await foreach (var item in source.ConfigureAwait(false))
        {
            if (itemIndex == index.Value)
            {
                return item.Some();
            }

            itemIndex++;
        }

        return Option.None<TSource>();
    }
}

#endif