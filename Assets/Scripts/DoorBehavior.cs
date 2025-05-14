using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehavior : MonoBehaviour
{
    public bool _isDoorOpen = false; // met un booléen et dans l'état donc faux = fermé donc porte fermée à la base ; public pour intéragir avec les autres scripts
    Vector3 _doorClosedPos; //coordonnées en 3 axes (a du mal avec la 2d) de la porte fermée et ouvrte en dessous
    Vector3 _doorOpenPos;
    public float _doorSpeed = 10f; //rapidité pour passer de l'état 1 à 2
    public float _doorHeight = 10f;

    void Awake()
    {
        _doorClosedPos = transform.position; // en gros chope la position actuelle de la porte au départ du jeu et lui assigne "l'état" closed
        _doorOpenPos = new Vector3(transform.position.x,transform.position.y +_doorHeight,transform.position.z); //donc on garde les coordonnées de base sauf le y
    }

    // Update is called once per frame
    void Update()
    {
        if (_isDoorOpen)
        {
            OpenDoor();
        }
        else if (!_isDoorOpen) //!_IsDoorOpen = inverse de l'état donc en gros close mais comme ça pas besoin de créer 2 états diff
        {
            CloseDoor();
        }

        void OpenDoor()
        {
            if (transform.position != _doorOpenPos) //apparement pour optimiser puisque ne va pas rappeler la fonction en boucle
            {
                transform.position = Vector3.MoveTowards(transform.position, _doorOpenPos, _doorSpeed*Time.deltaTime); // donc si la position est pas celle de la porte ouverte (donc fermée) on va la mettre à cette position à la vitesse définie plus haut
            }
        }

        void CloseDoor()
        {
            if (transform.position != _doorClosedPos)
            {
                transform.position = Vector3.MoveTowards(transform.position, _doorClosedPos, _doorSpeed * Time.deltaTime); 
            }
        }

    }
}
