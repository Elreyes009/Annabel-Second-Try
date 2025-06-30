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
    [Header("Collider del objetivo de patrulla")]
    public Collider2D patrolCollider;

    [Header("Audio")]
    public AudioClip alertaClip;
    public AudioClip muerteClip;
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

    private Vector2 ultimaPosicionObjetivo;
    private Vector2 ultimoIntentoObjetivo = Vector2.positiveInfinity;
    private Vector2 ultimaRawPosicionObjetivo = Vector2.positiveInfinity;

    private float tiempoUltimoPasoLuz = 0f;
    private Coroutine movimientoActual;

    private float tiempoUltimoIntento = 0f;
    private float intervaloReintento = 1.0f;

    private void Awake()
    {
        Vector2 aligned = AlignToGrid(transform.position);
        transform.position = aligned;

        anim = GetComponent<Animator>();
        player = FindAnyObjectByType<Mov>()?.transform;
        if (player != null)
            playerMovement = player.GetComponent<Mov>();

        int oscuroLayer = anim.GetLayerIndex("Oscuro");
        int normalLayer = anim.GetLayerIndex("Default");
        if (oscuroLayer >= 0) anim.SetLayerWeight(oscuroLayer, spriteOscuro ? 1f : 0f);
        if (normalLayer >= 0) anim.SetLayerWeight(normalLayer, spriteOscuro ? 0f : 1f);

        posicionInicial = transform.position;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void Update()
    {
        Vector2 objetivo;
        Collider2D objetivoCollider = null;

        if (inVision && player != null)
        {
            objetivo = player.position;
            objetivoCollider = player.GetComponent<Collider2D>();
        }
        else if (patrolEmty != null)
        {
            objetivo = patrolEmty.position;
            objetivoCollider = patrolCollider;
        }
        else
        {
            objetivo = transform.position;
        }

        Vector2 rawObjetivo = objetivo;

        if (objetivoCollider != null)
        {
            objetivo = GetNearestGridPointTouchingCollider(objetivoCollider);
        }

        bool objetivoCambio = (ultimaPosicionObjetivo != AlignToGrid(objetivo)) || (rawObjetivo != ultimaRawPosicionObjetivo);
        ultimaPosicionObjetivo = AlignToGrid(objetivo);
        ultimaRawPosicionObjetivo = rawObjetivo;

        bool debeMover = false;

        if (playerIsHiding)
        {
            debeMover = !inLight || (Time.time - tiempoUltimoPasoLuz >= 1f);
            if (inLight) tiempoUltimoPasoLuz = Time.time;
        }
        else
        {
            if (inLight)
            {
                if (playerMovement != null && playerMovement.IsPlayerMoving() && !pasoJugadorProcesado)
                {
                    debeMover = true;
                    pasoJugadorProcesado = true;
                }
                if (playerMovement != null && !playerMovement.IsPlayerMoving())
                {
                    pasoJugadorProcesado = false;
                }
            }
            else
            {
                debeMover = true;
            }
        }

        bool forzarReintento = !isMoving && (Time.time - tiempoUltimoIntento > intervaloReintento);
        Vector2 posObjetivo = AlignToGrid(objetivo);

        if ((debeMover && (!isMoving || objetivoCambio)) || forzarReintento)
        {
            if (posObjetivo != ultimoIntentoObjetivo || forzarReintento)
            {
                if (movimientoActual != null)
                    StopCoroutine(movimientoActual);
                movimientoActual = StartCoroutine(MoverEnemigo(objetivoCollider));
                ultimoIntentoObjetivo = posObjetivo;
                tiempoUltimoIntento = Time.time;
            }
        }

        UpdateAnimation();
    }

    private Vector2 GetNearestGridPointTouchingCollider(Collider2D objetivoCollider)
    {
        Vector2 mejorPunto = AlignToGrid(objetivoCollider.bounds.center);
        float mejorDist = float.MaxValue;

        Bounds b = objetivoCollider.bounds;
        for (float x = b.min.x; x <= b.max.x; x += gridSize.x)
        {
            for (float y = b.min.y; y <= b.max.y; y += gridSize.y)
            {
                Vector2 punto = AlignToGrid(new Vector2(x, y));
                if (objetivoCollider.OverlapPoint(punto))
                {
                    float dist = Vector2.Distance(transform.position, punto);
                    if (dist < mejorDist)
                    {
                        mejorDist = dist;
                        mejorPunto = punto;
                    }
                }
            }
        }
        return mejorPunto;
    }

    IEnumerator MoverEnemigo(Collider2D objetivoCollider)
    {
        if (!flowchart.GetBooleanVariable("Puede_moverse") || isMoving) yield break;

        isMoving = true;
        Vector2 start = AlignToGrid(transform.position);
        Vector2 goal = GetNearestGridPointTouchingCollider(objetivoCollider);

        List<Vector2> path = FindPathAStar(start, goal);

        // Si ya está tocando el collider, no hace falta moverse
        if (colliderMovimiento != null && objetivoCollider != null && colliderMovimiento.IsTouching(objetivoCollider))
        {
            isMoving = false;
            yield break;
        }

        // Si no hay path directo, buscar el punto más cercano posible al objetivo
        if (path == null || path.Count == 0)
        {
            // Buscar el punto más cercano al objetivo al que sí se puede llegar
            Vector2 mejorPunto = start;
            float mejorDist = float.MaxValue;
            for (float x = -5; x <= 5; x += gridSize.x)
            {
                for (float y = -5; y <= 5; y += gridSize.y)
                {
                    Vector2 posible = AlignToGrid(goal + new Vector2(x, y));
                    if (!IsObstacle(posible))
                    {
                        float dist = Vector2.Distance(posible, goal);
                        if (dist < mejorDist)
                        {
                            mejorDist = dist;
                            mejorPunto = posible;
                        }
                    }
                }
            }

            // Si el mejor punto es diferente al start, intenta moverse hasta ahí
            if (mejorPunto != start)
            {
                List<Vector2> pathCercano = FindPathAStar(start, mejorPunto);
                if (pathCercano != null && pathCercano.Count > 0)
                {
                    foreach (var step in pathCercano)
                    {
                        ultimaDireccion = (step - (Vector2)transform.position).normalized;
                        UpdateAnimation();
                        yield return Move(step);
                    }
                }
                // Reintentar moverse al objetivo real después de acercarse
                isMoving = false;
                movimientoActual = StartCoroutine(MoverEnemigo(objetivoCollider));
                yield break;
            }

            // Notificar que no puede llegar más cerca
            Debug.LogWarning("[Enemigo] Objetivo inaccesible, llegué lo más cerca posible.");
            isMoving = false;
            if (objetivoCollider != null)
            {
                var box = objetivoCollider.GetComponent<RandomBoxPosition>();
                if (box != null)
                    box.NotificarInaccesible();
            }
            yield break;
        }

        // Movimiento normal por el path
        if (inLight)
        {
            Vector2 step = path[0];
            if (!IsObstacle(step))
            {
                ultimaDireccion = (step - (Vector2)transform.position).normalized;
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
                    // Reintentar siempre que se bloquee
                    movimientoActual = StartCoroutine(MoverEnemigo(objetivoCollider));
                    yield break;
                }
                ultimaDireccion = (step - (Vector2)transform.position).normalized;
                UpdateAnimation();
                yield return Move(step);

                // Si después de moverse ya está tocando el collider, termina
                if (colliderMovimiento != null && objetivoCollider != null && colliderMovimiento.IsTouching(objetivoCollider))
                {
                    break;
                }
            }
        }

        ultimaDireccion = Vector2.zero;
        UpdateAnimation();
        isMoving = false;
    }

    private void UpdateAnimation()
    {
        if (anim == null) return;

        anim.SetBool("Derecha", isMoving && ultimaDireccion.x > 0);
        anim.SetBool("Izquierda", isMoving && ultimaDireccion.x < 0);
        anim.SetBool("Arriba", isMoving && ultimaDireccion.y > 0);
        anim.SetBool("Abajo", isMoving && ultimaDireccion.y < 0);
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

        // --- NUEVO: Alinear si está desfasado ---
        Vector2 aligned = AlignToGrid(transform.position);
        if ((Vector2)transform.position != aligned)
        {
            transform.position = aligned;
        }
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !playerIsHiding)
        {
            inVision = true;
            if (!haReproducidoAlerta && alertaClip != null)
            {
                audioSource.PlayOneShot(alertaClip);
                haReproducidoAlerta = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inVision = false;
            haReproducidoAlerta = false;
        }
    }

    public void SetPlayerHiding(bool isHiding)
    {
        playerIsHiding = isHiding;
        if (isHiding)
        {
            inVision = false;
            haReproducidoAlerta = false;
        }
    }

    public IEnumerator MatarJugador()
    {
        Debug.Log("[Enemigo] MatarJugador()");
        Mov respawnScript = player.GetComponent<Mov>();
        StartCoroutine(respawnScript.RespawnCoroutine());

        audioSource.PlayOneShot(muerteClip);
        if (playerMovement != null)
            playerMovement.enabled = false;
        else
            Debug.LogError("[Enemigo] Movimiento del jugador no encontrado.");

        yield return new WaitForSeconds(2f);
        enemManager.VolverAposicionInicial();

        flowchart.SetBooleanVariable("Muerte", true);
        int reinicio = flowchart.GetIntegerVariable("Reinicio");
        flowchart.ExecuteBlock($"Reinicio_{reinicio + 1}");
    }
}
