using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Inventario : MonoBehaviour
{
    public static Inventario instance;

    private Dictionary<string, int> items = new Dictionary<string, int>();
    public TextMeshProUGUI contadorUI;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        contadorUI.text = "Inventario:\n";
        foreach (var item in items)
        {
            contadorUI.text += $"{item.Key}: {item.Value}\n";
        }
    }

    public void AddItem(string itemName)
    {
        if (items.ContainsKey(itemName))
        {
            items[itemName]++;
        }
        else
        {
            items[itemName] = 1;
        }

        UpdateUI();
    }

    public bool TieneObjeto(string itemName)
    {
        return items.ContainsKey(itemName) && items[itemName] > 0; // 🔹 Verifica que el objeto existe y tiene al menos 1 cantidad
    }

    // Método para usar la llave y destruirla
    public void UsarLlave(string itemName)
    {
        if (items.ContainsKey(itemName) && items[itemName] > 0)
        {
            // Reduce la cantidad del item
            items[itemName]--;

            // Si la cantidad es 0, elimina el item del inventario
            if (items[itemName] == 0)
            {
                items.Remove(itemName);
            }

            UpdateUI(); // Actualiza la UI después de eliminar la llave
            Debug.Log(itemName + " usada y rota.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("recogible"))
        {
            Recogible recogibleComponent = other.GetComponent<Recogible>();
            if (recogibleComponent != null)
            {
                AddItem(recogibleComponent.itemName);
                Destroy(other.gameObject);
            }
        }
    }
}