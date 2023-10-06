#if NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
using Optional.Collections;

namespace Optional.Tests.Collections;

public static partial class AsyncStreamExtensions
{
    [TestFixtureSource(nameof(Booleans))]
    public sealed class Values_On_Either_Should
    {
        public static bool[] Booleans => new[] { true, false };

        private readonly bool continueOnCapturedContext;

        public Values_On_Either_Should(bool continueOnCapturedContext)
        {
            this.continueOnCapturedContext = continueOnCapturedContext;
        }

        [Test]
        public async Task Give_Values()
        {
            var sequence = new[]
            {
                1.Some<int, string>(),
                Option.None<int, string>("failure"),
                2.Some<int, string>(),
                Option.None<int, string>("fail"),
                Option.None<int, string>("deny"),
                3.Some<int, string>()
            }.ToAsyncEnumerable();

            var values = await sequence.Values(continueOnCapturedContext).ToListAsync().ConfigureAwait(false);

            values.Should().Equal(1, 2, 3);
        }

        [Test]
        public void Throw_On_Null()
        {
            IAsyncEnumerable<Option<string, int>>? sequence = null;

            Assert.Throws<ArgumentNullException>(() => sequence!.Values(continueOnCapturedContext));
        }

        [Test]
        public async Task Pass_CancellationToken_To_Underlying([Range(0, 3)] int index)
        {
            var sequence = new[]
            {
                Option.None<int, string>("fail"),
                Option.None<int, string>("failure"),
                1.Some<int, string>(),
                2.Some<int, string>(),
                Option.None<int, string>("deny"),
                3.Some<int, string>()
            }.ToAsync();

            var values = sequence.Values(continueOnCapturedContext);

            await values.AssertThrowsBeforeAsync(index).ConfigureAwait(false);
        }
    }
}
#endif