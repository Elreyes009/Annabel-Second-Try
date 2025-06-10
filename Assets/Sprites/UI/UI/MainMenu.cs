using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Nombre de la escena de juego, cámbialo si tu escena tiene otro nombre
    [SerializeField] private string NameScene = "Juego";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene(1);
        }
    }

    // Este método se debe asignar al botón de salir en la UI
    public void SalirDelJuego()
    {
        Application.Quit();
        print("Saliendo del juego...");
    }
}
