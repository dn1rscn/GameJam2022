using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Displays pistol properties</summary>
public class PistolBehavior : MonoBehaviour, IShooting
{
    public int
        maxClip = 50,
        minRounds = 4,
        maxRounds = 20,
        bulletsPerShot = 1;

    public float
        bulletSpeed = 8f,
        fireRate = 2.2f,
        spreadRatio = 1f,
        damagePerShot = 10f;

    private WeaponClock clock = new WeaponClock();

    public bool CanShoot()
    {
        return clock.Ready;
    }

    public void Shoot()
    {
        clock.Reset();
        Debug.Log("Shoot!");
    }

    // Start is called before the first frame update
    void Start()
    {
        clock.Jitter = 4f;
        clock.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
