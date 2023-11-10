namespace Optional;

/// <summary>
///     Represents an optional value, along with a potential exceptional value.
/// </summary>
/// <typeparam name="T">The type of the value to be wrapped.</typeparam>
/// <typeparam name="TException">A exceptional value describing the lack of an actual value.</typeparam>
#if NET20_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP1_0_OR_GREATER
[Serializable]
#endif
public readonly struct Option<T, TException> : IEquatable<Option<T, TException>>, IComparable<Option<T, TException>>
{
    private readonly T value;
    private readonly TException exception;

    /// <summary>
    ///     Checks if a value is present.
    /// </summary>
    public bool HasValue { get; }

    internal T Value => value;
    internal TException Exception => exception;

    internal Option(T value, TException exception, bool hasValue)
    {
        this.value = value;
        HasValue = hasValue;
        this.exception = exception;
    }

    /// <summary>
    ///     Determines whether two optionals are equal.
    /// </summary>
    /// <param name="other">The optional to compare with the current one.</param>
    /// <returns>A boolean indicating whether or not the optionals are equal.</returns>
    [Pure]
    public bool Equals(Option<T, TException> other)
    {
        if (!HasValue && !other.HasValue)
        {
            return EqualityComparer<TException>.Default.Equals(exception, other.exception);
        }

        if (HasValue && other.HasValue)
        {
            return EqualityComparer<T>.Default.Equals(value, other.value);
        }

        return false;
    }

    /// <summary>
    ///     Determines whether two optionals are equal.
    /// </summary>
    /// <param name="obj">The optional to compare with the current one.</param>
    /// <returns>A boolean indicating whether or not the optionals are equal.</returns>
    [Pure]
    public override bool Equals(object? obj) => obj is Option<T, TException> option && Equals(option);

    /// <summary>
    ///     Determines whether two optionals are equal.
    /// </summary>
    /// <param name="left">The first optional to compare.</param>
    /// <param name="right">The second optional to compare.</param>
    /// <returns>A boolean indicating whether or not the optionals are equal.</returns>
    public static bool operator ==(Option<T, TException> left, Option<T, TException> right) => left.Equals(right);

    /// <summary>
    ///     Determines whether two optionals are unequal.
    /// </summary>
    /// <param name="left">The first optional to compare.</param>
    /// <param name="right">The second optional to compare.</param>
    /// <returns>A boolean indicating whether or not the optionals are unequal.</returns>
    public static bool operator !=(Option<T, TException> left, Option<T, TException> right) => !left.Equals(right);

    /// <summary>
    ///     Generates a hash code for the current optional.
    /// </summary>
    /// <returns>A hash code for the current optional.</returns>
    [Pure]
    public override int GetHashCode() => HasValue
        ? EqualityComparer<T>.Default.GetHashCode(value)
        : EqualityComparer<TException>.Default.GetHashCode(exception) ^ 7;

    /// <summary>
    ///     Compares the relative order of two optionals. An empty optional is
    ///     ordered by its exceptional value and always before a non-empty one.
    /// </summary>
    /// <param name="other">The optional to compare with the current one.</param>
    /// <returns>An integer indicating the relative order of the optionals being compared.</returns>
    [Pure]
    public int CompareTo(Option<T, TException> other)
    {
        if (HasValue && !other.HasValue)
        {
            return 1;
        }

        if (!HasValue && other.HasValue)
        {
            return -1;
        }

        return HasValue
            ? Comparer<T>.Default.Compare(value, other.value)
            : Comparer<TException>.Default.Compare(exception, other.exception);
    }

    /// <summary>
    ///     Determines if an optional is less than another optional.
    /// </summary>
    /// <param name="left">The first optional to compare.</param>
    /// <param name="right">The second optional to compare.</param>
    /// <returns>A boolean indicating whether or not the left optional is less than the right optional.</returns>
    public static bool operator <(Option<T, TException> left, Option<T, TException> right) => left.CompareTo(right) < 0;

    /// <summary>
    ///     Determines if an optional is less than or equal to another optional.
    /// </summary>
    /// <param name="left">The first optional to compare.</param>
    /// <param name="right">The second optional to compare.</param>
    /// <returns>A boolean indicating whether or not the left optional is less than or equal the right optional.</returns>
    public static bool operator <=(Option<T, TException> left, Option<T, TException> right) => left.CompareTo(right) <= 0;

    /// <summary>
    ///     Determines if an optional is greater than another optional.
    /// </summary>
    /// <param name="left">The first optional to compare.</param>
    /// <param name="right">The second optional to compare.</param>
    /// <returns>A boolean indicating whether or not the left optional is greater than the right optional.</returns>
    public static bool operator >(Option<T, TException> left, Option<T, TException> right) => left.CompareTo(right) > 0;

    /// <summary>
    ///     Determines if an optional is greater than or equal to another optional.
    /// </summary>
    /// <param name="left">The first optional to compare.</param>
    /// <param name="right">The second optional to compare.</param>
    /// <returns>A boolean indicating whether or not the left optional is greater than or equal the right optional.</returns>
    public static bool operator >=(Option<T, TException> left, Option<T, TException> right) => left.CompareTo(right) >= 0;

    /// <summary>
    ///     Returns a string that represents the current optional.
    /// </summary>
    /// <returns>A string that represents the current optional.</returns>
    [Pure]
    public override string ToString()
    {
        if (HasValue)
        {
            return value == null
                ? "Some(null)"
                : $"Some({value})";
        }

        return exception == null
            ? "None(null)"
            : $"None({exception})";
    }

    /// <summary>
    ///     Converts the current optional into an enumerable with one or zero elements.
    /// </summary>
    /// <returns>A corresponding enumerable.</returns>
    [Pure]
    public IEnumerable<T> ToEnumerable()
    {
        if (HasValue)
        {
            yield return value;
        }
    }

    /// <summary>
    ///     Returns an enumerator for the optional.
    /// </summary>
    /// <returns>The enumerator.</returns>
    [Pure]
    public IEnumerator<T> GetEnumerator()
    {
        if (HasValue)
        {
            yield return value;
        }
    }

    /// <summary>
    ///     Determines if the current optional contains a specified value.
    /// </summary>
    /// <param name="expectation">The value to locate.</param>
    /// <returns>A boolean indicating whether or not the value was found.</returns>
    [Pure]
    public bool Contains(T expectation) => HasValue && EqualityComparer<T>.Default.Equals(value, expectation);

    /// <summary>
    ///     Determines if the current optional contains a value
    ///     satisfying a specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <returns>A boolean indicating whether or not the predicate was satisfied.</returns>
    [Pure]
    public bool Exists([InstantHandle] Func<T, bool> predicate)
    {
        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        return HasValue && predicate(value);
    }

    /// <summary>
    ///     Returns the existing value if present, and otherwise an alternative value.
    /// </summary>
    /// <param name="alternative">The alternative value.</param>
    /// <returns>The existing or alternative value.</returns>
    [Pure]
    public T ValueOr(T alternative) => HasValue
        ? value
        : alternative;

    /// <summary>
    ///     Returns the existing value if present, and otherwise an alternative value.
    /// </summary>
    /// <param name="alternativeFactory">A factory function to create an alternative value.</param>
    /// <returns>The existing or alternative value.</returns>
    [Pure]
    public T ValueOr([InstantHandle] Func<T> alternativeFactory)
    {
        if (alternativeFactory == null)
        {
            throw new ArgumentNullException(nameof(alternativeFactory));
        }

        return HasValue
            ? value
            : alternativeFactory();
    }

    /// <summary>
    ///     Returns the existing value if present, and otherwise an alternative value.
    /// </summary>
    /// <param name="alternativeFactory">A factory function to map the exceptional value to an alternative value.</param>
    /// <returns>The existing or alternative value.</returns>
    [Pure]
    public T ValueOr([InstantHandle] Func<TException, T> alternativeFactory)
    {
        if (alternativeFactory == null)
        {
            throw new ArgumentNullException(nameof(alternativeFactory));
        }

        return HasValue
            ? value
            : alternativeFactory(exception);
    }

    /// <summary>
    ///     Uses an alternative value, if no existing value is present.
    /// </summary>
    /// <param name="alternative">The alternative value.</param>
    /// <returns>A new optional, containing either the existing or alternative value.</returns>
    [Pure]
    public Option<T, TException> Or(T alternative) => HasValue
        ? this
        : Option.Some<T, TException>(alternative);

    /// <summary>
    ///     Uses an alternative value, if no existing value is present.
    /// </summary>
    /// <param name="alternativeFactory">A factory function to create an alternative value.</param>
    /// <returns>A new optional, containing either the existing or alternative value.</returns>
    [Pure]
    public Option<T, TException> Or([InstantHandle] Func<T> alternativeFactory)
    {
        if (alternativeFactory == null)
        {
            throw new ArgumentNullException(nameof(alternativeFactory));
        }

        return HasValue
            ? this
            : Option.Some<T, TException>(alternativeFactory());
    }

    /// <summary>
    ///     Uses an alternative value, if no existing value is present.
    /// </summary>
    /// <param name="alternativeFactory">A factory function to map the exceptional value to an alternative value.</param>
    /// <returns>A new optional, containing either the existing or alternative value.</returns>
    [Pure]
    public Option<T, TException> Or([InstantHandle] Func<TException, T> alternativeFactory)
    {
        if (alternativeFactory == null)
        {
            throw new ArgumentNullException(nameof(alternativeFactory));
        }

        return HasValue
            ? this
            : Option.Some<T, TException>(alternativeFactory(exception));
    }

    /// <summary>
    ///     Uses an alternative optional, if no existing value is present.
    /// </summary>
    /// <param name="alternativeOption">The alternative optional.</param>
    /// <returns>The alternative optional, if no value is present, otherwise itself.</returns>
    [Pure]
    public Option<T, TException> Else([InstantHandle] Option<T, TException> alternativeOption) => HasValue
        ? this
        : alternativeOption;

    /// <summary>
    ///     Uses an alternative optional, if no existing value is present.
    /// </summary>
    /// <param name="alternativeOptionFactory">A factory function to create an alternative optional.</param>
    /// <returns>The alternative optional, if no value is present, otherwise itself.</returns>
    [Pure]
    public Option<T, TException> Else([InstantHandle] Func<Option<T, TException>> alternativeOptionFactory)
    {
        if (alternativeOptionFactory == null)
        {
            throw new ArgumentNullException(nameof(alternativeOptionFactory));
        }

        return HasValue
            ? this
            : alternativeOptionFactory();
    }

    /// <summary>
    ///     Uses an alternative optional, if no existing value is present.
    /// </summary>
    /// <param name="alternativeOptionFactory">A factory function to map the exceptional value to an alternative optional.</param>
    /// <returns>The alternative optional, if no value is present, otherwise itself.</returns>
    [Pure]
    public Option<T, TException> Else([InstantHandle] Func<TException, Option<T, TException>> alternativeOptionFactory)
    {
        if (alternativeOptionFactory == null)
        {
            throw new ArgumentNullException(nameof(alternativeOptionFactory));
        }

        return HasValue
            ? this
            : alternativeOptionFactory(exception);
    }

    /// <summary>
    ///     Forgets any attached exceptional value.
    /// </summary>
    /// <returns>An optional without an exceptional value.</returns>
    [Pure]
    public Option<T> WithoutException()
    {
        return Match(
            Option.Some,
            _ => Option.None<T>()
        );
    }

    /// <summary>
    ///     Evaluates a specified function, based on whether a value is present or not.
    /// </summary>
    /// <param name="some">The function to evaluate if the value is present.</param>
    /// <param name="none">The function to evaluate if the value is missing.</param>
    /// <returns>The result of the evaluated function.</returns>
    [Pure]
    public TResult Match<TResult>([InstantHandle] Func<T, TResult> some, [InstantHandle] Func<TException, TResult> none)
    {
        if (some == null)
        {
            throw new ArgumentNullException(nameof(some));
        }

        if (none == null)
        {
            throw new ArgumentNullException(nameof(none));
        }

        return HasValue
            ? some(value)
            : none(exception);
    }

    /// <summary>
    ///     Evaluates a specified action, based on whether a value is present or not.
    /// </summary>
    /// <param name="some">The action to evaluate if the value is present.</param>
    /// <param name="none">The action to evaluate if the value is missing.</param>
    public void Match([InstantHandle] Action<T> some, [InstantHandle] Action<TException> none)
    {
        if (some == null)
        {
            throw new ArgumentNullException(nameof(some));
        }

        if (none == null)
        {
            throw new ArgumentNullException(nameof(none));
        }

        if (HasValue)
        {
            some(value);
        }
        else
        {
            none(exception);
        }
    }

    /// <summary>
    ///     Evaluates a specified action if a value is present.
    /// </summary>
    /// <param name="some">The action to evaluate if the value is present.</param>
    public void MatchSome([InstantHandle] Action<T> some)
    {
        if (some == null)
        {
            throw new ArgumentNullException(nameof(some));
        }

        if (HasValue)
        {
            some(value);
        }
    }

    /// <summary>
    ///     Evaluates a specified action if no value is present.
    /// </summary>
    /// <param name="none">The action to evaluate if the value is missing.</param>
    public void MatchNone([InstantHandle] Action<TException> none)
    {
        if (none == null)
        {
            throw new ArgumentNullException(nameof(none));
        }

        if (!HasValue)
        {
            none(exception);
        }
    }

    /// <summary>
    ///     Transforms the inner value in an optional.
    ///     If the instance is empty, an empty optional is returned.
    /// </summary>
    /// <param name="mapping">The transformation function.</param>
    /// <returns>The transformed optional.</returns>
    [Pure]
    public Option<TResult, TException> Map<TResult>([InstantHandle] Func<T, TResult> mapping)
    {
        if (mapping == null)
        {
            throw new ArgumentNullException(nameof(mapping));
        }

        return HasValue
            ? Option.Some<TResult, TException>(mapping(value))
            : Option.None<TResult, TException>(exception);
    }

    /// <summary>
    ///     Transforms the exceptional value in an optional.
    ///     If the instance is not empty, no transformation is carried out.
    /// </summary>
    /// <param name="mapping">The transformation function.</param>
    /// <returns>The transformed optional.</returns>
    [Pure]
    public Option<T, TExceptionResult> MapException<TExceptionResult>(
        [InstantHandle] Func<TException, TExceptionResult> mapping)
    {
        if (mapping == null)
        {
            throw new ArgumentNullException(nameof(mapping));
        }

        return HasValue
            ? Option.Some<T, TExceptionResult>(value)
            : Option.None<T, TExceptionResult>(mapping(exception));
    }

    /// <summary>
    ///     Transforms the inner value in an optional
    ///     into another optional. The result is flattened,
    ///     and if either is empty, an empty optional is returned.
    /// </summary>
    /// <param name="mapping">The transformation function.</param>
    /// <returns>The transformed optional.</returns>
    [Pure]
    public Option<TResult, TException> FlatMap<TResult>([InstantHandle] Func<T, Option<TResult, TException>> mapping)
    {
        if (mapping == null)
        {
            throw new ArgumentNullException(nameof(mapping));
        }

        return Match(
            mapping,
            Option.None<TResult, TException>
        );
    }

    /// <summary>
    ///     Transforms the inner value in an optional
    ///     into another optional. The result is flattened,
    ///     and if either is empty, an empty optional is returned,
    ///     with a specified exceptional value.
    /// </summary>
    /// <param name="mapping">The transformation function.</param>
    /// <param name="fallback">The exceptional value to attach.</param>
    /// <returns>The transformed optional.</returns>
    [Pure]
    public Option<TResult, TException> FlatMap<TResult>(
        [InstantHandle] Func<T, Option<TResult>> mapping,
        TException fallback)
    {
        if (mapping == null)
        {
            throw new ArgumentNullException(nameof(mapping));
        }

        return HasValue
            ? mapping(value).WithException(fallback)
            : Option.None<TResult, TException>(exception);
    }

    /// <summary>
    ///     Transforms the inner value in an optional
    ///     into another optional. The result is flattened,
    ///     and if either is empty, an empty optional is returned,
    ///     with a specified exceptional value.
    /// </summary>
    /// <param name="mapping">The transformation function.</param>
    /// <param name="exceptionFactory">A factory function to create an exceptional value to attach.</param>
    /// <returns>The transformed optional.</returns>
    [Pure]
    public Option<TResult, TException> FlatMap<TResult>(
        [InstantHandle] Func<T, Option<TResult>> mapping,
        [InstantHandle] Func<TException> exceptionFactory)
    {
        if (mapping == null)
        {
            throw new ArgumentNullException(nameof(mapping));
        }

        if (exceptionFactory == null)
        {
            throw new ArgumentNullException(nameof(exceptionFactory));
        }

        return HasValue
            ? mapping(value).WithException(exceptionFactory)
            : Option.None<TResult, TException>(exception);
    }

    /// <summary>
    ///     Empties an optional, and attaches an exceptional value,
    ///     if a specified condition is not satisfied.
    /// </summary>
    /// <param name="condition">The condition.</param>
    /// <param name="fallback">The exceptional value to attach.</param>
    /// <returns>The filtered optional.</returns>
    [Pure]
    public Option<T, TException> Filter(bool condition, TException fallback) =>
        HasValue && !condition
            ? Option.None<T, TException>(fallback)
            : this;

    /// <summary>
    ///     Empties an optional, and attaches an exceptional value,
    ///     if a specified condition is not satisfied.
    /// </summary>
    /// <param name="condition">The condition.</param>
    /// <param name="exceptionFactory">A factory function to create an exceptional value to attach.</param>
    /// <returns>The filtered optional.</returns>
    [Pure]
    public Option<T, TException> Filter(bool condition, [InstantHandle] Func<TException> exceptionFactory)
    {
        if (exceptionFactory == null)
        {
            throw new ArgumentNullException(nameof(exceptionFactory));
        }

        return HasValue && !condition
            ? Option.None<T, TException>(exceptionFactory())
            : this;
    }

    /// <summary>
    ///     Empties an optional, and attaches an exceptional value,
    ///     if a specified predicate is not satisfied.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="fallback">The exceptional value to attach.</param>
    /// <returns>The filtered optional.</returns>
    [Pure]
    public Option<T, TException> Filter([InstantHandle] Func<T, bool> predicate, TException fallback)
    {
        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        return HasValue && !predicate(value)
            ? Option.None<T, TException>(fallback)
            : this;
    }

    /// <summary>
    ///     Empties an optional, and attaches an exceptional value,
    ///     if a specified predicate is not satisfied.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="exceptionFactory">A factory function to create an exceptional value to attach.</param>
    /// <returns>The filtered optional.</returns>
    [Pure]
    public Option<T, TException> Filter(
        [InstantHandle] Func<T, bool> predicate,
        [InstantHandle] Func<TException> exceptionFactory)
    {
        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        if (exceptionFactory == null)
        {
            throw new ArgumentNullException(nameof(exceptionFactory));
        }

        return HasValue && !predicate(value)
            ? Option.None<T, TException>(exceptionFactory())
            : this;
    }

    /// <summary>
    ///     Swaps value and exceptional value.
    /// </summary>
    /// <returns>The swapped optional.</returns>
    [Pure]
    public Option<TException, T> Swap() => new(exception, value, !HasValue);
}