using Optional.Collections;
using static Optional.Tests.Collections.MockAsyncEnumerable.Stage;

namespace Optional.Tests.Collections;

public static partial class AsyncStreamExtensions
{
    [TestFixtureSource(nameof(Booleans))]
    public sealed class MaxOrNone_Should
    {
        public static bool[] Booleans => new[] { true, false };

        private readonly bool continueOnCapturedContext;

        public MaxOrNone_Should(bool continueOnCapturedContext)
        {
            this.continueOnCapturedContext = continueOnCapturedContext;
        }

        [Test]
        public async Task Give_Max_Using_Order()
        {
            var sequence = new[] { 21, 17, 42 }.ToAsync();
            var max = await sequence
                .MaxOrNoneAsync(ByLastDigit, continueOnCapturedContext: continueOnCapturedContext)
                .ConfigureAwait(false);
            max.Should().Be(17.Some());
        }

        [Test]
        public async Task Give_None_Using_Order_On_Empty_Sequence()
        {
            var sequence = Array.Empty<int>().ToAsync();
            var max = await sequence
                .MaxOrNoneAsync(ByLastDigit, continueOnCapturedContext: continueOnCapturedContext)
                .ConfigureAwait(false);
            max.Should().Be(Option.None<int>());
        }

        [Test]
        public async Task Give_Max_Without_Order()
        {
            var sequence = new[] { 21, 17, 42 }.ToAsync();
            var max = await sequence
                .MaxOrNoneAsync(continueOnCapturedContext: continueOnCapturedContext)
                .ConfigureAwait(false);
            max.Should().Be(42.Some());
        }

        [Test]
        public async Task Give_None_Without_Order_On_Empty_Sequence()
        {
            var sequence = Array.Empty<int>().ToAsync();
            var max = await sequence
                .MaxOrNoneAsync(continueOnCapturedContext: continueOnCapturedContext)
                .ConfigureAwait(false);
            max.Should().Be(Option.None<int>());
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
                () => mockSequence.MaxOrNoneAsync(mockSequence.Token, continueOnCapturedContext)
            );
        }
    }
}