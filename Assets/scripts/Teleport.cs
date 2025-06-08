using UnityEngine;
using Fungus;

public class Teleport : MonoBehaviour
{
    
    [SerializeField] Vector3 NewPosition; //Posici�n a la que nos vamos a trasladar
    [SerializeField] Vector3 OldPosition; //Posici�n a la que nos vamos a trasladar
    [SerializeField] Transform Respawn; //Posici�n a la que volveremos al cambiar

    [SerializeField] Flowchart flowchart;

    [SerializeField] bool Player;

    private void Update()
    {
        if (flowchart != null && flowchart.GetBooleanVariable("Cambio") == true) //Aqu�, flowchart le dice al c�digo que hay que cambiar de Annabel a Marco
        {
            transform.position = NewPosition; //La posici�n cambia para transportar al jugador a la posici�n correcta
        }

        if (flowchart !=null && flowchart.GetBooleanVariable("Regreso") == true && !Player) //Aqu�, flowchart le dice al c�digo que hay que cambiar de Marco a Annabel
        {
            transform.position = Respawn.position;
        }

        if (flowchart != null && flowchart.GetBooleanVariable("Regreso") == true && Player && flowchart.GetIntegerVariable("Di�logo") <= 16)
        {
            transform.position = OldPosition;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("exterior_puerta")) //Aqu� estamos revizando que el jugador haya entrado en el gatillo correcto y se haya quedado dentro
        {
            OldPosition = transform.position; //Se asigna la posici�n en la que el jugador entr� al gatillo como OldPosition
                                                       //de esta forma, cuando termin� de jugar como Marco, el jugador ser� transportado a la misma posici�n en la que estaba
        }
    }
}
