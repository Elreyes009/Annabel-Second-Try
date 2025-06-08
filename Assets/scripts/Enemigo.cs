using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class Enemigo : MonoBehaviour
{
    public GestorDeEnemigos enemManager;
    public float moveSpeed = 3f;
    public Vector2 gridSize = new Vector2(1f, 1f);
    public LayerMask obstacleLayer;

    private bool playerIsHiding = false;
    public bool isMoving = false;
    private Transform player;
    public Transform patrolEmty; // Debe tener el script Teletransportador
    private Teletransportador patrolTarget;
    private Mov playerMovement;
    private Animator anim;

    public Vector3 posicionInicial;

    public bool inLight;
    public bool inVision;

    [SerializeField] Flowchart flowchart;

    private Vector2 ultimaDireccion = Vector2.zero;

    // NUEVO: Para controlar el paso en la luz
    private bool pasoJugadorProcesado = false;

    [Header("Collider de colisión real (NO el de detección)")]
    [SerializeField] private BoxCollider2D colliderMovimiento;

    // Flags para logs únicos
    private bool logInLight = false;
    private bool logOutLight = false;
    private bool logPlayerMovement = false;
    private bool logPatrolTarget = false;
    private bool logColliderMovimiento = false;

    private void Start()
    {
        if (!logPlayerMovement)
        {
            player = FindAnyObjectByType<Mov>()?.transform;
            if (player != null)
            {
                playerMovement = player.GetComponent<Mov>();
                Debug.Log("[Enemigo] playerMovement asignado correctamente.");
            }
            else
            {
                Debug.LogError("[Enemigo] No se encontró el objeto del jugador en la escena.");
            }
            logPlayerMovement = true;
        }

        posicionInicial = transform.position;
        anim = GetComponent<Animator>();

        if (!logPatrolTarget)
        {
            if (patrolEmty != null)
            {
                patrolTarget = patrolEmty.GetComponent<Teletransportador>();
                Debug.Log("[Enemigo] patrolTarget asignado correctamente.");
            }
            else
            {
                Debug.LogWarning("[Enemigo] patrolEmty no asignado.");
            }
            logPatrolTarget = true;
        }

        if (!logColliderMovimiento)
        {
            if (colliderMovimiento == null)
                Debug.LogWarning("[Enemigo] colliderMovimiento NO asignado en el Inspector.");
            else
                Debug.Log("[Enemigo] colliderMovimiento asignado correctamente.");
            logColliderMovimiento = true;
        }
    }

    private void FixedUpdate()
    {
        Vector2 moveTo;
        if (inVision)
        {
            moveTo = player.position;
        }
        else
        {
            moveTo = patrolEmty.position;
        }

        // Verifica si ya está en el objetivo (alineado al grid)
        Vector2 posActual = AlignToGrid(transform.position);
        Vector2 posObjetivo = AlignToGrid(moveTo);
        if (posActual == posObjetivo)
            return; // Ya está en el objetivo, no hace falta mover

        // Log de estado de luz solo cuando cambia
        if (inLight && !logInLight)
        {
            Debug.Log("[Enemigo] Está en la luz.");
            logInLight = true;
            logOutLight = false;
        }
        else if (!inLight && !logOutLight)
        {
            Debug.Log("[Enemigo] Está FUERA de la luz.");
            logOutLight = true;
            logInLight = false;
        }

        if (inLight)
        {
            if (playerMovement != null && playerMovement.IsPlayerMoving() && !isMoving && !playerIsHiding && !pasoJugadorProcesado)
            {
                Debug.Log("[Enemigo] Condición para mover en luz CUMPLIDA. Llamando a MoverEnemigo.");
                StartCoroutine(MoverEnemigo(moveTo));
                pasoJugadorProcesado = true;
            }
            if (playerMovement != null && !playerMovement.IsPlayerMoving())
            {
                if (pasoJugadorProcesado)
                    Debug.Log("[Enemigo] El jugador dejó de moverse. Reseteando pasoJugadorProcesado.");
                pasoJugadorProcesado = false;
            }
        }
        else
        {
            if (!isMoving)
            {
                Debug.Log("[Enemigo] Condición para mover fuera de la luz CUMPLIDA. Llamando a MoverEnemigo.");
                StartCoroutine(MoverEnemigo(moveTo));
            }
        }

        UpdateAnimation();
    }

    IEnumerator MoverEnemigo(Vector2 targetPosition)
    {
        Debug.Log($"[Enemigo] MoverEnemigo llamado hacia {targetPosition}");
        if (isMoving)
        {
            Debug.Log("[Enemigo] Ya está moviéndose, saliendo de MoverEnemigo.");
            yield break;
        }
        isMoving = true;

        Vector2 start = AlignToGrid(transform.position);
        Vector2 goal = AlignToGrid(targetPosition);

        Debug.Log($"[Enemigo] Pathfinding de {start} a {goal}");
        List<Vector2> path = FindPathAStar(start, goal);
        if (path == null || path.Count == 0)
        {
            Debug.LogWarning($"[Enemigo] No se encontró camino de {start} a {goal}");
            isMoving = false;
            yield break;
        }

        if (inLight)
        {
            Vector2 step = path[0];
            Debug.Log($"[Enemigo] En luz, intentando dar un paso a {step}");
            if (!IsObstacle(step))
            {
                ultimaDireccion = ((Vector2)step - (Vector2)transform.position).normalized;
                UpdateAnimation();
                yield return Move(step);
                Debug.Log($"[Enemigo] Paso dado en la luz a {step}");
            }
            else
            {
                Debug.LogWarning($"[Enemigo] Obstáculo detectado en {step} (en luz)");
            }
        }
        else
        {
            foreach (var step in path)
            {
                Debug.Log($"[Enemigo] Fuera de la luz, intentando mover a {step}");
                if (IsObstacle(step))
                {
                    Debug.LogWarning($"[Enemigo] Obstáculo detectado en {step} (fuera de luz), reintentando.");
                    isMoving = false;
                    StartCoroutine(MoverEnemigo(targetPosition));
                    yield break;
                }

                ultimaDireccion = ((Vector2)step - (Vector2)transform.position).normalized;
                UpdateAnimation();
                yield return Move(step);
                Debug.Log($"[Enemigo] Paso dado fuera de la luz a {step}");
            }
        }

        if (!inVision && patrolTarget != null)
        {
            float distancia = Vector2.Distance(transform.position, patrolEmty.position);
            Debug.Log($"[Enemigo] Distancia a patrolEmty: {distancia}");
            if (distancia < 0.1f)
            {
                Debug.Log("[Enemigo] Llegó a patrolEmty, notificando a Teletransportador.");
                patrolTarget.NotificarLlegadaEnemigo();
            }
        }

        ultimaDireccion = Vector2.zero;
        UpdateAnimation();
        isMoving = false;
        Debug.Log("[Enemigo] Movimiento terminado.");
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

    private Vector2 AlignToGrid(Vector2 pos)
    {
        Vector2 aligned = new Vector2(
            Mathf.Round((pos.x) / gridSize.x) * gridSize.x,
            Mathf.Round((pos.y) / gridSize.y) * gridSize.y
        );
        Debug.Log($"[Enemigo] AlignToGrid: {pos} -> {aligned}");
        return aligned;
    }

    IEnumerator Move(Vector2 targetPosition)
    {
        Debug.Log($"[Enemigo] Move hacia {targetPosition}");
        while (((Vector2)transform.position - targetPosition).sqrMagnitude > 0.01f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition;
        Debug.Log($"[Enemigo] Move completado en {targetPosition}");
    }

    bool IsObstacle(Vector2 targetPosition)
    {
        Vector2 aligned = AlignToGrid(targetPosition);
        bool resultado;
        if (colliderMovimiento == null)
            resultado = Physics2D.OverlapCircle(aligned, 0.5f, obstacleLayer) != null;
        else
            resultado = Physics2D.OverlapBox(
                aligned + colliderMovimiento.offset,
                colliderMovimiento.size,
                0f,
                obstacleLayer
            ) != null;
        Debug.Log($"[Enemigo] IsObstacle en {aligned}: {resultado}");
        return resultado;
    }

    private List<Vector2> FindPathAStar(Vector2 start, Vector2 goal)
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
            {
                Debug.LogWarning("[Enemigo] Pathfinding: se superó el máximo de nodos explorados.");
                return null;
            }

            open.Sort((a, b) => a.f.CompareTo(b.f));
            Nodo actual = open[0];
            open.RemoveAt(0);

            if (Vector2.Distance(actual.pos, goal) < 0.01f)
            {
                List<Vector2> path = new List<Vector2>();
                Nodo paso = actual;
                while (paso.padre != null)
                {
                    path.Insert(0, paso.pos);
                    paso = paso.padre;
                }
                Debug.Log($"[Enemigo] Path encontrado de longitud {path.Count}");
                return path;
            }

            closed.Add(actual.pos);

            foreach (var dir in dirs)
            {
                Vector2 vecino = AlignToGrid(actual.pos + dir);
                if (closed.Contains(vecino) || IsObstacle(vecino))
                    continue;

                float g = actual.g + gridSize.magnitude;
                float h = Vector2.Distance(vecino, goal);

                Nodo existente = open.Find(n => n.pos == vecino);
                if (existente != null && existente.g <= g)
                    continue;

                open.Add(new Nodo(vecino, actual, g, h));
            }
        }
        Debug.LogWarning("[Enemigo] Pathfinding: no se encontró camino.");
        return null;
    }

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"[Enemigo] OnTriggerEnter2D con {collision.gameObject.name} ({collision.tag})");
        if (collision.CompareTag("Player"))
        {
            // Solo activa inVision si el jugador NO está escondido
            if (!playerIsHiding)
            {
                inVision = true;
                Debug.Log("[Enemigo] inVision = true (jugador entró en trigger y no está escondido)");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log($"[Enemigo] OnTriggerExit2D con {collision.gameObject.name} ({collision.tag})");
        if (collision.CompareTag("Player"))
        {
            inVision = false;
            Debug.Log("[Enemigo] inVision = false (jugador salió del trigger)");
        }
    }

    public void SetPlayerHiding(bool isHiding)
    {
        Debug.Log($"[Enemigo] SetPlayerHiding({isHiding})");
        playerIsHiding = isHiding;
        if (isHiding)
        {
            inVision = false;
            Debug.Log("[Enemigo] inVision = false (jugador se escondió)");
        }
        // Si el jugador deja de estar escondido y está dentro del trigger, puedes reactivar inVision aquí si lo necesitas.
    }

    public IEnumerator MatarJugador()
    {
        Debug.Log("[Enemigo] MatarJugador()");
        Mov respawnScript = player.GetComponent<Mov>();
        StartCoroutine(respawnScript.RespawnCoroutine());
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
        else
        {
            Debug.LogError("[Enemigo] Movimiento del jugador no encontrado.");
        }

        yield return new WaitForSeconds(1f);

        enemManager.VolverAposicionInicial();

        flowchart.SetBooleanVariable("Muerte", true);

        if (flowchart.GetIntegerVariable("Reinicio") == 0)
        {
            flowchart.ExecuteBlock("Reinicio_1");
        }
        else if (flowchart.GetIntegerVariable("Reinicio") == 1)
        {
            flowchart.ExecuteBlock("Reinicio_2");
        }
        else if (flowchart.GetIntegerVariable("Reinicio") == 2)
        {
            flowchart.ExecuteBlock("Reinicio_3");
        }
        else if (flowchart.GetIntegerVariable("Reinicio") == 3)
        {
            flowchart.ExecuteBlock("Reinicio_4");
        }
    }
}
