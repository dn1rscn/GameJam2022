using System;
using UnityEngine;

/// <summary>
///     A simple collision event listener interface.
/// </summary>
[Obsolete]
public interface ICollisionAcceptor
{
    void OnCollisionEnter(GameObject from, Collision other) { }
    void OnCollisionExit(GameObject from, Collision other) { }
    void OnCollisionStay(GameObject from, Collision other) { }
}