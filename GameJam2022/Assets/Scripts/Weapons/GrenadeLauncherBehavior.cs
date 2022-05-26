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
        launchForce = 500f,
        splashRadius = 10f,
        splashFalloff = 1f,
        bounciness = 1f,
        detonationTime = 4f;

    private WeaponClock clock = new WeaponClock();
    private GameObject grenade;

    public bool CanShoot()
    {
        return clock.Ready;
    }

    public void Shoot()
    {
        clock.Reset();
        var dir = -transform.forward * launchForce;
        var orig = grenade.transform;
        var shot = Instantiate(grenade, orig.position, orig.rotation);
        var script = shot.GetComponent<GrenadeBehavior>();
        script.Lifetime = detonationTime * 1000f;
        script.BaseDamage = damagePerGrenade;
        script.Falloff = splashFalloff;
        script.SplashRadius = splashRadius;
        shot.SetActive(true);
        var phys = shot.GetComponent<Rigidbody>();
        phys.AddForce(dir);
    }

    // Start is called before the first frame update
    void Start()
    {
        clock.Time = (long)((1d / fireRate) * 1000d);
        clock.Jitter = 0;
        clock.Start();
        grenade = transform.Find("Grenade").gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
