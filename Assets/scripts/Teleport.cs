using UnityEngine;
using Fungus;

public class Teleport : MonoBehaviour
{
    
    [SerializeField] Vector3 NewPosition; //Posición a la que nos vamos a trasladar
    [SerializeField] Vector3 OldPosition; //Posición a la que nos vamos a trasladar
    [SerializeField] Transform Respawn; //Posición a la que volveremos al cambiar

    [SerializeField] Flowchart flowchart;

    [SerializeField] bool Player;

    private void Update()
    {
        if (flowchart != null && flowchart.GetBooleanVariable("Cambio") == true) //Aquí, flowchart le dice al código que hay que cambiar de Annabel a Marco
        {
            transform.position = NewPosition; //La posición cambia para transportar al jugador a la posición correcta
        }

        if (flowchart !=null && flowchart.GetBooleanVariable("Regreso") == true && !Player) //Aquí, flowchart le dice al código que hay que cambiar de Marco a Annabel
        {
            transform.position = Respawn.position;
        }

        if (flowchart != null && flowchart.GetBooleanVariable("Regreso") == true && Player && flowchart.GetIntegerVariable("Diálogo") <= 16)
        {
            transform.position = OldPosition;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("exterior_puerta")) //Aquí estamos revizando que el jugador haya entrado en el gatillo correcto y se haya quedado dentro
        {
            OldPosition = transform.position; //Se asigna la posición en la que el jugador entró al gatillo como OldPosition
                                                       //de esta forma, cuando terminé de jugar como Marco, el jugador será transportado a la misma posición en la que estaba
        }
    }
}
