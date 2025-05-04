using UnityEngine;
using Fungus;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject pause;
    public GameObject Options;
    public GameObject Controls;


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && pause.activeSelf == false)
        {
            pause.SetActive(true);
            Time.timeScale = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && pause.activeSelf == true)
        {
            pause.SetActive(false);
            Time.timeScale = 1;
        }


    }

    public void Controles()
    {
        if(Controls.activeSelf == false)
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
