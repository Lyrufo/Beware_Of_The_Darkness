using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimerAndDie : MonoBehaviour
{

    public float timer;
    public float maxtime;
    public UnityEvent OnDeath; //event de quand on meurt 
   

    void Update()
    {
       
        timer += Time.deltaTime;

        if (timer > maxtime) //en gros qu'est ce qu'il se passe qd le temps est arrivé
        {
           OnDeath.Invoke(); //declenche l'event'
            enabled = false; //desactive ce script
       
        }
    }
}
