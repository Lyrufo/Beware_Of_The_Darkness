using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{


    public Animator _animator;

    [Tooltip("le truc qui affiche appuyer sur E")]
    public GameObject interactionPopUp;

    public bool _playerInRange = false; //par défaut joueur pas dans la portée pour détecter 





    // AFFICHER E INTERACTION DONC COMMUN DOORS 

    void OnTriggerEnter2D(Collider2D collision) //qd qqc entre en collision
    {
        if (collision.CompareTag("Player")) //et a le tag player
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

    

}
