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
        disableTime = 4f,
        beamOffset = 0.9f;

    private WeaponClock clock = new WeaponClock();
    private GameObject beamPhantasm;

    public bool CanShoot()
    {
        return clock.Ready;
    }

    void IShooting.Shoot()
    {
        clock.Reset();
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, -transform.forward, out hit, Mathf.Infinity))
        {
            var script = hit.collider.gameObject.GetComponentInParent<IDamageAcceptor>();
            if (script != null)
            {
                var beam = Instantiate(beamPhantasm, transform.position, transform.rotation);
                beam.SetActive(true);
                var start = beam.transform.Find("Start").gameObject;
                var end = beam.transform.Find("End").gameObject;
                start.transform.position = transform.position + -transform.forward * beamOffset;
                end.transform.position = hit.point;
                script.TakeDamage(new Damage(baseDamage, Damage.Type.ELECTRIC));
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        beamPhantasm = transform.Find("BeamPhantasm").gameObject;
        clock.Time = (long)(this.coolDownTime * 1000);
        clock.Jitter = 0;
        clock.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
