using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class Enemigo : MonoBehaviour
{
    public GestorDeEnemigos enemManager;
    public float moveSpeed = 3f;
    public Vector2 gridSize = new Vector2(1f, 1f);
    public LayerMask obstacleLayer;


    private bool playerIsHiding = false;
    public bool isMoving = false;
    private Transform player;
    public Transform patrolEmty;
    private Mov playerMovement;
    private Animator playerAnimator;

    public Vector3 posicionInicial;

    public bool inLight;
    public bool inVision;
    void Start()
    {
        player = FindAnyObjectByType<Mov>().transform;
        if (player != null)
        {
            playerMovement = player.GetComponent<Mov>();
            playerAnimator = player.GetComponent<Animator>();
        }
        else
        {
            Debug.LogError("No se encontró el objeto del jugador en la escena.");
        }

        posicionInicial = transform.position;

        //StartCoroutine(Movement());
    }

    //private IEnumerator Movement()
    //{
    //    while(true){

    //        if(
    //        yield return new WaitForSecondsRealtime(3);

    //    }
    //}

    void FixedUpdate()
    {
        Vector2 moveTo;
        if (inVision)
        {
            moveTo = player.position;
        }
        else
        {
            moveTo = patrolEmty.position;
        }

        if (inLight)
        {
            if (playerMovement != null && playerMovement.IsPlayerMoving() && !isMoving && !playerIsHiding)
            {
                MoverEnemigo(moveTo);
            }
        }
        else
        {
            MoverEnemigo(moveTo);
        }

    }

    public void MoverEnemigo(Vector2 playerPosition)
    {
        if (isMoving || player == null) return;

        Vector2 enemyPosition = transform.position;
        Vector2 direction = (playerPosition - enemyPosition).normalized;

        Vector2 moveDirection = Vector2.zero;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            moveDirection = new Vector2(Mathf.Sign(direction.x), 0);
        }
        else
        {
            moveDirection = new Vector2(0, Mathf.Sign(direction.y));
        }

        Vector2 targetPosition = enemyPosition + new Vector2(moveDirection.x * gridSize.x, moveDirection.y * gridSize.y);

        if (!IsObstacle(targetPosition))
        {
            StartCoroutine(Move(targetPosition));
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

    bool IsObstacle(Vector3 targetPosition)
    {
        Collider2D obstacle = Physics2D.OverlapCircle(targetPosition, 0.2f, obstacleLayer);
        return obstacle != null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SetPlayerHiding(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SetPlayerHiding(true);
        }
    }



    public void SetPlayerHiding(bool isHiding)
    {

        if (isHiding)
        {
            inVision = false;
            
        }
        else
        {
            inVision = true;
        }
    }

    public IEnumerator MatarJugador()
    {
        Mov respawnScript = player.GetComponent<Mov>();
        StartCoroutine(respawnScript.RespawnCoroutine());
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
        else
        {
            Debug.LogError("Movimiento del jugador no encontrado.");
        }

        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Muerte");
        }
        else
        {
            Debug.LogError("Animator del jugador no encontrado.");
        }

        yield return new WaitForSeconds(1f);

        enemManager.VolverAposicionInicial();
    }
}
