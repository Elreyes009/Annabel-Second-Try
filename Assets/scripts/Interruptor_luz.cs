using UnityEngine;
using Fungus;

public class Interruptor_luz : MonoBehaviour
{
    public bool encendido;
    [SerializeField] GameObject luz;

    [SerializeField] Flowchart flowchart;

    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();

        encendido = false;
        luz.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (encendido)
        {
            luz.SetActive(true);
            anim.SetBool("Encender", true);
            gameObject.gameObject.tag = "Untagged";
        }
        else if (!encendido)
        {
            luz.SetActive(false);
            anim.SetBool("Encender", false);
        }

        if (flowchart != null && flowchart.GetBooleanVariable("Muerte") == true)
        {
            encendido = false;
            anim.SetBool("Encender", false);
        }
    }
}
