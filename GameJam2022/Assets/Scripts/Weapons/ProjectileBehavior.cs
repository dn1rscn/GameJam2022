using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class ProjectileBehavior : MonoBehaviour
{
    private Stopwatch timer = new Stopwatch();

    public float Lifetime { set; get; } = 1000f;
    public float BaseDamage { get; set; } = 0f;

    private GameObject AliveEffects, DeadEffects;
    private bool dead = false;

    // Start is called before the first frame update
    void Start()
    {
        timer.Start();
        AliveEffects = transform.Find("AliveEffects").gameObject;
        DeadEffects = transform.Find("DeadEffects").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer.ElapsedMilliseconds > Lifetime || dead && timer.ElapsedMilliseconds > 3000f)
        {
            Destroy(this.transform.gameObject);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        timer.Restart();
        dead = true;
        AliveEffects.SetActive(false);
        DeadEffects.SetActive(true);
        var rigid = GetComponent<Rigidbody>();
        rigid.velocity = new Vector3(0, 0, 0);
        rigid.detectCollisions = false;
    }
}
