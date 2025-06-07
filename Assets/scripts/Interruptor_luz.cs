using UnityEngine;

public class Interruptor_luz : MonoBehaviour
{
    public bool encendido;
    [SerializeField] GameObject luz;

    private void Awake()
    {
        encendido = false;
        luz = GetComponentInChildren<GameObject>();
        luz.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (encendido)
        {
            luz.SetActive(true);
        }
    }
}
