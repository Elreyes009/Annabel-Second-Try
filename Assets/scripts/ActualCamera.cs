using UnityEngine;

public class ActualCamera : MonoBehaviour
{
    [SerializeField] GameObject cameraObject;
    private RayCast ray;

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
