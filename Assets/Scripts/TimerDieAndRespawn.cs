using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TimerDieAndRespawn : MonoBehaviour
{

    public float timer;
    public float maxtime;

    public Transform playerTransform;

    public DeathHandler deathHandler;


    void Update()
    {

        timer += Time.deltaTime;

        if (timer > maxtime) //en gros qu'est ce qu'il se passe qd le temps est arrivé
        {
            deathHandler.HandleDeath(playerTransform); //declenche l'event de mort

            enabled = false; //desactive ce script

        }
    }
}
