using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;

public class Ending : MonoBehaviour
{

    public GameObject EndPanel;
    public TextMeshProUGUI ending;

    void OnTriggerEnter2D(Collider2D collision) //qd qqc entre en collision
    {
        if (collision.CompareTag("Player")) //et a le tag player
        {
            Debug.Log("caca");
            if (EndPanel != null) //ofc que dans le cas où j'ai rempli l'interactionpopup
                EndPanel.SetActive(true); //afficher appuyer sur E (interactive popup)
        }
    }
}

    