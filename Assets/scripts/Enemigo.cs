using Fungus;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class Enemigo : MonoBehaviour
{
    [Header("Movimiento y Grid")]
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Vector2 gridSize = new Vector2(1f, 1f);
    [Tooltip("Offset del grid en X e Y. Dejar en X(-0.4) Y(0.6). Si el tilemap se desplaza o cambia de tamaño, llamar al Diego para que lo vuelva a calcular.")]
    public Vector2 gridOffset = Vector2.zero;

    [Header("Referencias")]
    public GameObject moveTo;
    [SerializeField] public string npcName;
    [SerializeField] private GameObject player;
    [SerializeField] private Flowchart flowchart;
    public AudioClip muerteClip;
    [SerializeField] AudioSource audioSource;

    [SerializeField] private Animator anim;
    private Vector2 ultimaDireccion = Vector2.zero;
    private bool isMoving = false;
    private bool hasArrived = false;

    public bool inLight;

    public Transform posicionInicial; //Se cambió el Vector3 por un Transform devido a que el primero estaba funcionando mal
    public GestorDeEnemigos enemManager;

    public enum EstadoEnemigo { Patrullando, Persiguiendo }
    public EstadoEnemigo estadoActual = EstadoEnemigo.Patrullando;

    public void PerseguirJugador(GameObject objetivo)
    {
        moveTo = objetivo;
        estadoActual = EstadoEnemigo.Persiguiendo;
        /*StopAllCoroutines();*/
        isMoving = false;
    }

    public void VolverAPatrullar()
    {
        moveTo = GameObject.FindWithTag(npcName + "Points");
        estadoActual = EstadoEnemigo.Patrullando;
        StopAllCoroutines();
        isMoving = false;
    }

    private void Awake()
    {
        //if (posicionInicial == null)
        //{
        //    posicionInicial.position = transform.position;
        //}

        if (moveTo == null)
        {
            moveTo = GameObject.FindWithTag(npcName + "Points");
        }

        Vector2 aligned = AlignToGrid(transform.position);
        transform.position = aligned;

        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdateAnimation();
        if (flowchart.GetBooleanVariable("Puede_moverse"))
        {
            if (moveTo != null)
            {
                Moove();
            }
        }
    }

    void Moove()
    {
        if (moveTo == null || !moveTo.activeSelf)
        {
            if (estadoActual == EstadoEnemigo.Patrullando)
            {
                moveTo = GameObject.FindWithTag(npcName + "Points");
                if (moveTo == null) return;
            }
            else
            {
                return;
            }
        }

        if (estadoActual == EstadoEnemigo.Persiguiendo)
        {
            // Solo intenta moverse si no está ya en rango
            if (Vector2.Distance(transform.position, moveTo.transform.position) > -0.000000001f)
            {
                if (!isMoving)
                {
                    StartCoroutine(MoverEnemigo(moveTo.transform.position));
                }
            }

            return;
        }
        Next nextComponent = moveTo.GetComponent<Next>();
        if (nextComponent == null) return;
        if (!nextComponent.seguir) return;
        if (hasArrived) return;

        if (Vector2.Distance(transform.position, moveTo.transform.position) < 1f)
        {
            if (nextComponent.nextObject != null)
            {
                nextComponent.nextObject.SetActive(true);
            }
            moveTo.SetActive(false);
        }

        if (!isMoving)
        {
            StartCoroutine(MoverEnemigo(moveTo.transform.position));
        }
    }

    IEnumerator MoverEnemigo(Vector2 targetPosition)
    {
        if (isMoving) yield break;
        isMoving = true;

        Vector2 start = AlignToGrid(transform.position);
        Vector2 goal = AlignToGrid(targetPosition);

        List<Vector2> path = FindPathAStar(start, goal);
        if (path == null || path.Count == 0)
        {
            isMoving = false;
            if (estadoActual == EstadoEnemigo.Persiguiendo)
            {
                yield return new WaitForSeconds(0.3f);
                StartCoroutine(MoverEnemigo(moveTo.transform.position));
            }
            yield break;

        }

        foreach (var step in path)
        {
            if (IsObstacle(step))
            {
                Debug.Log($"Obstáculo encontrado en {step}, cancelando movimiento.");
                break; // no reintentar inmediatamente
            }

            ultimaDireccion = ((Vector2)step - (Vector2)transform.position).normalized;
            UpdateAnimation();
            yield return Move(step);
        }

        ultimaDireccion = Vector2.zero;
        UpdateAnimation();
        isMoving = false;
    }


    private void UpdateAnimation()
    {
        if (anim == null) return;

        if (isMoving)
        {
            if (ultimaDireccion.x > 0)
            {
                anim.SetBool("Derecha", true);
                anim.SetBool("Izquierda", false);
                anim.SetBool("Abajo", false);
                anim.SetBool("Arriba", false);
            }

            if (ultimaDireccion.x < 0)
            {
                anim.SetBool("Izquierda", true);
                anim.SetBool("Derecha", false);
                anim.SetBool("Arriba", false);
                anim.SetBool("Abajo", false);
            }

            if (ultimaDireccion.y > 0)
            {
                anim.SetBool("Arriba", true);
                anim.SetBool("Abajo", false);
                anim.SetBool("Izquierda", false);
                anim.SetBool("Derecha", false);
            }

            if (ultimaDireccion.y < 0)
            {
                anim.SetBool("Abajo", true);
                anim.SetBool("Arriba", false);
                anim.SetBool("Izquierda", false);
                anim.SetBool("Derecha", false);
            }

        }
        else if (!isMoving)
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

        if (inLight)
        {
            yield return new WaitForSeconds(0.6f);
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
        }
    }

    bool IsObstacle(Vector2 targetPosition)
    {
        Vector2 aligned = AlignToGrid(targetPosition);
        var box = GetComponent<BoxCollider2D>();
        if (box == null)
            return Physics2D.OverlapCircle(aligned, 0.5f, obstacleLayer) != null;
        else
            return Physics2D.OverlapBox(
                aligned + box.offset,
                box.size,
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
                // Reconstruir el camino
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

    public IEnumerator MatarJugador()
    {
        flowchart.SetBooleanVariable("Hablando", true);
        Mov respawnScript = player.GetComponent<Mov>();
        respawnScript.enabled = true;
        StartCoroutine(respawnScript.RespawnCoroutine());
        respawnScript.enabled = false;
        audioSource.PlayOneShot(muerteClip);
        yield return new WaitForSeconds(2f);
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
        //int reinicio = flowchart.GetIntegerVariable("Reinicio");
        //flowchart.ExecuteBlock($"Reinicio_{reinicio + 1}");
    }
}
