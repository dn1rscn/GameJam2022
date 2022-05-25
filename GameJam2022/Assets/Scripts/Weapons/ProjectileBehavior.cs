using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class ProjectileBehavior : MonoBehaviour
{
    private Stopwatch timer = new Stopwatch();

    public float Lifetime { set; get; } = 1000f;
    public float BaseDamage { get; set; } = 0f;

    // Start is called before the first frame update
    void Start()
    {
        timer.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer.ElapsedMilliseconds > Lifetime)
        {
            Destroy(this.transform.gameObject);
        }
    }
}
