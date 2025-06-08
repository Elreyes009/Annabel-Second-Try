using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestorDeEnemigos : MonoBehaviour
{
    public List<Enemigo> enemigos; // Lista de enemigos
    public GameObject playerSprite;
    public GameObject playerMove;
    [SerializeField] private Flowchart flowchart;

    private bool estadoEscondidoPrevio = false;

    void Start()
    {
        playerMove = FindFirstObjectByType<Mov>().gameObject;
        playerSprite = FindFirstObjectByType<PlayerAnimations>().gameObject;
        flowchart = FindFirstObjectByType<Flowchart>();
        estadoEscondidoPrevio = flowchart.GetBooleanVariable("Escondido");
        AplicarEstadoEscondido(estadoEscondidoPrevio);
    }

    private void Update()
    {
        bool estadoEscondidoActual = flowchart.GetBooleanVariable("Escondido");
        if (estadoEscondidoActual != estadoEscondidoPrevio)
        {
            AplicarEstadoEscondido(estadoEscondidoActual);
            estadoEscondidoPrevio = estadoEscondidoActual;
        }
    }

    private void AplicarEstadoEscondido(bool estaEscondido)
    {
        NotificarEscondite(estaEscondido);
        playerMove.GetComponent<Mov>().enabled = !estaEscondido;
        playerSprite.GetComponent<PlayerAnimations>().enabled = !estaEscondido;
        playerSprite.GetComponent<SpriteRenderer>().enabled = !estaEscondido;
        playerSprite.GetComponent<BoxCollider2D>().enabled = !estaEscondido;
    }

    public void NotificarEscondite(bool estaEscondido)
    {
        foreach (Enemigo enemigo in enemigos)
        {
            enemigo.SetPlayerHiding(estaEscondido); // Llama al método SetPlayerHiding para cada enemigo
        }
    }

    public void VolverAposicionInicial()
    {
        foreach (Enemigo enemigo in enemigos)
        {
            enemigo.StopAllCoroutines();
            enemigo.isMoving = false;
            enemigo.transform.position = enemigo.posicionInicial;
        }
    }
}