namespace Optional;

/// <summary>
///     Provides a set of functions for creating optional values.
/// </summary>
public static class Option
{
    /// <summary>
    ///     Wraps an existing value in an Option&lt;T&gt; instance.
    /// </summary>
    /// <param name="value">The value to be wrapped.</param>
    /// <returns>An optional containing the specified value.</returns>
    public static Option<T> Some<T>(T value) => new(value, true);

    /// <summary>
    ///     Wraps an existing value in an Option&lt;T, TException&gt; instance.
    /// </summary>
    /// <param name="value">The value to be wrapped.</param>
    /// <returns>An optional containing the specified value.</returns>
    public static Option<T, TException> Some<T, TException>(T value) =>
        new(value, exception: default!, hasValue: true);

    /// <summary>
    ///     Creates an empty Option&lt;T&gt; instance.
    /// </summary>
    /// <returns>An empty optional.</returns>
    public static Option<T> None<T>() => new(value: default!, hasValue: false);

    /// <summary>
    ///     Creates an empty Option&lt;T, TException&gt; instance,
    ///     with a specified exceptional value.
    /// </summary>
    /// <param name="exception">The exceptional value.</param>
    /// <returns>An empty optional.</returns>
    public static Option<T, TException> None<T, TException>(TException exception) =>
        new(value: default!, exception, hasValue: false);

#if NET7_0_OR_GREATER

    /// <summary>
    ///     Creates an Option&lt;T&gt; instance from a parsing result.
    /// </summary>
    /// <param name="argument">The span of characters to parse.</param>
    /// <param name="formatProvider">
    ///     An object that provides culture-specific formatting information about <paramref name="argument" />.
    /// </param>
    /// <returns>An optional containing the parsed value if parsing was successful, an empty one otherwise.</returns>
    public static Option<T> Parse<T>(ReadOnlySpan<char> argument, IFormatProvider? formatProvider = null)
        where T : ISpanParsable<T> =>
        T.TryParse(argument, formatProvider, out var result)
            ? result.Some()
            : None<T>();

    /// <summary>
    ///     Creates an Option&lt;T&gt; instance from a parsing result.
    /// </summary>
    /// <param name="argument">The string to parse.</param>
    /// <param name="formatProvider">
    ///     An object that provides culture-specific formatting information about <paramref name="argument" />.
    /// </param>
    /// <returns>An optional containing the parsed value if parsing was successful, an empty one otherwise.</returns>
    public static Option<T> Parse<T>(string? argument, IFormatProvider? formatProvider = null)
        where T : IParsable<T> =>
        T.TryParse(argument, formatProvider, out var result)
            ? result.Some()
            : None<T>();
#endif
}