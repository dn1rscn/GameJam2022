using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     Acts like an inverse trigger detector. Whenever a trigger
///     feeds with the collision event, it is reflected back to the trigger.
/// </summary>
public class TriggerReflector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        other.gameObject.GetComponent<ITriggerAcceptor>()?.OnTriggerEnter(transform.gameObject, other);
    }
    private void OnTriggerExit(Collider other) {
        other.gameObject.GetComponent<ITriggerAcceptor>()?.OnTriggerExit(transform.gameObject, other);
    }
    private void OnTriggerStay(Collider other) {
        other.gameObject.GetComponent<ITriggerAcceptor>()?.OnTriggerStay(transform.gameObject, other);
    }
}
