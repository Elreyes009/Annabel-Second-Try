////using UnityEngine;
////using System.Collections.Generic;

////public class RandomBoxPosition : MonoBehaviour
////{
////    public Collider2D zonaMovimiento; // Asigna en el Inspector
////    [SerializeField] private LayerMask obstacleLayer; // Asigna en el Inspector
////    [SerializeField] private float cooldownTeletransporte = 0.2f; // Cooldown entre teleports
////    [SerializeField] private float maxDistanciaTeletransporte = 4f; // Distancia máxima para el teletransporte
////    [SerializeField] private float tiempoMaxEsperaEnemigo = 7f; // Tiempo máximo de espera para el enemigo
////    [Header("Grid")]
////    [SerializeField] private Vector2 gridSize = new Vector2(1f, 1f);
////    [SerializeField] private Vector2 gridOffset = Vector2.zero; // Offset manual para centrar la tile

////    private bool enemigoDentro = false;
////    private float tiempoUltimoTeletransporte = 0f;
////    private float tiempoDesdeUltimoTP = 0f;
////    private Vector2 ultimaPosicionTP;

////    private void Start()
////    {
////        Alinear la caja al grid al iniciar
////       Vector2 alineada = AlignToGrid(transform.position);
////        transform.position = alineada;
////        ultimaPosicionTP = alineada;
////        tiempoDesdeUltimoTP = Time.time;
////    }

////    private void OnTriggerEnter2D(Collider2D collision)
////    {
////        if (collision.CompareTag("Enemigo"))
////        {
////            enemigoDentro = true;
////            Teletransporte inmediato si el enemigo llega
////            Teletransportar();
////            tiempoUltimoTeletransporte = Time.time;
////        }
////    }

////    private void OnTriggerExit2D(Collider2D collision)
////    {
////        if (collision.CompareTag("Enemigo"))
////        {
////            enemigoDentro = false;
////        }
////    }

////    private void Update()
////    {
////        Teletransporte si el jugador PRESIONA una tecla de movimiento y el enemigo está dentro
////        if (enemigoDentro &&
////            (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
////             Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) ||
////             Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) ||
////             Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)) &&
////            (Time.time - tiempoUltimoTeletransporte > cooldownTeletransporte))
////        {
////            Teletransportar();
////            tiempoUltimoTeletransporte = Time.time;
////        }
////    }

////    Método público para que el enemigo notifique que no puede llegar
////    public void NotificarInaccesible()
////    {
////        Teletransportar();
////    }

////    private void Teletransportar()
////    {
////        Vector2 nuevaPosicion = ObtenerPosicionValidaCercana();
////        Si no se encontró una posición válida, mueve al centro de la zona
////        if (nuevaPosicion == (Vector2)transform.position)
////        {
////            Debug.LogWarning("No se encontró una posición válida, moviendo al centro de la zona.");
////            nuevaPosicion = zonaMovimiento.bounds.center;
////        }
////        Alinear siempre al grid
////        nuevaPosicion = AlignToGrid(nuevaPosicion);
////        transform.position = nuevaPosicion;
////        ultimaPosicionTP = nuevaPosicion;
////        tiempoDesdeUltimoTP = Time.time;
////    }

////    private Vector2 ObtenerPosicionValidaCercana()
////    {
////        Bounds boundsMovimiento = zonaMovimiento.bounds;
////        Vector2 posicionActual = transform.position;
////        Vector2 posicionAleatoria = posicionActual; // Por defecto, posición actual
////        int intentos = 0;
////        bool encontrada = false;
////        do
////        {
////            float angulo = Random.Range(0f, 2f * Mathf.PI);
////            float distancia = Random.Range(gridSize.x, maxDistanciaTeletransporte);
////            float x = posicionActual.x + Mathf.Cos(angulo) * distancia;
////            float y = posicionActual.y + Mathf.Sin(angulo) * distancia;

////            Limitar dentro de la zona de movimiento
////           x = Mathf.Clamp(x, boundsMovimiento.min.x, boundsMovimiento.max.x);
////            y = Mathf.Clamp(y, boundsMovimiento.min.y, boundsMovimiento.max.y);

////            Alinear al grid con offset
////           Vector2 alineada = AlignToGrid(new Vector2(x, y));
////            posicionAleatoria = alineada;

////            if (!HayObstaculo(posicionAleatoria))
////            {
////                encontrada = true;
////                break;
////            }

////            intentos++;
////        }
////        while (intentos < 200);

////        if (!encontrada)
////        {
////            Debug.LogWarning("No se encontró una posición válida para el punto de patrulla. Considera revisar la zona de movimiento.");
////        }

////        return posicionAleatoria;
////    }

////    private Vector2 AlignToGrid(Vector2 pos)
////    {
////        return new Vector2(
////            Mathf.Round((pos.x - gridOffset.x) / gridSize.x) * gridSize.x + gridOffset.x,
////            Mathf.Round((pos.y - gridOffset.y) / gridSize.y) * gridSize.y + gridOffset.y
////        );
////    }

////    private bool HayObstaculo(Vector2 posicion)
////    {
////        float radio = 0.3f;
////        return Physics2D.OverlapCircle(posicion, radio, obstacleLayer) != null;
////    }
////}