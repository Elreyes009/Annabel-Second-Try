using UnityEngine;
using Fungus;

public class Teleport : MonoBehaviour
{
    [SerializeField] Animator OldAnim; //El animator que asignaremos al volver
    Animator CurrentAnim;
    [SerializeField] Animator NewAnim; //El animator que aplicaremos al hacer el cambio
    
    [SerializeField] Transform NewPosition; //Posición a la que nos vamos a trasladar
    [SerializeField] Vector3 Oldposition; //Posición a la que volveremos al cambiar

    [SerializeField] Sprite NewSprite; //El nuevo sprite que usaremos
    [SerializeField] Sprite OldSprite; //El viejo sprite;

    SpriteRenderer cambiador; //El renderer del sprite


    [SerializeField] Flowchart flowchart;

    private void Awake()
    {
        CurrentAnim = GetComponentInChildren<Animator>();
        cambiador = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (flowchart != null && flowchart.GetBooleanVariable("Cambio") == true) //Aquí, flowchart le dice al código que hay que cambiar de Annabel a Marco
        {
            transform.position = NewPosition.position; //La posición cambia para transportar al jugador a la posición correcta
            //cambiador.sprite = NewSprite; //El renderer cambia el sprite de Annabel por el de Marco
            //CurrentAnim = NewAnim; //El animator de Annabel es cambiado por el de Marco
        }

        if (flowchart !=null && flowchart.GetBooleanVariable("Regreso") == true) //Aquí, flowchart le dice al código que hay que cambiar de Marco a Annabel
        {
            transform.position = Oldposition;
            //cambiador.sprite = OldSprite; //El renderer cambia el sprite de Marco por el de Annabel
            //CurrentAnim = OldAnim; //El animator de Marco es cambiado por el de Annabel
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("exterior_puerta")) //Aquí estamos revizando que el jugador haya entrado en el gatillo correcto
        {
            Oldposition = transform.position; //Se asigna la posición en la que el jugador entró al gatillo como OldPosition
                                                       //de esta forma, cuando terminé de jugar como Marco, el jugador será transportado a la misma posición en la que estaba
            Debug.Log("posición");
        }
    }
}
