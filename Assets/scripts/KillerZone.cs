using UnityEngine;

public class KillerZone : MonoBehaviour
{
    public Monsters2 monsters2;
    public GameObject player;
    private AudioSource au;

    private void Start()
    {
        au = GetComponent<AudioSource>();
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == player)
        {
            monsters2.inVision = true;
            au.Play();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision == player)
        {
            monsters2.inVision = false;
        }
    }

}
