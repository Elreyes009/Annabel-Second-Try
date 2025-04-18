//using Fungus;//Hay que utilizar Fungus para que el sistema funcione
//using UnityEngine;

//public class Raycaster : MonoBehaviour
//{
//    float DistanciaRayo;

//    [SerializeField] Flowchart flowchart; //Referencia al Flowchart de la historia, este flowchart es el que utilizan los personajes importantes en la historia
//                                          // y es único

//    private void Awake()
//    {
//        DistanciaRayo = 3f;
//    }

//    private void Update()
//    {
//        Debug.DrawRay(transform.position, transform.right * DistanciaRayo, Color.green);
//        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, DistanciaRayo);

//        if (hit.collider != null) //Verifica que el raycast esté detectando un collider
//                                    //Sere honesto, no sé por qué, pero si no le agregamos esta linea, el programa no funciona
//        {
//            if (hit.transform.tag == "Charachter") //Esta condición es diferente a la de los personajes con nombre
//                                                    // OJO
//            {
//                hit.transform.GetComponent<Flowchart>().SetBooleanVariable("Disponible", true); //Esta linea accede al flowchart del objetivo, este es diferente
//                                                                                                // del flowchart de la historia, por lo que cada personaje de
//                                                                                                // fondo necesita uno propio
//            }

//            if (hit.transform.tag == "Daina")
//            {
//                flowchart.SetBooleanVariable("Disponible", true);
//                flowchart.SetBooleanVariable("EsDaina", true);
//            }

//            if (hit.transform.tag == "Marco")
//            {
//                flowchart.SetBooleanVariable("Disponible", true);
//                flowchart.SetBooleanVariable("EsMarco", true);
//            }
//        }
//        else //Si el raycast no está detectando nada
//        {
//            flowchart.SetBooleanVariable("Disponible", false);
//            flowchart.SetBooleanVariable("EsDaina", false);
//            flowchart.SetBooleanVariable("EsMarco", false);
//        }
//    }
//}
