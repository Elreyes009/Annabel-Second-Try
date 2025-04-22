using UnityEngine;

public class Recogible : MonoBehaviour
{
    [SerializeField] string nombre;
    public string itemName { get { return nombre; } }


    private void Start()
    {
        if (nombre == null)
        {
            Debug.LogError("Error, Objeto sin nombre en script, poner nombre urgentemente");
        }
    }

}
