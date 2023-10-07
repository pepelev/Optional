using Optional.Collections;

namespace Optional.Tests.Collections;

public static partial class AsyncStreamExtensions
{
    [TestFixtureSource(nameof(Booleans))]
    public sealed class MinOrNone_Should
    {
        public static bool[] Booleans => new[] { true, false };

        private readonly bool continueOnCapturedContext;

        public MinOrNone_Should(bool continueOnCapturedContext)
        {
            this.continueOnCapturedContext = continueOnCapturedContext;
        }

        [Test]
        public async Task Give_Min_Using_Order()
        {
            var sequence = new[] { 21, 17, 42 }.ToAsync();
            var min = await sequence
                .MinOrNoneAsync(ByLastDigit, continueOnCapturedContext: continueOnCapturedContext)
                .ConfigureAwait(false);
            min.Should().Be(21.Some());
        }

        [Test]
        public async Task Give_None_Using_Order_On_Empty_Sequence()
        {
            var sequence = Array.Empty<int>().ToAsync();
            var min = await sequence
                .MinOrNoneAsync(ByLastDigit, continueOnCapturedContext: continueOnCapturedContext)
                .ConfigureAwait(false);
            min.Should().Be(Option.None<int>());
        }

        [Test]
        public async Task Give_Min_Without_Order()
        {
            var sequence = new[] { 21, 17, 42 }.ToAsync();
            var min = await sequence
                .MinOrNoneAsync(continueOnCapturedContext: continueOnCapturedContext)
                .ConfigureAwait(false);
            min.Should().Be(17.Some());
        }

        [Test]
        public async Task Give_None_Without_Order_On_Empty_Sequence()
        {
            var sequence = Array.Empty<int>().ToAsync();
            var min = await sequence
                .MinOrNoneAsync(continueOnCapturedContext: continueOnCapturedContext)
                .ConfigureAwait(false);
            min.Should().Be(Option.None<int>());
        }

        [Test]
        public void Pass_CancellationToken_To_Underlying()
        {
            using var mockSequence = new[] { 1, 2, 3 }.Mock(
                MockAsyncEnumerable.Stage.GetEnumeratorCall,
                MockAsyncEnumerable.Stage.MoveNextCall,
                MockAsyncEnumerable.Stage.CurrentCall,
                MockAsyncEnumerable.Stage.TokenCancellation,
                MockAsyncEnumerable.Stage.MoveNextCall,
                MockAsyncEnumerable.Stage.DisposeCall
            );

            Assert.CatchAsync<OperationCanceledException>(
                () => mockSequence.MinOrNoneAsync(mockSequence.Token, continueOnCapturedContext)
            );
        }
    }
}