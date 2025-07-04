using UnityEngine;
using Fungus;

public class Puerta : MonoBehaviour
{
    [SerializeField] string Requerimiento;

    [SerializeField] Flowchart flowchart;

    Animator anim;

    public string requerimiento { get { return Requerimiento; } }

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        if (Requerimiento == null)
        {
            Requerimiento = "null";
        }
    }

    private void FixedUpdate()
    {
        if (flowchart != null)
        {
            if (flowchart.GetBooleanVariable("Muerte") == true)
            {
                anim.SetBool("Puerta", false);
            }
        }
    }
}
