using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    private Animator anim;
    private GameObject panelDiaologos;

    private enum Direccion { Ninguna, Derecha, Izquierda, Arriba, Abajo }
    private Direccion direccionActiva = Direccion.Ninguna;

    void Start()
    {

        panelDiaologos = GameObject.FindWithTag("DialogPanel");
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (panelDiaologos.activeSelf)
        {
            anim.SetBool("Derecha", false);
            anim.SetBool("Izquierda", false);
            anim.SetBool("Arriba", false);
            anim.SetBool("Abajo", false);
            direccionActiva = Direccion.Ninguna; // Reinicia también la dirección
            return;
        }

        Animations();
    }

    private void Animations()
    {
        // Actualiza dirección si la tecla activa se ha soltado
        if (direccionActiva != Direccion.Ninguna)
        {
            switch (direccionActiva)
            {
                case Direccion.Derecha:
                    if (!Input.GetKey(KeyCode.D)) direccionActiva = Direccion.Ninguna;
                    break;
                case Direccion.Izquierda:
                    if (!Input.GetKey(KeyCode.A)) direccionActiva = Direccion.Ninguna;
                    break;
                case Direccion.Arriba:
                    if (!Input.GetKey(KeyCode.W)) direccionActiva = Direccion.Ninguna;
                    break;
                case Direccion.Abajo:
                    if (!Input.GetKey(KeyCode.S)) direccionActiva = Direccion.Ninguna;
                    break;
            }
        }

        // Si está en "Ninguna", buscar la nueva dirección, priorizando orden
        if (direccionActiva == Direccion.Ninguna)
        {
            if (Input.GetKey(KeyCode.D))
                direccionActiva = Direccion.Derecha;
            else if (Input.GetKey(KeyCode.A))
                direccionActiva = Direccion.Izquierda;
            else if (Input.GetKey(KeyCode.W))
                direccionActiva = Direccion.Arriba;
            else if (Input.GetKey(KeyCode.S))
                direccionActiva = Direccion.Abajo;
        }

        // Actualiza animaciones
        anim.SetBool("Derecha", direccionActiva == Direccion.Derecha);
        anim.SetBool("Izquierda", direccionActiva == Direccion.Izquierda);
        anim.SetBool("Arriba", direccionActiva == Direccion.Arriba);
        anim.SetBool("Abajo", direccionActiva == Direccion.Abajo);
    }

}
