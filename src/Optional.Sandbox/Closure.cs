using System.Collections.Generic;
using System.IO;
using System.Linq;
using Optional.Collections;

namespace Optional.Sandbox;

public static class Closure
{
    public static void Disposable(Option<int> a)
    {
        using var stream = new MemoryStream();

        var option = a.Map(x => x + stream.ReadByte());
    }

    public static void A(IEnumerable<Option<int>> sequence)
    {
        using var stream = new MemoryStream();
        var lastOrNone =
            sequence
                .Select(x => x)
                .Values()
                .Select(p => p + stream.ReadByte())
                .ElementAtOrNone(5);
    }
}