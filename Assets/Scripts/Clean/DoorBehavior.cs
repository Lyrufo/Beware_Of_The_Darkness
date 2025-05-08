using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehavior : MonoBehaviour
{
    public bool _isDoorOpen = false; // met un bool�en et dans l'�tat donc faux = ferm� donc porte ferm�e � la base ; public pour int�ragir avec les autres scripts
    Vector3 _doorClosedPos; //coordonn�es en 3 axes (a du mal avec la 2d) de la porte ferm�e et ouvrte en dessous
    Vector3 _doorOpenPos;
    public float _doorSpeed = 10f; //rapidit� pour passer de l'�tat 1 � 2
    public float _doorHeight = 10f;

    void Awake()
    {
        _doorClosedPos = transform.position; // en gros chope la position actuelle de la porte au d�part du jeu et lui assigne "l'�tat" closed
        _doorOpenPos = new Vector3(transform.position.x,transform.position.y +_doorHeight,transform.position.z); //donc on garde les coordonn�es de base sauf le y
    }

    // Update is called once per frame
    void Update()
    {
        if (_isDoorOpen)
        {
            OpenDoor();
        }
        else if (!_isDoorOpen) //!_IsDoorOpen = inverse de l'�tat donc en gros close mais comme �a pas besoin de cr�er 2 �tats diff
        {
            CloseDoor();
        }

        void OpenDoor()
        {
            if (transform.position != _doorOpenPos) //apparement pour optimiser puisque ne va pas rappeler la fonction en boucle
            {
                transform.position = Vector3.MoveTowards(transform.position, _doorOpenPos, _doorSpeed*Time.deltaTime); // donc si la position est pas celle de la porte ouverte (donc ferm�e) on va la mettre � cette position � la vitesse d�finie plus haut
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
