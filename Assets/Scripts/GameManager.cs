using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Références")]
    public TimerDieAndRespawn timer;
    public PlayerCharacter2D player;
    public GameObject startCanvas;
    public GameObject transitionContainer; // Désactivé au départ
    public Animator transitionAnimator; // Animator enfant activé

    [Header("Timings")]
    public float startDelay = 1.04f;
    public float transitionDuration = 1.5f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        Time.timeScale = 0f;
        // Initialisation sécurisée
        if (player != null)
        {
            player.canMove = false; // Désactive les contrôles sans utiliser l'Animator
            player.gameObject.SetActive(false);
        }

        if (timer != null)
            timer.enabled = false;

        // Vérification de la transition
        if (transitionContainer != null)
        {
            transitionContainer.SetActive(false);

            // Assure-toi que l'Animator a un contrôleur
            if (transitionAnimator != null && transitionAnimator.runtimeAnimatorController == null)
            {
                Debug.LogError("L'Animator de transition n'a pas de Animator Controller assigné!");
            }
        }
    }

    public void StartGame()
    {
        Time.timeScale = 1;
        StartCoroutine(GameStartSequence());
    }

    private IEnumerator GameStartSequence()
    {
        // 1. Active le container de transition
        if (transitionContainer != null)
        {
            transitionContainer.SetActive(true);

            // Petit délai pour permettre l'initialisation
            yield return null;

            // Déclenche l'animation
            if (transitionAnimator != null)
            {
                transitionAnimator.SetTrigger("StartTransition");
            }
            else
            {
                Debug.LogError("Transition Animator manquant!");
            }
        }

        // 2. Attend le délai avant de changer de scène
        yield return new WaitForSeconds(startDelay);

        // 3. Désactive le menu
        if (startCanvas != null)
            startCanvas.SetActive(false);

        // 4. Active le joueur (toujours en mode cinéma)
        if (player != null)
        {
            player.gameObject.SetActive(true);
            player.canMove = false; // Contrôles toujours bloqués
        }

        // 5. Démarre le timer
        if (timer != null)
        {
            timer.enabled = true;
            timer.StartTimer();
        }

        // 6. Attend la fin de la transition
        float remainingTime = transitionDuration - startDelay;
        if (remainingTime > 0)
            yield return new WaitForSeconds(remainingTime);

        // 7. Finalise l'activation
        if (player != null)
        {
            player.canMove = true; // Débloque les contrôles

            // Réinitialise l'animator si nécessaire
            if (player.GetComponent<Animator>() != null)
            {
                var playerAnim = player.GetComponent<Animator>();
                if (playerAnim.runtimeAnimatorController != null)
                {
                    playerAnim.SetBool("IsMoving", false);
                }
            }
        }

        // 8. Désactive la transition
        if (transitionContainer != null)
            transitionContainer.SetActive(false);
    }
}