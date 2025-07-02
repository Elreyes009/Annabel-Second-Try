using UnityEngine;

public class Detecter : MonoBehaviour
{
    public Enemigo monsters2;
    public GameObject player;
    private AudioSource au;

    private void Start()
    {
        au = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            monsters2.moveTo = player;
            if (au != null) au.Play();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Vuelve a buscar el punto de patrulla usando el npcName del enemigo
            string patrolTag = monsters2.npcName + "Points";
            GameObject patrolPoint = GameObject.FindWithTag(patrolTag);
            monsters2.moveTo = patrolPoint;
        }
    }
}
