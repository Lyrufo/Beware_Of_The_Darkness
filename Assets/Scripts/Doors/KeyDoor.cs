using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;

public class KeyDoor : Doors
{
    [SerializeField] InventoryManager.AllItems _requiredItem;



    void Update()
    {
        if (_playerInRange && Input.GetKeyDown(KeyCode.E)) //si player dans la zone et appuie sur e
        {
            TryOpenDoor();
        }

    }

    public void TryOpenDoor()
    {
        bool hasKey = HasRequiredItem(_requiredItem);

        if (hasKey)
        {
            _isOpen = true;
            _animator.SetTrigger("OpenDoor"); // Déclenche l'animation d'ouverture
            OpenDoor(); // Gère la logique d'ouverture
        }
        else
        {
            _animator.SetTrigger("TryDoor"); // Déclenche l'animation d'échec
        }
    }

  
    

    public bool HasRequiredItem(InventoryManager.AllItems itemRequired)
    {
        return InventoryManager.Instance._inventoryItems.Contains(itemRequired);
    }
}
