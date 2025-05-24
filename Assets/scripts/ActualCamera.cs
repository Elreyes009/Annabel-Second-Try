using System.Collections.Generic;
using UnityEngine;

public class ActualCamera : MonoBehaviour
{
    [SerializeField] GameObject cameraObject;
    private RayCast ray;
    [SerializeField] List<GameObject> salasLejanas;
    [SerializeField] List<GameObject> salasCercanas;
    private void Start()
    {
        if (ray == null)
        {
            ray = FindFirstObjectByType<RayCast>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == ray.gameObject)
        {
            cameraObject.SetActive(true);
            foreach (GameObject obj in salasLejanas)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
            foreach (GameObject obj in salasCercanas)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == ray.gameObject)
        {
            cameraObject.SetActive(false);

        }
    }
}
