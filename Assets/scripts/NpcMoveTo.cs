using Fungus;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    [SerializeField] private Animator anim;
    private Vector2 ultimaDireccion = Vector2.zero;
    private bool isMoving = false;
    private bool hasArrived = false;
    bool compañero;

    private void Awake()
    {
        npcName = gameObject.GetComponent<NPC>().name;
        if (moveTo == null && !siguiendo)
        {
            moveTo = GameObject.FindWithTag(npcName + "Points");
        }

        if (npcName == "Serena" || npcName == "Angelina") //A menos que se trate de Serena, el PNJ no puede seguir al PJ
        {
            compañero = true;
        }
        else
        {
            compañero = false;
        }

        // Alinear posición inicial al centro del tile
        Vector2 aligned = AlignToGrid(transform.position);
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
        if (flowchart.GetBooleanVariable("Puede_moverse") && moveTo != null && !siguiendo)
        {
            Moove();
        }

        if (flowchart.GetBooleanVariable("Seguir") == true && compañero)
        {
            siguiendo = true;

            if (siguiendo)
            {
                if (player == null) return;
                if (Vector2.Distance(transform.position, player.transform.position) < 2f)
                    return;
                if (!isMoving)
                {
                    StartCoroutine(MoverEnemigo(player.transform.position));
                }
                return;
            }
        }


        UpdateAnimation();

        int oscuroLayer = anim.GetLayerIndex("Oscuro");
        int normalLayer = anim.GetLayerIndex("Default");
        if (oscuroLayer >= 0) anim.SetLayerWeight(oscuroLayer, spriteOscuro ? 1f : 0f);
        if (normalLayer >= 0) anim.SetLayerWeight(normalLayer, spriteOscuro ? 0f : 1f);
    }

    void Moove()
    {
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
            yield break;
        }

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

        ultimaDireccion = Vector2.zero;
        UpdateAnimation();
        isMoving = false;
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

                // Evitar nodos duplicados en open con peor g
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

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.yellow;
    //    for (float x = -10; x < 10; x += gridSize.x)
    //    {
    //        for (float y = -10; y < 10; y += gridSize.y)
    //        {
    //            Vector2 pos = AlignToGrid(new Vector2(x, y));
    //            if (IsObstacle(pos))
    //            {
    //                Gizmos.DrawCube(pos, gridSize * 0.9f);
    //            }
    //        }
    //    }
    //}
}