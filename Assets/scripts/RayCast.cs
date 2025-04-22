using Fungus;
using UnityEngine;

public class RayCast : MonoBehaviour
{
    [Header("Fungus Flowchart")]
    [SerializeField] private Flowchart flowchart;

    [Header("Raycast Settings")]
    [SerializeField] private float rayDistance = 2f;
    [SerializeField] private LayerMask interactionMask;   // Crea una capa “Interactuable” y asígnala aquí

    private Vector2 lastMovementDirection = Vector2.down;

    private void Awake()
    {
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
            Debug.Log("InteraccionDisponible");

            if (hit.collider.CompareTag("Interactuable"))
            {
                NPC npc = hit.collider.GetComponent<NPC>();
                if (npc != null)
                {
                    

                    flowchart.SetBooleanVariable("Personaje", true);
                    string name = npc.Name;
                    flowchart.SetStringVariable("Name", name);
                }
            }

        }
        else
        {
            Debug.Log("Ya no");
            flowchart.SetBooleanVariable("Personaje", false);
            flowchart.SetStringVariable("Name", null);
        }
    }
}
