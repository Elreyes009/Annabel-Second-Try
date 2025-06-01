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
    


    void Start()
    {
        // Asegúrate de que la lista de enemigos esté correctamente llena
        //enemigos = new List<Enemigo>(FindObjectsOfType<Enemigo>());
        playerMove = FindFirstObjectByType <Mov>().gameObject;
        playerSprite = FindFirstObjectByType<PlayerAnimations>().gameObject;
        flowchart = FindFirstObjectByType<Flowchart>();
    }

    private void Update()
    {

        if (flowchart.GetBooleanVariable("Escondido") == true)
        {
            NotificarEscondite(true);
            playerMove.GetComponent<Mov>().enabled = false;
            playerSprite.GetComponent<SpriteRenderer>().enabled = false;
            playerSprite.GetComponent<PlayerAnimations>().enabled = false;
            playerSprite.GetComponent<BoxCollider2D>().enabled = false;
            
        }
        
        if(flowchart.GetBooleanVariable("Escondido") == false)
        {
            
            NotificarEscondite(false);
            playerMove.GetComponent<Mov>().enabled = true;
            playerSprite.GetComponent<PlayerAnimations>().enabled = true;
            playerSprite.GetComponent<SpriteRenderer>().enabled = true;
            playerSprite.GetComponent<BoxCollider2D>().enabled = true;
            

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

            Debug.Log("Funcionando");
        }
    }

}