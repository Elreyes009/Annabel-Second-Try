using System.Collections;
using Fungus;
using UnityEngine;
using System.Collections.Generic;

public class NpcMoveTo : MonoBehaviour
{
    [Header("Movimiento y Grid")]
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Vector2 gridSize = new Vector2(1f, 1f);
    [Tooltip("Offset del grid en X e Y. Dejar en X(-0.4) Y(0.6). Si el tilemap se desplaza o cambia de tamaño, llamar al Diego para que lo vuelva a calcular.")]
    public Vector2 gridOffset = Vector2.zero;

    [Header("Referencias")]
    public GameObject moveTo;
    [SerializeField] private string npcName;
    [SerializeField] private Flowchart flowchart;
    [SerializeField] public bool spriteOscuro;
    [SerializeField] public bool siguiendo = false;
    [SerializeField] private GameObject player;

    private Animator anim;
    private Vector2 ultimaDireccion = Vector2.zero;
    private bool isMoving = false;
    private bool hasArrived = false;

    private void Awake()
    {
        npcName = gameObject.GetComponent<NPC>().name;
        if (moveTo == null && !siguiendo)
        {
            moveTo = GameObject.FindWithTag(npcName + "Points");
        }

        // Alinear posición inicial al centro del tile
        Vector2 aligned = new Vector2(
            Mathf.Round((transform.position.x - gridOffset.x) / gridSize.x) * gridSize.x + gridOffset.x,
            Mathf.Round((transform.position.y - gridOffset.y) / gridSize.y) * gridSize.y + gridOffset.y
        );
        transform.position = aligned;

        anim = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");

        int oscuroLayer = anim.GetLayerIndex("Oscuro");
        int normalLayer = anim.GetLayerIndex("Default");

        if (oscuroLayer >= 0) anim.SetLayerWeight(oscuroLayer, 0f);
        if (normalLayer >= 0) anim.SetLayerWeight(normalLayer, 1f);

        if (spriteOscuro)
        {
            if (oscuroLayer >= 0) anim.SetLayerWeight(oscuroLayer, 1f);
            if (normalLayer >= 0) anim.SetLayerWeight(normalLayer, 0f);
        }
    }

    private void Update()
    {
        if (flowchart.GetBooleanVariable("Puede_moverse") && moveTo != null)
        {
            Moove();
        }

        UpdateAnimation();

        int oscuroLayer = anim.GetLayerIndex("Oscuro");
        int normalLayer = anim.GetLayerIndex("Default");
        if (oscuroLayer >= 0) anim.SetLayerWeight(oscuroLayer, spriteOscuro ? 1f : 0f);
        if (normalLayer >= 0) anim.SetLayerWeight(normalLayer, spriteOscuro ? 0f : 1f);
    }

    public void MoverEnemigo(Vector2 targetPosition)
    {
        if (isMoving) return;

        Vector2 enemyPosition = transform.position;

        // Pathfinding si está siguiendo al jugador
        if (siguiendo)
        {
            Vector2 alignedTarget = new Vector2(
                Mathf.Round((targetPosition.x - gridOffset.x) / gridSize.x) * gridSize.x + gridOffset.x,
                Mathf.Round((targetPosition.y - gridOffset.y) / gridSize.y) * gridSize.y + gridOffset.y
            );

            if (IsObstacle(alignedTarget))
            {
                ultimaDireccion = Vector2.zero;
                return;
            }

            Vector2? nextStep = GetNextStepAStar(enemyPosition, alignedTarget);
            if (nextStep.HasValue && !IsObstacle(nextStep.Value))
            {
                Vector2 step = nextStep.Value;
                Vector2 delta = step - enemyPosition;

                // Bloquear movimiento diagonal: solo permitir X o Y, no ambos
                if (Mathf.Abs(delta.x) > Mathf.Epsilon && Mathf.Abs(delta.y) > Mathf.Epsilon)
                {
                    if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                        step = new Vector2(enemyPosition.x + Mathf.Sign(delta.x) * gridSize.x, enemyPosition.y);
                    else
                        step = new Vector2(enemyPosition.x, enemyPosition.y + Mathf.Sign(delta.y) * gridSize.y);
                }

                // Alinear el paso al grid
                step = new Vector2(
                    Mathf.Round((step.x - gridOffset.x) / gridSize.x) * gridSize.x + gridOffset.x,
                    Mathf.Round((step.y - gridOffset.y) / gridSize.y) * gridSize.y + gridOffset.y
                );

                ultimaDireccion = (step - enemyPosition).normalized;
                StartCoroutine(Move(step));
            }
            else
            {
                // Si no hay ruta o el siguiente tile está bloqueado, detén el NPC
                ultimaDireccion = Vector2.zero;
            }
            return;
        }

        // Movimiento por puntos normales (ya está bloqueado a un solo eje)
        Vector2 direction = (targetPosition - enemyPosition);

        if (direction.sqrMagnitude < 0.01f)
        {
            ultimaDireccion = Vector2.zero;
            return;
        }

        Vector2 moveDirection = Vector2.zero;
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            moveDirection = new Vector2(Mathf.Sign(direction.x), 0);
        else
            moveDirection = new Vector2(0, Mathf.Sign(direction.y));

        Vector2 finalTarget = enemyPosition + new Vector2(moveDirection.x * gridSize.x, moveDirection.y * gridSize.y);

        // Intentar centrar en el tile
        Vector2 alignedFinalTarget = new Vector2(
            Mathf.Round((finalTarget.x - gridOffset.x) / gridSize.x) * gridSize.x + gridOffset.x,
            Mathf.Round((finalTarget.y - gridOffset.y) / gridSize.y) * gridSize.y + gridOffset.y
        );

        if (!IsObstacle(alignedFinalTarget))
        {
            ultimaDireccion = moveDirection;
            StartCoroutine(Move(alignedFinalTarget));
        }
        else
        {
            // Si no puede centrarse, intenta avanzar al siguiente tile sin alinear
            Vector2 nextTile = enemyPosition + moveDirection * gridSize;
            if (!IsObstacle(nextTile))
            {
                ultimaDireccion = moveDirection;
                StartCoroutine(Move(nextTile));
            }
            else
            {
                ultimaDireccion = Vector2.zero;
            }
        }
    }

    private void UpdateAnimation()
    {
        if (anim == null) return;

        if (isMoving)
        {
            anim.SetBool("Derecha", ultimaDireccion.x > 0);
            anim.SetBool("Izquierda", ultimaDireccion.x < 0);
            anim.SetBool("Arriba", ultimaDireccion.y > 0);
            anim.SetBool("Abajo", ultimaDireccion.y < 0);
        }
        else
        {
            anim.SetBool("Derecha", false);
            anim.SetBool("Izquierda", false);
            anim.SetBool("Arriba", false);
            anim.SetBool("Abajo", false);
        }
    }

    void Moove()
    {
        if (siguiendo)
        {
            if (player == null) return;
            if (Vector2.Distance(transform.position, player.transform.position) < 1f)
                return;
            if (!isMoving)
            {
                MoverEnemigo(player.transform.position);
            }
            return;
        }

        if (moveTo == null || !moveTo.activeSelf)
        {
            moveTo = GameObject.FindWithTag(npcName + "Points");
            if (moveTo == null) return;
        }

        Next nextComponent = moveTo.GetComponent<Next>();
        if (nextComponent == null) return;
        if (!nextComponent.seguir) return;
        if (hasArrived) return;

        if (Vector2.Distance(transform.position, moveTo.transform.position) < 0.2f)
        {
            if (nextComponent.nextObject != null)
            {
                nextComponent.nextObject.SetActive(true);
            }
            moveTo.SetActive(false);
        }

        if (!isMoving)
        {
            MoverEnemigo(moveTo.transform.position);
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
        // Alinea la posición antes de chequear
        Vector2 aligned = new Vector2(
            Mathf.Round((targetPosition.x - gridOffset.x) / gridSize.x) * gridSize.x + gridOffset.x,
            Mathf.Round((targetPosition.y - gridOffset.y) / gridSize.y) * gridSize.y + gridOffset.y
        );

        var box = GetComponent<BoxCollider2D>();
        bool result;
        if (box == null)
            result = Physics2D.OverlapCircle(aligned, 0.5f, obstacleLayer) != null;
        else
            result = Physics2D.OverlapBox(
                aligned + box.offset,
                box.size,
                0f,
                obstacleLayer
            ) != null;

        if (result)
            Debug.LogWarning($"NPC {name} detecta obstáculo en {aligned}");

        return result;
    }

    // Nodo para A*
    private class Nodo
    {
        public Vector2 pos;
        public Nodo padre;
        public float g, h;
        public float f => g + h;
        public Nodo(Vector2 pos, Nodo padre, float g, float h)
        {
            this.pos = pos;
            this.padre = padre;
            this.g = g;
            this.h = h;
        }
    }

    // Devuelve el siguiente paso hacia el objetivo usando A*
    private Vector2? GetNextStepAStar(Vector2 start, Vector2 goal)
    {
        Vector2[] dirs = new Vector2[]
        {
            new Vector2(gridSize.x, 0),
            new Vector2(-gridSize.x, 0),
            new Vector2(0, gridSize.y),
            new Vector2(0, -gridSize.y)
        };

        var open = new List<Nodo>();
        var closed = new HashSet<Vector2>();
        open.Add(new Nodo(start, null, 0, Vector2.Distance(start, goal)));

        int maxNodos = 500;
        int nodosExplorados = 0;

        while (open.Count > 0)
        {
            if (++nodosExplorados > maxNodos)
                return null;

            open.Sort((a, b) => a.f.CompareTo(b.f));
            Nodo actual = open[0];
            open.RemoveAt(0);

            if (Vector2.Distance(actual.pos, goal) < 0.01f)
            {
                Nodo paso = actual;
                Nodo anterior = null;
                while (paso.padre != null && paso.padre.padre != null)
                {
                    anterior = paso;
                    paso = paso.padre;
                }
                return anterior != null ? anterior.pos : actual.pos;
            }

            closed.Add(actual.pos);

            foreach (var dir in dirs)
            {
                Vector2 vecino = new Vector2(
                    Mathf.Round((actual.pos.x + dir.x - gridOffset.x) / gridSize.x) * gridSize.x + gridOffset.x,
                    Mathf.Round((actual.pos.y + dir.y - gridOffset.y) / gridSize.y) * gridSize.y + gridOffset.y
                );
                if (closed.Contains(vecino) || IsObstacle(vecino))
                    continue;

                float g = actual.g + gridSize.magnitude;
                float h = Vector2.Distance(vecino, goal);
                open.Add(new Nodo(vecino, actual, g, h));
            }
        }
        return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        var box = GetComponent<BoxCollider2D>();
        Vector2 center = transform.position;
        if (box != null)
            center += box.offset;

        Vector2 pos = new Vector2(
            Mathf.Round((center.x - gridOffset.x) / gridSize.x) * gridSize.x + gridOffset.x,
            Mathf.Round((center.y - gridOffset.y) / gridSize.y) * gridSize.y + gridOffset.y
        );
        Gizmos.DrawWireCube(pos, gridSize);
    }
}