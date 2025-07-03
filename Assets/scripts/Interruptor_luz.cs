using UnityEngine;
using Fungus;

public class Interruptor_luz : MonoBehaviour
{
    public bool encendido;
    bool Recogida; //booleana que determina si la llave asociada al interruptor ha sido recogida

    [SerializeField] GameObject luz, Llave, Enemigo;

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
                Recogida = true;

                if (Recogida) //Si la llave ya ha sido recogida se desasocia del código para evitar que vuelva a reaparecer al salir de su casilla
                {
                    Llave.SetActive(false);
                }
            }

            if (Enemigo != null && flowchart.GetIntegerVariable("Diálogo") == 17) //Si nos encontramos en el punto adecuado de la historia y no se han activado otros enemigos antes
            {
                Enemigo.SetActive(true);
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

            if (Enemigo != null)
            {
                Enemigo.SetActive(false);
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
