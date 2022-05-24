using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Displays flamethrower properties</summary>
public class FlamethrowerBehavior : MonoBehaviour, IShooting
{
    public float
        spreadAngle = 20f,
        damagePerSecond = 4f,
        distance = 10f,
        particlesPerSecond = 10f,
        particleSpeed = 5f,
        maxPetrol = 100f,
        minPetrol = 10f,
        tankSize = 200f;

    private WeaponClock clock = new WeaponClock();

    public bool CanShoot()
    {
        return clock.Ready;
    }

    public void Shoot()
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
