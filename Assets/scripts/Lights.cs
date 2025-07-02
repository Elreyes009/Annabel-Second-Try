using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Lights : MonoBehaviour
{
    [SerializeField] GameObject pLight;
    [SerializeField] GameObject player;

    private void Awake()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player");
        if (pLight == null)
            pLight = GameObject.FindWithTag("PlayerLight");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemigo"))
        {
            var enem = collision.GetComponent<Enemigo>();
            if (enem != null)
                enem.inLight = true;
        }

        if (collision.CompareTag("Player"))
        {
            var panim = player != null ? player.GetComponent<PlayerAnimations>() : null;
            if (panim != null)
                panim.spriteOscuro = false;

            var light2D = pLight != null ? pLight.GetComponent<Light2D>() : null;
            if (light2D != null)
                light2D.intensity = 0f;
        }

        if (collision.GetComponent<NpcMoveTo>() != null)
        {
            var npc = collision.GetComponent<NpcMoveTo>();
            if (npc != null)
            {
                npc.spriteOscuro = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemigo"))
        {
            var enem = collision.GetComponent<Enemigo>();
            if (enem != null)
                enem.inLight = false;
        }

        if (collision.CompareTag("Player"))
        {
            var light2D = pLight != null ? pLight.GetComponent<Light2D>() : null;
            if (light2D != null)
                light2D.intensity = 0.5f;

            var panim = player != null ? player.GetComponent<PlayerAnimations>() : null;
            if (panim != null)
                panim.spriteOscuro = true;
        }

        if (collision.GetComponent<NpcMoveTo>() != null)
        {
            var npc = collision.GetComponent<NpcMoveTo>();
            if (npc != null)
                npc.spriteOscuro = true;
        }
    }
}
