using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlPlayer : MonoBehaviour
{
    private Rigidbody playerRigidbody;
    private PlayerInput playerInput;

    Vector2 movimiento;
    Vector2 rotate;
    public float velocidad;
    
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
        Vector3 m = new Vector3(movimiento.x,0.0f,movimiento.y)*velocidad;
        playerRigidbody.velocity = new Vector3(m.x,0,m.z);

        Vector2 r = new Vector2(0f, -rotate.x) * 100f * Time.deltaTime;
        transform.Rotate(r, Space.World);
    }

    void Disparar()
    {
        print("DISPARO!!!");
    }
}
