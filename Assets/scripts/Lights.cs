using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Lights : MonoBehaviour
{

    [SerializeField] GameObject pLight;

    private void Awake()
    {
        pLight = GameObject.FindWithTag("PlayerLight");
        if(pLight == null)
        {
            pLight = FindAnyObjectByType<Mov>().pl.gameObject;
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemigo"))
        {
            Enemigo enem = collision.gameObject.GetComponent<Enemigo>();
            enem.inLight = true;
        }

        if (collision.CompareTag("Player"))
        {
            pLight.GetComponent<Light2D>().intensity = 0f;
        }

        if (collision.CompareTag("Interactuable"))
        {
            NpcMoveTo npc = collision.gameObject.GetComponent<NpcMoveTo>();
            if (npc != null)
            {
                npc.spriteOscuro = true;
                Debug.Log("spriteOscuro activado en " + npc.name);
            }
            else
            {
                Debug.LogWarning("NpcMoveTo component not found on " + collision.gameObject.name);
            }
        }


    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemigo"))
        {
            Enemigo enem = collision.gameObject.GetComponent<Enemigo>();
            enem.inLight = false;
        }

        if (collision.CompareTag("Player"))
        {
            pLight.GetComponent<Light2D>().intensity = 0.5f;
        }

        if (collision.CompareTag("Interactuable"))
        {
            NpcMoveTo npc = collision.gameObject.GetComponent<NpcMoveTo>();
            if (npc != null)
            {
                npc.spriteOscuro = false;
                Debug.Log("spriteOscuro desactivado en " + npc.name);
            }
            else
            {
                               Debug.LogWarning("NpcMoveTo component not found on " + collision.gameObject.name);
            }
        }

    }

}
