using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///   ピカチュウみたいね？　ピカちゃんは凄くピカピカ✨！
///   後で、雷⚡ができった…
/// </summary>
public class LightningBehavior : MonoBehaviour, IShooting
{
    public int
        maxEnemies = 5,
        minMag = 3,
        maxMag = 6,
        maxClip = 10;

    public float
        baseDamage = 100f,
        radius = 50f,
        windupTime = 1f,
        coolDownTime = 2f,
        falloff = 10f,
        disableTime = 4f;

    private WeaponClock clock = new WeaponClock();

    public bool CanShoot()
    {
        return clock.Ready;
    }

    void IShooting.Shoot()
    {
        clock.Reset();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
