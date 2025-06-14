using UnityEngine;

public class BGM_Selecter : MonoBehaviour
{
    public GameManager zettings;
    private AudioSource audioSource;
    public AudioClip music;

    private void Start()
    {
        GameManager gm = zettings.GetComponent<GameManager>();
        audioSource = gm.Music;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            audioSource.clip = music;
            audioSource.Play();
            Destroy(this);
        }
    }
}

