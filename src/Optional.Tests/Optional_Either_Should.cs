using Optional.Linq;

namespace Optional.Tests;

public sealed class Optional_Either_Should
{
    #region Linq

    [Test]
    public void Pass_Value_When_Predicate_Match_On_Where()
    {
        var result =
            from a in Option.Some<int, string>(42)
            where (a <= 50, "more than 50")
            select a;

        result.Should().Be(Option.Some<int, string>(42));
    }

    [Test]
    public void Use_Exception_When_Predicate_Not_Match_On_Where()
    {
        var result =
            from a in Option.Some<int, string>(42)
            where (a <= 30, "more than 30")
            select a;

        result.Should().Be(Option.None<int, string>("more than 30"));
    }

    [Test]
    public void Left_Exception_Value_As_Is_On_Where()
    {
        var result =
            from a in Option.None<int, string>("Could not get value")
            where (a <= 75, "more than 75")
            select a;

        result.Should().Be(Option.None<int, string>("Could not get value"));
    }

    #endregion

    #region Swap

    [Test]
    public void Swap_Value_And_Exception()
    {
        var some = Option.Some<int, string>(150);
        var none = some.Swap();
        var someAgain = none.Swap();

        (none, someAgain).Should().Be(
            (Option.None<string, int>(150), Option.Some<int, string>(150))
        );
    }

    #endregion
}