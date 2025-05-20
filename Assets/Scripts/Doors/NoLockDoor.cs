using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
public class NoLockDoor : Doors
{
    private bool _isNoLockDoorOpen = false;

    private Collider2D _NoLockDoorCollider;

    void Update()
    {
        if (_playerInRange && Input.GetKeyDown(KeyCode.E)) //si player dans la zone et appuie sur e
        {
            _isNoLockDoorOpen = true;
            Interaction();
        }
       
    }

    //pour ce script c'est easy, les seules conditions pour ouvrir la porte c'est être à côté et appuyer sur E
    //Et ça c'est égal à :  Mettre Bool "open" à true, lancer l'anim et désactiver le collider

    void Interaction ()
    {
        if (_isNoLockDoorOpen)
        {
            _animator.SetBool("isNoLockDoorOpen", true);

            _NoLockDoorCollider.enabled = false;
        }

        //Donc simple pour les no lock doors : si on appuie sur e à proximité, ça lance l'anim et désactive le collider

    }

}
