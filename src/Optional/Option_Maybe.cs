namespace Optional;

/// <summary>
///     Represents an optional value.
/// </summary>
/// <typeparam name="T">The type of the value to be wrapped.</typeparam>
#if NET20_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP1_0_OR_GREATER
[Serializable]
#endif
public struct Option<T> : IEquatable<Option<T>>, IComparable<Option<T>>
{
    private readonly T value;

    /// <summary>
    ///     Checks if a value is present.
    /// </summary>
    public bool HasValue { get; }

    internal T Value => value;

    internal Option(T value, bool hasValue)
    {
        this.value = value;
        HasValue = hasValue;
    }

    /// <summary>
    ///     Determines whether two optionals are equal.
    /// </summary>
    /// <param name="other">The optional to compare with the current one.</param>
    /// <returns>A boolean indicating whether or not the optionals are equal.</returns>
    [Pure]
    public bool Equals(Option<T> other)
    {
        if (!HasValue && !other.HasValue)
        {
            return true;
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
    public override bool Equals(object? obj) => obj is Option<T> option && Equals(option);

    /// <summary>
    ///     Determines whether two optionals are equal.
    /// </summary>
    /// <param name="left">The first optional to compare.</param>
    /// <param name="right">The second optional to compare.</param>
    /// <returns>A boolean indicating whether or not the optionals are equal.</returns>
    public static bool operator ==(Option<T> left, Option<T> right) => left.Equals(right);

    /// <summary>
    ///     Determines whether two optionals are unequal.
    /// </summary>
    /// <param name="left">The first optional to compare.</param>
    /// <param name="right">The second optional to compare.</param>
    /// <returns>A boolean indicating whether or not the optionals are unequal.</returns>
    public static bool operator !=(Option<T> left, Option<T> right) => !left.Equals(right);

    /// <summary>
    ///     Generates a hash code for the current optional.
    /// </summary>
    /// <returns>A hash code for the current optional.</returns>
    [Pure]
    public override int GetHashCode()
    {
        if (HasValue)
        {
            if (value == null)
            {
                return 1;
            }

            return value.GetHashCode();
        }

        return 0;
    }

    /// <summary>
    ///     Compares the relative order of two optionals. An empty optional is
    ///     ordered before a non-empty one.
    /// </summary>
    /// <param name="other">The optional to compare with the current one.</param>
    /// <returns>An integer indicating the relative order of the optionals being compared.</returns>
    [Pure]
    public int CompareTo(Option<T> other)
    {
        if (HasValue && !other.HasValue)
        {
            return 1;
        }

        if (!HasValue && other.HasValue)
        {
            return -1;
        }

        return Comparer<T>.Default.Compare(value, other.value);
    }

    /// <summary>
    ///     Determines if an optional is less than another optional.
    /// </summary>
    /// <param name="left">The first optional to compare.</param>
    /// <param name="right">The second optional to compare.</param>
    /// <returns>A boolean indicating whether or not the left optional is less than the right optional.</returns>
    public static bool operator <(Option<T> left, Option<T> right) => left.CompareTo(right) < 0;

    /// <summary>
    ///     Determines if an optional is less than or equal to another optional.
    /// </summary>
    /// <param name="left">The first optional to compare.</param>
    /// <param name="right">The second optional to compare.</param>
    /// <returns>A boolean indicating whether or not the left optional is less than or equal the right optional.</returns>
    public static bool operator <=(Option<T> left, Option<T> right) => left.CompareTo(right) <= 0;

    /// <summary>
    ///     Determines if an optional is greater than another optional.
    /// </summary>
    /// <param name="left">The first optional to compare.</param>
    /// <param name="right">The second optional to compare.</param>
    /// <returns>A boolean indicating whether or not the left optional is greater than the right optional.</returns>
    public static bool operator >(Option<T> left, Option<T> right) => left.CompareTo(right) > 0;

    /// <summary>
    ///     Determines if an optional is greater than or equal to another optional.
    /// </summary>
    /// <param name="left">The first optional to compare.</param>
    /// <param name="right">The second optional to compare.</param>
    /// <returns>A boolean indicating whether or not the left optional is greater than or equal the right optional.</returns>
    public static bool operator >=(Option<T> left, Option<T> right) => left.CompareTo(right) >= 0;

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

        return "None";
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
    /// <param name="value">The value to locate.</param>
    /// <returns>A boolean indicating whether or not the value was found.</returns>
    [Pure]
    public bool Contains(T value)
    {
        if (HasValue)
        {
            if (this.value == null)
            {
                return value == null;
            }

            return this.value.Equals(value);
        }

        return false;
    }

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
    ///     Uses an alternative value, if no existing value is present.
    /// </summary>
    /// <param name="alternative">The alternative value.</param>
    /// <returns>A new optional, containing either the existing or alternative value.</returns>
    [Pure]
    public Option<T> Or(T alternative) => HasValue
        ? this
        : Option.Some(alternative);

    /// <summary>
    ///     Uses an alternative value, if no existing value is present.
    /// </summary>
    /// <param name="alternativeFactory">A factory function to create an alternative value.</param>
    /// <returns>A new optional, containing either the existing or alternative value.</returns>
    [Pure]
    public Option<T> Or([InstantHandle] Func<T> alternativeFactory)
    {
        if (alternativeFactory == null)
        {
            throw new ArgumentNullException(nameof(alternativeFactory));
        }

        return HasValue
            ? this
            : Option.Some(alternativeFactory());
    }

    /// <summary>
    ///     Uses an alternative optional, if no existing value is present.
    /// </summary>
    /// <param name="alternativeOption">The alternative optional.</param>
    /// <returns>The alternative optional, if no value is present, otherwise itself.</returns>
    [Pure]
    public Option<T> Else(Option<T> alternativeOption) => HasValue
        ? this
        : alternativeOption;

    /// <summary>
    ///     Uses an alternative optional, if no existing value is present.
    /// </summary>
    /// <param name="alternativeOptionFactory">A factory function to create an alternative optional.</param>
    /// <returns>The alternative optional, if no value is present, otherwise itself.</returns>
    [Pure]
    public Option<T> Else([InstantHandle] Func<Option<T>> alternativeOptionFactory)
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
    ///     Attaches an exceptional value to an empty optional.
    /// </summary>
    /// <param name="exception">The exceptional value to attach.</param>
    /// <returns>An optional with an exceptional value.</returns>
    [Pure]
    public Option<T, TException> WithException<TException>(TException exception)
    {
        return Match(
            Option.Some<T, TException>,
            () => Option.None<T, TException>(exception)
        );
    }

    /// <summary>
    ///     Attaches an exceptional value to an empty optional.
    /// </summary>
    /// <param name="exceptionFactory">A factory function to create an exceptional value to attach.</param>
    /// <returns>An optional with an exceptional value.</returns>
    [Pure]
    public Option<T, TException> WithException<TException>([InstantHandle] Func<TException> exceptionFactory)
    {
        if (exceptionFactory == null)
        {
            throw new ArgumentNullException(nameof(exceptionFactory));
        }

        return Match(
            Option.Some<T, TException>,
            () => Option.None<T, TException>(exceptionFactory())
        );
    }

    /// <summary>
    ///     Evaluates a specified function, based on whether a value is present or not.
    /// </summary>
    /// <param name="some">The function to evaluate if the value is present.</param>
    /// <param name="none">The function to evaluate if the value is missing.</param>
    /// <returns>The result of the evaluated function.</returns>
    [Pure]
    public TResult Match<TResult>([InstantHandle] Func<T, TResult> some, [InstantHandle] Func<TResult> none)
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
            : none();
    }

    /// <summary>
    ///     Evaluates a specified action, based on whether a value is present or not.
    /// </summary>
    /// <param name="some">The action to evaluate if the value is present.</param>
    /// <param name="none">The action to evaluate if the value is missing.</param>
    public void Match([InstantHandle] Action<T> some, [InstantHandle] Action none)
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
            none();
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
    public void MatchNone([InstantHandle] Action none)
    {
        if (none == null)
        {
            throw new ArgumentNullException(nameof(none));
        }

        if (!HasValue)
        {
            none();
        }
    }

    /// <summary>
    ///     Transforms the inner value in an optional.
    ///     If the instance is empty, an empty optional is returned.
    /// </summary>
    /// <param name="mapping">The transformation function.</param>
    /// <returns>The transformed optional.</returns>
    [Pure]
    public Option<TResult> Map<TResult>([InstantHandle] Func<T, TResult> mapping)
    {
        if (mapping == null)
        {
            throw new ArgumentNullException(nameof(mapping));
        }

        return Match(
            value => Option.Some(mapping(value)),
            Option.None<TResult>
        );
    }

    /// <summary>
    ///     Transforms the inner value in an optional
    ///     into another optional. The result is flattened,
    ///     and if either is empty, an empty optional is returned.
    /// </summary>
    /// <param name="mapping">The transformation function.</param>
    /// <returns>The transformed optional.</returns>
    [Pure]
    public Option<TResult> FlatMap<TResult>([InstantHandle] Func<T, Option<TResult>> mapping)
    {
        if (mapping == null)
        {
            throw new ArgumentNullException(nameof(mapping));
        }

        return Match(
            mapping,
            Option.None<TResult>
        );
    }

    /// <summary>
    ///     Transforms the inner value in an optional
    ///     into another optional. The result is flattened,
    ///     and if either is empty, an empty optional is returned.
    ///     If the option contains an exception, it is removed.
    /// </summary>
    /// <param name="mapping">The transformation function.</param>
    /// <returns>The transformed optional.</returns>
    [Pure]
    public Option<TResult> FlatMap<TResult, TException>([InstantHandle] Func<T, Option<TResult, TException>> mapping)
    {
        if (mapping == null)
        {
            throw new ArgumentNullException(nameof(mapping));
        }

        return FlatMap(value => mapping(value).WithoutException());
    }

    /// <summary>
    ///     Empties an optional if a specified condition
    ///     is not satisfied.
    /// </summary>
    /// <param name="condition">The condition.</param>
    /// <returns>The filtered optional.</returns>
    [Pure]
    public Option<T> Filter(bool condition) => HasValue && !condition
        ? Option.None<T>()
        : this;

    /// <summary>
    ///     Empties an optional if a specified predicate
    ///     is not satisfied.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <returns>The filtered optional.</returns>
    [Pure]
    public Option<T> Filter([InstantHandle] Func<T, bool> predicate)
    {
        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        return HasValue && !predicate(value)
            ? Option.None<T>()
            : this;
    }
}