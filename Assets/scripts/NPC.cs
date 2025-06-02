using UnityEngine;

public class NPC : MonoBehaviour
{
    public string Name;

    private void Start()
    {
        if (string.IsNullOrEmpty(Name))
        {
            Name = gameObject.name;
        }
    }
}
