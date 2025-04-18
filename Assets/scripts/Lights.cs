using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Lights : MonoBehaviour
{
    private GameObject pLight;

    private void Awake()
    {
        pLight = GameObject.FindWithTag("PlayerLight");
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
            pLight.GetComponent<Light2D>().intensity = 0.5f * Time.deltaTime;
        }
    }
}
