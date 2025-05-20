using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;

public class KeyDoor : Doors
{
    private bool _isKeyDoorOpen = false;

    private Collider2D _keyDoorCollider;

    [SerializeField] InventoryManager.AllItems _requiredItem;



    void Update()
    {
        if (_playerInRange && Input.GetKeyDown(KeyCode.E)) //si player dans la zone et appuie sur e
        {
            if (HasRequiredItem(_requiredItem))
            {

                _isKeyDoorOpen = true;
                Interaction();
            }
            else
            {
                _isKeyDoorOpen = false;
                Interaction();
            }

        }

    }


    void Interaction()
    {
        if (_isKeyDoorOpen)
        {
            _animator.SetBool("hasKey", true);
            OpenKeyDoor();
        }
        else
        {
            _animator.SetBool("hasKey", false);
            ClosedDoor();
        }

        void OpenKeyDoor()
        {
            _animator.SetBool("isKeyDoorOpen", true);
            _keyDoorCollider.enabled = false;
        }

        void ClosedDoor()
        {
            _animator.SetBool("isKeyDoorOpen", false);  //voir pour jouer une seule fois 

            //_keyDoorCollider.enabled = true;         Laisser collider comme il est 
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
