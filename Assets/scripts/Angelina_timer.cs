using UnityEngine;
using Fungus;
using Unity.VisualScripting;

public class Angelina_tmer : MonoBehaviour
{
    [SerializeField] Flowchart MainFlowchart;
    [SerializeField] Flowchart AngelinaFlowchart;

    float timer;

    bool regulator;

    private void Awake()
    {
        timer = 20f;
        regulator = true;
    }

    private void FixedUpdate()
    {
        if (regulator)
        {
            if (MainFlowchart.GetIntegerVariable("Diálogo") == 13)
            {
                Debug.Log(timer);
                timer -= Time.deltaTime;

                if (timer <= 0)
                {
                    AngelinaFlowchart.SetBooleanVariable("Atrapado", true);
                    AngelinaFlowchart.ExecuteBlock("Atrapar_Marco");
                    timer = 0f;

                    regulator = false;
                }
            }
        }
    }
}
