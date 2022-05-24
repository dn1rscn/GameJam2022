using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlPlayer : MonoBehaviour
{
    public CharacterController controller;
    private Rigidbody playerRigidbody;
    private PlayerInput playerInput;
    public Transform cam;

    Vector2 movimiento;
    Vector2 rotate;
    public float velocidad;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    
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

        playerInputActions.Player.Giro.performed += ctx => rotate = ctx.ReadValue<Vector2>();
        playerInputActions.Player.Giro.canceled += ctx => rotate = Vector2.zero;

        playerInputActions.Player.Disparo.performed += ctx => Disparar();
    }

    void Update()
    {
        Vector3 m = new Vector3(movimiento.x,0.0f,movimiento.y).normalized;

    
        if(m.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(m.x, m.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y,targetAngle,ref turnSmoothVelocity,turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f,angle,0f);
            Vector3 moveDir = Quaternion.Euler(0f,targetAngle,0f)*Vector3.forward;
            controller.Move(moveDir.normalized*velocidad*Time.deltaTime);
        }
    }

    void Disparar()
    {
        print("DISPARO!!!");
    }
}
