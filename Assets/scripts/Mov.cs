﻿using System.Collections;
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
    [SerializeField] Vector3 puntoDeRespawn;                                                        

    private Mov playerMovement;                                  
    public GameObject sprite;

    public GameObject pl;

    [SerializeField] Flowchart flowchart;

    #endregion

    void Awake()
    {
        playerMovement = GetComponent<Mov>();

        obstacleLayer = LayerMask.GetMask("detalle");
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
                StartCoroutine(Move(targetPosition));
            }
            // Si hay obstáculo, simplemente no se mueve. El jugador debe elegir otra dirección.
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

    public void ActualizarPuntoDeRespawn(Vector3 nuevaPosicion)
    {
        puntoDeRespawn = nuevaPosicion;
    }

    public IEnumerator RespawnCoroutine()
    { 
        yield return new WaitForSeconds(1f);       
        transform.position = puntoDeRespawn;              
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
    #endregion
}
