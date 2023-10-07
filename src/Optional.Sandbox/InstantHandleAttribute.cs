#if NET7_0
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Optional.Collections;

namespace Optional.Sandbox;

public static class InstantHandleAttribute
{
    public static async Task RequireAwait(IAsyncEnumerable<int> a)
    {
        await using var stream = new FileStream("a", FileMode.Append);

        var _ = a.MaxByOrNoneAsync(_ => stream.ReadByte());
    }
}
#endif