using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class ControlPlayer : MonoBehaviour
{
    public Animator animatorPlayer;
    private Rigidbody playerRigidbody;
    private PlayerInput playerInput;
    public Transform cam;
    public CinemachineFreeLook thirdPersonCamera;
    LootSystem lootSystem;

    Vector2 movimiento, rotate;
    public bool disparo,apuntar,esquivar;
    public float velocidad, velocidadGiro, velocidadEsquivar;
    //public float fuerza;

    public GameObject arma;
    [Header("recolectables")]
    public int energia;
    public int municion;

    private void Start()
    {
        Cursor.visible = false;
    }
    private void Awake()
    {
        print("iniciamos");
        playerRigidbody = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        //var thirdPersonCamera = GameObject.Find("Third Person Camera").GetComponent<CinemachineFreeLook>().m_XAxis.m_MaxSpeed = 300;

        //ACTIVAMOS LOS PLAYER INPUTS
        PlayerInputActions playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Movimiento.performed += ctx => movimiento = ctx.ReadValue<Vector2>();
        playerInputActions.Player.Movimiento.canceled += ctx => movimiento=Vector2.zero;

        playerInputActions.Player.Giro.performed += ctx => rotate = ctx.ReadValue<Vector2>();
        playerInputActions.Player.Giro.canceled += ctx => rotate = Vector2.zero;

        playerInputActions.Player.Disparo.performed += ctx => disparo = ctx.ReadValueAsButton();
        playerInputActions.Player.Disparo.canceled += ctx => disparo = ctx.ReadValueAsButton();

        playerInputActions.Player.Correr.performed += ctx => velocidad=velocidad*2;
        playerInputActions.Player.Correr.canceled += ctx => velocidad=velocidad/2;

        playerInputActions.Player.esquivar.performed += ctx => Esquivar();

        playerInputActions.Player.Apuntar.performed += ctx => apuntar = true;
        playerInputActions.Player.Apuntar.canceled += ctx => apuntar = false;
    }

    void Update()
    {
        //MOVIMIENTO
        Vector3 m = new Vector3(movimiento.x,0.0f,movimiento.y) * velocidad * Time.deltaTime;
        Vector3 d = new Vector2(0, rotate.x) * velocidadGiro*Time.deltaTime;
        transform.Rotate(d, Space.World);
        transform.Translate(m, Space.Self);

        //APUNTAR
        arma.SetActive(apuntar);
         
        
        //DISPARO
        if(disparo&&apuntar)
        {
            if (municion <= 0) Debug.LogError("NO TIENES MUNICION");
            else 
            {
                print("DISPARO!!!");
                //TODO:hacer la llamada a la función de disparo
            }
        }

        //ESQUIVAR
        if (esquivar)
        {
            Vector3 me = new Vector3(movimiento.x, 0.0f, movimiento.y) * velocidadEsquivar * Time.deltaTime;
            transform.Translate(me, Space.Self);
            Invoke("esquivarOff", 0.2f);
        }
    }

    void Esquivar()
    {
        esquivar = true;
        animatorPlayer.Play("Anim_PruebaEsquivar");
    }
    void esquivarOff()
    {
        esquivar = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        print(other.tag);
        switch(other.tag)
        {
            //RECOLECTABLES
            case "Trigger Energia"://RECOGEMOS ENERGIA
                Destroy(other.gameObject);
                energia++;
                break;
            case "Trigger Municion":
                lootSystem.calculateLoot();
                Destroy(other.gameObject);
                break;

            //OBJECTOS
            case "Trigger Puerta":
                break;
        }
    }
}
