using UnityEngine;

public class Interruptor_luz : MonoBehaviour
{
    public bool encendido;
    [SerializeField] GameObject luz;

    private void Awake()
    {
        encendido = false;
        luz.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (encendido)
        {
            luz.SetActive(true);
        }
        else
        {
            luz.SetActive(false);
        }
    }
}
