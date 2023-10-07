using Optional.Collections;

namespace Optional.Tests.Collections;

public static partial class OptionCollectionExtensions
{
    public sealed class MaxOrNone_Should
    {
        [Test]
        public void Give_Max_Using_Order()
        {
            var max = new[] { 21, 17, 42 }.MaxOrNone(ByLastDigit);
            max.Should().Be(17.Some());
        }

        [Test]
        public void Give_None_Using_Order_On_Empty_Sequence()
        {
            var max = Array.Empty<int>().MaxOrNone(ByLastDigit);
            max.Should().Be(Option.None<int>());
        }

        [Test]
        public void Give_Max_Without_Order()
        {
            var max = new[] { 21, 17, 42 }.MaxOrNone();
            max.Should().Be(42.Some());
        }

        [Test]
        public void Give_None_Without_Order_On_Empty_Sequence()
        {
            var max = Array.Empty<int>().MaxOrNone();
            max.Should().Be(Option.None<int>());
        }
    }
}