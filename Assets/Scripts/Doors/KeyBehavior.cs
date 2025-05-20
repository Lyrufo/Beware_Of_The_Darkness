using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Represents an activable lever in the scene.
/// </summary>
public class KeyBehavior : MonoBehaviour
{
    [SerializeField] InventoryManager.AllItems _itemType;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) //si la collision est avec ce qui a le tag player (donc le player logique bebou
        {
            InventoryManager.Instance.AddItem(_itemType); //on ajoute la clef à l'inv
            Destroy(gameObject); // detruit l'objet auqel le script est attaché donc keys
        }
    }
}

