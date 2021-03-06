using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prota_Anims : MonoBehaviour
{

    Animator animatorPlayer;
    bool andar, correr;

    // Start is called before the first frame update
    void Start()
    {
        if(gameObject.GetComponent<Animator>())
        {
            animatorPlayer = gameObject.GetComponent<Animator>();
        }
    }


    // Update is called once per frame
    void Update()
    {
        /*if(Input.GetAxis("Horizontal") >= 0.5f || Input.GetAxis("Horizontal") <= -0.5f)
        {
            animatorPlayer.SetFloat("EjeX", Input.GetAxis("Horizontal"));
        }
        if (Input.GetAxis("Vertical") >= 0.5f || Input.GetAxis("Vertical") <= -0.5f)
        {
            animatorPlayer.SetFloat("EjeZ", Input.GetAxis("Vertical"));
        }
        */

        /*
        animatorPlayer.SetFloat("EjeX", GameObject.FindGameObjectWithTag("Prota").GetComponent<Rigidbody>().velocity.x);
        animatorPlayer.SetFloat("EjeZ", GameObject.FindGameObjectWithTag("Prota").GetComponent<Rigidbody>().velocity.z);
        */
    }


    public void Andar(Vector3 direccion)
    {
        //andar = true;
        animatorPlayer.SetFloat("EjeX", direccion.x);
        animatorPlayer.SetFloat("EjeZ", direccion.z);
        if(direccion.x!=0 || direccion.z!=0) animatorPlayer.SetBool("andar",true);
    }
    public void AndarOff()
    {
        //andar = false;
        animatorPlayer.SetBool("andar", false);
    }

    public void Correr(Vector3 direccion)
    {
        if (direccion.x < -0.8f || 0.8f < direccion.x || direccion.z < -0.8f || 0.8f < direccion.z)
        {
            if (direccion.x > 0.8) animatorPlayer.SetFloat("EjeX", direccion.x + 0.3f);
            if (direccion.x < -0.8) animatorPlayer.SetFloat("EjeX", direccion.x - 0.3f);
            if (direccion.z > 0.8) animatorPlayer.SetFloat("EjeX", direccion.z + 0.3f);
            if (direccion.z < -0.8) animatorPlayer.SetFloat("EjeZ", direccion.z - 0.3f);
            print("CORREMOS");
        }
        animatorPlayer.SetBool("correr", true);

    }
    public void CorrerOff()
    {
        animatorPlayer.SetBool("correr", false);
    }

    public void Apuntar()
    {
        animatorPlayer.SetLayerWeight(1, 1.0f);
    }

    public void ApuntarOff()
    {
        animatorPlayer.SetLayerWeight(1, 0.0f);
    }

    public void Muerte()
    {
        Time.timeScale = 0.25f;
        Invoke("tiempoNormal", 5.0f);
        animatorPlayer.Play("MuerteProta");

    }

    void tiempoNormal()
    {
        Time.timeScale = 1.0f;
    }

}
