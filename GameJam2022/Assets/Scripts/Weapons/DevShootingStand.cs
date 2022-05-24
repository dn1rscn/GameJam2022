using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevShootingStand : MonoBehaviour
{
    public GameObject pistol, flamer, grenade, lightning;

    private IShooting[] weapons;

    // Start is called before the first frame update
    void Start()
    {
        weapons = new IShooting[]{
            pistol.GetComponent<IShooting>(),
            flamer.GetComponent<IShooting>(),
            grenade.GetComponent<IShooting>(),
            lightning.GetComponent<IShooting>()
        };
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var weapon in weapons)
        {
            if (weapon.CanShoot())
                weapon.Shoot();
        }
    }
}
