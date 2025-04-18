//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;

//public class Player_controller : MonoBehaviour
//{
//    float Velocidad;

//    Rigidbody2D rb;

//    Vector2 Movement;

//    [SerializeField] List<string> inventario = new List<string>();

//    [SerializeField] Transform respawn;

//    //[SerializeField] bool muerte;
//    //public bool Muerte { get { return muerte; } }

//    private void Awake()
//    {
//        rb = GetComponent<Rigidbody2D>();

//        Velocidad = 5f;
//    }

//    private void Update()
//    {
//        float h = Input.GetAxisRaw("Horizontal"); //Eje X
//        float y = Input.GetAxisRaw("Vertical"); //Eje Y
//        Movement = new Vector2(h, y).normalized; //El Vector2 que usaremos para mover al personaje.
//                                                 //Normalized est� ah� para evitar que las diagonales sean m�s r�pida
//    }

//    private void FixedUpdate()
//    {
//        rb.MovePosition(rb.position + Movement * Velocidad * Time.fixedDeltaTime); //Se uza la posici�n de RigidBody y el vector de movimiento multiplicado por la velocidad y el tiempo

//        if (Movement != Vector2.zero) //Si el personaje no est� quieto
//        {
//            float angle = Mathf.Atan2(Movement.y, Movement.x) * Mathf.Rad2Deg; //Se calcula el �ngulo en los radianes de la direcci�n y luego son convertidos a los grados que ser�n utilizados para la rotaci�n
//            transform.rotation = Quaternion.Euler(0, 0, angle); //Se aplica los grados de rotaci�n en el eje Z
//        }
//    }

//    private void OnTriggerEnter2D(Collider2D collision)
//    {
//        if (collision.transform.GetComponent<Recogible>())
//        {
//            collision.transform.gameObject.SetActive(false);
//            inventario.Add(collision.transform.GetComponent<Recogible>().itemName);
//        }

//        if (collision.CompareTag("Puerta"))
//        {
//            foreach (string nombre in inventario)
//            {
//                if (nombre == collision.transform.GetComponent<Puerta>().requerimiento)
//                {
//                    collision.transform.GetComponent<Animator>().SetBool("Acci�n", true);
//                    return;
//                }
//            }
//        }

//        //if (collision.CompareTag("Muerte"))
//        //{
//        //    StartCoroutine(Contador_muerte());
//        //}

//        if (collision.CompareTag("Check point"))
//        {
//            respawn.position = collision.transform.position;
//        }
//    }

//    //IEnumerator Contador_muerte()
//    //{
//    //    muerte = true;
//    //    rb.transform.position = respawn.position;
//    //    yield return new WaitForSeconds(0.002f);
//    //    muerte = false;
//    //    yield return null;
//    //}
//}
