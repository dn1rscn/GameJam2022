using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class ControlPlayer : MonoBehaviour
{
    PlayerInputActions playerInputActions;
    public Animator animatorPlayer;
    private Rigidbody playerRigidbody;
    private PlayerInput playerInput;
    public Transform cam;
    public CinemachineFreeLook thirdPersonCamera;
    LootSystem lootSystem;

    Vector2 movimiento, rotate;
    public bool disparo,apuntar,esquivar,mouse;
    public float velocidad, velocidadGiro, velocidadEsquivar;
    float turnSmothTime = 0.1f,turnSmoothVelocity;
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
        playerInputActions = new PlayerInputActions();
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
        //DETECTAMOS EL INPUT
        InputSystem.onActionChange += (obj, change) =>
        {
            if (change == InputActionChange.ActionPerformed)
            {
                var inputAction = (InputAction)obj;
                var lastControl = inputAction.activeControl;
                var lastDevice = lastControl.device;

                //bug.Log($"device: {lastDevice.name}");
                if (lastDevice.name == "Mouse") mouse = true;
                else mouse = false;
            }
        };

        //MOVIMIENTO
        Vector3 m = Vector3.zero;
        Vector3 direction = new Vector3(movimiento.x, 0.0f, movimiento.y).normalized;
            //giro sin apuntar
        if(direction.magnitude>=0.1f&&!apuntar)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z)*Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
        m = direction * velocidad * Time.deltaTime;
        transform.Translate(m, Space.World);

        //giro al apuntar
        if (apuntar)
        {
            if (mouse) //GIRO CON EL MOUSE
            {
                Vector2 d = new Vector2(0, rotate.x) * velocidadGiro * Time.deltaTime;
                transform.Rotate(d, Space.World);
            }
            else//GIRO CON EL MANDO
            {
                Vector2 dirGiro = new Vector2(rotate.x, rotate.y).normalized;// * velocidadGiro * Time.deltaTime;
                if (dirGiro.magnitude >= 0.1f)
                {
                    float targetAngleG = Mathf.Atan2(dirGiro.x, dirGiro.y) * Mathf.Rad2Deg;
                    //Vector2 d = new Vector2(targetAngleG, 0f);
                    transform.rotation = Quaternion.Euler(0f, targetAngleG, 0f);
                    //transform.Rotate(d, Space.World);
                }
            }

        }

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

    public void OnJoin(InputAction.CallbackContext ctx)
    {
        if (ctx.control.device is Keyboard) print("teclado");
        else print("Mando");
    }
}
