using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     A weapon clock is an utility helper that tracks the shooting
///     intervals of your weapon.
/// </summary>
public class WeaponClock
{
    public bool Ready { get; }
    public void Reset() { }
    public float Time { get; set; } = 1f;
    public float Jitter { get; set; } = 0f;
}
