#if NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
using Optional.Collections;
using static Optional.Tests.Cases;
using static Optional.Tests.Collections.MockAsyncEnumerable.Stage;

namespace Optional.Tests.Collections;

public static partial class AsyncStreamExtensions
{
    [TestFixtureSource(nameof(Booleans))]
    public sealed class SingleOrNone_Should
    {
        public static bool[] Booleans => new[] { true, false };

        private readonly bool continueOnCapturedContext;

        public SingleOrNone_Should(bool continueOnCapturedContext)
        {
            this.continueOnCapturedContext = continueOnCapturedContext;
        }

        public static TestCaseData[] Cases => new[]
        {
            Case("Empty_Sequence", Array.Empty<int>()).Returns(Option.None<int>()),
            Case("Single_Element_Sequence", new[] { 42 }).Returns(42.Some()),
            Case("Two_Elements_Sequence", new[] { 2, -5 }).Returns(Option.None<int>())
        };

        [Test]
        [TestCaseSource(nameof(Cases))]
        public async Task<Option<int>> Give_When(int[] arguments)
        {
            var result = arguments.ToAsync().SingleOrNoneAsync(continueOnCapturedContext: continueOnCapturedContext);
            return await result.ConfigureAwait(false);
        }

        [Test]
        public void Pass_CancellationToken_To_Underlying()
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
                () => mockSequence.SingleOrNoneAsync(mockSequence.Token, continueOnCapturedContext)
            );
        }
    }
}
#endif