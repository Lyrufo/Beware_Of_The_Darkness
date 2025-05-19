using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehavior : MonoBehaviour
{
    public bool _isDoorOpen = false; // met un booléen et dans l'état donc faux = fermé donc porte fermée à la base ; public pour intéragir avec les autres scripts
    public Animator _animator;
    public Collider2D _doorCollider;

    // Update is called once per frame
    void Update()
    {
        if (_isDoorOpen)
        {
            OpenDoor();
        }
        else 
        {
            CloseDoor();
        }

        void OpenDoor()
        {
            _animator.SetBool("isDoorOpen", true);

            DisableCollider();
        }

        void CloseDoor()
        {
            _animator.SetBool("isDoorOpen", false);

            EnableCollider();
        }


    }

    public void DisableCollider() => _doorCollider.enabled = false; // Désactive la collision
    public void EnableCollider() => _doorCollider.enabled = true; // Active la collision
}
