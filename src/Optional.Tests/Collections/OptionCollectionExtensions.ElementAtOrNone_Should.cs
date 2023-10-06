using System.Collections.Immutable;
using Optional.Collections;
using static Optional.Tests.Cases;

namespace Optional.Tests.Collections;

public static class OptionCollectionExtensions
{
    public sealed class ElementAtOrNone_Should
    {
        private static readonly (string Name, IEnumerable<string> Sequence)[] sequences =
        {
            ("Enumerable", Enumerable.Range('a', 3).Select(code => $"{(char)code}")),
            ("List", new List<string> { "a", "b", "c" }),
            ("ReadOnlyList", new List<string> { "a", "b", "c" }.AsReadOnly()),
            ("Array", new[] { "a", "b", "c" }),
            ("ImmutableList", ImmutableList.CreateRange(new[] { "a", "b", "c" }))
        };

        public static IEnumerable<TestCaseData> Cases()
        {
            foreach (var (name, sequence) in sequences)
            {
                yield return Case($"{name}.First", sequence, (Index)0).Returns("a".Some());
                yield return Case($"{name}.Second", sequence, (Index)1).Returns("b".Some());
                yield return Case($"{name}.Third", sequence, (Index)2).Returns("c".Some());
                yield return Case($"{name}.Fourth", sequence, (Index)3).Returns(Option.None<string>());
                yield return Case($"{name}.After_Last", sequence, ^0).Returns(Option.None<string>());
                yield return Case($"{name}.Last", sequence, ^1).Returns("c".Some());
                yield return Case($"{name}.Second_From_End", sequence, ^2).Returns("b".Some());
                yield return Case($"{name}.Third_From_End", sequence, ^3).Returns("a".Some());
                yield return Case($"{name}.Fourth_From_End", sequence, ^4).Returns(Option.None<string>());
            }
        }

        [Test]
        [TestCaseSource(nameof(Cases))]
        public Option<string> Give(IEnumerable<string> items, Index index) => items.ElementAtOrNone(index);
    }
}