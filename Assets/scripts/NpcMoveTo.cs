using System.Collections;
using UnityEngine;
using Fungus;

public class NpcMoveTo : MonoBehaviour
{
    [SerializeField] private LayerMask obstacleLayer;
    public GameObject moveTo;
    [SerializeField] private float moveSpeed = 3f;

    [SerializeField] private Vector2 gridSize = new Vector2(1f, 1f);

    private GameObject panelDiaologos;
    private bool isMoving = false;
    private bool hasArrived = false;

    private float gridOffsetX = 0.5f; // Offset de la grid en X

    [SerializeField] Flowchart flowchart;

    private void Start()
    {
        panelDiaologos = GameObject.FindWithTag("DialogPanel");

        //if (moveTo == null)
        //{
        //    moveTo = GameObject.FindWithTag("MM");
        //}

        // Alinear el NPC a la cuadrícula al inicio con offset
        Vector2 aligned = new Vector2(
            Mathf.Round((transform.position.x - gridOffsetX) / gridSize.x) * gridSize.x + gridOffsetX,
            Mathf.Round(transform.position.y / gridSize.y) * gridSize.y
        );
        transform.position = aligned;
    }

    private void Update()
    {
        if (flowchart.GetBooleanVariable("Puede_moverse") == true && moveTo != null)
        {
            Moovement();
        }

    }

    public void MoverEnemigo(Vector2 targetPosition)
    {
        if (isMoving) return;

        Vector2 enemyPosition = transform.position;
        Vector2 direction = (targetPosition - enemyPosition);

        // Cancelar si ya estamos muy cerca
        if (direction.sqrMagnitude < 0.01f)
            return;

        Vector2 moveDirection = Vector2.zero;

        // Elegir dirección cardinal dominante
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            moveDirection = new Vector2(Mathf.Sign(direction.x), 0);
        else
            moveDirection = new Vector2(0, Mathf.Sign(direction.y));

        Vector2 finalTarget = enemyPosition + new Vector2(moveDirection.x * gridSize.x, moveDirection.y * gridSize.y);

        // Redondear a la cuadrícula con offset en X
        finalTarget = new Vector2(
            Mathf.Round((finalTarget.x - gridOffsetX) / gridSize.x) * gridSize.x + gridOffsetX,
            Mathf.Round(finalTarget.y / gridSize.y) * gridSize.y
        );

        if (!IsObstacle(finalTarget))
        {
            StartCoroutine(Move(finalTarget));
        }
    }

    IEnumerator Move(Vector3 targetPosition)
    {
        isMoving = true;

        while ((targetPosition - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition;
        isMoving = false;
    }

    bool IsObstacle(Vector3 targetPosition)
    {
        Collider2D obstacle = Physics2D.OverlapCircle(targetPosition, 0.2f, obstacleLayer);
        return obstacle != null;
    }
    void Moovement()
    {
        // Esperar si hay diálogo activo
        if (panelDiaologos != null && panelDiaologos.activeSelf)
            return;

        // Buscar el objetivo si no está asignado
        if (moveTo == null || !moveTo.activeSelf)
        {
            moveTo = GameObject.FindWithTag("MM");
            if (moveTo == null) return;
        }

        Next nextComponent = moveTo.GetComponent<Next>();
        if (nextComponent == null) return;

        // No seguir si no está activado
        if (!nextComponent.seguir)
            return;

        // Si ya llegamos, no moverse más
        if (hasArrived) return;

        // Comprobar si llegamos al destino
        if (Vector2.Distance(transform.position, moveTo.transform.position) < 0.2f)
        {
            if (nextComponent.nextObject != null)
            {
                nextComponent.nextObject.SetActive(true);
            }

            moveTo.SetActive(false);
        }
        // Solo mover si no estamos en movimiento
        if (!isMoving)
        {
            MoverEnemigo(moveTo.transform.position);
        }
    }
}
