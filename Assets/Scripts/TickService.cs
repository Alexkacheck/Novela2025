using System;
using System.Timers;

public class TickService : ITickService
{
    public event Action OnTick;

    private readonly Timer _timer;

    public TickService(float interval = 1000f)
    {
        _timer = new Timer(interval);
        _timer.Elapsed += OnTimerElapsed;
        _timer.AutoReset = true;
        _timer.Enabled = true;
    }

    private void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {
        OnTick?.Invoke();
    }
}
