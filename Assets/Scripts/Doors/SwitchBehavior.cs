using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchBehavior : MonoBehaviour
{
    [SerializeField] Doors _doorBehavior;   //pour drag and drop la porte avec laquelle le script doit intéragir : en gros pour chaque switch on peut assiegner une porte différente également qui fait ref au script porte individuellement et aussi pouvoir y faire ref
    [SerializeField] bool _isDoorOpenSwitch; //pour savoir si c'est un switch pour ouvrir, fermer ou les deux la porte
    [SerializeField] bool _isDoorCloseSwitch;

    
    bool _isPressingSwitch = false; //si le bouton est déja pressed ou pas donc faux au départ en théorie ------------- A VOIR AVEC LE RESPAWN !!!!!!!!!  ------------

    [SerializeField] InventoryManager.AllItems _requiredItem;
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player")) //si c'est qqc avec le tag player qui intéragit avec 
        {

            _isPressingSwitch = !_isPressingSwitch; //en gros au lieu de mettre false ou true on inverse son état et ça c'est cool donc premier check et chagenement : on inverse l'état du switch

            if(HasRequiredItem(_requiredItem)) //donc si porte ouverte
            {
                if (_isDoorOpenSwitch && !_doorBehavior._isDoorOpen) //si c'est un bouton, qui permet d'ouvrir et si la door est pas deja open 
                {
                    _doorBehavior._isDoorOpen = !_doorBehavior._isDoorOpen; // paf on inverse (donc ici on ouvre)
                }
                else if (_isDoorCloseSwitch && _doorBehavior._isDoorOpen)  //si c'est un bouton, qui permet de fermer et si la door est open
                {
                    _doorBehavior._isDoorOpen = !_doorBehavior._isDoorOpen; // paf on ferme
                }

            }
            
        }
    }

    public bool HasRequiredItem(InventoryManager.AllItems itemRequired)
    {
        if (InventoryManager.Instance._inventoryItems.Contains(itemRequired)) 
        {
            return true ;
        }
        else
        { 
            return false ; 
        }
    }


}
