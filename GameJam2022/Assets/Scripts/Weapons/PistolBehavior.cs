using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Displays pistol properties</summary>
public class PistolBehavior : MonoBehaviour, IShooting
{
    public GameObject projectile;
    public int
        maxClip = 50,
        minRounds = 4,
        maxRounds = 20,
        bulletsPerShot = 1;

    public float
        bulletSpeed = 1000f,
        fireRate = 2f,
        fireRateJitter = 0f,
        spreadCone = 0f,
        damagePerShot = 10f,
        bulletLifetime = 5000f;

    private WeaponClock clock = new WeaponClock();

    public bool CanShoot()
    {
        return clock.Ready;
    }

    public void Shoot()
    {
        clock.Reset();
        var dir = -transform.forward * bulletSpeed;
        var orig = projectile.transform;
        var bullet = Instantiate(projectile, orig.position, orig.rotation);
        var script = bullet.GetComponent<ProjectileBehavior>();
        script.Lifetime = bulletLifetime;
        script.BaseDamage = damagePerShot;
        bullet.SetActive(true);
        var phys = bullet.GetComponent<Rigidbody>();
        phys.AddForce(dir);
        phys.freezeRotation = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        clock.Time = (long)((1d / fireRate) * 1000d);
        clock.Jitter = fireRateJitter * 1000f;
        clock.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
