using UnityEngine;
using Fungus;
using TMPro;
using System.Collections;

public class QuestPanelHandler : MonoBehaviour
{
    Animator anim;

    float temporizador;

    [SerializeField] Flowchart flowchart;

    [SerializeField] string[] misiones;
    int index;

    TMP_Text texto;

    private void Awake()
    {
        temporizador = 5f;

        anim = GetComponent<Animator>();
        texto = GetComponentInChildren<TMP_Text>();

        texto.text = misiones[0];
    }

    private void Update()
    {
        if (flowchart.GetBooleanVariable("Show") == true)
        {
            Show();
            temporizador -= Time.deltaTime;

            if (temporizador <= 0f)
            {
                Hide();
                temporizador = 5f;
                flowchart.SetBooleanVariable("Show", false);
            }
        }

        if (flowchart.GetBooleanVariable("Regulator") == true)
        {
            UpdateQuest();
        }

        if (Time.timeScale == 0f)
        {
            Show();
        }
        else if (Time.timeScale == 1f && flowchart.GetBooleanVariable("Show") == false)
        {
            Hide();
        }
    }

    void Show()
    {
        anim.SetBool("Show", true);
        anim.SetBool("Hide", false);
    }

    void Hide()
    {
        anim.SetBool("Show", false);
        anim.SetBool("Hide", true);
    }

    void UpdateQuest()
    {
        index++;
        texto.text = misiones[index];
        flowchart.SetBooleanVariable("Regulator", false);
    }
}
