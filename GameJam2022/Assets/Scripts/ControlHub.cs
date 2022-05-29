using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ControlHub : MonoBehaviour
{
    [Header("Munición")]
    public TMP_Text textoMunicion;
    public Image imagenMunicion;
    public Sprite[] iconos;

    [Header("Energia")]
    public TMP_Text textoEnergia;
    public void ActualizarHub(int _municion,int _arma)
    {
        print("Actualizamos Hub");
        textoMunicion.text = _municion.ToString();
        imagenMunicion.sprite = iconos[_arma];
    }

    public void ActualizarHubEnergia(int _energia)
    {
        textoEnergia.text = _energia.ToString();
    }
}
