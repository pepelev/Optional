using System.Linq.Expressions;
using Optional.Unsafe;

namespace Optional.Tests;

public sealed class Should_Throw_ArgumentNullException
{
    private static IEnumerable<TestCaseData> MethodCases(Expression<Func<object>> expression)
    {
        var visitor = new MemberVisitor();
        visitor.Visit(expression);
        var call = visitor.MethodCall.ValueOrFailure();
        var method = call.Method;
        var parameters = method.GetParameters();
        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            if (parameter.ParameterType.IsValueType)
            {
                continue;
            }

            var name = $"{method.DeclaringType?.Name}.{method.Name}({parameters.Length})_When_{parameter.Name}_Is_Null";
            var arguments = call.Arguments.ToList();
            arguments[i] = Expression.Constant(null, parameter.ParameterType);
            var func = (Func<object>)Expression.Lambda(
                Expression.Convert(
                    Expression.Call(
                        instance: null,
                        method,
                        arguments
                    ),
                    typeof(object)
                )
            ).Compile(preferInterpretation: true);

            yield return Tests.Cases.Case(name, func).Returns(parameter.Name);
        }
    }

    private static IEnumerable<TestCaseData> Cases()
    {
        var stringSequence = new[] { "a", "b", "c" };
        // var optionalSequence = new[] { "a".Some() };
        // var optionalEitherSequence = new[] { "a".Some().WithException(42) };
        var stringAsyncSequence = new[] { "a", "b", "c" }.ToAsync();
        var optionalAsyncSequence = new[] { "a".Some() }.ToAsync();
        var optionalEitherAsyncSequence = new[] { "a".Some().WithException(42) }.ToAsync();
        var keyValuePairs = new[] { new KeyValuePair<int, string>(42, "a") };
        var order = StringComparer.Ordinal;
        var key = (string x) => x;
        var token = CancellationToken.None;
        const bool captureContext = false;

        var calls = new Expression<Func<object>>[]
        {
            // todo uncomment for v5
            // () => Optional.Collections.OptionCollectionExtensions.Values(optionalSequence),
            // () => Optional.Collections.OptionCollectionExtensions.Values(optionalEitherSequence),
            // () => Optional.Collections.OptionCollectionExtensions.Exceptions(optionalEitherSequence),
            () => Optional.Collections.OptionCollectionExtensions.GetValueOrNone(keyValuePairs, 42),
            () => Optional.Collections.OptionCollectionExtensions.FirstOrNone(stringSequence),
            () => Optional.Collections.OptionCollectionExtensions.FirstOrNone(stringSequence, str => str.Length < 3),
            () => Optional.Collections.OptionCollectionExtensions.LastOrNone(stringSequence),
            () => Optional.Collections.OptionCollectionExtensions.LastOrNone(stringSequence, str => str.Length < 3),
            () => Optional.Collections.OptionCollectionExtensions.SingleOrNone(stringSequence),
            () => Optional.Collections.OptionCollectionExtensions.SingleOrNone(stringSequence, str => str.Length < 3),
            () => Optional.Collections.OptionCollectionExtensions.ElementAtOrNone(stringSequence, 5),
            () => Optional.Collections.OptionCollectionExtensions.ElementAtOrNone(stringSequence, new Index(0, true)),
            () => Optional.Collections.OptionCollectionExtensions.MaxByOrNone(stringSequence, key, order),
            () => Optional.Collections.OptionCollectionExtensions.MaxByOrNone(stringSequence, key),
            () => Optional.Collections.OptionCollectionExtensions.MaxOrNone(stringSequence, order),
            () => Optional.Collections.OptionCollectionExtensions.MaxOrNone(stringSequence),
            () => Optional.Collections.OptionCollectionExtensions.MinByOrNone(stringSequence, key, order),
            () => Optional.Collections.OptionCollectionExtensions.MinByOrNone(stringSequence, key),
            () => Optional.Collections.OptionCollectionExtensions.MinOrNone(stringSequence, order),
            () => Optional.Collections.OptionCollectionExtensions.MinOrNone(stringSequence),

            () => Optional.Collections.AsyncStreamExtensions.Values(optionalAsyncSequence, captureContext),
            () => Optional.Collections.AsyncStreamExtensions.Values(optionalEitherAsyncSequence, captureContext),
            () => Optional.Collections.AsyncStreamExtensions.Exceptions(optionalEitherAsyncSequence, captureContext),
            () => Optional.Collections.AsyncStreamExtensions.FirstOrNoneAsync(stringAsyncSequence, token, captureContext),
            () => Optional.Collections.AsyncStreamExtensions.LastOrNoneAsync(stringAsyncSequence, token, captureContext),
            () => Optional.Collections.AsyncStreamExtensions.SingleOrNoneAsync(stringAsyncSequence, token, captureContext),
            () => Optional.Collections.AsyncStreamExtensions.ElementAtOrNoneAsync(stringAsyncSequence, new Index(0, true), token, captureContext),
            () => Optional.Collections.AsyncStreamExtensions.MaxByOrNoneAsync(stringAsyncSequence, key, order, token, captureContext),
            () => Optional.Collections.AsyncStreamExtensions.MaxByOrNoneAsync(stringAsyncSequence, key, token, captureContext),
            () => Optional.Collections.AsyncStreamExtensions.MaxOrNoneAsync(stringAsyncSequence, order, token, captureContext),
            () => Optional.Collections.AsyncStreamExtensions.MaxOrNoneAsync(stringAsyncSequence, token, captureContext),
            () => Optional.Collections.AsyncStreamExtensions.MinByOrNoneAsync(stringAsyncSequence, key, order, token, captureContext),
            () => Optional.Collections.AsyncStreamExtensions.MinByOrNoneAsync(stringAsyncSequence, key, token, captureContext),
            () => Optional.Collections.AsyncStreamExtensions.MinOrNoneAsync(stringAsyncSequence, order, token, captureContext),
            () => Optional.Collections.AsyncStreamExtensions.MinOrNoneAsync(stringAsyncSequence, token, captureContext)
        };

        return calls.SelectMany(MethodCases);
    }

    [Test]
    [TestCaseSource(nameof(Cases))]
    public string? Throw_On_Null_Source(Func<object> action) => Assert.ThrowsAsync<ArgumentNullException>(
        async () =>
        {
            var result = action();
            if (result is Task task)
            {
                await task.ConfigureAwait(false);
            }
        }
    )?.ParamName;

    private sealed class MemberVisitor : ExpressionVisitor
    {
        public Option<MethodCallExpression> MethodCall { get; private set; }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            MethodCall = node.Some();
            return node;
        }
    }
}