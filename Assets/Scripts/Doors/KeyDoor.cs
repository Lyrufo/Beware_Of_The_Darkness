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
            if (HasRequiredItem(_requiredItem))
            {

                _isOpen = true;
                Interaction();
            }
            else
            {
                _isOpen = false;
                Interaction();
            }

        }

    }

    void Interaction()
{
        if (_isOpen)
        {
            _animator.SetBool("hasKey", true);
            OpenDoor(); // Utilise la méthode de base
        }
        else
        {
        _animator.SetBool("hasKey", false);
        _animator.SetBool("isOpen", false);
        }
}
    

    public bool HasRequiredItem(InventoryManager.AllItems itemRequired)
    {
        if (InventoryManager.Instance._inventoryItems.Contains(itemRequired))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
