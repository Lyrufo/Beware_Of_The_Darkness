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

            if (timer > maxtime) //en gros qu'est ce qu'il se passe qd le temps est arriv�
            {
                //joue anim
                //son (tin tin tinnnnnn)
                //animation de respawn et respawn  ? ----------Probablement anim se joue devant l'�cran pdt que le player est d�j� respawn et se lance que quand la premi�re touche est appuy�e
                Destroy(Player);
                Debug.Log("mort");

       //     }
        }
    }
}
