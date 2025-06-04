using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Lights : MonoBehaviour
{

    [SerializeField] GameObject pLight;
    [SerializeField] GameObject player;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
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
            PlayerAnimations panim = player.GetComponent<PlayerAnimations>();
            panim.spriteOscuro = false;
            pLight.GetComponent<Light2D>().intensity = 0f;
        }

        if (collision.CompareTag("Interactuable") && gameObject.GetComponent<NpcMoveTo>() != null)
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
            PlayerAnimations panim = player.GetComponent<PlayerAnimations>();
            panim.spriteOscuro = true;

        }

        if (collision.CompareTag("Interactuable") && gameObject.GetComponent<NpcMoveTo>() != null)
        {
            NpcMoveTo npc = collision.gameObject.GetComponent<NpcMoveTo>();
            if (npc != null)
            {
                npc.spriteOscuro = false;
            }
            else
            {
                               Debug.LogWarning("NpcMoveTo component not found on " + collision.gameObject.name);
            }
        }

    }

}
