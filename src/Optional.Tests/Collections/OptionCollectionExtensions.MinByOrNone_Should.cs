using Comparation;
using Optional.Collections;

namespace Optional.Tests.Collections;

public static partial class OptionCollectionExtensions
{
    public sealed class MinByOrNone_Should
    {
        [Test]
        public void Give_Min_Using_Order()
        {
            var min = new[] { 21, 17, 42 }.MinByOrNone(x => x % 10, Comparer<int>.Default.Invert());
            min.Should().Be(17.Some());
        }

        [Test]
        public void Give_None_Using_Order_On_Empty_Sequence()
        {
            var min = Array.Empty<int>().MinByOrNone(x => x % 10, Comparer<int>.Default.Invert());
            min.Should().Be(Option.None<int>());
        }

        [Test]
        public void Give_Min_Without_Order()
        {
            var min = new[] { 21, 17, 42 }.MinByOrNone(x => x % 10);
            min.Should().Be(21.Some());
        }

        [Test]
        public void Give_None_Without_Order_On_Empty_Sequence()
        {
            var min = Array.Empty<int>().MinByOrNone(x => x % 10);
            min.Should().Be(Option.None<int>());
        }
    }
}