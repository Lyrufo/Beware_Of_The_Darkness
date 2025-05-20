using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;
public class CodeDoor : Doors
{
    [Tooltip("va  permettre de choisir la porte qu'on veut ouvrir avec ce code")]
    public Doors targetDoor;

    private bool _isCodeDoorOpen = false;

    private Collider2D _codeDoorCollider;

    void Update()
    {
        if (_playerInRange && Input.GetKeyDown(KeyCode.E)) //si player dans la zone et appuie sur e
        {
            _isCodeDoorOpen = true;
            Interaction();
        }

    }
    

    void Interaction()
    {
        if (_isCodeDoorOpen)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }

        void OpenDoor()
        {
            _animator.SetBool("isCodeDoorOpen", true);

            _codeDoorCollider.enabled = false;
        }

        void CloseDoor()
        {
            _animator.SetBool("isCodeDoorOpen", false);

            _codeDoorCollider.enabled = true;
        }


        

    }
}
