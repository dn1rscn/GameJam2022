using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlPlayer : MonoBehaviour
{
    public Animator animatorPlayer;
    private Rigidbody playerRigidbody;
    private PlayerInput playerInput;
    public Transform cam;

    Vector2 movimiento;
    Vector2 rotate;
    public bool disparo;
    public float velocidad;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public float fuerza;
    
    private void Awake()
    {
        print("iniciamos");
        playerRigidbody = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        //ACTIVAMOS LOS PLAYER INPUTS
        PlayerInputActions playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Movimiento.performed += ctx => movimiento = ctx.ReadValue<Vector2>();
        playerInputActions.Player.Movimiento.canceled += ctx => movimiento=Vector2.zero;

        /*playerInputActions.Player.Giro.performed += ctx => rotate = ctx.ReadValue<Vector2>();
        playerInputActions.Player.Giro.canceled += ctx => rotate = Vector2.zero;*/

        playerInputActions.Player.Disparo.performed += ctx => disparo = ctx.ReadValueAsButton();
        playerInputActions.Player.Disparo.canceled += ctx => disparo = ctx.ReadValueAsButton();

        playerInputActions.Player.Correr.performed += ctx => velocidad=velocidad*2;
        playerInputActions.Player.Correr.canceled += ctx => velocidad=velocidad/2;

        playerInputActions.Player.esquivar.performed += ctx => Esquivar();
        //playerInputActions.Player.Correr.performed += ctx =>
    }

    void Update()
    {
        //MOVIMIENTO
        Vector3 moveDir = Vector3.zero;
        Vector3 m = new Vector3(movimiento.x,0.0f,movimiento.y).normalized;
    
        if(m.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(m.x, m.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y,targetAngle,ref turnSmoothVelocity,turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f,angle,0f);
            moveDir = Quaternion.Euler(0f,targetAngle,0f)*Vector3.forward;
            //controller.Move(moveDir.normalized*velocidad*Time.deltaTime);
        }
        playerRigidbody.velocity=new Vector3(moveDir.x,0,moveDir.z).normalized*velocidad;

        //DISPARO
        if(disparo)
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
