using UnityEngine;
using Fungus;
using Unity.VisualScripting;
using System;

public class Angelina_tmer : MonoBehaviour
{
    [SerializeField] Flowchart MainFlowchart; //El flowchart principal, para saber que estamos en la parte correcta del nivel
    [SerializeField] Flowchart AngelinaFlowchart; //El flowchart con los diálogos de Angelina

    float timer; //Un temporizador

    bool regulator; //Booleana reguladora. Sin esta, el bloque de código que invoquemos se reproducirá indefinidamente.

    [SerializeField] Mov mov;

    [SerializeField] Vector3 Respawn_point;

    private void Awake()
    {
        timer = 20f;
        regulator = true;
    }

    private void FixedUpdate()
    {
        Atrapar_Marco();

        Atrapar_Annabel();
    }

    void Atrapar_Marco() //Función que provoca que atrapen a Marco en la oficina de Angelina
    {
        if (regulator) //Revisamos que el regulador sea verdadero
        {
            if (MainFlowchart.GetBooleanVariable("Marco") == true) //Si estámos en la escena con el valor de Diálogo indicado
            {
                Debug.Log(timer);
                timer -= Time.deltaTime;

                if (timer <= 0) //Cuando el temporizador llega a 0
                {
                    AngelinaFlowchart.SetBooleanVariable("Atrapado", true); //Esta variable deve ser verdadera para reproducir el bloque de código
                    AngelinaFlowchart.ExecuteBlock("Atrapar_Marco");  //Se reproduce el bloque de código indicado
                    timer = 0f;

                    regulator = false; //Se desactiva el regulador para que el bloque no se reproduzca indefinidamente
                }
            }
        }

        if (!regulator && AngelinaFlowchart.GetBooleanVariable("Terminó") == true) //Esta variable permitirá que el sistema se reinicie
        {
            mov.ActualizarPuntoDeRespawn(Respawn_point);
            StartCoroutine(mov.RespawnCoroutine());
            timer = 20f;
            AngelinaFlowchart.SetBooleanVariable("Atrapado", false);
            AngelinaFlowchart.SetBooleanVariable("Terminó", false);
            regulator = true;
            Debug.Log(timer);
        }
    }

    void Atrapar_Annabel() //Función que provoca que atrapen a Annabel en la oficina de Angelina
    {

        if (regulator) //Revisamos que el regulador sea verdadero
        {
            if (MainFlowchart.GetBooleanVariable("Timer") == true && MainFlowchart.GetBooleanVariable("Cambio") == false) //Si estámos en la escena con el valor de Diálogo indicado
            {
                timer -= Time.deltaTime;
                Debug.Log(timer);

                if (timer <= 0 && AngelinaFlowchart.GetIntegerVariable("Reinicio") == 0) //Cuando el temporizador llega a 0 y no haya habido reinicios
                {
                    AngelinaFlowchart.SetBooleanVariable("Atrapadas", true); //Esta variable deve ser verdadera para reproducir el bloque de código
                    AngelinaFlowchart.ExecuteBlock("Atrapadas_1");  //Se reproduce el bloque de código indicado
                    timer = 0f;

                    regulator = false; //Se desactiva el regulador para que el bloque no se reproduzca indefinidamente
                }
                else if (timer <= 0 && AngelinaFlowchart.GetIntegerVariable("Reinicio") == 1) //Cuando el temporizador llega a 0 y no haya habido reinicios
                {
                    AngelinaFlowchart.SetBooleanVariable("Atrapadas", true); //Esta variable deve ser verdadera para reproducir el bloque de código
                    AngelinaFlowchart.ExecuteBlock("Atrapadas_2");  //Se reproduce el bloque de código indicado
                    timer = 0f;

                    regulator = false; //Se desactiva el regulador para que el bloque no se reproduzca indefinidamente
                }
                else if (timer <= 0 && AngelinaFlowchart.GetIntegerVariable("Reinicio") == 2) //Cuando el temporizador llega a 0 y no haya habido reinicios
                {
                    AngelinaFlowchart.SetBooleanVariable("Atrapadas", true); //Esta variable deve ser verdadera para reproducir el bloque de código
                    AngelinaFlowchart.ExecuteBlock("Atrapadas_3");  //Se reproduce el bloque de código indicado
                    timer = 0f;

                    regulator = false; //Se desactiva el regulador para que el bloque no se reproduzca indefinidamente
                }
                else if (timer <= 0 && AngelinaFlowchart.GetIntegerVariable("Reinicio") == 3) //Cuando el temporizador llega a 0 y no haya habido reinicios
                {
                    AngelinaFlowchart.SetBooleanVariable("Atrapadas", true); //Esta variable deve ser verdadera para reproducir el bloque de código
                    AngelinaFlowchart.ExecuteBlock("Atrapadas_4");  //Se reproduce el bloque de código indicado
                    timer = 0f;

                    regulator = false; //Se desactiva el regulador para que el bloque no se reproduzca indefinidamente
                }
            }
        }

        if (!regulator && AngelinaFlowchart.GetBooleanVariable("Terminó") == true) //Esta variable permitirá que el sistema se reinicie
        {
            mov.ActualizarPuntoDeRespawn(Respawn_point);
            StartCoroutine(mov.RespawnCoroutine());
            timer = 20f;
            AngelinaFlowchart.SetBooleanVariable("Atrapado", false);
            AngelinaFlowchart.SetBooleanVariable("Terminó", false);
            regulator = true;
            Debug.Log(timer);
        }
    }
}
