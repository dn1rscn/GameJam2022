using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface State
{
    void Update(EnemyRobotBehavior actor) { }
    void Start(EnemyRobotBehavior actor) { }
    State GoPatrol() { return new Patrol(); }
    State GoDormant() { return new Dormant(); }
    State GoVigilant() { return new Vigilant(); }
    State GoChasing(GameObject target) { return new Chasing(target); }
    void OnCollisionEnter(Collision other) { }
}

class Patrol : State
{
    public bool willSleep = false;
}

class Dormant : State
{
    State State.GoPatrol()
    {
        var state = new Patrol();
        state.willSleep = true;
        return state;
    }
}

class Vigilant : State
{
    State State.GoPatrol()
    {
        var state = new Patrol();
        state.willSleep = true;
        return state;
    }

}

class Chasing : State
{
    public GameObject target;
    public Chasing(GameObject target) { this.target = target; }
}

public class EnemyRobotBehavior : MonoBehaviour, IDamageAcceptor, ITriggerEnterListener
{
    public enum InitialState
    {
        Dormant,
        Vigilant,
        Awake
    }
    [Header("Initial attributes")]
    public InitialState initialState = InitialState.Vigilant;

    [Header("Intrinsic attributes")]
    private State currentState;
    private GameObject wakeGO, hearGO;

    public void TakeDamage(Damage damage)
    {
        Debug.Log($"Rawrbot goes doh {damage.amount} of {damage} damage.");
    }

    // Start is called before the first frame update
    void Start()
    {
        wakeGO = transform.Find("AwakeSphere").gameObject;
        hearGO = transform.Find("HearSphere").gameObject;
        switch (initialState)
        {
            case InitialState.Vigilant:
                currentState = new Vigilant();
                break;
            case InitialState.Dormant:
                currentState = new Dormant();
                break;
            case InitialState.Awake:
                currentState = new Patrol();
                break;
        }
        currentState.Start(this);
    }

    // Update is called once per frame
    void Update()
    {
        currentState.Update(this);
    }

    void ITriggerEnterListener.OnTriggerEnter(GameObject source, Collider other)
    {
        Debug.LogError("FAFAFAFA");
        // if (source == wakeGO)
        // {
        //     Debug.LogWarning($"I SENSE NEAR! you, {other}...");
        // }
        // else if (from == hearGO)
        // {
        //     Debug.LogWarning($"I hear you, {other}...");
        // }
    }
}
