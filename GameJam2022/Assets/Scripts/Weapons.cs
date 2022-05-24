using UnityEngine;
using System;

public enum WeaponType
{
    PISTOL,
    GRENADE,
    FLAMETHROWER,
    LIGHTNING
}

[Serializable]
public abstract class Weapon : ScriptableObject
{
    private static PistolType _pistol;
    private static GrenadeLauncherType _grenade;
    private static FlamethrowerType _flamer;
    private static LightningType _lightning;
    public static PistolType PISTOL
    {
        get
        {
            if (_pistol == null)
            {
                _pistol = Weapon.CreateInstance<PistolType>();
            }
            return _pistol;
        }
    }
    public static FlamethrowerType FLAMETHROWER
    {
        get
        {
            if (_flamer == null)
            {
                _flamer = Weapon.CreateInstance<FlamethrowerType>();
            }
            return _flamer;
        }
    }
    public static GrenadeLauncherType GRENADE
    {
        get
        {
            if (_grenade == null)
            {
                _grenade = Weapon.CreateInstance<GrenadeLauncherType>();
            }
            return _grenade;
        }
    }
    public static LightningType LIGHTNING
    {
        get
        {
            if (_lightning == null)
            {
                _lightning = Weapon.CreateInstance<LightningType>();
            }
            return _lightning;
        }
    }
}

[Serializable]
public class PistolType : Weapon
{
    public int
        maxClip = 50,
        minRounds = 4,
        maxRounds = 20,
        bulletsPerShot = 1;
    public float
        fireRate = 2.2f,
        spreadRatio = 1f,
        damagePerShot = 10f;
}

[Serializable]
public class FlamethrowerType : Weapon
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
}

[Serializable]
public class GrenadeLauncherType : Weapon
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
}

[Serializable]
public class LightningType : Weapon
{
    public int
        maxEnemies = 5,
        minMag = 3,
        maxMag = 6,
        maxClip = 10;
    public float
        baseDamage = 100f,
        falloff = 10f,
        disableTime = 4f;
}
