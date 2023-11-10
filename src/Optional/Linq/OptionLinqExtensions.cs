namespace Optional.Linq;

public static class OptionLinqExtensions
{
    [Pure]
    public static Option<TResult> Select<TSource, TResult>(
        this Option<TSource> source,
        [InstantHandle] Func<TSource, TResult> selector)
    {
        if (selector == null)
        {
            throw new ArgumentNullException(nameof(selector));
        }

        return source.Map(selector);
    }

    [Pure]
    public static Option<TResult> SelectMany<TSource, TResult>(
        this Option<TSource> source,
        [InstantHandle] Func<TSource, Option<TResult>> selector)
    {
        if (selector == null)
        {
            throw new ArgumentNullException(nameof(selector));
        }

        return source.FlatMap(selector);
    }

    [Pure]
    public static Option<TResult> SelectMany<TSource, TCollection, TResult>(
        this Option<TSource> source,
        [InstantHandle] Func<TSource, Option<TCollection>> collectionSelector,
        [InstantHandle] Func<TSource, TCollection, TResult> resultSelector)
    {
        if (collectionSelector == null)
        {
            throw new ArgumentNullException(nameof(collectionSelector));
        }

        if (resultSelector == null)
        {
            throw new ArgumentNullException(nameof(resultSelector));
        }

        return source.FlatMap(src => collectionSelector(src).Map(elem => resultSelector(src, elem)));
    }

    [Pure]
    public static Option<TSource> Where<TSource>(
        this Option<TSource> source,
        [InstantHandle] Func<TSource, bool> predicate)
    {
        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        return source.Filter(predicate);
    }

    [Pure]
    public static Option<TResult, TException> Select<TSource, TException, TResult>(
        this Option<TSource, TException> source,
        [InstantHandle] Func<TSource, TResult> selector)
    {
        if (selector == null)
        {
            throw new ArgumentNullException(nameof(selector));
        }

        return source.Map(selector);
    }

    [Pure]
    public static Option<TResult, TException> SelectMany<TSource, TException, TResult>(
        this Option<TSource, TException> source,
        [InstantHandle] Func<TSource, Option<TResult, TException>> selector)
    {
        if (selector == null)
        {
            throw new ArgumentNullException(nameof(selector));
        }

        return source.FlatMap(selector);
    }

    [Pure]
    public static Option<TResult, TException> SelectMany<TSource, TException, TCollection, TResult>(
        this Option<TSource, TException> source,
        [InstantHandle] Func<TSource, Option<TCollection, TException>> collectionSelector,
        [InstantHandle] Func<TSource, TCollection, TResult> resultSelector)
    {
        if (collectionSelector == null)
        {
            throw new ArgumentNullException(nameof(collectionSelector));
        }

        if (resultSelector == null)
        {
            throw new ArgumentNullException(nameof(resultSelector));
        }

        return source.FlatMap(src => collectionSelector(src).Map(elem => resultSelector(src, elem)));
    }

#if NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
    [Pure]
    public static Option<TSource, TException> Where<TSource, TException>(
        this Option<TSource, TException> source,
        [InstantHandle] Func<TSource, (bool Match, TException Exception)> predicate)
    {
        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        if (!source.HasValue)
        {
            return source;
        }

        var (match, exception) = predicate(source.Value);
        return match
            ? source
            : Option.None<TSource, TException>(exception);
    }
#endif

    [Pure]
    public static Option<TSource, TException> Where<TSource, TException>(
        this Option<TSource, TException> source,
        [InstantHandle] Func<TSource, Option<TException>> invertedPredicate)
    {
        if (invertedPredicate == null)
        {
            throw new ArgumentNullException(nameof(invertedPredicate));
        }

        if (!source.HasValue)
        {
            return source;
        }

        var exception = invertedPredicate(source.Value);
        return exception.HasValue
            ? Option.None<TSource, TException>(exception.Value)
            : source;
    }
}