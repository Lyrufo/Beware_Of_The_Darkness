using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{


    public Animator _animator;

    [Tooltip("le truc qui affiche appuyer sur E")]
    public GameObject interactionPopUp;

    public bool _playerInRange = false; //par défaut joueur pas dans la portée pour détecter 

    protected bool _isOpen = false;

    [Header("Colliders")]
    [Tooltip("Collider de déclenchement (Doit être un trigger)")]
    public Collider2D triggerCollider; // Pour détection joueur

    [Tooltip("Collider physique (ne pas cocher Is Trigger)")]
    public Collider2D physicalCollider; // Pour bloquer le passage


    protected virtual void Awake()
    { 
        _animator = GetComponent<Animator>();

        if (triggerCollider == null || physicalCollider == null) //si j'ai pas assigné oups
        {
            Collider2D[] colliders = GetComponents<Collider2D>();
            if (colliders.Length >= 2)
            {
                // Par défaut, le premier est le trigger, le second le physique
                triggerCollider = colliders[0];
                physicalCollider = colliders[1];
            }
        }
    }


    // AFFICHER E INTERACTION DONC COMMUN DOORS 

    void OnTriggerEnter2D(Collider2D collision) //qd qqc entre en collision
    {
        if (!_isOpen && collision.CompareTag("Player")) //et a le tag player
        {
            _playerInRange = true; //donc dans la zone de detection
            if (interactionPopUp != null) //ofc que dans le cas où j'ai rempli l'interactionpopup
                interactionPopUp.SetActive(true); //afficher appuyer sur E (interactive popup)
        }
    }

    void OnTriggerExit2D(Collider2D collision) //qd on quitte le trigger
    {
        if (collision.CompareTag("Player")) // et si ct le joueur ofc
        {
            _playerInRange = false; //donc bah plus joueur
            if (interactionPopUp != null)
                interactionPopUp.SetActive(false); //cache le appuie sur e
        }

    }

    public virtual void OpenDoor()
    {
        _isOpen = true;
        _animator.SetBool("isOpen", true);
        if (physicalCollider != null)
            physicalCollider.enabled = false; // Désactive seulement le collider physique
    }

    public virtual void CloseDoor()
    {
        _isOpen = false;
        _animator.SetBool("isOpen", false);
        if (physicalCollider != null)
            physicalCollider.enabled = true;
    }
}
