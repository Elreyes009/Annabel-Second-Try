using UnityEngine;
using Fungus;

public class Serena_handler : MonoBehaviour
{
    int layer;

    [SerializeField] Flowchart flowchart;

    [SerializeField] GameObject Manager; //Referencia al manager al que pertenece esta instancia de Serena

    // Update is called once per frame
    void Update()
    {
        if (flowchart.GetIntegerVariable("Di�logo") == 3)
        {
            layer = LayerMask.NameToLayer("Ignore Raycast"); //Asignamos a layer el valor num�rico de la capa a la que queremos mover el objeto
            gameObject.layer = layer; //Le indicamos al objeto que cambie a la capa de valor num�rico correspondiente
        }

        if (Manager.activeInHierarchy == true) //Mientras el Manager est� activo
        {
            if (gameObject.tag != "Interactuable") //Y no sea una instancia de Serena con la que haya que interactuar
            {
                gameObject.tag = "Serena"; //Se le asignar� la etiqueta de Serena
            }
        }
        else if (Manager.activeInHierarchy == false)
        {
            gameObject.tag = "Untagged";
        }
    }
}
