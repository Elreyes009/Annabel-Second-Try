using UnityEngine;

public class Puerta : MonoBehaviour
{
    [SerializeField] string Requerimiento;

    public string requerimiento { get { return Requerimiento; } }

    private void Start()
    {
        if (Requerimiento == null)
        {
            Requerimiento = "null";
        }
    }

}
