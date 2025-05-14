using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    public void HandleDeath(Transform playerTransform)
    {
        StartCoroutine(DeathSequence(playerTransform));
    }

    private IEnumerator DeathSequence(Transform playerTransform)
    {
        Debug.Log("D�but s�quence de mort");

        var playerController = playerTransform.GetComponent<PlayerCharacter2D>();
        if (playerController != null)
        {
            playerController.SetCinematicMode(true);
        }


        Animator animator = playerTransform.GetComponent<Animator>(); //anim de mort � jouer du perso 
        if (animator != null && !string.IsNullOrEmpty(deathAnimation))
        {
            animator.Play(deathAnimation);
        }

        if (monsterEffect != null) // V�rifie si l'effet du monstre existe
        {
            Animator monsterEffectAnimator = monsterEffect.GetComponent<Animator>();
            if (monsterEffectAnimator != null && !string.IsNullOrEmpty(monsterEffectAnimationName))
            {
                monsterEffectAnimator.Play(monsterEffectAnimationName); // Joue l'animation de l'effet du monstre
            }
        }


        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, playerTransform.position); //son de mort � ajouter
        }

        foreach (GameObject ui in GameObject.FindGameObjectsWithTag("UI")) //enlever toutes les UI
        {
            ui.SetActive(false);
        }

        
        var controls = playerTransform.GetComponent<PlayerCharacter2D>(); //desactive controles
        if (controls != null)
        {
            controls.canMove = false;
        }

        yield return cameraMovement.ZoomAndCenterCoroutine(playerTransform); //zoom de la cam
        yield return new WaitForSeconds(delayBeforeDestroy); // Attente apr�s le zoom

        
        Destroy(playerTransform.gameObject);
        Debug.Log("Joueur d�truit apr�s s�quence de mort"); //joueur d�truit 

        //respawn, game over, reload etc � ajouter ici
    }
}

