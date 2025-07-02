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
            //monsters2.siguiendo = true;
            monsters2.moveTo = player;
            au.Play();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //monsters2.siguiendo = false;
            monsters2.moveTo = null;
        }
    }

}
