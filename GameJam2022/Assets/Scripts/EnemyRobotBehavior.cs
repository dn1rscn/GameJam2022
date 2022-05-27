using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRobotBehavior : MonoBehaviour, IDamageAcceptor, ITriggerEnterListener, ITriggerExitListener
{
    const int ANIMATION_IDLE = 0;
    const int ANIMATION_WALK = 1;
    const int ANIMATION_CHASE = 2;
    const int ANIMATION_ATTACK = 3;
    abstract class State
    {
        private EnemyRobotBehavior actor;
        public State(EnemyRobotBehavior actor) { this.actor = actor; }
        public EnemyRobotBehavior Actor { get => actor; }
        public virtual void Update() { }
        public virtual void Start() { }
        public virtual void TriggerAwake(Collider collider) { }
        public virtual void TriggerHear(Collider collider) { }
    }
    /// <summary> Represents an active patrol state </summary>
    class Patrol : State
    {
        public bool willSleep = false;
        public Patrol(EnemyRobotBehavior actor) : base(actor) { }

        IEnumerator PatrolAndWait()
        {
            Actor.Animate(ANIMATION_WALK);
            // Actor.animator.Play("Enemigo_Andar");
            var r = Actor.patrolRadiusFromOrigin;
            var x = Random.Range(-r, r);
            var z = Random.Range(-r, r);
            var dest = Actor.origin + new Vector3(x, Actor.origin.y, z);
            Actor.nav.SetDestination(dest);
            yield return new WaitUntil(() => Actor.nav.pathStatus == NavMeshPathStatus.PathComplete);
            var idleTime = Random.Range(Actor.patrolIdleMinTime, Actor.patrolIdleMaxTime);
            // Actor.animator.Play("Enemigo_Standby");
            yield return new WaitForSeconds(1f);
            Actor.Animate(ANIMATION_IDLE);
            yield return new WaitForSeconds(idleTime);
            Actor.StartCoroutine(PatrolAndWait());
        }
        public override void Update()
        {
            if (Actor.playerInRadius && Actor.PlayerInCone)
            {
                Debug.Log("Stopping all routines.");
                Actor.StopAllCoroutines();
                Actor.currentState = new Chasing(Actor);
                Actor.currentState.Start();
            }
        }
        public override void Start()
        {
            Debug.Log("Starting patrol...");
            Actor.StartCoroutine(PatrolAndWait());
        }
    }
    /// <summary> Shared methods for vigilant and dormant </summary>
    abstract class ShutdownMode : State
    {
        protected ShutdownMode(EnemyRobotBehavior actor) : base(actor) { }
        protected IEnumerator AwakeRoutine()
        {
            Actor.animator.speed = 1;
            yield return new WaitForSeconds(6f);
            var patrol = new Patrol(Actor);
            patrol.willSleep = true;
            Actor.currentState = patrol;
            yield return null;
            Actor.currentState.Start();
        }
        public override void Start()
        {
            // Actor.animator.Play("Enemigo_Activarse");
            Actor.animator.speed = 0;
        }
    }
    /// <summary> Represent a sleep state that won't react to players </summary>
    class Dormant : ShutdownMode
    {
        public Dormant(EnemyRobotBehavior actor) : base(actor) { }
    }
    /// <summary> Represents a sleep state that will react to players </summary>
    class Vigilant : ShutdownMode
    {
        public Vigilant(EnemyRobotBehavior actor) : base(actor) { }
        override public void TriggerAwake(UnityEngine.Collider collider)
        {
            Actor.StartCoroutine(AwakeRoutine());
        }
    }

    class Chasing : State
    {
        public Chasing(EnemyRobotBehavior actor) : base(actor) { }
        public override void Start()
        {
            // Actor.animator.Play("Enemigo_Perseguir");
            Actor.Animate(ANIMATION_CHASE);
        }
        IEnumerator GoSleep()
        {
            Debug.Log("Enemy coasting to sleep...");
            // Actor.animator.Play("Enemigo_Standby");
            Actor.Animate(ANIMATION_IDLE);
            yield return new WaitForSeconds(4f);
            Actor.StopAllCoroutines();
            Actor.currentState = new Patrol(Actor);
            Actor.currentState.Start();
        }
        IEnumerator KillFocus()
        {
            yield return new WaitForSeconds(Actor.patrolGiveUpTime);
            Actor.StartCoroutine(GoSleep());
        }
        private bool killingFocus = false;
        public override void Update()
        {
            Actor.nav.SetDestination(Actor.player.transform.position);
            if (!killingFocus && !Actor.playerInRadius)
            {
                Debug.Log("Killing focus (Lost sight)");
                killingFocus = true;
                Actor.StartCoroutine(KillFocus());
            }
        }
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
    [Range(0f, 360f)]
    public float viewCone = 90f;
    [Range(0f, .9f)]
    public float armorReduction = 0;
    public float patrolGiveUpTime = 20f;
    [Range(1f, 100f)]
    public float patrolRadiusFromOrigin = 30f;
    [Range(0f, 60f)]
    public float patrolIdleMaxTime = 10f;
    [Range(0f, 60f)]
    public float patrolIdleMinTime = 2f;

    private Vector3 PlayerDir { get => (player.transform.position - transform.position).normalized; }
    private float FacingFactor { get => Vector3.Dot(transform.forward, PlayerDir); }
    private float PlayerAngle { get => (1f - ((FacingFactor + 1f) / 2f)) * 360f; }
    public bool PlayerInCone { get => PlayerAngle <= viewCone; }

    /* PRIVATE FIELDS */
    private GameObject player;
    private State currentState;
    private GameObject wakeGO, hearGO;
    private Animator animator;
    private float health;
    private NavMeshAgent nav;

    /// <summary> Called when health reaches 0 after damage. </summary>
    void Die() { }

    void Animate(int id)
    {
        animator.SetInteger("state", id);
    }
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

    private Transform body;
    private Vector3 origin;
    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position + new Vector3(0, 1f, 0);
        nav = GetComponent<NavMeshAgent>();
        body = transform.Find("Body");
        var aid = body.Find("VisualAid");
        {
            var cone = aid.Find("ViewCone");
            var lines = cone.transform.GetComponentsInChildren<LineRenderer>();
            var tipPos = lines[0].GetPosition(1);
            var length = -tipPos.magnitude;
            var displacement = length * Mathf.Tan(viewCone * 0.5f);
            lines[0].SetPosition(1, tipPos + new Vector3(displacement, 0, 0));
            lines[1].SetPosition(1, tipPos + new Vector3(-displacement, 0, 0));
        }
        player = GameObject.FindGameObjectWithTag("Player");
        wakeGO = transform.Find("AwakeSphere").gameObject;
        hearGO = transform.Find("HearSphere").gameObject;
        animator = body.Find("Todas").GetComponent<Animator>();
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
        if (other.gameObject != player) return;
        playerInRadius = true;
        if (source == wakeGO)
            currentState.TriggerAwake(other);
        else if (source == hearGO)
            currentState.TriggerHear(other);
    }
    void ITriggerExitListener.OnTriggerExit(GameObject source, Collider other)
    {
        if (other.gameObject != player) return;
        if (source == hearGO)
            playerInRadius = false;
    }
}
