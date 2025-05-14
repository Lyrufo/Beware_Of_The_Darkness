using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    public void HandleDeath(Transform playerTransform)
    {
        StartCoroutine(DeathSequence(playerTransform));
    }

    private IEnumerator DeathSequence(Transform playerTransform)
    {
        Debug.Log("Début séquence de mort");

        var playerController = playerTransform.GetComponent<PlayerCharacter2D>();
        if (playerController != null)
        {
            playerController.SetCinematicMode(true);
        }


        Animator animator = playerTransform.GetComponent<Animator>(); //anim de mort à jouer du perso 
        if (animator != null && !string.IsNullOrEmpty(deathAnimation))
        {
            animator.Play(deathAnimation);
        }

        if (monsterEffect != null) // Vérifie si l'effet du monstre existe
        {
            Animator monsterEffectAnimator = monsterEffect.GetComponent<Animator>();
            if (monsterEffectAnimator != null && !string.IsNullOrEmpty(monsterEffectAnimationName))
            {
                monsterEffectAnimator.Play(monsterEffectAnimationName); // Joue l'animation de l'effet du monstre
            }
        }


        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, playerTransform.position); //son de mort à ajouter
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
        yield return new WaitForSeconds(delayBeforeDestroy); // Attente après le zoom

        
        Destroy(playerTransform.gameObject);
        Debug.Log("Joueur détruit après séquence de mort"); //joueur détruit 

        //respawn, game over, reload etc à ajouter ici
    }
}

