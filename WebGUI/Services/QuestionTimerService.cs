namespace WebGUI.Services;

public sealed class QuestionTimerService : IDisposable
{
    private readonly object syncRoot = new();
    private CancellationTokenSource? cancellationTokenSource;

    public static int DefaultSeconds { get; set; } = 30;

    public int TimeLeft { get; private set; } = DefaultSeconds;

    public bool IsRunning { get; private set; }

    public void Dispose()
    {
        Stop();
    }

    public event Action? Changed;

    public event Action? Completed;

    public void Start(int? seconds = null)
    {
        var duration = ResolveDuration(seconds);

        Stop();

        TimeLeft = duration;
        IsRunning = true;
        Changed?.Invoke();

        var nextTokenSource = new CancellationTokenSource();

        lock (syncRoot)
        {
            cancellationTokenSource = nextTokenSource;
        }

        _ = RunCountdownAsync(nextTokenSource.Token);
    }

    public void Stop()
    {
        CancellationTokenSource? tokenSourceToCancel;

        lock (syncRoot)
        {
            tokenSourceToCancel = cancellationTokenSource;
            cancellationTokenSource = null;
            IsRunning = false;
        }

        tokenSourceToCancel?.Cancel();
        tokenSourceToCancel?.Dispose();
    }

    private static int ResolveDuration(int? seconds)
    {
        if (seconds.HasValue && seconds.Value > 0)
        {
            DefaultSeconds = seconds.Value;
        }

        return DefaultSeconds > 0 ? DefaultSeconds : 30;
    }

    private async Task RunCountdownAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested && TimeLeft > 0)
            {
                await Task.Delay(1000, cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                TimeLeft--;
                Changed?.Invoke();
            }

            if (!cancellationToken.IsCancellationRequested)
            {
                IsRunning = false;
                Completed?.Invoke();
            }
        }
        catch (OperationCanceledException)
        {
        }
    }
}
