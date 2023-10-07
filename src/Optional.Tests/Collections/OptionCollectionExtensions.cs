namespace Optional.Tests.Collections;

public static partial class OptionCollectionExtensions
{
    private static IComparer<int> ByLastDigit => Comparation.Order.Of<int>().By(number => number % 10);
}