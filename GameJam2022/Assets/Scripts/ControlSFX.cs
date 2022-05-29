using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlSFX : MonoBehaviour
{

    GameObject Canvas;
    GameObject geometria_Arma;
    public GameObject vfxDisparo;


    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("Canvas"))
        {
            Canvas = GameObject.Find("Canvas");
        }

        if (GameObject.Find("GeometriaArma"))
        {
            geometria_Arma = GameObject.Find("GeometriaArma");
        }
    }

    public void sfx_recogerEnergia ()
    {
        print("ActivarAnimacionCanvasEnergia");
        Canvas.GetComponent<Animator>().Play("Anim_RecogerEnergia_Canvas");
    }

    public void sfx_recogerMunicion ()
    {
        Canvas.GetComponent<Animator>().Play("Anim_RecogerMunicion_Canvas");
    }

    public void sfx_Disparo()
    {
        vfxDisparo.SetActive(true);
        Invoke("sfx_Disparo_Desactivar", 0.5f);
    }

    void sfx_Disparo_Desactivar()
    {
        vfxDisparo.SetActive(false);
    }

    public void sfx_Muerte()
    {
        Canvas.GetComponent<Animator>().Play("Anim_Muerte_Canvas");
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
