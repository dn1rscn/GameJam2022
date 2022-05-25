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

    Vector2 movimiento, rotate;
    public bool disparo,apuntar;
    public float velocidad, velocidadGiro, turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public float fuerza;

    public GameObject arma;
    
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
        //playerInputActions.Player.Correr.performed += ctx =>
        playerInputActions.Player.Apuntar.performed += ctx => apuntar = true;
        playerInputActions.Player.Apuntar.canceled += ctx => apuntar = false;
    }

    void Update()
    {
        //MOVIMIENTO
        Vector3 moveDir = Vector3.zero;
        Vector3 m = new Vector3(movimiento.x,0.0f,movimiento.y) * velocidad * Time.deltaTime;
        Vector3 d = new Vector2(0, rotate.x) * velocidadGiro*Time.deltaTime;
        transform.Rotate(d, Space.World);

        /*if(m.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(m.x, m.z) * Mathf.Rad2Deg + transform.eulerAngles.y;
            //float targetAngle = Mathf.Atan2(d.x, d.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y,targetAngle,ref turnSmoothVelocity,turnSmoothTime);
            playerRigidbody.rotation = Quaternion.Euler(0f,d.y,0f);
            moveDir = Quaternion.Euler(0f,targetAngle,0f)*Vector3.forward;
            //controller.Move(moveDir.normalized*velocidad*Time.deltaTime);
        //}
        playerRigidbody.velocity=new Vector3(moveDir.x,0,moveDir.z).normalized*velocidad;*/
        transform.Translate(m, Space.Self);
        //APUNTAR
        arma.SetActive(apuntar);
         
        
        //DISPARO
        if(disparo&&apuntar)
        { 
            print("DISPARO!!!");
            //TODO:hacer la llamada a la función de disparo
        }
    }

    void Esquivar()
    {
        playerRigidbody.AddForce(playerRigidbody.velocity*fuerza,ForceMode.Acceleration);
        animatorPlayer.Play("Anim_PruebaEsquivar");
    }
}
