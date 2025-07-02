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

    [Header("Otros")]
    private Vector2 lastMovementDirection = Vector2.down;
    private GameObject player;
    [SerializeField] private float pushForce = 1f;

    public PlayerAnimations sprite;

    [SerializeField] Animator anim;

    private void Awake()
    {
        player = gameObject;
    }

    private void Update()
    {
        HandleInput();
        TryInteract();

        if (flowchart.GetBooleanVariable("Hablando") == true)
        {
            anim.SetBool("SeRepliega", true);
            anim.SetBool("SeDespliega", false);
        }

        if (!gameObject.activeSelf)
        {
            anim.SetBool("SeRepliega", true);
            anim.SetBool("SeDespliega", false);
        }
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
            rayDistance
        );

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Interactuable"))
            {
                NPC npc = hit.collider.GetComponent<NPC>();
                anim.SetBool("SeDespliega", true);
                anim.SetBool("SeRepliega", false);

                if (npc != null)
                {

                    flowchart.SetBooleanVariable("InterObject", false);
                    flowchart.SetBooleanVariable("Personaje", true);
                    string name = npc.Name;
                    flowchart.SetStringVariable("Name", name);

                    if (name == "Luz")
                    {
                        Interruptor_luz interruptor = hit.collider.GetComponent<Interruptor_luz>();
                        if (Input.GetKeyDown(KeyCode.E))
                        {
                            anim.SetBool("SeRepliega", true);
                            anim.SetBool("SeDespliega", false);
                            interruptor.encendido = true;
                        }
                    }


                    return;
                }

                if (npc == null)
                {

                    flowchart.SetBooleanVariable("Personaje", false);
                    flowchart.SetStringVariable("Name", null);

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        anim.SetBool("SeRepliega", true);
                        anim.SetBool("SeDespliega", false);
                        Vector2 pushDir = (hit.transform.position - player.transform.position).normalized;
                        Rigidbody2D rb = hit.collider.GetComponent<Rigidbody2D>();
                        if (Mathf.Abs(pushDir.x) > 0.1f)
                        {

                            rb.constraints = RigidbodyConstraints2D.FreezePositionY;
                        }
                        else
                        {
                            // Empujando vertical → bloquea movimiento horizontal
                            rb.constraints = RigidbodyConstraints2D.FreezePositionX;
                        }
                        rb.AddForce(pushDir * pushForce, ForceMode2D.Impulse);
                        AudioSource audio = hit.collider.GetComponent<AudioSource>();
                        if (audio != null)
                        {
                            audio.Play();
                        }
                        StartCoroutine(ReleasePlayerConstraints(rb));
                    }
                }
            }

            if (hit.collider.CompareTag("Escondite"))
            {
                anim.SetBool("SeDespliega", true);
                anim.SetBool("SeRepliega", false);
            }

            if (hit.collider.CompareTag("Puerta"))
            {
                anim.SetBool("SeDespliega", true);
                anim.SetBool("SeRepliega", false);

                Mov mov = GetComponent<Mov>();


                //foreach (string nombre in inventario)
                //{
                //    if (nombre == hit.collider.GetComponent<Puerta>().requerimiento)
                //    {
                //        puertaAnim.SetBool("Puerta", true);
                //        puertaAnim.SetBool("Puerta", true);
                //        AudioSource audio = hit.collider.transform.GetComponent<AudioSource>();
                //        return;
                //    }
                //}
            }

            if (hit.collider.CompareTag("Escondite") && Input.GetKeyDown(KeyCode.E))
            {
                sprite.escondido = true;

                AudioSource audio = hit.collider.GetComponent<AudioSource>();
                if (audio != null)
                {
                    audio.Play();
                }
            }
        }
        else
        {
            flowchart.SetBooleanVariable("Personaje", false);
            flowchart.SetStringVariable("Name", null);
            flowchart.SetBooleanVariable("InterObject", false);
            anim.SetBool("SeRepliega", true);
            anim.SetBool("SeDespliega", false);
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

            if (rec != null)
            {
                collision.transform.gameObject.gameObject.SetActive(false);
                inventario.Add(collision.transform.GetComponent<Recogible>().itemName);
                AudioSource audio = GetComponent<AudioSource>();
                if (audio != null)
                {
                    audio.Play();
                }
                return;
            }

            if (puerta != null)
            {
                if (puerta.requerimiento == "null")
                {
                    puertaAnim.SetBool("Puerta", true);
                    AudioSource audio = collision.transform.GetComponent<AudioSource>();
                    if (audio != null)
                    {
                        audio.Play();
                    }
                    return;
                }

                foreach (string nombre in inventario)
                {
                    if (nombre == puerta.requerimiento)
                    {
                        puertaAnim.SetBool("Puerta", true);
                        puertaAnim.SetBool("Puerta", true);
                        AudioSource audio = collision.transform.GetComponent<AudioSource>();
                        return;
                    }
                }
            }
        }
    }
}
