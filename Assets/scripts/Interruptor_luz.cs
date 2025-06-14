using UnityEngine;
using Fungus;

public class Interruptor_luz : MonoBehaviour
{
    public bool encendido;
    [SerializeField] GameObject luz;

    [SerializeField] Flowchart flowchart;

    private void Awake()
    {
        encendido = false;
        luz.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (encendido)
        {
            luz.SetActive(true);
        }
        else if (!encendido)
        {
            luz.SetActive(false);
        }

        if (flowchart != null && flowchart.GetBooleanVariable("Muerte") == true)
        {
            luz.SetActive(false);
        }
    }
}
