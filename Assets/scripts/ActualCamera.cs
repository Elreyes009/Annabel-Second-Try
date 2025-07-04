using System.Collections.Generic;
using UnityEngine;

public class ActualCamera : MonoBehaviour
{
    [SerializeField] GameObject cameraObject;
    [SerializeField] List<GameObject> salasLejanas;
    [SerializeField] List<GameObject> salasCercanas;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
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
        if (collision.CompareTag("Player"))
        {
            cameraObject.SetActive(false);

        }
    }
}
