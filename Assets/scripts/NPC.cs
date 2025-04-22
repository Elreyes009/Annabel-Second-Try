using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public string Name;


    private void Start()
    {
        if (Name == null)
        {
            Name = gameObject.name;
        }
    }

}