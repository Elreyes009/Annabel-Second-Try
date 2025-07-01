using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    private bool sobreBotonSalir = false;
    private Animator anim; 


    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.anyKeyDown && !sobreBotonSalir)
        {
            SceneManager.LoadScene(1);
        }
    }

    public void SalirDelJuego()
    {
        Application.Quit();
        print("Saliendo del juego...");
    }

    public void OnPointerEnterSalir()
    {
        sobreBotonSalir = true;
        anim.SetBool("Si",true);
    }

    public void OnPointerExitSalir()
    {
        sobreBotonSalir = false;
        anim.SetBool("Si", false);
    }
}
