using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;

public class InteractiveObject : MonoBehaviour
{
    //liste de trucs � assigner ds l'inspector
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


    [Header("Post-Interaction param�tres")]

    [Tooltip("R�f�rence � l'Animator de l'objet")]
    public Animator objectAnimator;
    
    private SpriteRenderer _spriteRenderer;
    private bool _hasBeenInteracted = false;



    private bool _playerInRange = false; //par d�faut joueur pas dans la port�e pour d�tecter 
    private bool _isDescriptionOpen = false; //si la description est deja open mais l� le met direct en false

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (objectAnimator != null )
            objectAnimator.SetBool ("Interacted", false);
    }



    void OnTriggerEnter2D(Collider2D collision) //qd qqc entre en collision
    {
        if (collision.CompareTag("Player") && !_hasBeenInteracted) //et a le tag player
        {
            _playerInRange = true; //donc dans la zone de detection
            if (interactionPopUp != null) //ofc que dans le cas o� j'ai rempli l'interactionpopup
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
            if (!_isDescriptionOpen && data != null && !_hasBeenInteracted) //et la description est pas open et la data est pas vide (scriptable object complet
            {
                OpenDescription();
                ApplyVisualChanges();
                _hasBeenInteracted = true;

            }
        }
        if (_isDescriptionOpen && Input.GetKeyDown(KeyCode.Escape)) //si la description est ouverte et on appuie echap
        {
           CloseDescription();
        }
    }

    void OpenDescription()
    {
        //rempli avec tout �a chopp� dans le scriptable object
        objectNameText.text = data.interactiveObjectName;
        descriptionText.text = data.description;
        objectImage.sprite = data.minimodel;

        interactionPopUp.SetActive(false);
        descriptionPanel.SetActive(true); //affiche le panel de description
        _isDescriptionOpen = true; //met � jour l�tat
        PauseGame();
    }

    void CloseDescription()
    {
        descriptionPanel.SetActive(false); //cache panel
        _isDescriptionOpen = false; //met a jour l'�tat
        ResumeGame();//reprend
    }

    void ApplyVisualChanges()
    {
        //pour changer sprite et anim
        if (_spriteRenderer != null && data.alternateSprite != null)
            _spriteRenderer.sprite = data.alternateSprite;

        if (objectAnimator != null)
            objectAnimator.SetBool("Interacted", true);
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
