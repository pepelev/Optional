using Optional.Collections;

namespace Optional.Tests.Collections;

public static partial class OptionCollectionExtensions
{
    public sealed class MinOrNone_Should
    {
        [Test]
        public void Give_Min_Using_Order()
        {
            var min = new[] { 21, 17, 42 }.MinOrNone(ByLastDigit);
            min.Should().Be(21.Some());
        }

        [Test]
        public void Give_None_Using_Order_On_Empty_Sequence()
        {
            var min = Array.Empty<int>().MinOrNone(ByLastDigit);
            min.Should().Be(Option.None<int>());
        }

        [Test]
        public void Give_Min_Without_Order()
        {
            var min = new[] { 21, 17, 42 }.MinOrNone();
            min.Should().Be(17.Some());
        }

        [Test]
        public void Give_None_Without_Order_On_Empty_Sequence()
        {
            var min = Array.Empty<int>().MinOrNone();
            min.Should().Be(Option.None<int>());
        }
    }
}