using Comparation;
using Optional.Collections;

namespace Optional.Tests.Collections;

public static partial class AsyncStreamExtensions
{
    [TestFixtureSource(nameof(Booleans))]
    public sealed class MaxByOrNone_Should
    {
        public static bool[] Booleans => new[] { true, false };

        private readonly bool continueOnCapturedContext;

        public MaxByOrNone_Should(bool continueOnCapturedContext)
        {
            this.continueOnCapturedContext = continueOnCapturedContext;
        }

        [Test]
        public async Task Give_Max_Using_Order()
        {
            var sequence = new[] { 21, 17, 42 }.ToAsync();
            var max = await sequence
                .MaxByOrNoneAsync(
                    number => number % 10,
                    Comparer<int>.Default.Invert(),
                    continueOnCapturedContext: continueOnCapturedContext
                ).ConfigureAwait(false);
            max.Should().Be(21.Some());
        }

        [Test]
        public async Task Give_None_Using_Order_On_Empty_Sequence()
        {
            var sequence = Array.Empty<int>().ToAsync();
            var max = await sequence
                .MaxByOrNoneAsync(
                    number => number % 10,
                    Comparer<int>.Default.Invert(),
                    continueOnCapturedContext: continueOnCapturedContext
                ).ConfigureAwait(false);
            max.Should().Be(Option.None<int>());
        }

        [Test]
        public async Task Give_Max_Without_Order()
        {
            var sequence = new[] { 21, 17, 42 }.ToAsync();
            var max = await sequence
                .MaxByOrNoneAsync(number => number % 10, continueOnCapturedContext: continueOnCapturedContext)
                .ConfigureAwait(false);
            max.Should().Be(17.Some());
        }

        [Test]
        public async Task Give_None_Without_Order_On_Empty_Sequence()
        {
            var sequence = Array.Empty<int>().ToAsync();
            var max = await sequence
                .MaxByOrNoneAsync(number => number % 10, continueOnCapturedContext: continueOnCapturedContext)
                .ConfigureAwait(false);
            max.Should().Be(Option.None<int>());
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
                () => mockSequence.MaxByOrNoneAsync(
                    number => number % 10,
                    mockSequence.Token,
                    continueOnCapturedContext
                )
            );
        }
    }
}