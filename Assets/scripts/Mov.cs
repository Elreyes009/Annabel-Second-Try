
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Mov : MonoBehaviour
{
    #region Parámetros
    [SerializeField] private float moveSpeed = 3f;               
    [SerializeField] private Vector2 gridSize = new Vector2(1f, 1f); 
    private LayerMask obstacleLayer;                              
    private Vector2 direction;                                   
    private Animator anim;                                       

    private bool isHiding = false;                               
    private bool isMoving = false;                               
    private Vector3 puntoDeRespawn;                                                        

    private Mov playerMovement;                                  
    private SpriteRenderer playerSprite;
    [SerializeField] List<string> inventario = new List<string>();
    
    #endregion

    void Awake()
    {
        playerMovement = GetComponent<Mov>();
        playerSprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        puntoDeRespawn = transform.position;

        obstacleLayer = LayerMask.GetMask("detalle");
    }
    void Update()
    {
        Animations();
    }
    void FixedUpdate()
    {
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (direction.x != 0)
            direction.y = 0;

        if (!isMoving && direction != Vector2.zero)
        {
            Vector3 targetPosition = transform.position + new Vector3(direction.x * gridSize.x, direction.y * gridSize.y, 0);

            if (!IsObstacle(targetPosition))
            {
                StartCoroutine(Move(targetPosition));
            }
        }
    }
    private void Animations()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            anim.SetInteger("Direccion", 3);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            anim.SetInteger("Direccion", 4);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            anim.SetInteger("Direccion", 2);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            anim.SetInteger("Direccion", 1);
        }

        if (!isMoving)
        {
            anim.SetInteger("Direccion", 0);
        }
    }

    IEnumerator Move(Vector3 targetPosition)
    {
        isMoving = true;

        while ((targetPosition - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition;
        isMoving = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.GetComponent<Recogible>())
        {
            collision.transform.gameObject.SetActive(false);
            inventario.Add(collision.transform.GetComponent<Recogible>().itemName);
        }

        //if (collision.CompareTag("Puerta"))
        //{
        //    foreach (string nombre in inventario)
        //    {
        //        if (nombre == collision.transform.GetComponent<Puerta>().requerimiento)
        //        {
        //            collision.transform.GetComponent<Animator>().SetBool("Acción", true);
        //            return;
        //        }
        //    }
        //}
    }
    public void Reaparecer()
    {
        StartCoroutine(RespawnCoroutine());
    }
    public void ActualizarPuntoDeRespawn(Vector3 nuevaPosicion)
    {
        puntoDeRespawn = nuevaPosicion;
    }

    IEnumerator RespawnCoroutine()
    {
        playerSprite.enabled = false;            
        yield return new WaitForSeconds(1f);       
        transform.position = puntoDeRespawn;       
        playerSprite.enabled = true;             
        playerMovement.enabled = true;           
    }

    #region Métodos Auxiliares

    bool IsObstacle(Vector3 targetPosition)
    {
        Collider2D obstacle = Physics2D.OverlapCircle(targetPosition, 0.2f, obstacleLayer);
        return obstacle != null;
    }

    public bool IsPlayerMoving()
    {
        return isMoving;
    }

    public void SetHiding(bool hiding)
    {
        isHiding = hiding;
        FindAnyObjectByType<Enemigo>()?.SetPlayerHiding(hiding);
    }
    #endregion
}
