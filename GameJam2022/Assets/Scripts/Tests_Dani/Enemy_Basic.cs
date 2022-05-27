using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Basic : MonoBehaviour
{

    GameObject Prota;

    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.FindGameObjectWithTag("Prota"))
        {
            Prota = GameObject.FindGameObjectWithTag("Prota");
        }

    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<Rigidbody>().transform.LookAt(Prota.transform);
    }
}
