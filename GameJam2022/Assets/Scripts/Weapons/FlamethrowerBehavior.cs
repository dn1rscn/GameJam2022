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
    private GameObject flames;
    private bool shooting = false;

    public bool CanShoot()
    {
        return true;
    }

    public void Shoot()
    {
        shooting = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        flames = transform.Find("Flames").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //flames.SetActive(shooting);
        if (shooting)
        {
            flames.GetComponent<ParticleSystem>().Play();
            foreach (var acceptor in acceptors)
            {
                acceptor.TakeDamage(new Damage(this.damagePerSecond, Damage.Type.HEAT));
            }
        } else if (!shooting)
        {
            flames.GetComponent<ParticleSystem>().Stop();

        }
        shooting = false;
    }

    private HashSet<IDamageAcceptor> acceptors = new HashSet<IDamageAcceptor>();

    void OnTriggerEnter(Collider other)
    {
        var script = other.GetComponentInParent<IDamageAcceptor>();
        if (script != null)
        {
            Debug.Log("Added enemy!");
            acceptors.Add(script);
        }
    }

    void OnTriggerExit(Collider other)
    {
        var script = other.GetComponentInParent<IDamageAcceptor>();
        if (script != null)
        {
            acceptors.Remove(script);
        }
    }
}
