using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPuertas : MonoBehaviour
{
    public Animator AnimPuertas;
    public int energiaNecesaria;
    public int energia;
    public bool puertaAbierta = false;

    // Start is called before the first frame update
    void Start()
    {
        //AnimPuertas = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(energiaNecesaria==energia&&!puertaAbierta)
        {
            print("Abrir puerta");
            //ABRIR PUERTA
            AnimPuertas.Play("AbrirPuerta2");
            puertaAbierta = true;
        }
    }
}
