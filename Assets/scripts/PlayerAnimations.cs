using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    private Animator anim;
    private GameObject panelDiaologos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        panelDiaologos = GameObject.FindWithTag("DialogPanel");
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (panelDiaologos.activeSelf)
        {
            return;
        }

        Animations();
    }
    private void Animations()
    {
        if (Input.GetKey(KeyCode.D))
        {
            anim.SetInteger("Direccion", 3); // Derecha
        }
        else if (Input.GetKey(KeyCode.A))
        {
            anim.SetInteger("Direccion", 4); // Izquierda
        }
        else if (Input.GetKey(KeyCode.W))
        {
            anim.SetInteger("Direccion", 2); // Arriba
        }
        else if (Input.GetKey(KeyCode.S))
        {
            anim.SetInteger("Direccion", 1); // Abajo
        }
        else
        {
            anim.SetInteger("Direccion", 0); // Idle
        }
    }

}
