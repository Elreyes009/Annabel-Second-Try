using UnityEngine;
using Fungus;

public class Interruptor_luz : MonoBehaviour
{
    public bool encendido;

    [SerializeField] GameObject luz, Llave;

    private string nameTag;

    [SerializeField] Flowchart flowchart;

    Animator anim;

    private void Awake()
    {
        nameTag = gameObject.tag;
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

            if (Llave != null)
            {
                Llave.SetActive(true);
            }
        }
        else if (!encendido)
        {
            luz.SetActive(false);
            anim.SetBool("Encender", false);

            if (Llave != null)
            {
                Llave.SetActive(false);
            }
        }

        if (flowchart != null && flowchart.GetBooleanVariable("Muerte") == true)
        {
            gameObject.gameObject.tag = nameTag;
            encendido = false;
            anim.SetBool("Encender", false);
        }
    }
}
