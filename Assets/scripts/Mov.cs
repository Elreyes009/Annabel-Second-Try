using System.Collections;
using UnityEngine;
using Fungus;
using UnityEngine.Splines;

public class Mov : MonoBehaviour
{
    #region Parámetros
    [SerializeField] private float moveSpeed = 3f;               
    [SerializeField] private Vector2 gridSize = new Vector2(1f, 1f); 
    private LayerMask obstacleLayer;                              
    private Vector2 direction;                                   
                                                                   
    private bool isMoving = false;                               
    [SerializeField] Transform puntoDeRespawn;                                                        

    private Mov playerMovement;                                  
    public GameObject sprite;
    private Coroutine moveCoroutine;
    public GameObject pl;

    [SerializeField] Flowchart flowchart;

    #endregion

    void Awake()
    {
        playerMovement = GetComponent<Mov>();

        obstacleLayer = LayerMask.GetMask("detalle");

        StartCoroutine(fixeoMovimiento());
    }

    IEnumerator fixeoMovimiento()
    {
        while (true)
        {
            Vector2 first = transform.position;
            yield return new WaitForSeconds(5f);
            Vector2 second = transform.position;

            if (Vector2.Distance(first, second) < 0.01f && moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
                moveCoroutine = null;
                isMoving = false;
            }
        }
    }

    void FixedUpdate()
    {
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Prioridad: horizontal sobre vertical
        if (direction.x != 0)
            direction.y = 0;

        if (!isMoving && direction != Vector2.zero && flowchart.GetBooleanVariable("Hablando")== false)
        {
            Vector3 targetPosition = transform.position + new Vector3(direction.x * gridSize.x, direction.y * gridSize.y, 0);

            if (!IsObstacle(targetPosition))
            {
                moveCoroutine = StartCoroutine(Move(targetPosition));
            }
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
        moveCoroutine = null;
    }

    //public void ActualizarPuntoDeRespawn(Vector3 nuevaPosicion)
    //{
    //    puntoDeRespawn = nuevaPosicion;
    //}

    public IEnumerator RespawnCoroutine()
    { 
        PlayerAnimations playeranimations = GetComponentInChildren<PlayerAnimations>();
        playeranimations.anim.SetBool("Muerte", true);
        yield return new WaitForSeconds(2f);
        transform.position = puntoDeRespawn.position;
        playeranimations.anim.SetBool("Muerte", false);
        //playerMovement.enabled = true;           
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
    #endregion
}
