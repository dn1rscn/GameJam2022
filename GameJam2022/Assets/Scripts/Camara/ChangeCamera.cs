using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ChangeCamera : MonoBehaviour
{ 
    [SerializeField]
    
    private CinemachineFreeLook _followCam;


    [SerializeField]

    private CinemachineVirtualCamera _staticCam;


    void OnTriggerEnter(Collider coli)
    {
        print("coli.name");
        if (coli.name == "ColliderCam_PuertaFinal")
        {
            print("Has chocado con la puerta final");
            _followCam.Priority = 0;
            _staticCam.Priority = 10;

        }
    }

    void OnTriggerExit(Collider coli)
    {
        if (coli.name == "ColliderCam_PuertaFinal")
        {
            print("Has salido de la puerta final");
            _followCam.Priority = 10;
            _staticCam.Priority = 0;
        }
       }
    
}
