using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ControlHub : MonoBehaviour
{
    public TMP_Text textoMunicion;
    public Image imagenMunicion;
    [Header("iconos munición")]
    public Sprite[] iconos;
    public void ActualizarHub(int _municion,int _arma)
    {
        print("Actualizamos Hub");
        textoMunicion.text = _municion.ToString();
        imagenMunicion.sprite = iconos[_arma];
    }
}
