using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Obsolete]
public class SimpleCollisionPropagator : MonoBehaviour
{
    private ICollisionAcceptor[] listeners;
    void Start()
    {
        listeners = this.GetComponentsInParent<ICollisionAcceptor>();
    }

    void OnCollisionEnter(Collision other)
    {
        foreach (var listener in listeners)
        {
            listener.OnCollisionEnter(gameObject, other);
        }
    }
    void OnCollisionExit(Collision other)
    {
        foreach (var listener in listeners)
        {
            listener.OnCollisionExit(gameObject, other);
        }
    }
    void OnCollisionStay(Collision other)
    {
        foreach (var listener in listeners)
        {
            listener.OnCollisionStay(gameObject, other);
        }
    }
}
