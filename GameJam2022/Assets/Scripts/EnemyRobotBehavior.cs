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
    }

    class Patrol : State
    {
        public bool willSleep = false;
        public Vector3 origin;
        public Patrol(EnemyRobotBehavior actor) : base(actor) { }

        IEnumerator PatrolAndWait()
        {
            Actor.animator.Play("Enemigo_Andar");
            for (int i = 0; i < 10; i++)
            {
                yield return new WaitForSeconds(.1f);
                Actor.transform.position += Actor.transform.forward;
            }
            yield return new WaitForSeconds(.1f);
            Actor.animator.Play("Enemigo_Standby");
            yield return new WaitForSeconds(1f);
            Actor.animator.Play("Enemigo_Andar");
            for (int i = 0; i < 10; i++)
            {
                yield return new WaitForSeconds(.1f);
                Actor.transform.position -= Actor.transform.forward;
            }
            yield return new WaitForSeconds(.1f);
            Actor.animator.Play("Enemigo_Standby");
            yield return new WaitForSeconds(1f);
            Actor.StartCoroutine(PatrolAndWait());
        }
        public override void Update()
        {
            if (Actor.playerInRadius && Actor.PlayerInCone)
            {
                Actor.StopAllCoroutines();
                Actor.currentState = new Chasing(Actor);
                Actor.currentState.Start();
            }
        }
        public override void Start()
        {
            Actor.StartCoroutine(PatrolAndWait());
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
        IEnumerator AwakeRoutine()
        {
            Actor.animator.speed = 1;
            yield return new WaitForSeconds(6f);
            var patrol = new Patrol(Actor);
            patrol.willSleep = true;
            patrol.origin = Actor.transform.position;
            Actor.currentState = patrol;
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
        public Chasing(EnemyRobotBehavior actor) : base(actor) { }
        public override void Start()
        {
            Actor.animator.Play("Enemigo_Perseguir");
        }
        public override void Update()
        {
            Actor.transform.position += Actor.PlayerDir * Actor.walkSpeed;
            Actor.body.forward = Actor.PlayerDir;
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
    public float walkSpeed = 0.1f;
    public float patrolGiveUpTime = 20f;

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

    private LineRenderer playerRay;
    private Transform body;
    // Start is called before the first frame update
    void Start()
    {
        body = transform.Find("Body");
        var aid = body.Find("VisualAid");
        playerRay = aid.Find("PlayerRay").gameObject.GetComponent<LineRenderer>();
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
        var loc = player.transform.position - transform.position;
        loc.y = 0;
        playerRay.SetPosition(1, loc);
        if (PlayerInCone)
        {
            playerRay.endColor = Color.magenta;
        }
        else
        {
            playerRay.endColor = Color.green;
        }
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
