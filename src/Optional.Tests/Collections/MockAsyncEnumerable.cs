using static Optional.Tests.Collections.MockAsyncEnumerable;

namespace Optional.Tests.Collections;

public static class MockAsyncEnumerable
{
    public enum Stage
    {
        GetEnumeratorCall,
        MoveNextCall,
        CurrentCall,
        DisposeCall,
        TokenCancellation
    }
}

public sealed class MockAsyncEnumerable<T> : IAsyncEnumerable<T>, IDisposable
{
    private readonly IAsyncEnumerable<T> inner;
    private readonly Stage[] allExpectedStages;
    private readonly Queue<Stage> expectedStages;
    private readonly CancellationTokenSource tokenSource = new();

    public MockAsyncEnumerable(IAsyncEnumerable<T> inner, params Stage[] expectedStages)
    {
        this.inner = inner;
        allExpectedStages = expectedStages;
        this.expectedStages = new Queue<Stage>(allExpectedStages);
        Token = tokenSource.Token;
    }

    public CancellationToken Token { get; }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = new())
    {
        Match(Stage.GetEnumeratorCall);
        return new Enumerator(this, inner.GetAsyncEnumerator(cancellationToken));
    }

    public void Dispose()
    {
        tokenSource.Dispose();
    }

    public override string ToString()
    {
        var list = allExpectedStages
            .Select(stage => stage.ToString("G"))
            .ToList();
        var currentPosition = allExpectedStages.Length - expectedStages.Count;
        list.Insert(currentPosition, "<- HERE ->");
        return string.Join("\n", list);
    }

    private void Match(Stage actualStage)
    {
        while (true)
        {
            if (expectedStages.Count == 0)
            {
                throw new Exception($"No more stages expected\n\n{this}");
            }

            var expectedStage = expectedStages.Dequeue();
            if (expectedStage == Stage.TokenCancellation)
            {
                tokenSource.Cancel();
                continue;
            }

            if (expectedStage == actualStage)
            {
                return;
            }

            throw new Exception($"Expected {expectedStage}, but {actualStage} meet\n\n{this}");
        }
    }

    private sealed class Enumerator : IAsyncEnumerator<T>
    {
        private readonly MockAsyncEnumerable<T> parent;
        private readonly IAsyncEnumerator<T> inner;

        public Enumerator(MockAsyncEnumerable<T> parent, IAsyncEnumerator<T> inner)
        {
            this.parent = parent;
            this.inner = inner;
        }

        public ValueTask DisposeAsync()
        {
            parent.Match(Stage.DisposeCall);
            return inner.DisposeAsync();
        }

        public ValueTask<bool> MoveNextAsync()
        {
            parent.Match(Stage.MoveNextCall);
            return inner.MoveNextAsync();
        }

        public T Current
        {
            get
            {
                parent.Match(Stage.CurrentCall);
                return inner.Current;
            }
        }
    }
}