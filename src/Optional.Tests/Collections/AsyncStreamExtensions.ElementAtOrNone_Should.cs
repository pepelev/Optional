#if NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
using Optional.Collections;
using static Optional.Tests.Cases;
using static Optional.Tests.Collections.MockAsyncEnumerable.Stage;

namespace Optional.Tests.Collections;

public static partial class AsyncStreamExtensions
{
    [TestFixtureSource(nameof(Booleans))]
    public sealed class ElementAtOrNone_Should
    {
        public static bool[] Booleans => new[] { true, false };
        public static Index[] Indices => new[] { 5, ^3 };

        private readonly bool continueOnCapturedContext;

        public ElementAtOrNone_Should(bool continueOnCapturedContext)
        {
            this.continueOnCapturedContext = continueOnCapturedContext;
        }

        public static TestCaseData[] Cases => new[]
        {
            Case("0_From_Empty_Sequence", Array.Empty<int>(), (Index)0).Returns(Option.None<int>()),
            Case("^1_From_Empty_Sequence", Array.Empty<int>(), ^1).Returns(Option.None<int>()),
            Case("0_From_Single_Element_Sequence", new[] { 42 }, (Index)0).Returns(42.Some()),
            Case("^1_From_Single_Element_Sequence", new[] { 42 }, ^1).Returns(42.Some()),
            Case("1_From_Single_Element_Sequence", new[] { 42 }, (Index)1).Returns(Option.None<int>()),
            Case("^0_From_Single_Element_Sequence", new[] { 42 }, ^0).Returns(Option.None<int>()),
            Case("^2_From_Single_Element_Sequence", new[] { 42 }, ^2).Returns(Option.None<int>()),
            Case("^2_From_Two_Elements_Sequence", new[] { 2, -5 }, ^2).Returns(2.Some())
        };

        [Test]
        [TestCaseSource(nameof(Cases))]
        public async Task<Option<int>> Give(int[] arguments, Index index)
        {
            var result = arguments.ToAsync().ElementAtOrNoneAsync(
                index,
                continueOnCapturedContext: continueOnCapturedContext
            );
            return await result.ConfigureAwait(false);
        }

        [Test]
        public void Pass_CancellationToken_To_Underlying([ValueSource(nameof(Indices))] Index index)
        {
            using var mockSequence = new[] { 1, 2, 3 }.Mock(
                GetEnumeratorCall,
                MoveNextCall,
                CurrentCall,
                TokenCancellation,
                MoveNextCall,
                DisposeCall
            );

            Assert.CatchAsync<OperationCanceledException>(
                () => mockSequence.ElementAtOrNoneAsync(5, mockSequence.Token, continueOnCapturedContext)
            );
        }
    }
}
#endif