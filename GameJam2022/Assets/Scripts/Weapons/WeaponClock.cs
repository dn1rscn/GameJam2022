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
    public bool Ready { get => (timer.ElapsedMilliseconds + lastJitter) >= Time; }
    public void Reset()
    {
        lastJitter = (long)Random.Range(0f, Jitter);
        timer.Reset();
    }
    public void Start()
    {
        timer.Start();
    }
    public void Stop()
    {
        timer.Stop();
    }
    public long Time { get; set; } = 1000;
    public float Jitter { get; set; } = 0f;
}
