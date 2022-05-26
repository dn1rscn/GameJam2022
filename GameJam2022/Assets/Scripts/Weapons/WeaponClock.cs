using System.Diagnostics;
using UnityEngine;

/// <summary>
///     A weapon clock is an utility helper that tracks the shooting
///     intervals of your weapon.
/// </summary>
public class WeaponClock
{
    private Stopwatch timer = new Stopwatch();
    private long lastJitter = 0;
    /// <summary>Tells if the timer is ready for next shot.</summary>
    public bool Ready { get => timer.ElapsedMilliseconds >= (Time + lastJitter); }
    /// <summary>Resets the clock, used on trigger.</summary>
    public void Reset()
    {
        lastJitter = (long)Random.Range(0f, Jitter);
        timer.Restart();
    }
    /// <summary>Starts the clock.</summary>
    public void Start()
    {
        timer.Start();
        Reset();
    }
    /// <summary>Stops the clock.</summary>
    public void Stop()
    {
        timer.Stop();
    }

    /// <summary>Time between ticks.</summary>
    public long Time { get; set; } = 1000;
    /// <summary>Random probable interval offset.</summary>
    public float Jitter { get; set; } = 0f;
}
