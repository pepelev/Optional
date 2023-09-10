using System;
using System.Diagnostics;

namespace Optional.Sandbox;

public static class Timing
{
    public static double RunningTimeInMs<TResult>(Func<TResult> action, int count)
    {
        return RunningTimeInMs(() => { _ = action(); }, count);
    }

    public static double RunningTimeInMs(Action action, int count)
    {
        var sw = Stopwatch.StartNew();
        for (var i = 0; i < count; i++)
        {
            action();
        }

        var ticks = sw.ElapsedTicks;

        return 1000.0 * ticks / ((double)Stopwatch.Frequency * count);
    }
}