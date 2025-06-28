using UnityEngine;
using Fungus;

public class BGM_Selecter : MonoBehaviour
{
    public GameManager zettings;
    private AudioSource audioSource;
    public AudioClip music;

    [SerializeField] Flowchart flowchart;

    bool active;

    private void Start()
    {
        GameManager gm = zettings.GetComponent<GameManager>();
        audioSource = gm.Music;
    }

    private void Update()
    {
        if (flowchart != null)
        {
            if (flowchart.GetBooleanVariable("Iniciar") == true)
            {
                audioSource.clip = music;
                audioSource.Play();
                flowchart.SetBooleanVariable("Iniciar", false);
            }
        }
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

