using System.Collections.Generic;
using UnityEngine;

public interface ITriggerEnterListener {
    void OnTriggerEnter(GameObject source, Collider other);
}

public class TriggerEnterDispatcher : MonoBehaviour {
    private ITriggerEnterListener[] listeners;
    private void Start() {
        listeners = GetComponentsInParent<ITriggerEnterListener>();
    }
    private void OnTriggerEnter(Collider other) {
        foreach (var listener in listeners)
            listener.OnTriggerEnter(transform.gameObject, other);
    }
}