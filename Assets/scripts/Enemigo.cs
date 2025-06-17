using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class Enemigo : MonoBehaviour
{
    [Header("Movimiento y Grid")]
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Vector2 gridSize = new Vector2(1f, 1f);
    [Tooltip("Offset del grid en X e Y. Dejar en X(-0.4) Y(0.6).")]
    public Vector2 gridOffset = Vector2.zero;

    [Header("Referencias")]
    public Transform patrolEmty;
    [SerializeField] private Flowchart flowchart;
    [SerializeField] public bool spriteOscuro;
    [SerializeField] private Animator anim;

    // --- AUDIO ---
    [Header("Audio")]
    public AudioClip alertaClip; // Asigna el clip desde el inspector
    private AudioSource audioSource;
    private bool haReproducidoAlerta = false;

    private Vector2 ultimaDireccion = Vector2.zero;
    public bool isMoving = false;
    private bool pasoJugadorProcesado = false;
    private bool playerIsHiding = false;

    private Transform player;
    private Mov playerMovement;

    public GestorDeEnemigos enemManager;

    public bool inLight;
    public bool inVision;

    public Vector3 posicionInicial;

    [SerializeField] BoxCollider2D colliderMovimiento;

    private void Awake()
    {
        Vector2 aligned = AlignToGrid(transform.position);
        transform.position = aligned;

        anim = GetComponent<Animator>();
        player = FindAnyObjectByType<Mov>()?.transform;
        if (player != null)
            playerMovement = player.GetComponent<Mov>();

        // Animación de sprite oscuro
        int oscuroLayer = anim.GetLayerIndex("Oscuro");
        int normalLayer = anim.GetLayerIndex("Default");
        if (oscuroLayer >= 0) anim.SetLayerWeight(oscuroLayer, spriteOscuro ? 1f : 0f);
        if (normalLayer >= 0) anim.SetLayerWeight(normalLayer, spriteOscuro ? 0f : 1f);

        posicionInicial = transform.position;

        // AUDIO
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void Update()
    {
        // Determinar objetivo
        Vector2 objetivo;
        if (inVision)
        {
            objetivo = player.position;
        }
        else
        {
            objetivo = patrolEmty != null ? patrolEmty.position : transform.position;
        }

        // Evitar recalcular si ya está en el objetivo
        if (Vector2.Distance(AlignToGrid(transform.position), AlignToGrid(objetivo)) < 0.01f)
            return;

        // Lógica de luz: solo avanza un paso por cada paso del jugador
        if (inLight)
        {
            if (playerMovement != null && playerMovement.IsPlayerMoving() && !isMoving && !playerIsHiding && !pasoJugadorProcesado)
            {
                StartCoroutine(MoverEnemigo(objetivo));
                pasoJugadorProcesado = true;
            }
            if (playerMovement != null && !playerMovement.IsPlayerMoving())
            {
                pasoJugadorProcesado = false;
            }
        }
        else
        {
            // Movimiento libre fuera de la luz
            if (!isMoving)
                StartCoroutine(MoverEnemigo(objetivo));
        }

        UpdateAnimation();
    }

    IEnumerator MoverEnemigo(Vector2 targetPosition)
    {
        if (flowchart.GetBooleanVariable("Puede_moverse") == true)
        {
            if (isMoving) yield break;
            isMoving = true;

            Vector2 start = AlignToGrid(transform.position);
            Vector2 goal = AlignToGrid(targetPosition);

            List<Vector2> path = FindPathAStar(start, goal);
            if (path == null || path.Count == 0)
            {
                isMoving = false;
                yield break;
            }

            // En la luz, solo da un paso por movimiento del jugador
            if (inLight)
            {
                Vector2 step = path[0];
                if (!IsObstacle(step))
                {
                    ultimaDireccion = ((Vector2)step - (Vector2)transform.position).normalized;
                    UpdateAnimation();
                    yield return Move(step);
                }
            }
            else
            {
                foreach (var step in path)
                {
                    if (IsObstacle(step))
                    {
                        isMoving = false;
                        StartCoroutine(MoverEnemigo(targetPosition)); // Reintenta con nueva ruta
                        yield break;
                    }
                    ultimaDireccion = ((Vector2)step - (Vector2)transform.position).normalized;
                    UpdateAnimation();
                    yield return Move(step);
                }
            }

            ultimaDireccion = Vector2.zero;
            UpdateAnimation();
            isMoving = false;
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

    private Vector2 AlignToGrid(Vector2 pos)
    {
        return new Vector2(
            Mathf.Round((pos.x - gridOffset.x) / gridSize.x) * gridSize.x + gridOffset.x,
            Mathf.Round((pos.y - gridOffset.y) / gridSize.y) * gridSize.y + gridOffset.y
        );
    }

    IEnumerator Move(Vector2 targetPosition)
    {
        while (((Vector2)transform.position - targetPosition).sqrMagnitude > 0.01f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition;
    }

    bool IsObstacle(Vector2 targetPosition)
    {
        Vector2 aligned = AlignToGrid(targetPosition);
        if (colliderMovimiento == null)
            return Physics2D.OverlapCircle(aligned, 0.5f, obstacleLayer) != null;
        else
            return Physics2D.OverlapBox(
                aligned + (Vector2)colliderMovimiento.transform.localPosition + colliderMovimiento.offset,
                colliderMovimiento.size,
                0f,
                obstacleLayer
            ) != null;
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
                return null;

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

    // --- Lógica de visión y escondite ---
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!playerIsHiding)
            {
                inVision = true;
                // AUDIO: Solo reproducir si no se ha reproducido ya
                if (!haReproducidoAlerta && alertaClip != null)
                {
                    audioSource.PlayOneShot(alertaClip);
                    haReproducidoAlerta = true;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inVision = false;
            // AUDIO: Permitir reproducir de nuevo si vuelve a ver al jugador
            haReproducidoAlerta = false;
        }
    }

    public void SetPlayerHiding(bool isHiding)
    {
        playerIsHiding = isHiding;
        if (isHiding)
        {
            inVision = false;
            // AUDIO: Permitir reproducir de nuevo si el jugador se esconde
            haReproducidoAlerta = false;
        }
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
