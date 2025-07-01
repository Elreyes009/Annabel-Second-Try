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
    public GameObject moveTo; // Punto actual de patrulla
    [SerializeField] private Flowchart flowchart;
    [SerializeField] private Animator anim;
    public Collider2D patrolCollider;
    [SerializeField] private BoxCollider2D colliderMovimiento;
    private GameObject player;

    [Header("Audio")]
    public AudioClip alertaClip;
    public AudioClip muerteClip;
    private AudioSource audioSource;

    private Vector2 ultimaDireccion = Vector2.zero;
    private Coroutine movimientoActual;

    private Vector2 ultimaPosicionObjetivo;
    private Vector2 ultimoIntentoObjetivo = Vector2.positiveInfinity;
    private Vector2 ultimaRawPosicionObjetivo = Vector2.positiveInfinity;

    private float tiempoUltimoIntento = 0f;
    private float intervaloReintento = 1.0f;
    private float tiempoUltimoPasoLuz = 0f;

    public bool isMoving = false;
    public bool inLight;
    public bool inVision;
    private bool pasoJugadorProcesado = false;
    private bool playerIsHiding = false;
    private bool haReproducidoAlerta = false;

    public Vector3 posicionInicial;
    public GestorDeEnemigos enemManager;

    private void Awake()
    {
        transform.position = AlignToGrid(transform.position);
        anim = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");

        posicionInicial = transform.position;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void Update()
    {
        UpdateAnimation();
        Vector2 objetivo;
        Collider2D objetivoCollider = null;

        // Si ve al jugador y las condiciones lo permiten, lo sigue
        if (inVision && player != null && !playerIsHiding)
        {
            objetivo = player.transform.position;
            objetivoCollider = player.GetComponent<Collider2D>();
        }
        else if (!inVision && moveTo != null)
        {
            objetivo = moveTo.transform.position;
            objetivoCollider = moveTo.GetComponent<Collider2D>();
        }
        else
        {
            objetivo = transform.position;
        }

        Vector2 rawObjetivo = objetivo;
        if (objetivoCollider != null)
            objetivo = GetNearestGridPointTouchingCollider(objetivoCollider);

        bool objetivoCambio = !SonIguales(ultimaPosicionObjetivo, AlignToGrid(objetivo)) || !SonIguales(rawObjetivo, ultimaRawPosicionObjetivo);
        ultimaPosicionObjetivo = AlignToGrid(objetivo);
        ultimaRawPosicionObjetivo = rawObjetivo;

        // Siempre debe moverse si tiene objetivo
        bool debeMover = true;

        // Ajusta la velocidad según la luz
        float velocidadActual = inLight ? moveSpeed * 0.5f : moveSpeed;

        bool forzarReintento = !isMoving && (Time.time - tiempoUltimoIntento > intervaloReintento);
        Vector2 posObjetivo = AlignToGrid(objetivo);

        if ((debeMover && (!isMoving || objetivoCambio)) || forzarReintento)
        {
            if (!SonIguales(posObjetivo, ultimoIntentoObjetivo) || forzarReintento)
            {
                if (movimientoActual != null)
                    StopCoroutine(movimientoActual);
                movimientoActual = StartCoroutine(MoverEnemigo(objetivo, velocidadActual));
                ultimoIntentoObjetivo = posObjetivo;
                tiempoUltimoIntento = Time.time;
            }
        }

        if (moveTo != null && moveTo.activeSelf && Vector2.Distance(transform.position, moveTo.transform.position) < 0.2f)
        {
            Next nextComponent = moveTo.GetComponent<Next>();
            if (nextComponent != null)
            {
                if (nextComponent.nextObject != null)
                    nextComponent.nextObject.SetActive(true);
                moveTo.SetActive(false);
            }
        }
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

    IEnumerator MoverEnemigo(Vector2 objetivo, float velocidad)
    {
        isMoving = true;
        Vector2 start = AlignToGrid(transform.position);
        Vector2 goal = AlignToGrid(objetivo);

        List<Vector2> path = FindPathAStar(start, goal);
        if (path == null || path.Count == 0)
        {
            isMoving = false;
            yield break;
        }

        foreach (var step in path)
        {
            if (IsObstacle(step))
            {
                isMoving = false;
                yield break;
            }
            ultimaDireccion = (step - (Vector2)transform.position).normalized;
            UpdateAnimation();
            yield return Move(step, velocidad);
        }

        ultimaDireccion = Vector2.zero;
        UpdateAnimation();
        isMoving = false;
    }

    private IEnumerator Move(Vector2 targetPosition, float velocidad)
    {
        while (((Vector2)transform.position - targetPosition).sqrMagnitude > 0.01f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, velocidad * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition;
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
        float x = Mathf.Round((pos.x - gridOffset.x) / gridSize.x) * gridSize.x + gridOffset.x;
        float y = Mathf.Round((pos.y - gridOffset.y) / gridSize.y) * gridSize.y + gridOffset.y;
        x = Mathf.Round(x * 1000f) / 1000f;
        y = Mathf.Round(y * 1000f) / 1000f;
        return new Vector2(x, y);
    }

    private bool SonIguales(Vector2 a, Vector2 b, float tolerancia = 0.01f)
    {
        return (a - b).sqrMagnitude < tolerancia * tolerancia;
    }

    private bool IsObstacle(Vector2 targetPosition)
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

            if (SonIguales(actual.pos, goal))
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

                Nodo existente = open.Find(n => SonIguales(n.pos, vecino));
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

    // Métodos públicos para esconderse y matar jugador (igual que antes)
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
        Mov respawnScript = player.GetComponent<Mov>();
        StartCoroutine(respawnScript.RespawnCoroutine());
        audioSource.PlayOneShot(muerteClip);
        if (player.GetComponent<Mov>() != null)
            player.GetComponent<Mov>().enabled = false;
        yield return new WaitForSeconds(2f);
        enemManager.VolverAposicionInicial();
        flowchart.SetBooleanVariable("Muerte", true);
        int reinicio = flowchart.GetIntegerVariable("Reinicio");
        flowchart.ExecuteBlock($"Reinicio_{reinicio + 1}");
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
}
