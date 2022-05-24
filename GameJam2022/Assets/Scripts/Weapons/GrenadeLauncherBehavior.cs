using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Displays grenade launcher properties</summary>
public class GrenadeLauncherBehavior : MonoBehaviour, IShooting
{
    public int
        grenadesPerShot = 1,
        maxRounds = 8,
        minRounds = 2,
        maxClip = 10;

    public float
        fireRate = 0.1f,
        damagePerGrenade = 100f,
        splashRadius = 10f,
        splashFalloff = 1f,
        bounciness = 1f,
        detonationTime = 4f;

    private WeaponClock clock = new WeaponClock();

    public bool CanShoot()
    {
        return clock.Ready;
    }

    public void Shoot()
    {
        clock.Reset();
        throw new System.NotImplementedException();
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
