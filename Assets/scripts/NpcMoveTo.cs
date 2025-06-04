using System.Collections;
using Fungus;
using UnityEngine;
using System.Collections.Generic;

public class NpcMoveTo : MonoBehaviour
{
    [SerializeField] private LayerMask obstacleLayer;
    public GameObject moveTo;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Vector2 gridSize = new Vector2(1f, 1f);

    private GameObject panelDiaologos;
    private bool isMoving = false;
    private bool hasArrived = false;

    private float gridOffsetX = 0.5f;
    [SerializeField] private string npcName;

    [SerializeField] private Flowchart flowchart;
    [SerializeField] public bool spriteOscuro;
    private Animator anim;
    private Vector2 ultimaDireccion = Vector2.zero;

    [SerializeField] public bool siguiendo = false;
    bool compañero; //booleana que indica si este personaje pede seggir al PJ
    [SerializeField] private GameObject player;

    private void Awake()
    {
        //panelDiaologos = GameObject.FindWithTag("DialogPanel");
        npcName = gameObject.GetComponent<NPC>().name;
        if (moveTo == null && !siguiendo)
        {
            moveTo = GameObject.FindWithTag(npcName + "Points");
        }

        if (npcName != "Serena") //A menos que se trate de Serena, el PNJ no puede seguir al PJ
        {
            compañero = false;
        }
        else
        {
            compañero= true;
        }

        Vector2 aligned = new Vector2(
                Mathf.Round((transform.position.x - gridOffsetX) / gridSize.x) * gridSize.x + gridOffsetX,
                Mathf.Round(transform.position.y / gridSize.y) * gridSize.y
            );
        transform.position = aligned;

        anim = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");

        int oscuroLayer = anim.GetLayerIndex("Oscuro");
        int normalLayer = anim.GetLayerIndex("Default");

        // Forzar pesos desde cero al inicio
        if (oscuroLayer >= 0) anim.SetLayerWeight(oscuroLayer, 0f);
        if (normalLayer >= 0) anim.SetLayerWeight(normalLayer, 1f);

        // Ahora asigna correctamente según sOscuro
        if (spriteOscuro)
        {
            if (oscuroLayer >= 0) anim.SetLayerWeight(oscuroLayer, 1f);
            if (normalLayer >= 0) anim.SetLayerWeight(normalLayer, 0f);
        }
    }

    private void Update()
    {
        Moove();

        // Actualiza animación cada frame según estado de movimiento
        UpdateAnimation();

        if (flowchart.GetBooleanVariable("Seguir")== true && compañero)
        {
            siguiendo = true;
        }


        int oscuroLayer = anim.GetLayerIndex("Oscuro");
        int normalLayer = anim.GetLayerIndex("Default");
        if (oscuroLayer >= 0) anim.SetLayerWeight(oscuroLayer, spriteOscuro ? 0f : 1f);
        if (normalLayer >= 0) anim.SetLayerWeight(normalLayer, spriteOscuro ? 1f : 0f);
    }

    public void MoverEnemigo(Vector2 targetPosition)
    {
        if (isMoving) return;

        Vector2 enemyPosition = transform.position;

        // Si está siguiendo al jugador, usa siempre A*
        if (siguiendo == true)
        {
            // Alinear el objetivo a la grilla
            Vector2 alignedTarget = new Vector2(
                Mathf.Round((targetPosition.x - gridOffsetX) / gridSize.x) * gridSize.x + gridOffsetX,
                Mathf.Round(targetPosition.y / gridSize.y) * gridSize.y
            );

            // Si el objetivo es un obstáculo, no buscar camino
            if (IsObstacle(alignedTarget))
            {
                ultimaDireccion = Vector2.zero;
                return;
            }

            Vector2? nextStep = GetNextStepAStar(enemyPosition, alignedTarget);
            if (nextStep.HasValue && !IsObstacle(nextStep.Value))
            {
                ultimaDireccion = (nextStep.Value - enemyPosition).normalized;
                StartCoroutine(Move(nextStep.Value));
            }
            else
            {
                ultimaDireccion = Vector2.zero;
            }
            return;
        }

        // --- Lógica original para puntos normales ---
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

        finalTarget = new Vector2(
            Mathf.Round((finalTarget.x - gridOffsetX) / gridSize.x) * gridSize.x + gridOffsetX,
            Mathf.Round(finalTarget.y / gridSize.y) * gridSize.y
        );

        if (!IsObstacle(finalTarget))
        {
            ultimaDireccion = moveDirection;
            StartCoroutine(Move(finalTarget));
        }
        else
        {
            ultimaDireccion = Vector2.zero;
        }
    }

    // Actualiza animaciones según el estado de movimiento y dirección
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
            // Seguir al jugador por tiles
            if (player == null) return;

            // Si ya está cerca del jugador, no moverse más
            if (Vector2.Distance(transform.position, player.transform.position) < 3f)
                return;

            if (!isMoving)
            {
                MoverEnemigo(player.transform.position);
            }
            return;
        }

        if (flowchart.GetBooleanVariable("Puede_moverse") == true && moveTo != null)
        {
            // Lógica normal de puntos
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
        var box = GetComponent<BoxCollider2D>();
        if (box == null)
            return Physics2D.OverlapCircle(targetPosition, 0.5f, obstacleLayer) != null;

        // Simula el box en la posición destino
        Collider2D hit = Physics2D.OverlapBox(
            (Vector2)targetPosition + box.offset, // posición destino + offset del collider
            box.size,
            0f,
            obstacleLayer
        );
        return hit != null;
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

        int maxNodos = 500; // Limita la cantidad de nodos para evitar bucles infinitos
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
                    Mathf.Round((actual.pos.x + dir.x - gridOffsetX) / gridSize.x) * gridSize.x + gridOffsetX,
                    Mathf.Round((actual.pos.y + dir.y) / gridSize.y) * gridSize.y
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
}
