using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRobotBehavior : MonoBehaviour, IDamageAcceptor, ITriggerEnterListener, ITriggerExitListener
{
    abstract class State
    {
        private EnemyRobotBehavior actor;
        public State(EnemyRobotBehavior actor) { this.actor = actor; }
        public EnemyRobotBehavior Actor { get => actor; }
        public virtual void Update() { }
        public virtual void Start() { }
        public virtual void TriggerAwake(Collider collider) { }
        public virtual void TriggerHear(Collider collider) { }
        public virtual State GoPatrol() { return new Patrol(Actor); }
        public virtual State GoDormant() { return new Dormant(Actor); }
        public virtual State GoVigilant() { return new Vigilant(Actor); }
        public virtual State GoChasing(GameObject target) { return new Chasing(Actor, target); }
        public virtual void OnCollisionEnter(Collision other) { }
    }

    class Patrol : State
    {
        public bool willSleep = false;
        public Vector3 origin;
        public Patrol(EnemyRobotBehavior actor) : base(actor)
        {
        }
    }
    abstract class ShutdownMode : State
    {
        protected ShutdownMode(EnemyRobotBehavior actor) : base(actor)
        {
        }
        public override void Start()
        {
            Actor.animator.Play("Enemigo_Activarse");
            Actor.animator.speed = 0;
        }
        override public State GoPatrol()
        {
            var state = new Patrol(Actor);
            state.willSleep = true;
            state.origin = Actor.transform.position;
            return state;
        }
        IEnumerator AwakeRoutine()
        {
            Actor.animator.speed = 1;
            yield return new WaitForSeconds(2f);
            Actor.currentState = new Patrol(Actor);
            yield return null;
            Actor.currentState.Start();
        }
        override public void TriggerAwake(UnityEngine.Collider collider)
        {
            Actor.StartCoroutine(AwakeRoutine());
        }
    }
    class Dormant : ShutdownMode
    {
        public Dormant(EnemyRobotBehavior actor) : base(actor) { }
    }

    class Vigilant : ShutdownMode
    {
        public Vigilant(EnemyRobotBehavior actor) : base(actor) { }
    }

    class Chasing : State
    {
        public GameObject target;
        public Chasing(EnemyRobotBehavior actor, GameObject target) : base(actor) { this.target = target; }
    }
    public enum InitialState
    {
        Dormant,
        Vigilant,
        Awake
    }

    [Header("Initial attributes")]
    public InitialState initialState = InitialState.Vigilant;
    [Header("Intrinsic attributes")]
    public float maxHealth = 100f;
    public float initialHealth = 100f;
    [Range(0, .9f)]
    public float armorReduction = 0;

    /* PRIVATE FIELDS */
    private State currentState;
    private GameObject wakeGO, hearGO;
    private Animator animator;
    private float health;

    /// <summary> Called when health reaches 0 after damage. </summary>
    void Die() { }

    void IDamageAcceptor.TakeDamage(Damage incoming)
    {
        if (incoming.type != Damage.Type.KINETIC) return;
        var damage = incoming.ApplyReduction(armorReduction);
        health -= damage.amount;
        if (health <= 0)
        {
            health = 0;
            Die();
        }
        Debug.Log($"Robot absorbs {damage.amount} of {incoming} damage.");
    }

    // Start is called before the first frame update
    void Start()
    {
        wakeGO = transform.Find("AwakeSphere").gameObject;
        hearGO = transform.Find("HearSphere").gameObject;
        animator = transform.Find("Todas").GetComponent<Animator>();
        switch (initialState)
        {
            case InitialState.Vigilant:
                currentState = new Vigilant(this);
                break;
            case InitialState.Dormant:
                currentState = new Dormant(this);
                break;
            case InitialState.Awake:
                currentState = new Patrol(this);
                break;
        }
        currentState.Start();
    }

    // Update is called once per frame
    void Update()
    {
        currentState.Update();
    }

    private bool playerInRadius = false;
    void ITriggerEnterListener.OnTriggerEnter(GameObject source, Collider other)
    {
        if (other.tag != "Player") return;
        playerInRadius = true;
        if (source == wakeGO)
            currentState.TriggerAwake(other);
        else if (source == hearGO)
            currentState.TriggerHear(other);
    }
    void ITriggerExitListener.OnTriggerExit(GameObject source, Collider other)
    {
        if (other.tag != "Player") return;
        if (source == hearGO)
            playerInRadius = false;
    }
}
