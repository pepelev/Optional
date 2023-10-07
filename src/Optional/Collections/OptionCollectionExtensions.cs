// Note: Several of the below implementations are closely inspired by the corefx source code for FirstOrDefault, etc.

using System.Linq;
using Optional.Order;

namespace Optional.Collections;

public static class OptionCollectionExtensions
{
    /// <summary>
    ///     Flattens a sequence of optionals into a sequence containing all inner values.
    ///     Empty elements are discarded.
    /// </summary>
    /// <param name="source">The sequence of optionals.</param>
    /// <returns>A flattened sequence of values.</returns>
    [Pure]
    [LinqTunnel]
    public static IEnumerable<T> Values<T>(this IEnumerable<Option<T>> source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        foreach (var option in source)
        {
            if (option.HasValue)
            {
                yield return option.Value;
            }
        }
    }

    /// <summary>
    ///     Flattens a sequence of optionals into a sequence containing all inner values.
    ///     Empty elements and their exceptional values are discarded.
    /// </summary>
    /// <param name="source">The sequence of optionals.</param>
    /// <returns>A flattened sequence of values.</returns>
    [Pure]
    [LinqTunnel]
    public static IEnumerable<T> Values<T, TException>(this IEnumerable<Option<T, TException>> source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        foreach (var option in source)
        {
            if (option.HasValue)
            {
                yield return option.Value;
            }
        }
    }

    /// <summary>
    ///     Flattens a sequence of optionals into a sequence containing all exceptional values.
    ///     Non-empty elements and their values are discarded.
    /// </summary>
    /// <param name="source">The sequence of optionals.</param>
    /// <returns>A flattened sequence of exceptional values.</returns>
    [Pure]
    [LinqTunnel]
    public static IEnumerable<TException> Exceptions<T, TException>(this IEnumerable<Option<T, TException>> source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        foreach (var option in source)
        {
            if (!option.HasValue)
            {
                yield return option.Exception;
            }
        }
    }

    /// <summary>
    ///     Returns the value associated with the specified key if such exists.
    ///     A dictionary lookup will be used if available, otherwise falling
    ///     back to a linear scan of the enumerable.
    /// </summary>
    /// <param name="source">The dictionary or enumerable in which to locate the key.</param>
    /// <param name="key">The key to locate.</param>
    /// <returns>An Option&lt;TValue&gt; instance containing the associated value if located.</returns>
    [Pure]
    public static Option<TValue> GetValueOrNone<TKey, TValue>(
        [InstantHandle] this IEnumerable<KeyValuePair<TKey, TValue>> source,
        TKey key)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (source is IDictionary<TKey, TValue> dictionary)
        {
            return dictionary.TryGetValue(key, out var value)
                ? value.Some()
                : value.None();
        }
#if NET45_OR_GREATER || NETSTANDARD1_0_OR_GREATER || NETCOREAPP1_0_OR_GREATER
        if (source is IReadOnlyDictionary<TKey, TValue> readOnlyDictionary)
        {
            return readOnlyDictionary.TryGetValue(key, out var value)
                ? value.Some()
                : value.None();
        }
#endif

        return source
            .FirstOrNone(pair => EqualityComparer<TKey>.Default.Equals(pair.Key, key))
            .Map(pair => pair.Value);
    }

    /// <summary>
    ///     Returns the first element of a sequence if such exists.
    /// </summary>
    /// <param name="source">The sequence to return the first element from.</param>
    /// <returns>An Option&lt;T&gt; instance containing the first element if present.</returns>
    [Pure]
    public static Option<TSource> FirstOrNone<TSource>([InstantHandle] this IEnumerable<TSource> source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (source is IList<TSource> list)
        {
            if (list.Count > 0)
            {
                return list[0].Some();
            }
        }
#if NET45_OR_GREATER || NETSTANDARD1_0_OR_GREATER || NETCOREAPP1_0_OR_GREATER
        else if (source is IReadOnlyList<TSource> readOnlyList)
        {
            if (readOnlyList.Count > 0)
            {
                return readOnlyList[0].Some();
            }
        }
#endif
        else
        {
            using var enumerator = source.GetEnumerator();
            if (enumerator.MoveNext())
            {
                return enumerator.Current.Some();
            }
        }

        return Option.None<TSource>();
    }

    /// <summary>
    ///     Returns the first element of a sequence, satisfying a specified predicate,
    ///     if such exists.
    /// </summary>
    /// <param name="source">The sequence to return the first element from.</param>
    /// <param name="predicate">The predicate to filter by.</param>
    /// <returns>An Option&lt;T&gt; instance containing the first element if present.</returns>
    [Pure]
    public static Option<TSource> FirstOrNone<TSource>(
        [InstantHandle] this IEnumerable<TSource> source,
        [InstantHandle] Func<TSource, bool> predicate)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        foreach (var element in source)
        {
            if (predicate(element))
            {
                return element.Some();
            }
        }

        return Option.None<TSource>();
    }

    /// <summary>
    ///     Returns the last element of a sequence if such exists.
    /// </summary>
    /// <param name="source">The sequence to return the last element from.</param>
    /// <returns>An Option&lt;T&gt; instance containing the last element if present.</returns>
    [Pure]
    [LinqTunnel]
    public static Option<TSource> LastOrNone<TSource>([InstantHandle] this IEnumerable<TSource> source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (source is IList<TSource> list)
        {
            var count = list.Count;
            if (count > 0)
            {
                return list[count - 1].Some();
            }
        }
#if NET45_OR_GREATER || NETSTANDARD1_0_OR_GREATER || NETCOREAPP1_0_OR_GREATER
        else if (source is IReadOnlyList<TSource> readOnlyList)
        {
            var count = readOnlyList.Count;
            if (count > 0)
            {
                return readOnlyList[count - 1].Some();
            }
        }
#endif
        else
        {
            using var enumerator = source.GetEnumerator();
            if (enumerator.MoveNext())
            {
                TSource result;
                do
                {
                    result = enumerator.Current;
                } while (enumerator.MoveNext());

                return result.Some();
            }
        }

        return Option.None<TSource>();
    }

    /// <summary>
    ///     Returns the last element of a sequence, satisfying a specified predicate,
    ///     if such exists.
    /// </summary>
    /// <param name="source">The sequence to return the last element from.</param>
    /// <param name="predicate">The predicate to filter by.</param>
    /// <returns>An Option&lt;T&gt; instance containing the last element if present.</returns>
    [Pure]
    public static Option<TSource> LastOrNone<TSource>(
        [InstantHandle] this IEnumerable<TSource> source,
        [InstantHandle] Func<TSource, bool> predicate)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        if (source is IList<TSource> list)
        {
            for (var i = list.Count - 1; i >= 0; --i)
            {
                var result = list[i];
                if (predicate(result))
                {
                    return result.Some();
                }
            }
        }
#if NET45_OR_GREATER || NETSTANDARD1_0_OR_GREATER || NETCOREAPP1_0_OR_GREATER
        else if (source is IReadOnlyList<TSource> readOnlyList)
        {
            for (var i = readOnlyList.Count - 1; i >= 0; --i)
            {
                var result = readOnlyList[i];
                if (predicate(result))
                {
                    return result.Some();
                }
            }
        }
#endif
        else
        {
            using var enumerator = source.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var result = enumerator.Current;
                if (predicate(result))
                {
                    while (enumerator.MoveNext())
                    {
                        var element = enumerator.Current;
                        if (predicate(element))
                        {
                            result = element;
                        }
                    }

                    return result.Some();
                }
            }
        }

        return Option.None<TSource>();
    }

    /// <summary>
    ///     Returns a single element from a sequence, if it exists
    ///     and is the only element in the sequence.
    /// </summary>
    /// <param name="source">The sequence to return the element from.</param>
    /// <returns>An Option&lt;T&gt; instance containing the element if present.</returns>
    [Pure]
    public static Option<TSource> SingleOrNone<TSource>([InstantHandle] this IEnumerable<TSource> source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (source is IList<TSource> list)
        {
            switch (list.Count)
            {
                case 0: return Option.None<TSource>();
                case 1: return list[0].Some();
            }
        }
#if NET45_OR_GREATER || NETSTANDARD1_0_OR_GREATER || NETCOREAPP1_0_OR_GREATER
        else if (source is IReadOnlyList<TSource> readOnlyList)
        {
            switch (readOnlyList.Count)
            {
                case 0: return Option.None<TSource>();
                case 1: return readOnlyList[0].Some();
            }
        }
#endif
        else
        {
            using var enumerator = source.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                return Option.None<TSource>();
            }

            var result = enumerator.Current;
            if (!enumerator.MoveNext())
            {
                return result.Some();
            }
        }

        return Option.None<TSource>();
    }

    /// <summary>
    ///     Returns a single element from a sequence, satisfying a specified predicate,
    ///     if it exists and is the only element in the sequence.
    /// </summary>
    /// <param name="source">The sequence to return the element from.</param>
    /// <param name="predicate">The predicate to filter by.</param>
    /// <returns>An Option&lt;T&gt; instance containing the element if present.</returns>
    [Pure]
    public static Option<TSource> SingleOrNone<TSource>(
        [InstantHandle] this IEnumerable<TSource> source,
        [InstantHandle] Func<TSource, bool> predicate)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        using (var enumerator = source.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                var result = enumerator.Current;
                if (predicate(result))
                {
                    while (enumerator.MoveNext())
                    {
                        if (predicate(enumerator.Current))
                        {
                            return Option.None<TSource>();
                        }
                    }

                    return result.Some();
                }
            }
        }

        return Option.None<TSource>();
    }

    /// <summary>
    ///     Returns an element at a specified position in a sequence if such exists.
    /// </summary>
    /// <param name="source">The sequence to return the element from.</param>
    /// <param name="index">The index in the sequence.</param>
    /// <returns>An Option&lt;T&gt; instance containing the element if found.</returns>
    public static Option<TSource> ElementAtOrNone<TSource>([InstantHandle] this IEnumerable<TSource> source, int index)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (index < 0)
        {
            return Option.None<TSource>();
        }

        if (source.GetType() == typeof(List<TSource>))
        {
            var list = (List<TSource>)source;
            return index < list.Count
                ? list[index].Some()
                : Option.None<TSource>();
        }

#if NET35_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        if (typeof(TSource).IsValueType && source is TSource[] array)
        {
            return index < array.Length
                ? array[index].Some()
                : Option.None<TSource>();
        }
#endif

        if (source is IList<TSource> iList)
        {
            return index < iList.Count
                ? iList[index].Some()
                : Option.None<TSource>();
        }

        foreach (var item in source)
        {
            if (index == 0)
            {
                return item.Some();
            }

            index--;
        }

        return Option.None<TSource>();
    }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
    /// <summary>
    ///     Returns an element at a specified position in a sequence if such exists.
    /// </summary>
    /// <param name="source">The sequence to return the element from.</param>
    /// <param name="index">The index in the sequence.</param>
    /// <returns>An Option&lt;T&gt; instance containing the element if found.</returns>
    public static Option<TSource> ElementAtOrNone<TSource>(
        [InstantHandle] this IEnumerable<TSource> source,
        Index index)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (source.GetType() == typeof(List<TSource>))
        {
            var list = (List<TSource>)source;
            var count = list.Count;
            var offset = index.GetOffset(count);
            return 0 <= offset && offset < count
                ? list[offset].Some()
                : Option.None<TSource>();
        }

        if (typeof(TSource).IsValueType && source is TSource[] array)
        {
            var count = array.Length;
            var offset = index.GetOffset(count);
            return 0 <= offset && offset < count
                ? array[offset].Some()
                : Option.None<TSource>();
        }

        if (source is IList<TSource> IList)
        {
            var count = IList.Count;
            var offset = index.GetOffset(count);
            return 0 <= offset && offset < count
                ? IList[offset].Some()
                : Option.None<TSource>();
        }

        if (index.IsFromEnd)
        {
#if NET6_0_OR_GREATER
            if (source.TryGetNonEnumeratedCount(out var actualCount))
            {
                var offset = index.GetOffset(actualCount);
                return Get(offset);
            }
#endif
            var capacity = index.Value;
            if (capacity == 0)
            {
                return Option.None<TSource>();
            }

            var buffer = new Queue<TSource>(capacity);
            foreach (var item in source)
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

        return Get(index.Value);

        Option<TSource> Get(int targetIndex)
        {
            if (targetIndex < 0)
            {
                return Option.None<TSource>();
            }

            var itemIndex = 0;
            foreach (var item in source)
            {
                if (itemIndex == targetIndex)
                {
                    return item.Some();
                }

                itemIndex++;
            }

            return Option.None<TSource>();
        }
    }
#endif

    /// <summary>
    ///     Returns a first element having maximal key from a sequence, if the sequence has elements.
    /// </summary>
    /// <param name="source">The sequence to return the element from.</param>
    /// <param name="key">The function for getting a key from an element.</param>
    /// <param name="order">The order that determines the maximal element.</param>
    /// <returns>An Option&lt;T&gt; instance containing the element if present.</returns>
    [Pure]
    public static Option<TSource> MaxByOrNone<TSource, TKey>(
        [InstantHandle] this IEnumerable<TSource> source,
        [InstantHandle] Func<TSource, TKey> key,
        IComparer<TKey> order)
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
        foreach (var currentItem in source)
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
    /// <returns>An Option&lt;T&gt; instance containing the element if present.</returns>
    [Pure]
    public static Option<TSource> MaxByOrNone<TSource, TKey>(
        [InstantHandle] this IEnumerable<TSource> source,
        [InstantHandle] Func<TSource, TKey> key)
        => source.MaxByOrNone(key, Comparer<TKey>.Default);

    /// <summary>
    ///     Returns a first minimal element from a sequence, if the sequence has elements.
    /// </summary>
    /// <param name="source">The sequence to return the element from.</param>
    /// <param name="key">The function for getting a key from an element.</param>
    /// <param name="order">The order that determines the minimal element.</param>
    /// <returns>An Option&lt;T&gt; instance containing the element if present.</returns>
    [Pure]
    public static Option<TSource> MinByOrNone<TSource, TKey>(
        [InstantHandle] this IEnumerable<TSource> source,
        [InstantHandle] Func<TSource, TKey> key,
        IComparer<TKey> order)
    {
        if (order == null)
        {
            throw new ArgumentNullException(nameof(order));
        }

        return source.MaxByOrNone(key, new InvertedOrder<TKey>(order));
    }

    /// <summary>
    ///     Returns a first minimal element from a sequence, if the sequence has elements.
    /// </summary>
    /// <param name="source">The sequence to return the element from.</param>
    /// <param name="key">The function for getting a key from an element.</param>
    /// <returns>An Option&lt;T&gt; instance containing the element if present.</returns>
    [Pure]
    public static Option<TSource> MinByOrNone<TSource, TKey>(
        [InstantHandle] this IEnumerable<TSource> source,
        [InstantHandle] Func<TSource, TKey> key)
        => source.MaxByOrNone(key, InvertedDefaultOrder<TKey>.Singleton);

    /// <summary>
    ///     Returns a first maximal element from a sequence, if the sequence has elements.
    /// </summary>
    /// <param name="source">The sequence to return the element from.</param>
    /// <param name="order">The order that determines the maximal element.</param>
    /// <returns>An Option&lt;T&gt; instance containing the element if present.</returns>
    [Pure]
    public static Option<TSource> MaxOrNone<TSource>(
        [InstantHandle] this IEnumerable<TSource> source,
        IComparer<TSource> order)
        => source.MaxByOrNone(x => x, order);

    /// <summary>
    ///     Returns a first maximal element from a sequence, if the sequence has elements.
    /// </summary>
    /// <param name="source">The sequence to return the element from.</param>
    /// <returns>An Option&lt;T&gt; instance containing the element if present.</returns>
    [Pure]
    public static Option<TSource> MaxOrNone<TSource>([InstantHandle] this IEnumerable<TSource> source)
        => source.MaxOrNone(Comparer<TSource>.Default);

    /// <summary>
    ///     Returns a first minimal element from a sequence, if the sequence has elements.
    /// </summary>
    /// <param name="source">The sequence to return the element from.</param>
    /// <param name="order">The order that determines the minimal element.</param>
    /// <returns>An Option&lt;T&gt; instance containing the element if present.</returns>
    [Pure]
    public static Option<TSource> MinOrNone<TSource>(
        [InstantHandle] this IEnumerable<TSource> source,
        IComparer<TSource> order)
    {
        if (order == null)
        {
            throw new ArgumentNullException(nameof(order));
        }

        return source.MaxOrNone(new InvertedOrder<TSource>(order));
    }

    /// <summary>
    ///     Returns a first minimal element from a sequence, if the sequence has elements.
    /// </summary>
    /// <param name="source">The sequence to return the element from.</param>
    /// <returns>An Option&lt;T&gt; instance containing the element if present.</returns>
    [Pure]
    public static Option<TSource> MinOrNone<TSource>([InstantHandle] this IEnumerable<TSource> source)
        => source.MaxOrNone(InvertedDefaultOrder<TSource>.Singleton);
}