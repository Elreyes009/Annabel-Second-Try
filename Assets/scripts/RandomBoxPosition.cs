using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Teletransportador : MonoBehaviour
{
    public Collider2D zonaMovimiento; // Asigna en el Inspector
    public float intervalo = 10f;     // Intervalo en segundos

    private List<Collider2D> zonasInLight = new List<Collider2D>();

    private void Start()
    {
        Lights[] luces = FindObjectsOfType<Lights>();
        foreach (Lights luz in luces)
        {
            Collider2D col = luz.GetComponent<Collider2D>();
            if (col != null)
            {
                zonasInLight.Add(col);
            }
        }

        StartCoroutine(TeletransportarCadaIntervalo());
    }

    private IEnumerator TeletransportarCadaIntervalo()
    {
        while (true)
        {
            yield return new WaitForSeconds(intervalo);
            Vector2 nuevaPosicion = ObtenerPosicionValida();
            transform.position = nuevaPosicion;
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
            posicionAleatoria = new Vector2(x, y);
            intentos++;
            if (intentos > 100)
            {
                Debug.LogWarning("No se encontró una posición válida fuera de las zonas de luz después de 100 intentos.");
                break;
            }
        }
        while (EstaEnZonaInLight(posicionAleatoria));

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
}
