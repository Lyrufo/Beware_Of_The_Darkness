using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchBehavior : MonoBehaviour
{
    [SerializeField] DoorBehavior _doorBehavior;   //pour drag and drop la porte avec laquelle le script doit int�ragir : en gros pour chaque switch on peut assiegner une porte diff�rente �galement qui fait ref au script porte individuellement et aussi pouvoir y faire ref
    [SerializeField] bool _isDoorOpenSwitch; //pour savoir si c'est un switch pour ouvrir, fermer ou les deux la porte
    [SerializeField] bool _isDoorCloseSwitch;

    float _switchSizeY; //pour "baisser le bouton" qd on appuie dessus 
    Vector3 _switchUpPos;
    Vector3 _switchDownPos;
    float _switchSpeed = 1f; //pareil que pour la prte du coup, vitesse
    float _switchDelay = 0.5f; //temps avant que le switch reprenne la pos initiale
    bool _isPressingSwitch = false; //si le bouton est d�ja pressed ou pas donc faux au d�part en th�orie ------------- A VOIR AVEC LE RESPAWN !!!!!!!!!  ------------

    [SerializeField] InventoryManager.AllItems _requiredItem;

    // Start is called before the first frame update
    void Awake()
    {
        _switchSizeY = transform.localScale.y/2; //calcule la moiti� du y et le colle � la variable

        _switchUpPos = transform.position; 
        _switchDownPos = new Vector3(transform.position.x, transform.position.y -_switchSizeY, transform.position.z); //change la position � l'aide la variable calcul�e
    }

    // Update is called once per frame
    void Update()
    {

        if (_isPressingSwitch) //donc changer l"�tat (d�gfini apr�s) selon le booleen cad s'il est pressed ou pas
        {
            MoveSwitchDown();
        }
        else if (!_isPressingSwitch) 
        {
            MoveSwitchUp();
        }
    }

    void MoveSwitchDown()
    {

        if (transform.position != _switchDownPos) //cf porte ppur comprendre �a
        {
            transform.position = Vector3.MoveTowards(transform.position, _switchDownPos, _switchSpeed * Time.deltaTime); 
        }
    }
    void MoveSwitchUp()
    {
        if (transform.position != _switchUpPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, _switchUpPos, _switchSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player")) //ig si c'est sur le m�me "calque" genre s'ils peuvent int�ragir si j'ai bien compris donc si collision avec les deux
        {
            _isPressingSwitch = !_isPressingSwitch; //en gros au lieu de mettre false ou true on inverse son �tat et �a c'est cool donc premier check et chagenement : on inverse l'�tat du switch

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

    private void OnTriggerExit2D(Collider2D collision) //cru comprendre que c'�tait pour �viter les bug quand le bouton remonte
    {

        if (collision.CompareTag("Player")) //si on est dessus ou quoi
        {
            StartCoroutine(SwitchUpDelay(_switchDelay)); // en gros va permettre de delay l'ex�cution de qqc (genre le bouton remonte) dans une situation (genre on est dessus) si j'ai bien compris
        }

    }

    IEnumerator SwitchUpDelay(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        _isPressingSwitch = false ; //apr�s le delay on joue �a

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
