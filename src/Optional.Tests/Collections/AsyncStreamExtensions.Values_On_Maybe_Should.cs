#if NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
using Optional.Collections;

namespace Optional.Tests.Collections;

public static partial class AsyncStreamExtensions
{
    [TestFixtureSource(nameof(Booleans))]
    public sealed class Values_On_Maybe_Should
    {
        public static bool[] Booleans => new[] { true, false };

        private readonly bool continueOnCapturedContext;

        public Values_On_Maybe_Should(bool continueOnCapturedContext)
        {
            this.continueOnCapturedContext = continueOnCapturedContext;
        }

        [Test]
        public async Task Give_Values()
        {
            var sequence = new[]
            {
                1.Some(),
                Option.None<int>(),
                2.Some(),
                Option.None<int>(),
                Option.None<int>(),
                3.Some()
            }.ToAsyncEnumerable();

            var values = await sequence.Values(continueOnCapturedContext).ToListAsync().ConfigureAwait(false);

            values.Should().Equal(1, 2, 3);
        }

        [Test]
        public async Task Pass_CancellationToken_To_Underlying([Range(0, 3)] int index)
        {
            var sequence = new[]
            {
                Option.None<int>(),
                1.Some(),
                Option.None<int>(),
                2.Some(),
                Option.None<int>(),
                3.Some()
            }.ToAsync();

            var values = sequence.Values(continueOnCapturedContext);

            await values.AssertThrowsBeforeAsync(index).ConfigureAwait(false);
        }
    }
}
#endif