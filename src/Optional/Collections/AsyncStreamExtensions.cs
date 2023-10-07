#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Optional.Order;

namespace Optional.Collections;

public static class AsyncStreamExtensions
{
    /// <summary>
    ///     Flattens a sequence of optionals into a sequence containing all inner values.
    ///     Empty elements are discarded.
    /// </summary>
    /// <param name="source">The sequence of optionals.</param>
    /// <param name="continueOnCapturedContext">
    ///     The argument of <see cref="TaskAsyncEnumerableExtensions.ConfigureAwait"/>.
    /// </param>
    /// <returns>A flattened sequence of values.</returns>
    public static IAsyncEnumerable<T> Values<T>(
        this IAsyncEnumerable<Option<T>> source,
        bool continueOnCapturedContext = false)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return Yield();

        async IAsyncEnumerable<T> Yield([EnumeratorCancellation] CancellationToken token = default)
        {
            await foreach (var option in source.WithCancellation(token).ConfigureAwait(continueOnCapturedContext))
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
    /// <param name="continueOnCapturedContext">
    ///     The argument of <see cref="TaskAsyncEnumerableExtensions.ConfigureAwait"/>.
    /// </param>
    /// <returns>A flattened sequence of values.</returns>
    public static IAsyncEnumerable<T> Values<T, TException>(
        this IAsyncEnumerable<Option<T, TException>> source,
        bool continueOnCapturedContext = false)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return Yield();

        async IAsyncEnumerable<T> Yield([EnumeratorCancellation] CancellationToken token = default)
        {
            await foreach (var option in source.WithCancellation(token).ConfigureAwait(continueOnCapturedContext))
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
    /// <param name="continueOnCapturedContext">
    ///     The argument of <see cref="TaskAsyncEnumerableExtensions.ConfigureAwait"/>.
    /// </param>
    /// <returns>A flattened sequence of exceptional values.</returns>
    public static IAsyncEnumerable<TException> Exceptions<T, TException>(
        this IAsyncEnumerable<Option<T, TException>> source,
        bool continueOnCapturedContext = false)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return Yield();

        async IAsyncEnumerable<TException> Yield([EnumeratorCancellation] CancellationToken token = default)
        {
            await foreach (var option in source.WithCancellation(token).ConfigureAwait(continueOnCapturedContext))
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
    /// <param name="token">The cancellation token.</param>
    /// <param name="continueOnCapturedContext">
    ///     The argument of <see cref="TaskAsyncEnumerableExtensions.ConfigureAwait"/>.
    /// </param>
    /// <returns>An Option&lt;T&gt; instance containing the first element if present.</returns>
    public static async Task<Option<TSource>> FirstOrNoneAsync<TSource>(
        this IAsyncEnumerable<TSource> source,
        CancellationToken token = default,
        bool continueOnCapturedContext = false)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        await foreach (var item in source.WithCancellation(token).ConfigureAwait(continueOnCapturedContext))
        {
            return item.Some();
        }

        return Option.None<TSource>();
    }

    /// <summary>
    ///     Returns the last element of a sequence if such exists.
    /// </summary>
    /// <param name="source">The sequence to return the last element from.</param>
    /// <param name="token">The cancellation token.</param>
    /// <param name="continueOnCapturedContext">
    ///     The argument of <see cref="TaskAsyncEnumerableExtensions.ConfigureAwait"/>.
    /// </param>
    /// <returns>An Option&lt;T&gt; instance containing the last element if present.</returns>
    public static async Task<Option<TSource>> LastOrNoneAsync<TSource>(
        this IAsyncEnumerable<TSource> source,
        CancellationToken token = default,
        bool continueOnCapturedContext = false)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        var result = Option.None<TSource>();
        await foreach (var item in source.WithCancellation(token).ConfigureAwait(continueOnCapturedContext))
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
    /// <param name="token">The cancellation token.</param>
    /// <param name="continueOnCapturedContext">
    ///     The argument of <see cref="TaskAsyncEnumerableExtensions.ConfigureAwait"/>.
    /// </param>
    /// <returns>An Option&lt;T&gt; instance containing the element if present.</returns>
    public static async Task<Option<TSource>> SingleOrNoneAsync<TSource>(
        this IAsyncEnumerable<TSource> source,
        CancellationToken token = default,
        bool continueOnCapturedContext = false)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        await using var enumerator = source.GetAsyncEnumerator(token);
        var hasOne = await enumerator.MoveNextAsync().ConfigureAwait(continueOnCapturedContext);
        if (!hasOne)
        {
            return Option.None<TSource>();
        }

        var potentialResult = enumerator.Current;
        var hasTwo = await enumerator.MoveNextAsync().ConfigureAwait(continueOnCapturedContext);
        return hasTwo
            ? Option.None<TSource>()
            : potentialResult.Some();
    }

    /// <summary>
    ///     Returns an element at a specified position in a sequence if such exists.
    /// </summary>
    /// <param name="source">The sequence to return the element from.</param>
    /// <param name="index">The index in the sequence.</param>
    /// <param name="token">The cancellation token.</param>
    /// <param name="continueOnCapturedContext">
    ///     The argument of <see cref="TaskAsyncEnumerableExtensions.ConfigureAwait"/>.
    /// </param>
    /// <returns>An Option&lt;T&gt; instance containing the element if found.</returns>
    public static async Task<Option<TSource>> ElementAtOrNoneAsync<TSource>(
        this IAsyncEnumerable<TSource> source,
        Index index,
        CancellationToken token = default,
        bool continueOnCapturedContext = false)
    {
        //            values: __ 10 20 30 40 50 __
        // start based index: -1  0  1  2  3  4  5
        //   end based index: ^6 ^5 ^4 ^3 ^2 ^1 ^0
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (index.IsFromEnd)
        {
            var capacity = index.Value;
            if (capacity == 0)
            {
                return Option.None<TSource>();
            }

            var buffer = new Queue<TSource>(capacity);
            await foreach (var item in source.WithCancellation(token).ConfigureAwait(continueOnCapturedContext))
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
        await foreach (var item in source.WithCancellation(token).ConfigureAwait(continueOnCapturedContext))
        {
            if (itemIndex == index.Value)
            {
                return item.Some();
            }

            itemIndex++;
        }

        return Option.None<TSource>();
    }

    /// <summary>
    ///     Returns a first element having maximal key from a sequence, if the sequence has elements.
    /// </summary>
    /// <param name="source">The sequence to return the element from.</param>
    /// <param name="key">The function for getting a key from an element.</param>
    /// <param name="order">The order that determines the maximal element.</param>
    /// <param name="token">The cancellation token to pass in <paramref name="source"/> enumeration.</param>
    /// <param name="continueOnCapturedContext">
    ///     The argument of <see cref="TaskAsyncEnumerableExtensions.ConfigureAwait"/>.
    /// </param>
    /// <returns>An Option&lt;T&gt; instance containing the element if present.</returns>
    [Pure]
    public static async Task<Option<TSource>> MaxByOrNoneAsync<TSource, TKey>(
        [InstantHandle(RequireAwait = true)] this IAsyncEnumerable<TSource> source,
        [InstantHandle(RequireAwait = true)] Func<TSource, TKey> key,
        IComparer<TKey> order,
        CancellationToken token = default,
        bool continueOnCapturedContext = false)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        if (order == null)
        {
            throw new ArgumentNullException(nameof(order));
        }

        var max = Option.None<(TKey, TSource)>();
        await foreach (var currentItem in source.WithCancellation(token).ConfigureAwait(continueOnCapturedContext))
        {
            var currentKey = key(currentItem);
            if (max.HasValue)
            {
                var maxKey = max.Value.Item1;

                max = order.Compare(maxKey, currentKey) >= 0
                    ? max
                    : (currentKey, currentItem).Some();
            }
            else
            {
                max = (currentKey, currentItem).Some();
            }
        }

        return max.Map(pair => pair.Item2);
    }

    /// <summary>
    ///     Returns a first maximal element from a sequence, if the sequence has elements.
    /// </summary>
    /// <param name="source">The sequence to return the element from.</param>
    /// <param name="key">The function for getting a key from an element.</param>
    /// <param name="token">The cancellation token to pass in <paramref name="source"/> enumeration.</param>
    /// <param name="continueOnCapturedContext">
    ///     The argument of <see cref="TaskAsyncEnumerableExtensions.ConfigureAwait"/>.
    /// </param>
    /// <returns>An Option&lt;T&gt; instance containing the element if present.</returns>
    [Pure]
    public static Task<Option<TSource>> MaxByOrNoneAsync<TSource, TKey>(
        [InstantHandle(RequireAwait = true)] this IAsyncEnumerable<TSource> source,
        [InstantHandle(RequireAwait = true)] Func<TSource, TKey> key,
        CancellationToken token = default,
        bool continueOnCapturedContext = false)
        => source.MaxByOrNoneAsync(
            key,
            Comparer<TKey>.Default,
            token,
            continueOnCapturedContext
        );

    /// <summary>
    ///     Returns a first minimal element from a sequence, if the sequence has elements.
    /// </summary>
    /// <param name="source">The sequence to return the element from.</param>
    /// <param name="key">The function for getting a key from an element.</param>
    /// <param name="order">The order that determines the minimal element.</param>
    /// <param name="token">The cancellation token to pass in <paramref name="source"/> enumeration.</param>
    /// <param name="continueOnCapturedContext">
    ///     The argument of <see cref="TaskAsyncEnumerableExtensions.ConfigureAwait"/>.
    /// </param>
    /// <returns>An Option&lt;T&gt; instance containing the element if present.</returns>
    [Pure]
    public static Task<Option<TSource>> MinByOrNoneAsync<TSource, TKey>(
        [InstantHandle(RequireAwait = true)] this IAsyncEnumerable<TSource> source,
        [InstantHandle(RequireAwait = true)] Func<TSource, TKey> key,
        IComparer<TKey> order,
        CancellationToken token = default,
        bool continueOnCapturedContext = false)
    {
        if (order == null)
        {
            throw new ArgumentNullException(nameof(order));
        }

        return source.MaxByOrNoneAsync(
            key,
            new InvertedOrder<TKey>(order),
            token,
            continueOnCapturedContext
        );
    }

    /// <summary>
    ///     Returns a first minimal element from a sequence, if the sequence has elements.
    /// </summary>
    /// <param name="source">The sequence to return the element from.</param>
    /// <param name="key">The function for getting a key from an element.</param>
    /// <param name="token">The cancellation token to pass in <paramref name="source"/> enumeration.</param>
    /// <param name="continueOnCapturedContext">
    ///     The argument of <see cref="TaskAsyncEnumerableExtensions.ConfigureAwait"/>.
    /// </param>
    /// <returns>An Option&lt;T&gt; instance containing the element if present.</returns>
    [Pure]
    public static Task<Option<TSource>> MinByOrNoneAsync<TSource, TKey>(
        [InstantHandle(RequireAwait = true)] this IAsyncEnumerable<TSource> source,
        [InstantHandle(RequireAwait = true)] Func<TSource, TKey> key,
        CancellationToken token = default,
        bool continueOnCapturedContext = false)
        => source.MaxByOrNoneAsync(
            key,
            InvertedDefaultOrder<TKey>.Singleton,
            token,
            continueOnCapturedContext
        );

    /// <summary>
    ///     Returns a first maximal element from a sequence, if the sequence has elements.
    /// </summary>
    /// <param name="source">The sequence to return the element from.</param>
    /// <param name="order">The order that determines the maximal element.</param>
    /// <param name="token">The cancellation token to pass in <paramref name="source"/> enumeration.</param>
    /// <param name="continueOnCapturedContext">
    ///     The argument of <see cref="TaskAsyncEnumerableExtensions.ConfigureAwait"/>.
    /// </param>
    /// <returns>An Option&lt;T&gt; instance containing the element if present.</returns>
    [Pure]
    public static Task<Option<TSource>> MaxOrNoneAsync<TSource>(
        [InstantHandle(RequireAwait = true)] this IAsyncEnumerable<TSource> source,
        IComparer<TSource> order,
        CancellationToken token = default,
        bool continueOnCapturedContext = false)
        => source.MaxByOrNoneAsync(
            x => x,
            order,
            token,
            continueOnCapturedContext
        );

    /// <summary>
    ///     Returns a first maximal element from a sequence, if the sequence has elements.
    /// </summary>
    /// <param name="source">The sequence to return the element from.</param>
    /// <param name="token">The cancellation token to pass in <paramref name="source"/> enumeration.</param>
    /// <param name="continueOnCapturedContext">
    ///     The argument of <see cref="TaskAsyncEnumerableExtensions.ConfigureAwait"/>.
    /// </param>
    /// <returns>An Option&lt;T&gt; instance containing the element if present.</returns>
    [Pure]
    public static Task<Option<TSource>> MaxOrNoneAsync<TSource>(
        [InstantHandle(RequireAwait = true)] this IAsyncEnumerable<TSource> source,
        CancellationToken token = default,
        bool continueOnCapturedContext = false)
        => source.MaxOrNoneAsync(
            Comparer<TSource>.Default,
            token,
            continueOnCapturedContext
        );

    /// <summary>
    ///     Returns a first minimal element from a sequence, if the sequence has elements.
    /// </summary>
    /// <param name="source">The sequence to return the element from.</param>
    /// <param name="order">The order that determines the minimal element.</param>
    /// <param name="token">The cancellation token to pass in <paramref name="source"/> enumeration.</param>
    /// <param name="continueOnCapturedContext">
    ///     The argument of <see cref="TaskAsyncEnumerableExtensions.ConfigureAwait"/>.
    /// </param>
    /// <returns>An Option&lt;T&gt; instance containing the element if present.</returns>
    [Pure]
    public static Task<Option<TSource>> MinOrNoneAsync<TSource>(
        [InstantHandle(RequireAwait = true)] this IAsyncEnumerable<TSource> source,
        IComparer<TSource> order,
        CancellationToken token = default,
        bool continueOnCapturedContext = false)
    {
        if (order == null)
        {
            throw new ArgumentNullException(nameof(order));
        }

        return source.MaxOrNoneAsync(
            new InvertedOrder<TSource>(order),
            token,
            continueOnCapturedContext
        );
    }

    /// <summary>
    ///     Returns a first minimal element from a sequence, if the sequence has elements.
    /// </summary>
    /// <param name="source">The sequence to return the element from.</param>
    /// <param name="token">The cancellation token to pass in <paramref name="source"/> enumeration.</param>
    /// <param name="continueOnCapturedContext">
    ///     The argument of <see cref="TaskAsyncEnumerableExtensions.ConfigureAwait"/>.
    /// </param>
    /// <returns>An Option&lt;T&gt; instance containing the element if present.</returns>
    [Pure]
    public static Task<Option<TSource>> MinOrNoneAsync<TSource>(
        [InstantHandle(RequireAwait = true)] this IAsyncEnumerable<TSource> source,
        CancellationToken token = default,
        bool continueOnCapturedContext = false)
        => source.MaxOrNoneAsync(
            InvertedDefaultOrder<TSource>.Singleton,
            token,
            continueOnCapturedContext
        );
}
#endif