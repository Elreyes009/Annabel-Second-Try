using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCast : MonoBehaviour
{
    [Header("Fungus Flowchart")]
    [SerializeField] private Flowchart flowchart;
    [SerializeField] List<string> inventario = new List<string>();

    [Header("Raycast Settings")]
    [SerializeField] private float rayDistance = 2f;
    [SerializeField] private LayerMask interactionMask;   // Crea una capa “Interactuable” y asígnala aquí

    private Vector2 lastMovementDirection = Vector2.down;
    private GameObject player;
    [SerializeField] private float pushForce = 1f;
    private void Awake()
    {
        player = gameObject;
        // Si no lo asignaste en el Inspector, lo busca en escena
        if (flowchart == null)
            flowchart = FindObjectOfType<Flowchart>();

    }

    private void Update()
    {
        interactionMask = LayerMask.GetMask("detalle");
        HandleInput();
        TryInteract();
    }

    private void HandleInput()
    {

        Vector2 rawInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (rawInput.x != 0) rawInput.y = 0;
        if (rawInput != Vector2.zero)
            lastMovementDirection = rawInput.normalized;


        
        
    }

    private void TryInteract()
    {

        Debug.DrawRay(transform.position, lastMovementDirection * rayDistance, Color.green, 0.5f);


        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            lastMovementDirection,
            rayDistance,
            interactionMask
        );

        if (hit.collider != null)
        {
            

            if (hit.collider.CompareTag("Interactuable"))
            {
                NPC npc = hit.collider.GetComponent<NPC>();

                if (npc != null)
                {
                    flowchart.SetBooleanVariable("InterObject", false);
                    flowchart.SetBooleanVariable("Personaje", true);
                    string name = npc.Name;
                    flowchart.SetStringVariable("Name", name);
                    return;
                }








                if (npc == null)
                {
                    flowchart.SetBooleanVariable("Personaje", false);
                    flowchart.SetStringVariable("Name", null);

                    if (Input.GetKeyDown(KeyCode.E))
                    {

                        print("e presed");
                        Vector2 pushDir = (hit.transform.position - player.transform.position).normalized;
                        Rigidbody2D rb = hit.collider.GetComponent<Rigidbody2D>();
                        if (Mathf.Abs(pushDir.x) > 0.1f)
                        {
                            // Empujando horizontal → bloquea movimiento vertical
                            rb.constraints = RigidbodyConstraints2D.FreezePositionY;
                        }
                        else
                        {
                            // Empujando vertical → bloquea movimiento horizontal
                            rb.constraints = RigidbodyConstraints2D.FreezePositionX;
                        }
                        rb.AddForce(pushDir * pushForce, ForceMode2D.Impulse);
                        StartCoroutine(ReleasePlayerConstraints(rb));
                    }
                }
            }

        }
        else
        {
            Debug.Log("Ya no");
            flowchart.SetBooleanVariable("Personaje", false);
            flowchart.SetStringVariable("Name", null);
            flowchart.SetBooleanVariable("InterObject", false);
        }
    }

    private IEnumerator ReleasePlayerConstraints(Rigidbody2D rb)
    {
        yield return new WaitForSeconds(1f);
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactuable"))
        {
            Puerta puerta = collision.transform.GetComponent<Puerta>();
            Animator puertaAnim = collision.transform.GetComponent<Animator>();
            Recogible rec = collision.transform.GetComponent<Recogible>();


            if (rec != null || Input.GetKeyDown(KeyCode.E))
            {
                collision.transform.gameObject.SetActive(false);
                inventario.Add(collision.transform.GetComponent<Recogible>().itemName);
                return;
            }

            if(puerta != null)
            {
                if (puerta.requerimiento == "null")
                {
                    puertaAnim.SetBool("Puerta", true);
                    return;
                }

                foreach (string nombre in inventario)
                {
                    if (nombre == puerta.requerimiento)
                    {
                        puertaAnim.SetBool("Puerta", true);
                        return;
                    }
                }
            }
        }
    }

}
