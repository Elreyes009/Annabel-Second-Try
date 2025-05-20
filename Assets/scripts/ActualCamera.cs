using UnityEngine;

public class ActualCamera : MonoBehaviour
{
    public GameObject cameraObject;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Mov mov = collision.GetComponent<Mov>();
        if (mov != null)
        {
            Debug.Log("Algo debería estar pasando");
            cameraObject.SetActive(true);
            mov.aCamara = cameraObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Mov mov = collision.GetComponent<Mov>();
        if (mov != null)
        {
            cameraObject.SetActive(false);
        }
    }
}