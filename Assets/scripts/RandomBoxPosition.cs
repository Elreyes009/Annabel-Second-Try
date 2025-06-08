using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomBoxPosition : MonoBehaviour
{
    public Collider2D zonaMovimiento; // Asigna en el Inspector
    [SerializeField] private LayerMask obstacleLayer; // Asigna en el Inspector
    [SerializeField] private float radioCercaniaEnemigo = 0.6f; // Radio para detectar al enemigo cerca
    private List<Collider2D> zonasInLight = new List<Collider2D>();

    private bool enemigoEnObjetivo = false;
    private bool playerDioPaso = false;
    private bool playerEstabaMoviendo = false; // NUEVO: para detectar el cambio de estado
    private Mov playerMovement;
    private Vector2 gridSize = new Vector2(1f, 1f);

    private void Start()
    {
        playerMovement = FindObjectOfType<Mov>();
    }

    public void Update()
    {
        // Detectar si hay un enemigo cerca
        enemigoEnObjetivo = HayEnemigoCerca();

        // Detectar un "paso" real del jugador (transición de quieto a moviendo)
        bool playerMoviendoAhora = playerMovement != null && playerMovement.IsPlayerMoving();
        if (playerMoviendoAhora && !playerEstabaMoviendo)
        {
            NotificarPasoJugador();
        }
        playerEstabaMoviendo = playerMoviendoAhora;

        zonasInLight.Clear();

        Lights[] luces = FindObjectsOfType<Lights>();
        foreach (Lights luz in luces)
        {
            Collider2D col = luz.GetComponent<Collider2D>();
            if (col != null)
            {
                zonasInLight.Add(col);
            }
        }
    }

    private bool HayEnemigoCerca()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radioCercaniaEnemigo);
        foreach (var col in colliders)
        {
            if (col.CompareTag("Enemigo"))
                return true;
        }
        return false;
    }

    public void NotificarPasoJugador()
    {
        playerDioPaso = true;
        IntentarTeletransportar();
    }

    private void IntentarTeletransportar()
    {
        if (enemigoEnObjetivo && playerDioPaso)
        {
            Vector2 nuevaPosicion = ObtenerPosicionValida();
            transform.position = nuevaPosicion;
            enemigoEnObjetivo = false;
            playerDioPaso = false;
        }
    }

    private Vector2 ObtenerPosicionValida()
    {
        Bounds boundsMovimiento = zonaMovimiento.bounds;

        Vector2 posicionAleatoria;
        int intentos = 0;
        do
        {
            float x = Random.Range(boundsMovimiento.min.x, boundsMovimiento.max.x);
            float y = Random.Range(boundsMovimiento.min.y, boundsMovimiento.max.y);

            // Alinear al grid del enemigo
            x = Mathf.Round(x / gridSize.x) * gridSize.x;
            y = Mathf.Round(y / gridSize.y) * gridSize.y;
            posicionAleatoria = new Vector2(x, y);

            intentos++;
            if (intentos > 100)
            {
                break;
            }
        }
        while (EstaEnZonaInLight(posicionAleatoria) || HayObstaculo(posicionAleatoria));

        return posicionAleatoria;
    }

    private bool EstaEnZonaInLight(Vector2 posicion)
    {
        foreach (Collider2D zona in zonasInLight)
        {
            if (zona.OverlapPoint(posicion))
            {
                return true;
            }
        }
        return false;
    }

    private bool HayObstaculo(Vector2 posicion)
    {
        // Ajusta el radio según el tamaño de la caja
        float radio = 0.3f;
        return Physics2D.OverlapCircle(posicion, radio, obstacleLayer) != null;
    }
}
