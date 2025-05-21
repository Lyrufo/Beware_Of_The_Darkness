using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class DeathHandler : MonoBehaviour
{
    [Tooltip("la cam pour zoomer")]
    public CameraMovement cameraMovement;

    [Tooltip("Temps d'attente apr�s le zoom")]
    public float delayBeforeDestroy = 3f;

    [Tooltip("Son de mort")]
    public AudioClip deathSound;

    [Tooltip("Nom de l'animation de mort � jouer")]
    public string deathAnimation = "DeathAnim";

    [Tooltip("Nom de l'animation d'arriv�e du monstre")]
    public string monsterEffectAnimationName = "MonsterEffectArrival";

    [Tooltip("L'objet repr�sentant l'effet du monstre")]
    public GameObject monsterEffect; // R�f�rence � l'objet de l'effet du monstre

    [Tooltip("Canvas pour l'�cran de mort")]
    public Canvas deathCanvas;

    [Tooltip("Image pour l'animation de mort plein �cran")]
    public Image fullScreenDeathImage;

    [Tooltip("Animator de l'UI de mort")]
    public Animator deathUIAnimator;

    public void HandleDeath(Transform playerTransform)
    {
        StartCoroutine(DeathSequence(playerTransform));
    }

    private IEnumerator DeathSequence(Transform playerTransform)
    {
        Debug.Log("D�but s�quence de mort");

        var playerController = playerTransform.GetComponent<PlayerCharacter2D>();
        playerController?.SetCinematicMode(true);

        if (playerController != null)
        {
            playerController.SetCinematicMode(true);
        }

        if (deathCanvas != null)
        {
            deathCanvas.gameObject.SetActive(true);
            deathCanvas.sortingOrder = 999; // Pour �tre s�r qu'il soit au premier plan
        }


       

        if (monsterEffect != null) // V�rifie si l'effet du monstre existe
        {
        Animator monsterEffectAnimator = monsterEffect.GetComponent<Animator>();
        if (monsterEffectAnimator != null && !string.IsNullOrEmpty(monsterEffectAnimationName))
        {
                monsterEffectAnimator.Play(monsterEffectAnimationName); // Joue l'animation de l'effet du monstre
        }
        }

        if (fullScreenDeathImage != null)
        {
             fullScreenDeathImage.gameObject.SetActive(true);
             if (deathUIAnimator != null)
             {
             deathUIAnimator.Play("FullScreenDeath", 0, 0f);
             yield return new WaitForSeconds(deathUIAnimator.GetCurrentAnimatorStateInfo(0).length); // Dur�e de l'animation
             }

        }

        
        

        yield return cameraMovement.ZoomAndCenterCoroutine(playerTransform); //zoom de la cam
        foreach (GameObject ui in GameObject.FindGameObjectsWithTag("UI"))
        {
           ui.SetActive(false);
        }

        AudioSource.PlayClipAtPoint(deathSound, playerTransform.position);
        if (monsterEffect != null)
        {
            monsterEffect.GetComponent<Animator>()?.Play(monsterEffectAnimationName);
            yield return new WaitForSeconds(delayBeforeDestroy);
        }

        Destroy(playerTransform.gameObject);
        Debug.Log("Joueur d�truit");

        // Reset de l'UI
        if (deathCanvas != null) deathCanvas.gameObject.SetActive(false);
    }
}

