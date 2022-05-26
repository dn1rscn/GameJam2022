using System.Diagnostics;
using UnityEngine;

public class GrenadeBehavior : MonoBehaviour
{
    public float Lifetime { get; set; }
    public float BaseDamage { get; set; }
    public float Falloff { get; set; }
    public float SplashRadius { get; set; }

    private Stopwatch timer = new Stopwatch();
    private bool dead = false;
    private GameObject Alive, Explode;

    // Start is called before the first frame update
    void Start()
    {
        timer.Start();
        Alive = transform.Find("Alive").gameObject;
        Explode = transform.Find("Explode").gameObject;
    }

    /// <summary> Detonates the object. </summary>
    void Kill()
    {
        dead = true;
        transform.rotation.Set(1, 1, 1, 0);
        Destroy(transform.GetComponent<Rigidbody>());
        Alive.SetActive(false);
        Explode.SetActive(true);
        timer.Restart();
        var sph = Physics.OverlapSphere(transform.position, SplashRadius);
        foreach (var elem in sph)
        {
            var script = elem.GetComponentInParent<IDamageAcceptor>();
            if (script == null) continue;
            var dist = Vector3.Distance(elem.transform.position, this.transform.position);
            var damage = BaseDamage * (1f - (dist / SplashRadius));
            script.TakeDamage(new Damage(damage, Damage.Type.EXPLOSIVE));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!dead && timer.ElapsedMilliseconds > Lifetime)
        {
            Kill();
        }
        // Kill the instance after 2 secs of detonation.
        if (dead && timer.ElapsedMilliseconds > 2000f)
        {
            Destroy(this.transform.gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        var found = other.GetComponentInParent<IDamageAcceptor>();
        if (found != null)
        {
            Kill();
        }
    }
}
