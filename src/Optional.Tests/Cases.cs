namespace Optional.Tests;

public static class Cases
{
    public static TestCaseData Case(string name, params object[] arguments) =>
        new TestCaseData(arguments).SetName($"{{m}}.{name}");
}