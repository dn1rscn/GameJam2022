using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlUI : MonoBehaviour
{
    public GameObject uiInicio;
    public GameObject uiKill;

    ControlPlayer player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<ControlPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (uiInicio.activeInHierarchy)
        {
            if (Input.anyKey)
            {
                // print("key");
                EmpezarJuego();
            }
        }
    }

    public void EmpezarJuego()
    {
        //uiInicio.SetActive(false);
        uiInicio.GetComponent<Animator>().Play("Anim_FundidoNegro");
        Invoke("habilitarMovimiento", 2.0f);
    }

    void habilitarMovimiento()
    {
        player.HabilitarMovimiento();

    }

    public void reintentar()
    {
        uiKill.SetActive(false);
        player.vida = 2;
        player.HabilitarMovimiento();
    }

    public void VolverMenu()
    {
        uiInicio.SetActive(true);
    }

}
