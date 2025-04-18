using UnityEngine;

public class Recogible : MonoBehaviour
{
    [SerializeField] string nombre;
    public string itemName { get { return nombre; } }
}
