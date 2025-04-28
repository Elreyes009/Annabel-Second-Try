using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestorDeEnemigos : MonoBehaviour
{
    public List<Enemigo> enemigos; // Lista de enemigos
    public GameObject player;
    private bool esco;
    [SerializeField] private Flowchart flowchart;
    


    void Start()
    {
        // Asegúrate de que la lista de enemigos esté correctamente llena
        enemigos = new List<Enemigo>(FindObjectsOfType<Enemigo>());
        player = FindFirstObjectByType <Mov>().gameObject;
        flowchart = FindFirstObjectByType<Flowchart>();
    }

    private void Update()
    {

        if (esco == true && flowchart.GetBooleanVariable("Escondido") == true && Input.GetKeyDown(KeyCode.F))
        {
            flowchart.SetBooleanVariable("Escondido", false);
        }
        if (flowchart.GetBooleanVariable("Escondido") == true)
        {
            player.GetComponent<Mov>().enabled = false;
            player.GetComponent<SpriteRenderer>().enabled = false;
            player.GetComponent<PlayerAnimations>().enabled = false;
            player.GetComponent<BoxCollider2D>().enabled = false;
            Debug.Log("1");
        }
        if(flowchart.GetBooleanVariable("Escondido") == false)
        {

            NotificarEscondite(false);
            player.GetComponent<Mov>().enabled = true;
            player.GetComponent<PlayerAnimations>().enabled = true;
            player.GetComponent<SpriteRenderer>().enabled = true;
            player.GetComponent<BoxCollider2D>().enabled = true;
            Debug.Log("2");

        }
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
            // Detén cualquier corrutina DE ESE enemigo
            enemigo.StopAllCoroutines();
            // Asegúrate de resetear su flag de movimiento
            enemigo.isMoving = false;
            // ¡Este transform sí es el del enemigo!
            enemigo.transform.position = enemigo.posicionInicial;
        }
    }

}