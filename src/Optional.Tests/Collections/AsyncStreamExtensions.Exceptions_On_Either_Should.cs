﻿#if NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
using Optional.Collections;

namespace Optional.Tests.Collections;

public static partial class AsyncStreamExtensions
{
    [TestFixtureSource(nameof(Booleans))]
    public sealed class Exceptions_On_Either_Should
    {
        public static bool[] Booleans => new[] { true, false };

        private readonly bool continueOnCapturedContext;

        public Exceptions_On_Either_Should(bool continueOnCapturedContext)
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

            var values = await sequence.Exceptions(continueOnCapturedContext).ToListAsync().ConfigureAwait(false);

            values.Should().Equal("failure", "fail", "deny");
        }

        [Test]
        public void Throw_On_Null()
        {
            IAsyncEnumerable<Option<string, int>>? sequence = null;

            Assert.Throws<ArgumentNullException>(() => sequence!.Exceptions(continueOnCapturedContext));
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

            var values = sequence.Exceptions(continueOnCapturedContext);

            await values.AssertThrowsBeforeAsync(index).ConfigureAwait(false);
        }
    }
}
#endif