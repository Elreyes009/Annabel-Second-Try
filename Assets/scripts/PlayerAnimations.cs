using UnityEngine;
using Fungus;
using UnityEngine.Splines;

public class PlayerAnimations : MonoBehaviour
{
    private Animator anim;
    [SerializeField] Animator OldAnim; //El animator que asignaremos al volver
    [SerializeField] Animator NewAnim; //El animator que aplicaremos al hacer el cambio

    private enum Direccion { Ninguna, Derecha, Izquierda, Arriba, Abajo }
    private Direccion direccionActiva = Direccion.Ninguna;

    public bool spriteOscuro;

    [SerializeField] Flowchart flowchart;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (flowchart.GetBooleanVariable("Hablando") == false)
        {
            Animations();
        }

        if (flowchart != null && flowchart.GetBooleanVariable("Cambio") == true) //Aquí, flowchart le dice al código que hay que cambiar de Annabel a Marco
        {
            anim = NewAnim; //El animator de Annabel es cambiado por el de Marco
            Debug.Log("cambio");
        }

        if (flowchart != null && flowchart.GetBooleanVariable("Regreso") == true) //Aquí, flowchart le dice al código que hay que cambiar de Marco a Annabel
        {
            anim = OldAnim; //El animator de Marco es cambiado por el de Annabel
        }

        int oscuroLayer = anim.GetLayerIndex("Oscuro");
        int normalLayer = anim.GetLayerIndex("Default");
        if (oscuroLayer >= 0) anim.SetLayerWeight(oscuroLayer, spriteOscuro ? 1f : 0f);
        if (normalLayer >= 0) anim.SetLayerWeight(normalLayer, spriteOscuro ? 0f : 1f);


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
