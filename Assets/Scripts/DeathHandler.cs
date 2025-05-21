using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class DeathHandler : MonoBehaviour
{
    [Tooltip("la cam pour zoomer")]
    public CameraMovement cameraMovement;

    [Tooltip("Temps d'attente après le zoom")]
    public float delayBeforeDestroy = 3f;

    [Tooltip("Son de mort")]
    public AudioClip deathSound;

    [Tooltip("Nom de l'animation de mort à jouer")]
    public string deathAnimation = "DeathAnim";

    [Tooltip("Nom de l'animation d'arrivée du monstre")]
    public string monsterEffectAnimationName = "MonsterEffectArrival";

    [Tooltip("L'objet représentant l'effet du monstre")]
    public GameObject monsterEffect; // Référence à l'objet de l'effet du monstre

    [Tooltip("Canvas pour l'écran de mort")]
    public Canvas deathCanvas;

    [Tooltip("Image pour l'animation de mort plein écran")]
    public Image fullScreenDeathImage;

    [Tooltip("Animator de l'UI de mort")]
    public Animator deathUIAnimator;

    public void HandleDeath(Transform playerTransform)
    {
        StartCoroutine(DeathSequence(playerTransform));
    }

    private IEnumerator DeathSequence(Transform playerTransform)
    {
        Debug.Log("Début séquence de mort");

        var playerController = playerTransform.GetComponent<PlayerCharacter2D>();
        playerController?.SetCinematicMode(true);

        if (playerController != null)
        {
            playerController.SetCinematicMode(true);
        }

        if (deathCanvas != null)
        {
            deathCanvas.gameObject.SetActive(true);
            deathCanvas.sortingOrder = 999; // Pour être sûr qu'il soit au premier plan
        }


       

        if (monsterEffect != null) // Vérifie si l'effet du monstre existe
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
             yield return new WaitForSeconds(deathUIAnimator.GetCurrentAnimatorStateInfo(0).length); // Durée de l'animation
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
        Debug.Log("Joueur détruit");

        // Reset de l'UI
        if (deathCanvas != null) deathCanvas.gameObject.SetActive(false);
    }
}

