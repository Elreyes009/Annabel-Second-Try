using UnityEngine;
using Fungus;

public class Serena_layer_change : MonoBehaviour
{
    int layer;

    [SerializeField] Flowchart flowchart;

    // Update is called once per frame
    void Update()
    {
        if (flowchart.GetIntegerVariable("Diálogo") == 3)
        {
            layer = LayerMask.NameToLayer("Ignore Raycast"); //Asignamos a layer el valor numérico de la capa a la que queremos mover el objeto
            gameObject.layer = layer; //Le indicamos al objeto que cambie a la capa de valor numérico correspondiente
        }
    }
}
