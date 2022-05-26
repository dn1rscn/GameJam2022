using UnityEngine;

public interface ITriggerAcceptor
{
    void OnTriggerEnter(GameObject source, Collider other) { }
    void OnTriggerExit(GameObject source, Collider other) { }
    void OnTriggerStay(GameObject source, Collider other) { }
}
