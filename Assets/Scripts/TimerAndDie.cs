using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerAndDie : MonoBehaviour
{

    public float timer;
    public float maxtime;

    public GameObject Player;

    void Update()
    {
        //if (Input.anyKey)
       // {
            timer += Time.deltaTime;

            if (timer > maxtime) //en gros qu'est ce qu'il se passe qd le temps est arrivé
            {
                //joue anim
                //son (tin tin tinnnnnn)
                //animation de respawn et respawn  ? ----------Probablement anim se joue devant l'écran pdt que le player est déjà respawn et se lance que quand la première touche est appuyée
                Destroy(Player);
                Debug.Log("mort");

       //     }
        }
    }
}
