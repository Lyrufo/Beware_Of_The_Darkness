using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using JetBrains.Annotations;


public class CodeDoor : Doors
{
    [Header("Password")]

    [Tooltip("le panel de password")]
    public GameObject EnterPasswordPanel;

    public int _currentPassword = 1234; //le mdp choisi 

    [Tooltip("le truc pour rentrer le code (la zone texte quoi)")]
    public TMP_InputField codeInput;

    [Tooltip("le message au dessus avec écrit password/wrong etc")]
    public TMP_Text messageText;

    private bool _isEnterPasswordPanelOpen = false; //si le panel est deja open mais là le met direct en false



    [Header("Camera et ciématique")]

    [Tooltip("la cam")]
    public CameraMovement cameraScript;

    [Tooltip("temps à attendre qd code bon avant de fermer auto")]
    [SerializeField] public float _delayBeforeClose = 1.5f;

    [Tooltip("le focus de la cam, donc la porte ou un empty sur la porte")]
    public Transform cameraFocusTarget;

    [Tooltip("temps où on regarde la porte fermée")]
    [SerializeField] public float _doorClosedViewTime = 0.3f;

    [Tooltip("temps à regarder l'ouverture")]
    [SerializeField] public float _doorOpenViewTime = 1f;

    [Tooltip("Le script du perso")]
    public PlayerCharacter2D playerCharacter; //utile pour le zoom sur le perso après 


    [Header("Le reste")]

    [Tooltip("va  permettre de choisir la porte qu'on veut ouvrir avec ce code")]

    public Doors targetDoor;
    [Tooltip("le nombre de fois que l'anim de porte qui s'ouvre pas joue")]
    public int repeatCount = 3;


    //ADD AU CODE DOOR UNIQUEMENT CAR SPECIFIQUE ET PAS DANS LES AUTRES
    void OnTriggerExit2D(Collider2D collision) //qd on quitte le trigger
    {
        if (collision.CompareTag("Player")) // et si ct le joueur ofc
        {
            _playerInRange = false; //donc bah plus joueur
            if (interactionPopUp != null)
                interactionPopUp.SetActive(false); //cache le appuie sur e
            if (EnterPasswordPanel != null)
                EnterPasswordPanel.SetActive(false); //cache le password panel
            _isEnterPasswordPanelOpen = false; //reset l'état
        }

    }


    void Update()
    {
        if (_playerInRange && Input.GetKeyDown(KeyCode.E)) //si player dans la zone et appuie sur e
        {
            if (_isEnterPasswordPanelOpen == false) //et que le truc pour entrer le password est pas ouvert
            {
                interactionPopUp.SetActive(false); //enleve le E 
                EnterPasswordPanel.SetActive(true); //affiche le panel de code etc
                _isEnterPasswordPanelOpen = true; //met à jour létat
                messageText.text = "Enter Password"; //reset message au dessus ofc
                codeInput.text = ""; // reset champ
                codeInput.ActivateInputField(); //hop pas besoin de cliquer dans le champ
                
            }
        }


        if (_isEnterPasswordPanelOpen && Input.GetKeyDown(KeyCode.Return)) //si le Password est ouvert et on appuie entrée
        {
            TryPassword(); //reviens à appuyer sur try 
            EventSystem.current.SetSelectedGameObject(codeInput.gameObject); //refocus sur le codeInput en gros
        }

        if (_isEnterPasswordPanelOpen && Input.GetKeyDown(KeyCode.Escape)) //si le password panel est ouvert et on appuie echap
        {
            EnterPasswordPanel.SetActive(false); //cache panel
            _isEnterPasswordPanelOpen = false; //met a jour l'état
            
        }
    }


    // ------------------SUPPRIMER LES 2 PREMIERES PARTIES MAIS JSP COMMENT GARDER LE PLAYER NUMBER 
    public void TryPassword() //hop la big boucle pour tester un mdp ; note, deja bloqué à 4 chiffres sur inspector fnumber

    {
        if (int.TryParse(codeInput.text, out int playerNumber))
        {
            if (playerNumber == _currentPassword)
            {
                messageText.text = "Code bon";
                StartCoroutine(CodeBon());
            }
            else
            {
                messageText.text = "Code faux";
                StartCoroutine(CodeFaux()); // <<--- On crée ça juste après
            }
        }
        else
        {
            // Pas de message, juste reset l'input
            ResetInputField();
        }

    }

    private void ResetInputField() //on reset le truc pour entrer le password
    {
        codeInput.text = ""; //on vide la case de la rentrée de code
        codeInput.ActivateInputField(); //et on reclique automatiquement dedans = pas besoin de reprednre la souris !
    }


    private IEnumerator CodeBon() //pour fermer le panel de password avec un coide bon mais en attendant qq secondes avant le temps de mettre un effet par ex
    {
        yield return new WaitForSeconds(_delayBeforeClose); //donc attends le delay mis en haut 
        EnterPasswordPanel.SetActive(false); //cache le password panel
        _isEnterPasswordPanelOpen = false; //reset l'état


        if (cameraScript != null && cameraFocusTarget != null) //donc ofc on vérifie que ça a été corretement assigné 
        {
            if (playerCharacter != null)
            {
                playerCharacter.GetComponent<Rigidbody2D>().velocity = Vector2.zero; //pf on met sa vélocity à 0
            }

            yield return cameraScript.StartCoroutine(cameraScript.DoorZoomIn(cameraFocusTarget)); //on lance le doorzoomIN de la cam (dans le script cam mvt)
            yield return new WaitForSeconds(_doorClosedViewTime); //on attend le temps défini en haut pour regarder la porte fermée 

            if (targetDoor != null)
            {
                targetDoor._animator.SetTrigger("CorrectCode");
                targetDoor.OpenDoor();
               
            }


            yield return new WaitForSeconds(_doorOpenViewTime); //on attend encore qq secondes sans bouger pour voir la porte s'ouvrir 
            yield return cameraScript.StartCoroutine(cameraScript.DoorZoomOut());

            cameraScript.isZooming = false; // et on remet le zoom à non 

            if (playerCharacter != null)
            {
                playerCharacter.SetCinematicMode(false);
            }
        }

        else if (targetDoor != null)
        {
            targetDoor.OpenDoor();
        }
    }

    public override void OpenDoor()
    {
        _isOpen = true;
        _animator.SetBool("isOpen", true);

        if (physicalCollider != null)
            physicalCollider.enabled = false; // Désactive seulement le collider physique
        if (interactionPopUp != null)
            interactionPopUp.SetActive(false); // cache définitivement l'interaction
    }

    private IEnumerator CodeFaux()
    {
        EnterPasswordPanel.SetActive(false);
        _isEnterPasswordPanelOpen = false;

        if (cameraScript != null && cameraFocusTarget != null)
        {
            if (playerCharacter != null)
            {
                playerCharacter.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }

            yield return cameraScript.StartCoroutine(cameraScript.DoorZoomIn(cameraFocusTarget));

            yield return new WaitForSeconds(_doorClosedViewTime);

            // Animation de porte "code faux"
            yield return StartCoroutine(PlayAnimationMultipleTimes("WrongCode", repeatCount));

            yield return new WaitForSeconds(_doorOpenViewTime);

            yield return cameraScript.StartCoroutine(cameraScript.DoorZoomOut());

            if (playerCharacter != null)
            {
                playerCharacter.SetCinematicMode(false);
            }

            EnterPasswordPanel.SetActive(true);
            _isEnterPasswordPanelOpen = true;
            messageText.text = "Enter Password";
            codeInput.text = "";
            codeInput.ActivateInputField();
        }
    }
    private IEnumerator PlayAnimationMultipleTimes(string triggerName, int repeatCount)
    {
        for (int i = 0; i < repeatCount; i++)
        {
            _animator.SetTrigger(triggerName);

            // Attendre la durée de l'animation actuelle
            yield return new WaitForSeconds(0.8f); //----------------------------SUIVANT LA DUREE DE l'anim 
        }
    }

}
