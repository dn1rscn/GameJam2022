using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRobotBehavior : MonoBehaviour, IDamageAcceptor, ITriggerEnterListener, ITriggerExitListener, IEnemyAnimationReceiver
{
    const int ANIMATION_IDLE = 0;
    const int ANIMATION_WALK = 1;
    const int ANIMATION_CHASE = 2;
    const int ANIMATION_ATTACK = 3;
    const int ANIMATION_SLEEP = 4;
    abstract class State
    {
        public void Log(string msg)
        {
            Debug.Log($"AI: {msg}");
        }
        public abstract string Tag { get; }
        private EnemyRobotBehavior actor;
        public State(EnemyRobotBehavior actor) { this.actor = actor; }
        public EnemyRobotBehavior Actor { get => actor; }
        public virtual void Update() { }
        public virtual void Start()
        {
            Log($"Starting {Tag} mode");
        }
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
            Actor.nav.isStopped = false;
            Actor.Animate(ANIMATION_WALK);
            // Actor.animator.Play("Enemigo_Andar");
            var r = Actor.patrolRadiusFromOrigin;
            var x = Random.Range(-r, r);
            var z = Random.Range(-r, r);
            var dest = Actor.origin + new Vector3(x, Actor.origin.y, z);
            Actor.nav.SetDestination(dest);
            var backupBailout = new System.Diagnostics.Stopwatch();
            backupBailout.Start();
            while (Vector3.Distance(dest, Actor.transform.position) > Actor.patrolDistanceThreshold)
            {
                if (backupBailout.ElapsedMilliseconds > Actor.failedPatrolBailoutTime * 1000f)
                {
                    Debug.LogWarning("Breaking patrol pattern, emergency bailout triggered: The AI considered that the patrol was stuck into an infinite loop.");
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
            Log("Resting patrol.");
            Actor.nav.isStopped = true;
            var idleTime = Random.Range(Actor.patrolIdleMinTime, Actor.patrolIdleMaxTime);
            // Actor.animator.Play("Enemigo_Standby");
            // TODO: Fix this!!!!!
            // Monster drifts BEFORE reaching the stop point. (???)
            Actor.Animate(ANIMATION_IDLE);
            yield return new WaitForSeconds(idleTime);
            Actor.Animate(ANIMATION_WALK);
            yield return new WaitForEndOfFrame();
            Actor.nav.isStopped = false;
            // DISABLED! This part of the behavior was disabled.
            // if (watch.ElapsedMilliseconds > Actor.sleepAgainTime * 1000f)
            // {
            //     Actor.DoBehavior(GoRestAgain());
            // }
            // else
            {
                Actor.DoBehavior(PatrolAndWait());
            }
        }
        IEnumerator GoRestAgain()
        {
            yield return null;
            var backupBailout = new System.Diagnostics.Stopwatch();
            backupBailout.Start();
            while (Vector3.Distance(Actor.origin, Actor.transform.position) > Actor.patrolDistanceThreshold)
            {
                if (backupBailout.ElapsedMilliseconds > Actor.failedPatrolBailoutTime * 1000f)
                {
                    Actor.origin = Actor.transform.position;
                    Debug.LogWarning("Breaking patrol pattern, emergency bailout triggered: The AI considered that the origin rest position was stuck into an infinite loop.");
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
            Actor.Animate(ANIMATION_SLEEP);
            yield return new WaitForSeconds(2f);
            Actor.currentState = new Vigilant(Actor);
            Actor.currentState.Start();
        }
        public override void Update()
        {
            if (Actor.playerInRadius && Actor.PlayerInCone)
            {
                Log("Player in radius and seen, starting chase...");
                Actor.currentState = new Chasing(Actor);
                Actor.currentState.Start();
            }
        }
        private System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

        public override string Tag => "Patrol";

        public override void Start()
        {
            base.Start();
            watch.Start();
            Actor.DoBehavior(PatrolAndWait());
        }
    }
    /// <summary> Shared methods for vigilant and dormant </summary>
    abstract class ShutdownMode : State
    {
        protected ShutdownMode(EnemyRobotBehavior actor) : base(actor) { }
        protected IEnumerator AwakeRoutine()
        {
            Log($"Awaking NPC...");
            Actor.animator.speed = 1;
            while (Actor.activating)
            {
                yield return new WaitForEndOfFrame();
            }
            var patrol = new Patrol(Actor);
            patrol.willSleep = true;
            Actor.currentState = patrol;
            yield return null;
            Actor.currentState.Start();
        }
        public override void Start()
        {
            base.Start();
            // Actor.animator.Play("Enemigo_Activarse");
            Debug.Log("Starting shutdown mode");
            Actor.animator.speed = 0;
        }
    }
    /// <summary> Represent a sleep state that won't react to players </summary>
    class Dormant : ShutdownMode
    {
        public Dormant(EnemyRobotBehavior actor) : base(actor) { }

        public override string Tag => "Dormant";
    }
    /// <summary> Represents a sleep state that will react to players </summary>
    class Vigilant : ShutdownMode
    {
        public Vigilant(EnemyRobotBehavior actor) : base(actor) { }

        public override string Tag => "Vigilant";

        override public void TriggerAwake(UnityEngine.Collider collider)
        {
            Actor.DoBehavior(AwakeRoutine());
        }
    }

    class Chasing : State
    {
        public Chasing(EnemyRobotBehavior actor) : base(actor) { }
        public override void Start()
        {
            base.Start();
            // Actor.animator.Play("Enemigo_Perseguir");
            Actor.Animate(ANIMATION_CHASE);
        }
        IEnumerator GoSleep()
        {
            Log("Enemy coasting to sleep...");
            // Actor.animator.Play("Enemigo_Standby");
            Actor.nav.isStopped = true;
            Actor.attacking = false;
            Actor.nav.SetDestination(Actor.transform.position);
            Actor.weapon.SetActive(false);
            Actor.Animate(ANIMATION_IDLE);
            yield return new WaitForSeconds(4f);
            Actor.nav.isStopped = false;
            killingFocus = false;
            Actor.currentState = new Patrol(Actor);
            Actor.currentState.Start();
        }
        IEnumerator KillFocus()
        {
            Actor.Animate(ANIMATION_WALK);
            yield return new WaitForSeconds(Actor.patrolGiveUpTime);
            Actor.DoBehavior(GoSleep());
        }
        IEnumerator StartAttacking()
        {
            Actor.attacking = true;
            Actor.Animate(ANIMATION_ATTACK);
            while (Actor.attacking)
            {
                yield return new WaitForEndOfFrame();
            }
        }
        private bool killingFocus = false;
        public override string Tag => "Chasing";

        public override void Update()
        {
            Actor.nav.SetDestination(Actor.player.transform.position);
            if (!Actor.attacking && Actor.playerInWakeRadius)
            {
                Log("Attempting to attack the player");
                Actor.nav.isStopped = true;
                Actor.DoBehavior(StartAttacking());
                return;
            }
            if (!killingFocus && !Actor.playerInRadius)
            {
                Log("Killing focus (Lost sight)");
                killingFocus = true;
                Actor.DoBehavior(KillFocus());
                return;
            }
        }
    }
    class ShockState : State
    {
        public override string Tag => "Electric Shock";

        public ShockState(EnemyRobotBehavior actor) : base(actor) { }
        IEnumerator AwaitShockRecovery()
        {
            Actor.Animate(ANIMATION_IDLE);
            Actor.electroBall.SetActive(true);
            yield return new WaitForSeconds(Actor.shockRecoverTime);
            Log("Shock end, resuming patrol.");
            Actor.currentState = new Patrol(Actor);
            Actor.currentState.Start();
            Actor.electroBall.SetActive(false);
            Actor.nav.isStopped = false;
        }
        public override void Start()
        {
            base.Start();
            Actor.DoBehavior(AwaitShockRecovery());
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
    public float patrolGiveUpTime = 10f;
    [Range(1f, 100f)]
    public float patrolRadiusFromOrigin = 30f;
    [Range(0f, 60f)]
    public float patrolIdleMaxTime = 10f;
    [Range(0f, 60f)]
    public float patrolIdleMinTime = 2f;
    public float
        patrolDistanceThreshold = 4f,
        chaseAttackRadius = 2.8f,
        failedPatrolBailoutTime = 30f,
        shockRecoverTime = 4f,
        sleepAgainTime = 40f;
    [Header("Development & References")]
    public bool testShock = false;

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

    public void EnableAttackCollider()
    {
        weapon.SetActive(true);
    }
    public void DisableAttackCollider()
    {
        weapon.SetActive(false);
    }

    bool attacking = false;
    public void AttackEnd()
    {
        attacking = false;
        nav.isStopped = false;
        Animate(ANIMATION_CHASE);
    }
    bool activating = true;
    public void ActivationEnd()
    {
        activating = false;
    }

    void DoBehavior(IEnumerator ik)
    {
        StopAllCoroutines();
        StartCoroutine(ik);
    }

    /// <summary> Called when health reaches 0 after damage. </summary>
    void Die()
    {
        print("ENEMIGO MUERTO");
        var loot = transform.Find("Loot");
        for (int i = 0; i < loot.childCount; i++)
        {
            var child = loot.GetChild(i).gameObject;
            Debug.Log($"Spawning {loot}...");
            var r = 1f;
            var x = Random.Range(-r, r);
            var z = Random.Range(-r, r);
            var pos = transform.position + new Vector3(x, 0.25f, z);
            var inst = Object.Instantiate(child, pos, Quaternion.identity);
            inst.SetActive(true);
        }
        Destroy(gameObject);
    }

    void Animate(int id)
    {
        animator.SetInteger("state", id);
    }
    void IDamageAcceptor.TakeDamage(Damage incoming)
    {
        var damage = incoming.ApplyReduction(armorReduction);
        Debug.Log($"Robot absorbs {incoming.amount} of {Damage.format(incoming.type)} damage (Which translates to {damage.amount} of true damage).");
        if (incoming.type == Damage.Type.ELECTRIC)
        {
            StopAllCoroutines();
            nav.isStopped = true;
            currentState = new ShockState(this);
            currentState.Start();
            return;
        }
        if (incoming.type == Damage.Type.KINETIC)
        {
            health -= damage.amount;
            Debug.Log($"Incoming: {damage.amount}, left: {health}");
            if (health <= 0)
            {
                health = 0;
                Die();
            }
            return;
        }
    }

    private Transform body;
    private Vector3 origin;
    private GameObject weapon, electroBall;
    // Start is called before the first frame update
    void Start()
    {
        health = initialHealth;
        weapon = GameObject.FindGameObjectWithTag("Ataque Enemigo");
        weapon.SetActive(false);
        origin = transform.position + new Vector3(0, 1f, 0);
        nav = GetComponent<NavMeshAgent>();
        body = transform.Find("Body");
        electroBall = body.Find("ElectroBall").gameObject;
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
        if (testShock)
        {
            StartCoroutine(WillShock());
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        var s = currentState;
        if (s is Patrol)
        {
            StopAllCoroutines();
            (s as Patrol).Start();
        }
    }
    IEnumerator WillShock()
    {
        yield return new WaitForSeconds(5f);
        ((IDamageAcceptor)this).TakeDamage(new Damage(1, Damage.Type.ELECTRIC));
    }

    // Update is called once per frame
    void Update()
    {
        currentState.Update();
    }

    private bool playerInRadius = false, playerInWakeRadius = false;
    void ITriggerEnterListener.OnTriggerEnter(GameObject source, Collider other)
    {
        if (other.gameObject != player) return;
        if (source == wakeGO)
        {
            playerInRadius = true;
            playerInWakeRadius = true;
            currentState.TriggerAwake(other);
        }
        else if (source == hearGO)
        {
            playerInRadius = true;
            currentState.TriggerHear(other);
        }
    }
    void ITriggerExitListener.OnTriggerExit(GameObject source, Collider other)
    {
        if (other.gameObject != player) return;
        if (source == hearGO)
            playerInRadius = false;
        if (source == wakeGO)
            playerInWakeRadius = false;
    }
}
