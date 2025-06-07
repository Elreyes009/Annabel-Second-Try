using UnityEngine;
using Fungus;

public class Teleport : MonoBehaviour
{
    
    [SerializeField] Vector3 NewPosition; //Posici�n a la que nos vamos a trasladar
    [SerializeField] Vector3 Oldposition; //Posici�n a la que volveremos al cambiar

    [SerializeField] Flowchart flowchart;

    private void Update()
    {
        if (flowchart != null && flowchart.GetBooleanVariable("Cambio") == true) //Aqu�, flowchart le dice al c�digo que hay que cambiar de Annabel a Marco
        {
            transform.position = NewPosition; //La posici�n cambia para transportar al jugador a la posici�n correcta
        }

        if (flowchart !=null && flowchart.GetBooleanVariable("Regreso") == true) //Aqu�, flowchart le dice al c�digo que hay que cambiar de Marco a Annabel
        {
            transform.position = Oldposition;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("exterior_puerta")) //Aqu� estamos revizando que el jugador haya entrado en el gatillo correcto y se haya quedado dentro
        {
            Oldposition = transform.position; //Se asigna la posici�n en la que el jugador entr� al gatillo como OldPosition
                                                       //de esta forma, cuando termin� de jugar como Marco, el jugador ser� transportado a la misma posici�n en la que estaba
        }
    }
}
