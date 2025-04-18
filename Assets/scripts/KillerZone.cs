using UnityEngine;

public class KillerZone : MonoBehaviour
{
    public Enemigo enemigo;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(enemigo.MatarJugador());
        }
    }
}
