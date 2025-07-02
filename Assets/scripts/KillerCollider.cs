using UnityEngine;

public class KillerCollider : MonoBehaviour
{
    public Enemigo enemigo; // Reference to the enemy script
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(enemigo.MatarJugador());
            Debug.Log("muerte");
        }
    }
}
