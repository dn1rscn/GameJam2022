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

    private HashSet<Collider> electrified = new HashSet<Collider>();

    /// <summary>
    ///     Recursive function that looks for enemies that can be attacked,
    ///     in order to chain the lightning bolt.
    /// </summary>
    void ChainNextAttack(int left, Vector3 origin)
    {
        if (left <= 0) return;
        var colliders = Physics.OverlapSphere(origin, radius);
        Collider nearest = null;
        var lastDist = 0.0f;
        foreach (var col in colliders)
        {
            if (nearest == null)
            {
                nearest = col;
                lastDist = Vector3.Distance(origin, nearest.transform.position);
                continue;
            }
            var dist = Vector3.Distance(origin, col.transform.position);
            if (dist == 0 || electrified.Contains(col)) continue;
            if (lastDist > dist)
            {
                lastDist = dist;
                nearest = col;
            }
        }
        if (nearest == null) return;
        var acceptor = nearest.GetComponentInParent<IDamageAcceptor>();
        if (acceptor != null)
        {
            electrified.Add(nearest);
            SpawnBeam(origin, nearest.transform.position, acceptor);
            ChainNextAttack(left - 1, nearest.transform.position);
        }
    }

    void SpawnBeam(Vector3 from, Vector3 to, IDamageAcceptor target)
    {
        var beam = Instantiate(beamPhantasm, transform.position, transform.rotation);
        beam.GetComponent<BeamPhantasmBehavior>().Radius = radius;
        beam.SetActive(true);
        var start = beam.transform.Find("Start").gameObject;
        var end = beam.transform.Find("End").gameObject;
        start.transform.position = from;
        end.transform.position = to;
        target.TakeDamage(new Damage(baseDamage, Damage.Type.ELECTRIC));
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
                SpawnBeam(transform.position + -transform.forward * beamOffset, hit.point, script);
                ChainNextAttack(maxEnemies - 1, hit.collider.transform.position);
                electrified.Clear();
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
