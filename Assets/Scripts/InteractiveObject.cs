using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;

public class InteractiveObject : MonoBehaviour
{
    //liste de trucs à assigner ds l'inspector
    [Tooltip("le scriptable object")]
    public InteractiveObjectsScriptable data;

    [Tooltip("le truc qui affiche appuyer sur E")]
    public GameObject interactionPopUp;

    [Tooltip("le 'support' de description de l'objet")]
    public GameObject descriptionPanel;

    [Tooltip("le nom de l'objet")]
    public TextMeshProUGUI objectNameText;

    [Tooltip("la descritption de l'objet")]
    public TextMeshProUGUI descriptionText;

    [Tooltip("image sprite de l'object")]
    public Image objectImage;

    private bool _playerInRange = false; //par défaut joueur pas dans la portée pour détecter 
    private bool _isDescriptionOpen = false; //si la description est deja open mais là le met direct en false


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
            if (descriptionPanel != null)
                descriptionPanel.SetActive(false); //cache description
        }

    }

void Update()
    {
        if (_playerInRange && Input.GetKeyDown(KeyCode.E)) //si player dans la zone et appuie sur e
        {
            if (!_isDescriptionOpen && data != null) //et la description est pas open et la data est pas vide (scriptable object complet
            {
                //rempli avec tout ça choppé dans le scriptable object
                objectNameText.text = data.interactiveObjectName;
                descriptionText.text = data.description;
                objectImage.sprite = data.minimodel;

                interactionPopUp.SetActive(false);
                descriptionPanel.SetActive(true); //affiche le panel de description
                _isDescriptionOpen = true; //met à jour létat
                PauseGame(); 
            }
        }
        if (_isDescriptionOpen && Input.GetKeyDown(KeyCode.Escape)) //si la description est ouverte et on appuie echap
        {
            descriptionPanel.SetActive(false); //cache panel
            _isDescriptionOpen = false ; //met a jour l'état
            ResumeGame();//reprend
        }
    }

    void PauseGame()
    {
        Time.timeScale = 0f;
    }

    void ResumeGame()
    {
        Time.timeScale = 1f;
    }
}
