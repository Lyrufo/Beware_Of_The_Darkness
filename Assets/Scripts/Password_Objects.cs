using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using JetBrains.Annotations;

public class Password_Objects : MonoBehaviour
{

    [Tooltip("le truc qui affiche appuyer sur E")]
    public GameObject interactionPopUp;

    [Tooltip("le panel de password")]
    public GameObject EnterPasswordPanel;

    private bool _playerInRange = false; //par défaut joueur pas dans la portée pour détecter 

    private bool _isEnterPasswordPanelOpen = false; //si le panel est deja open mais là le met direct en false

    public int _currentPassword = 1234; //le mdp choisi 

    [Tooltip("le truc pour rentrer le code (la zone texte quoi)")]
    public TMP_InputField codeInput;

    [Tooltip("le message au dessus avec écrit password/wrong etc")]
    public TMP_Text messageText;

    [Tooltip("temps à attendre qd code bon avant de fermer auto")]
    [SerializeField] public float _delayBeforeClose = 1.5f;

    [Tooltip("va  permettre de choisir la porte qu'on veut ouvrir avec ce code")]
    public DoorBehavior targetDoor;

    [Tooltip("la cam")]
    public CameraMovement cameraScript;

    [Tooltip("le focus de la cam, donc la porte ou un empty sur la porte")]
    public Transform cameraFocusTarget;

    [Tooltip("temps où on regarde la porte fermée")]
    [SerializeField] public float _doorClosedViewTime = 0.3f;

    [Tooltip("temps à regarder l'ouverture")]
    [SerializeField] public float _doorOpenViewTime = 1f;




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

    public void TryPassword() //hop la big boucle pour tester un mdp

    {
        if (string.IsNullOrWhiteSpace(codeInput.text)) // avec cette fonction on empeche le joueur de rien mettre ou mettre autre chose que des chiffres (en tout cas on lui signifie)
                                                       //déjà limité à 4 caractères dans l'inspector sur fnumber !! hehe
        {
            messageText.text = "Please enter a valid number.";
            ResetInputField(); //ici partout on va appeler cette fonction/boucle définie en bas, sauf pour le "code bon" tu vas capter
            return; //stop si champ libre
        }

        if (!int.TryParse(codeInput.text, out int playerNumber)) //ici vérifie bien chiffres
        {
            messageText.text = "Numbers only";
            ResetInputField();
            return;
        }

        if (playerNumber == _currentPassword) // dans le cas où c'est le même 
        {
            messageText.text = "Code bon";
            StartCoroutine(CodeBon());
           

        }
        if (playerNumber != _currentPassword) // dans le cas où on s'est planté 
        {
            messageText.text = "Code faux";
            ResetInputField();
        }

    }

    private IEnumerator CodeBon() //pour fermer le panel de password avec un coide bon mais en attendant qq secondes avant le temps de mettre un effet par ex
    {
        yield return new WaitForSeconds(_delayBeforeClose);
        EnterPasswordPanel.SetActive(false); //cache le password panel
        _isEnterPasswordPanelOpen = false; //reset l'état


        if (cameraScript != null && cameraFocusTarget != null)
        {
            yield return cameraScript.StartCoroutine(cameraScript.DoorZoom(cameraFocusTarget));
            yield return new WaitForSeconds(_doorClosedViewTime); 

            if (targetDoor != null) targetDoor._isDoorOpen = true;

            yield return new WaitForSeconds(_doorOpenViewTime);

            cameraScript.isZooming = false;
        }

        else if (targetDoor != null)
            targetDoor._isDoorOpen = true;

    }

    private IEnumerator FocusCameraOnTarget(Transform target, float duration)
    {
        cameraScript.isZooming = true;

        float elapsed = 0f;
        float startZoom = Camera.main.orthographicSize;
        float endZoom = cameraScript.targetZoom;
        Vector3 startPos = Camera.main.transform.position;
        Vector3 endPos = new Vector3(target.position.x , target.position.y + cameraScript.yOffset, -10f);

        while (elapsed < duration)
        {
            Camera.main.orthographicSize = Mathf.Lerp(startZoom, endZoom, elapsed / duration);
            Camera.main.transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        elapsed = 0f;
        while (elapsed < duration)
        {
            Camera.main.orthographicSize = Mathf.Lerp(endZoom, startZoom, elapsed / duration);
            Camera.main.transform.position = Vector3.Lerp(endPos, startPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cameraScript.isZooming = false;
    }


    private void ResetInputField() //on reset le truc pour entrer le password
    {
        codeInput.text = ""; //on vide la case de la rentrée de code
        codeInput.ActivateInputField(); //et on reclique automatiquement dedans = pas besoin de reprednre la souris !
    }


}
