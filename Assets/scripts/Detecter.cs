using UnityEngine;
using static Enemigo;

public class Detecter : MonoBehaviour
{
    public Enemigo monsters2;
    public GameObject player;
    private AudioSource au;

    private void Start()
    {
        au = GetComponent<AudioSource>();
    }

    private void Update()
    {
        Collider2D col = player.GetComponent<Collider2D>();
        if (col != enabled)
        {
            monsters2.VolverAPatrullar();
        }
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (monsters2.estadoActual != EstadoEnemigo.Persiguiendo)
            {
                monsters2.PerseguirJugador(collision.gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            monsters2.PerseguirJugador(collision.gameObject);
            if (au != null) au.Play();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            monsters2.VolverAPatrullar();
        }
    }
}
