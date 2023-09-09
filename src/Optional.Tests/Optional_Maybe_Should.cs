using System.Globalization;
using System.Numerics;
using Optional.Unsafe;

namespace Optional.Tests;

public sealed class Optional_Maybe_Should
{
    #region Creation

    [Test]
    public void Be_Created_Through_Factory_Some_Method()
    {
        var sut = Option.Some("value");

        (sut.HasValue, sut.ValueOrDefault()).Should().Be((true, "value"));
    }

    [Test]
    public void Be_Created_Through_Factory_None_Method()
    {
        var sut = Option.None<string>();

        (sut.HasValue, sut.ValueOrDefault()).Should().Be((false, null));
    }

    [Test]
    public void Be_Created_Through_Extension_Some_Method()
    {
        var sut = "new-value".Some();

        (sut.HasValue, sut.ValueOrDefault()).Should().Be((true, "new-value"));
    }

    [Test]
    public void Be_Created_Through_Extension_None_Method()
    {
        var sut = "string".None();

        (sut.HasValue, sut.ValueOrDefault()).Should().Be((false, null));
    }

    #endregion

    #region Equality

    private sealed record EqualityCase(Option<int> A, Option<int> B, bool Expectation);

    private static readonly EqualityCase[] equalityCases =
    {
        new(10.Some(), 10.Some(), Expectation: true),
        new(10.Some(), 9.Some(), Expectation: false),
        new(Option.None<int>(), 7.Some(), Expectation: false),
        new(11.Some(), Option.None<int>(), Expectation: false),
        new(Option.None<int>(), Option.None<int>(), Expectation: true)
    };

    private static IEnumerable<TestCaseData> EqualityCases() => equalityCases.Select(
        @case => new TestCaseData(@case.A, @case.B).Returns(@case.Expectation)
    );

    public static IEnumerable<TestCaseData> HashCodeCases => equalityCases
        .Where(@case => @case.Expectation)
        .Select(@case => new TestCaseData(@case.A, @case.B));

    [Test]
    [TestCaseSource(nameof(EqualityCases))]
    public bool Determine_Equality(Option<int> a, Option<int> b) => a.Equals(b);

    [Test]
    [TestCaseSource(nameof(EqualityCases))]
    public bool Determine_EqualityOperator(Option<int> a, Option<int> b) => a == b;

    [Test]
    [TestCaseSource(nameof(EqualityCases))]
    public bool Determine_InequalityOperator(Option<int> a, Option<int> b) => !(a != b);

    [Test]
    [TestCaseSource(nameof(HashCodeCases))]
    public void Give_Equal_HashCode_For_Equal_Options(Option<int> a, Option<int> b)
    {
        var aHashCode = a.GetHashCode();
        var bHashCode = b.GetHashCode();

        aHashCode.Should().Be(bHashCode);
    }

    #endregion

    #region Order

    private sealed record OrderCase(Option<int> A, Option<int> B, int Expectation)
    {
        public OrderCase Flip() => new(B, A, -Expectation);
    }

    private static readonly OrderCase[] orderCases =
    {
        new(1.Some(), 2.Some(), Expectation: -1),
        new(41.Some(), 41.Some(), Expectation: 0),
        new(Option.None<int>(), 0.Some(), Expectation: -1),
        new(Option.None<int>(), Option.None<int>(), Expectation: 0)
    };

    private static IEnumerable<OrderCase> FlippedOrderCases => orderCases.SelectMany(
        @case => @case.Expectation == 0
            ? new[] { @case }
            : new[] { @case, @case.Flip() }
    );

    private static IEnumerable<TestCaseData> OrderCases() => FlippedOrderCases.Select(
        @case => new TestCaseData(@case.A, @case.B).Returns(@case.Expectation)
    );

    private static IEnumerable<TestCaseData> OrderConsistencyCases() => FlippedOrderCases.Select(
        @case => new TestCaseData(@case.A, @case.B)
    );

    [Test]
    [TestCaseSource(nameof(OrderCases))]
    public int Compare_It_Self(Option<int> a, Option<int> b) => Math.Sign(a.CompareTo(b));

    [Test]
    [TestCaseSource(nameof(OrderConsistencyCases))]
    public void Has_Order_Operator_Consistent(Option<int> a, Option<int> b)
    {
        var eq = a == b;
        var gt = a > b;
        var gte = a >= b;
        var lt = a < b;
        var lte = a <= b;
        var comparison = a.CompareTo(b);

        (eq, gt, gte, lt, lte).Should().Be(
            (
                comparison == 0,
                comparison > 0,
                comparison >= 0,
                comparison < 0,
                comparison <= 0
            )
        );
    }

    #endregion

    #region Parsing

    [Test]
    public void Be_Parsed()
    {
        var option = Option.Parse<int>("123", CultureInfo.InvariantCulture);

        option.Should().Be(123.Some());
    }

    [Test]
    public void Be_Parsed_From_Span()
    {
        var option = Option.Parse<BigInteger>("500".AsSpan(), CultureInfo.InvariantCulture);

        option.Should().Be(new BigInteger(500).Some());
    }

    [Test]
    public void Respect_Culture()
    {
        var option = Option.Parse<float>("0,1", CultureInfo.GetCultureInfo("ru-ru"));

        option.Should().Be(0.1f.Some());
    }

    [Test]
    public void Respect_Culture_For_Span()
    {
        var option = Option.Parse<float>("0,1".AsSpan(), CultureInfo.GetCultureInfo("ru-ru"));

        option.Should().Be(0.1f.Some());
    }

    #endregion
}