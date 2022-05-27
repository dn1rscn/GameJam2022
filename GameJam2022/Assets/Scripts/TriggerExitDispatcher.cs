using System.Collections.Generic;
using UnityEngine;

public interface ITriggerExitListener
{
    void OnTriggerExit(GameObject source, Collider other);
}

public class TriggerExitDispatcher : MonoBehaviour
{
    private ITriggerExitListener[] listeners;
    private void Start()
    {
        listeners = GetComponentsInParent<ITriggerExitListener>();
    }
    private void OnTriggerExit(Collider other)
    {
        foreach (var listener in listeners)
            listener.OnTriggerExit(transform.gameObject, other);
    }
}