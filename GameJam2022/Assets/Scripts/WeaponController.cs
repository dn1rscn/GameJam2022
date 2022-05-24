using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/// <summary>
///   A simple controller that will react on input and
///   Base parameters.
/// </summary>
[Serializable]
[ExecuteInEditMode]
public class WeaponController : MonoBehaviour
{
    [SerializeField]
    public Weapon weapon;

    [SerializeField]
    public GameObject pistolBody, flamerBody, grenadeBody, lightningBody;

    IEnumerable<GameObject> Bodies
    {
        get
        {
            yield return pistolBody;
            yield return flamerBody;
            yield return grenadeBody;
            yield return lightningBody;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var body in Bodies) body.SetActive(false);
        if (weapon is PistolType)
        {
            pistolBody.SetActive(true);
        }
        else if (weapon is GrenadeLauncherType)
        {
            grenadeBody.SetActive(true);
        }
        else if (weapon is FlamethrowerType)
        {
            flamerBody.SetActive(true);
        }
        else if (weapon is LightningType)
        {
            lightningBody.SetActive(true);
        }
    }
}
