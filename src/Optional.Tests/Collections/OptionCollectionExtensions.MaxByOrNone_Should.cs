using Comparation;
using Optional.Collections;

namespace Optional.Tests.Collections;

public static partial class OptionCollectionExtensions
{
    public sealed class MaxByOrNone_Should
    {
        [Test]
        public void Give_Max_Using_Order()
        {
            var max = new[] { 21, 17, 42 }.MaxByOrNone(x => x % 10, Comparer<int>.Default.Invert());
            max.Should().Be(21.Some());
        }

        [Test]
        public void Give_None_Using_Order_On_Empty_Sequence()
        {
            var max = Array.Empty<int>().MaxByOrNone(x => x % 10, Comparer<int>.Default.Invert());
            max.Should().Be(Option.None<int>());
        }

        [Test]
        public void Give_Max_Without_Order()
        {
            var max = new[] { 21, 17, 42 }.MaxByOrNone(x => x % 10);
            max.Should().Be(17.Some());
        }

        [Test]
        public void Give_None_Without_Order_On_Empty_Sequence()
        {
            var max = Array.Empty<int>().MaxByOrNone(x => x % 10);
            max.Should().Be(Option.None<int>());
        }
    }
}