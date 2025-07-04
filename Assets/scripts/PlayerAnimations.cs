using UnityEngine;
using Fungus;

public class PlayerAnimations : MonoBehaviour
{
    public Animator anim;
    [SerializeField] Animator OldAnim; //El animator que asignaremos al volver
    public GameObject panel;

    private enum Direccion { Ninguna, Derecha, Izquierda, Arriba, Abajo }
    private Direccion direccionActiva = Direccion.Ninguna;

    public bool spriteOscuro;
    public GameObject r;
    public bool escondido = false;

    [SerializeField] Flowchart flowchart;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (panel.activeSelf)
        {
            anim.SetBool("Derecha", false);
            anim.SetBool("Izquierda", false);
            anim.SetBool("Arriba", false);
            anim.SetBool("Abajo", false);
        }

        if (flowchart.GetBooleanVariable("Hablando") == false)
        {
            Animations();
        }

        int oscuroLayer = anim.GetLayerIndex("Oscuro");
        int normalLayer = anim.GetLayerIndex("Default");
        if (oscuroLayer >= 0) anim.SetLayerWeight(oscuroLayer, spriteOscuro ? 1f : 0f);
        if (normalLayer >= 0) anim.SetLayerWeight(normalLayer, spriteOscuro ? 0f : 1f);

        Escon();
    }

    public void Escon()
    {
        Mov mov = r.GetComponent<Mov>();
        SpriteRenderer serena_render = GameObject.FindWithTag("Serena")?.GetComponent<SpriteRenderer>();

        if (escondido)
        {
            if (serena_render != null)
                serena_render.enabled = false;

            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
            mov.enabled = false;
        }
        else
        {
            if (serena_render != null)
                serena_render.enabled = true;

            GetComponent<SpriteRenderer>().enabled = true;
            GetComponent<Collider2D>().enabled = true;
            mov.enabled = true;
        }
    }

    private void Animations()
    {
        // Actualiza dirección si la tecla activa se ha soltado
        if (direccionActiva != Direccion.Ninguna)
        {
            switch (direccionActiva)
            {
                case Direccion.Derecha:
                    if (!Input.GetKey(KeyCode.D) || !Input.GetKey(KeyCode.RightArrow)) direccionActiva = Direccion.Ninguna;
                    break;
                case Direccion.Izquierda:
                    if (!Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) direccionActiva = Direccion.Ninguna;
                    break;
                case Direccion.Arriba:
                    if (!Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) direccionActiva = Direccion.Ninguna;
                    break;
                case Direccion.Abajo:
                    if (!Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) direccionActiva = Direccion.Ninguna;
                    break;
            }
        }

        // Si está en "Ninguna", buscar la nueva dirección, priorizando orden
        if (direccionActiva == Direccion.Ninguna)
        {
            if (Input.GetKey(KeyCode.D) || Input.GetKey (KeyCode.RightArrow))
                direccionActiva = Direccion.Derecha;
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                direccionActiva = Direccion.Izquierda;
            else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                direccionActiva = Direccion.Arriba;
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                direccionActiva = Direccion.Abajo;
        }

        // Actualiza animaciones
        anim.SetBool("Derecha", direccionActiva == Direccion.Derecha);
        anim.SetBool("Izquierda", direccionActiva == Direccion.Izquierda);
        anim.SetBool("Arriba", direccionActiva == Direccion.Arriba);
        anim.SetBool("Abajo", direccionActiva == Direccion.Abajo);
    }

}
