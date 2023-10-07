#if !NETCOREAPP1_0_OR_GREATER && !NETSTANDARD2_0_OR_GREATER && !NET47_OR_GREATER
// ReSharper disable CheckNamespace
namespace System;
// ReSharper restore CheckNamespace

internal readonly struct ValueTuple<T1, T2>
{
    public readonly T1 Item1;
    public readonly T2 Item2;

    public ValueTuple(T1 item1, T2 item2)
    {
        Item1 = item1;
        Item2 = item2;
    }
}
#endif