using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPuertas : MonoBehaviour
{
    public Animator AnimPuertas;
    public int energiaNecesaria;
    public int energia;
    public bool puertaAbierta = false;
    public LineRenderer lineaEnergiaRoja;
    public LineRenderer lineaEnergiaAzul;
    public bool darEnergia;
    float velocidadAnim = 1.5f,valor;

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
        //ActualizarEnergia();
        if(darEnergia)
        {
            valor += velocidadAnim * Time.deltaTime * ((8.2f * energia) / energiaNecesaria);
            lineaEnergiaRoja.SetPosition(0, new Vector3(0, valor, 0));
            lineaEnergiaAzul.SetPosition(1, new Vector3(0, valor, 0));
            if (valor >= ((8.2f * energia) / energiaNecesaria))
            {
                darEnergia = false;
            }
        }
    }

    public void ActualizarEnergia()
    {
        valor += velocidadAnim * Time.deltaTime * ((8.2f * energia) / energiaNecesaria);
        darEnergia = true;
    }
}
