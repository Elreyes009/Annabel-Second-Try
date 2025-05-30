using UnityEngine;
using Fungus;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    public GameObject pause;
    public GameObject Options;
    public GameObject Controls;

    public AudioMixer audioMixer;
    [SerializeField] AudioSource sorrow;
    [SerializeField] AudioSource unknown;

    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;

    void Start()
    {
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        bgmSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        // Inicializa sliders con valores del mixer
        float vol;
        if (audioMixer.GetFloat("Master", out vol)) masterSlider.value = vol;
        if (audioMixer.GetFloat("BGM", out vol)) bgmSlider.value = vol;
        if (audioMixer.GetFloat("SFX", out vol)) sfxSlider.value = vol;
    }

    void SetMasterVolume(float dB)
    {
        audioMixer.SetFloat("Master", dB);
    }

    void SetMusicVolume(float dB)
    {
        audioMixer.SetFloat("BGM", dB);
    }

    void SetSFXVolume(float dB)
    {
        audioMixer.SetFloat("SFX", dB);
    }


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && pause.activeSelf == false)
        {
            pause.SetActive(true);
            Time.timeScale = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && pause.activeSelf == true && Controls.activeSelf == false && Options.activeSelf == false)
        {
            pause.SetActive(false);
            Time.timeScale = 1;
        }


    }

    public void Controles()
    {
        if (Controls.activeSelf == false)
        {
            Controls.SetActive(true);
        }
        else
        {
            Controls.SetActive(false);
            pause.SetActive(true);
        }
    }

    public void Opciones()
    {
        if (Options.activeSelf == false)
        {
            Options.SetActive(true);
        }
        else
        {
            Options.SetActive(false);
            pause.SetActive(true);
        }
    }


    public void SceneLoader()
    {
        int escenaActual = SceneManager.GetActiveScene().buildIndex;
        if (escenaActual == 0)
        {
            SceneManager.LoadScene(1);
        }
        else if (escenaActual == 1)
        {
            SceneManager.LoadScene(0);
        }
        Time.timeScale = 1;
    }
}
