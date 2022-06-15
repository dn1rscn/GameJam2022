using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.SceneManagement;


public class ControlPlayer : MonoBehaviour
{
    PlayerInputActions playerInputActions;
    public Animator animatorPlayer;
    private Rigidbody playerRigidbody;
    public Camera cam;
    public CinemachineFreeLook thirdPersonCamera;
    private CinemachineBasicMultiChannelPerlin cameraNoise;
    LootSystem lootSystem;
    ControlHub controlHub;

    Vector2 movimiento, rotate;
    bool disparo, apuntar, esquivar, mouse;
    public bool canMove = false;

    [Header("Movimiento")]
    public float velocidad, velocidadGiro, velocidadEsquivar, fuerzaRetroceso;
    float turnSmothTime = 0.1f, turnSmoothVelocity;
    float camRayLength = 100f;
    int floorMask;
    public int vida = 2;
    public float radioCancelacionJoystick;
    public float ejeX;
    public float ejeY;

    public Transform firePoint;
    //public float fuerza;

    [Header("Apuntar")]
    public LineRenderer lineApuntar;
    public GameObject arma;
    GameObject geometria_Arma;

    [Header("Grounded")]
    public bool isGrounded;
    //public Transform feetPos;
    public float checkRatius;
    public LayerMask whatIsGround;
    public float fuerzaCaida;

    [Header("Recolectables")]
    public int energia;
    public int municion;

    Prota_Anims scr_protaAnims;
    ControlSFX scr_controlSFX;

    [Header("Armas")]
    public int armaSeleccionada = 0;
    public string armaSeleccionada_name;
    public GameObject pistol, flamer, grenade, lightning;
    private GameObject[] weapons_GObj;
    private IShooting[] weapons;

    [Header("UI")]
    public GameObject uiKill;
    public GameObject uiHubMunicion;
    public GameObject uiHubEnergia;

    private void Start()
    {
        //ursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;

        controlHub = GameObject.Find("ControlHub").GetComponent<ControlHub>();
        lootSystem = GameObject.Find("ControlLoot").GetComponent<LootSystem>();

        weapons_GObj = new GameObject[]{
            pistol,
            flamer,
            grenade,
            lightning
        };

        weapons = new IShooting[]{
            pistol.GetComponent<IShooting>(),
            flamer.GetComponent<IShooting>(),
            grenade.GetComponent<IShooting>(),
            lightning.GetComponent<IShooting>()
        };

        

    }
    private void Awake()
    {
        floorMask = LayerMask.GetMask("Floor");
        print("iniciamos");
        playerRigidbody = GetComponent<Rigidbody>();

        if (GameObject.FindGameObjectWithTag("Prota_Anims").GetComponent<Prota_Anims>())
        {
            scr_protaAnims = GameObject.FindGameObjectWithTag("Prota_Anims").GetComponent<Prota_Anims>();
        }
        if (GameObject.Find("Player").GetComponent<ControlSFX>())
        {
            scr_controlSFX = GameObject.Find("Player").GetComponent<ControlSFX>();
        }
        if (GameObject.Find("GeometriaArma"))
        {
            geometria_Arma = GameObject.Find("GeometriaArma");
        }
        //ACTIVAMOS LOS PLAYER INPUTS
        playerInputActions = new PlayerInputActions();

        if (canMove) playerInputActions.Player.Enable();
        else playerInputActions.Player.Disable();

        playerInputActions.Player.Movimiento.performed += ctx => movimiento = ctx.ReadValue<Vector2>();
        playerInputActions.Player.Movimiento.performed += ctx => Andar();
        playerInputActions.Player.Movimiento.canceled += ctx => movimiento = Vector2.zero;
        playerInputActions.Player.Movimiento.canceled += ctx => AndarOff();


        playerInputActions.Player.Giro.performed += ctx => rotate = ctx.ReadValue<Vector2>();
        playerInputActions.Player.Giro.canceled += ctx => rotate = Vector2.zero;

        playerInputActions.Player.Disparo.performed += ctx => disparo = ctx.ReadValueAsButton();
        playerInputActions.Player.Disparo.canceled += ctx => disparo = ctx.ReadValueAsButton();
        playerInputActions.Player.Disparo.canceled += ctx => pararShakeCamara();

        playerInputActions.Player.Correr.performed += ctx => velocidad = velocidad * 2.5f;
        playerInputActions.Player.Correr.performed += ctx => Correr();
        playerInputActions.Player.Correr.canceled += ctx => velocidad = velocidad / 2.5f;
        playerInputActions.Player.Correr.canceled += ctx => CorrerOff();

        //playerInputActions.Player.esquivar.performed += ctx => Esquivar();


        playerInputActions.Player.Apuntar.performed += ctx => apuntar = true;
        playerInputActions.Player.Apuntar.performed += ctx => Apuntar();
        playerInputActions.Player.Apuntar.canceled += ctx => apuntar = false;
        playerInputActions.Player.Apuntar.canceled += ctx => ApuntarOff();

    }

    void Update()
    {
        if (apuntar) GameObject.Find("Third Person Camera_Cerca").GetComponentInChildren<CinemachineFreeLook>().m_YAxis.Value = 0.5f;
        else GameObject.Find("Third Person Camera_Cerca").GetComponentInChildren<CinemachineFreeLook>().m_YAxis.Value = 1f;
        isGrounded = Physics.Raycast(transform.position, -Vector3.up, checkRatius, whatIsGround);
        if(isGrounded == false)
        {
            print("caemos");
            //playerRigidbody.velocity = Vector3.up * -fuerzaCaida;
            playerRigidbody.AddForce(Vector3.down * fuerzaCaida, ForceMode.Force);
        }

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
        if (canMove)
        {
            Vector3 m = Vector3.zero;
            Vector3 direction = Vector3.zero;
            //Anulamos el joystick de movimiento si los valores que recoge son menores del valor "Radio de inactividad" en cualquier sentido
           

            if (movimiento.x >= radioCancelacionJoystick || movimiento.x <= -radioCancelacionJoystick)
            {
                ejeX = movimiento.x;
            }
            else ejeX = 0.0f;

            if (movimiento.y >= radioCancelacionJoystick || movimiento.y <= -radioCancelacionJoystick)
            {
                ejeY = movimiento.y;
            }
            else ejeY = 0.0f;

            direction = new Vector3(-ejeX, 0.0f, -ejeY)/*.normalized*/;
            //giro sin apuntar
            if (direction.magnitude >= 0.1f&&!apuntar)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
            }
            m = direction * velocidad * Time.deltaTime;
            //transform.Translate(m, Space.World);
            playerRigidbody.velocity = direction*velocidad;
        }

        //giro al apuntar
        if (apuntar)
        {
            /*RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, firePoint.forward);
            if(hitInfo)
            {
                lineApuntar.SetPosition(0, firePoint.position);
                lineApuntar.SetPosition(1, hitInfo.point);
            }
            else
            {
                lineApuntar.SetPosition(0, firePoint.position);
                lineApuntar.SetPosition(1, firePoint.position + firePoint.forward * 100);
            }*/
            if (mouse) //GIRO CON EL MOUSE
            {
                Ray camRay = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit floorHit;
                if (Physics.Raycast(camRay, out floorHit, camRayLength, floorMask))
                {
                    Vector3 playerToMouse = floorHit.point - transform.position;
                    playerToMouse.y = 0;

                    var rotation = Quaternion.LookRotation(playerToMouse);
                    playerRigidbody.MoveRotation(rotation);
                }
            }
            else//GIRO CON EL MANDO
            {
                /*Vector2 dirGiro = new Vector2(rotate.x, 0).normalized;
                if (dirGiro.magnitude >= 0.1f)
                {
                    float targetAngleG = Mathf.Atan2(dirGiro.x, dirGiro.y) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0f, targetAngleG, 0f);
                }*/
                Vector2 r = new Vector2(0, rotate.x) * velocidadGiro * Time.deltaTime;
                transform.Rotate(r, Space.Self);
            }
        }

        //APUNTAR
        arma.SetActive(apuntar);
        geometria_Arma.SetActive(apuntar);

        //ACTIVAMOS EL GAMEOBJECT DEL ARMA SELECCIONADA

        if (armaSeleccionada <= 3)
        {
            armaSeleccionada_name = weapons_GObj[armaSeleccionada].name;
        }
        else
            armaSeleccionada_name = weapons_GObj[0].name;

        foreach (var weapon_GObj in weapons_GObj)
        {
            if (weapon_GObj.name != armaSeleccionada_name)
            {
                weapon_GObj.SetActive(false);
            }
            else
            {
                weapon_GObj.SetActive(true);
            }
        }

        //DISPARO
        if (disparo && apuntar)
        {
            if (municion <= 0) Debug.Log("NO TIENES MUNICION");
            else
            {
                //TODO:hacer la llamada a la función de disparo
                if (weapons[armaSeleccionada].CanShoot())
                {
                    weapons[armaSeleccionada].Shoot();
                    municion--;
                    controlHub.ActualizarHub(municion, armaSeleccionada);

                    print("DISPARO!!!");

                    //VFX + Sonido de disparo
                    scr_controlSFX.sfx_Disparo();

                    //Shake camara
                    cameraNoise = GameObject.Find("Third Person Camera").GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();
                    cameraNoise.m_AmplitudeGain = 3;
                    cameraNoise.m_FrequencyGain = 0.5f;
                    Invoke("pararShakeCamara", 0.5f);

                    //playerRigidbody.AddForce(Vector3.back * fuerzaRetroceso,ForceMode.Force);
                }
            }
        }

        //ESQUIVAR
        if (esquivar)
        {
            Vector3 me = new Vector3(movimiento.x, 0.0f, movimiento.y) * velocidadEsquivar * Time.deltaTime;
            transform.Translate(me);
            Invoke("esquivarOff", 0.2f);
        }
    }

    void pararShakeCamara()
    {
        // print("PararShake");
        cameraNoise = GameObject.Find("Third Person Camera").GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();
        cameraNoise.m_AmplitudeGain = 0;
        cameraNoise.m_FrequencyGain = 0;
    }

    //ANIMACIONES***************************
    void Andar()
    {
        scr_protaAnims.Andar(new Vector3 (ejeX,0,ejeY));
    }
    void AndarOff()
    {
        scr_protaAnims.AndarOff();
    }
    
    void Correr()
    {
//        if (movimiento.x < -0.8f || 0.8f < movimiento.x || movimiento.y < -0.8f || 0.8f < movimiento.y)
  //      {
            //velocidad = velocidad * 2.5f;
            scr_protaAnims.Correr(new Vector3(ejeX, 0, ejeY));
    //    }
    }
    void CorrerOff()
    {
        //velocidad = velocidad / 2.5f;
        scr_protaAnims.CorrerOff();
    }


    void Apuntar()
    {
        scr_protaAnims.Apuntar();

    }
    void ApuntarOff()
    {
        scr_protaAnims.ApuntarOff();
    }

/*    void Esquivar()
    {
        esquivar = true;
        animatorPlayer.Play("Anim_PruebaEsquivar");
    }
  
    void esquivarOff()
    {
        esquivar = false;
    }
    */

    //************************************
    private void OnTriggerStay(Collider other)
    {
        switch (other.tag)
        {
            //OBJECTOS
            case "Trigger Puerta":
                if (other.GetComponent<ControlPuertas>().energia < other.GetComponent<ControlPuertas>().energiaNecesaria && energia > 0)
                {
                    other.GetComponent<ControlPuertas>().energia++;
                    energia--;
                    controlHub.ActualizarHubEnergia(energia);
                    other.GetComponent<ControlPuertas>().ActualizarEnergia();
                }
                break;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        // print(other.tag);
        switch (other.tag)
        {
            //Enemigos
            case "Ataque Enemigo pequeño":
                vida--;
                Kill();
                break;
            case "Ataque Enemigo":
                vida -= 2;
                Kill();
                break;

            //RECOLECTABLES
            case "Trigger Energia"://RECOGEMOS ENERGIA
                uiHubEnergia.SetActive(true);
                Destroy(other.gameObject);
                scr_controlSFX.sfx_recogerEnergia();
                energia++;
                controlHub.ActualizarHubEnergia(energia);
                break;
            case "Trigger Municion":
                uiHubMunicion.SetActive(true);
                lootSystem.calculateLoot();
                //controlHub.ActualizarHub(municion, armaSeleccionada);
                scr_controlSFX.sfx_recogerMunicion();
                Destroy(other.gameObject);
                break;

                //OBJECTOS
                /*case "Trigger Puerta":
                    if (other.GetComponent<ControlPuertas>().energia < other.GetComponent<ControlPuertas>().energiaNecesaria)
                    {
                        other.GetComponent<ControlPuertas>().energia++;
                        energia--;
                    }
                    break;*/
        }
    }

    void Kill()
    {
        if (vida <= 0)
        {
            //bloquear movimiento
            canMove = false;
            playerInputActions.Player.Disable();
            //ShakeCamaraMuerte
            cameraNoise = GameObject.Find("Third Person Camera").GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();
            cameraNoise.m_AmplitudeGain = 3;
            cameraNoise.m_FrequencyGain = 0.4f;
            Invoke("pararShakeCamara", 0.5f);

            //Animacion muerte
            scr_protaAnims.Muerte();
            scr_controlSFX.sfx_Muerte();

            //Activamos ui
            Invoke("activarUIMuerte", 2.0f);

        }
    }

    void activarUIMuerte()
    {
        //**Activar menu de muerte
        //uiKill.SetActive(true);

        //**Recargar escena**
        //Scene scene = SceneManager.GetActiveScene();
        //SceneManager.LoadScene(0);

        //Recuperar Vidas y habilitar movimiento directamente
        vida = 2;
        HabilitarMovimiento();
        animatorPlayer.Play("Idle_01");
        scr_controlSFX.sfx_ReiniciarNivel();
        Time.timeScale = 1.0f;


    }

    public void HabilitarMovimiento()
    {
        canMove = true;
        playerInputActions.Player.Enable();
    }
}
