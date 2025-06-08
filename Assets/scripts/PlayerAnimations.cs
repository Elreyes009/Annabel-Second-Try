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
