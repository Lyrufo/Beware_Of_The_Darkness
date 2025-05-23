using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("R�f�rences")]
    public TimerDieAndRespawn timer;
    public PlayerCharacter2D player;
    public GameObject startCanvas;
    public GameObject transitionContainer; // D�sactiv� au d�part
    public Animator transitionAnimator; // Animator enfant activ�

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
        // Initialisation s�curis�e
        if (player != null)
        {
            player.canMove = false; // D�sactive les contr�les sans utiliser l'Animator
            player.gameObject.SetActive(false);
        }

        if (timer != null)
            timer.enabled = false;

        // V�rification de la transition
        if (transitionContainer != null)
        {
            transitionContainer.SetActive(false);

            // Assure-toi que l'Animator a un contr�leur
            if (transitionAnimator != null && transitionAnimator.runtimeAnimatorController == null)
            {
                Debug.LogError("L'Animator de transition n'a pas de Animator Controller assign�!");
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

            // Petit d�lai pour permettre l'initialisation
            yield return null;

            // D�clenche l'animation
            if (transitionAnimator != null)
            {
                transitionAnimator.SetTrigger("StartTransition");
            }
            else
            {
                Debug.LogError("Transition Animator manquant!");
            }
        }

        // 2. Attend le d�lai avant de changer de sc�ne
        yield return new WaitForSeconds(startDelay);

        // 3. D�sactive le menu
        if (startCanvas != null)
            startCanvas.SetActive(false);

        // 4. Active le joueur (toujours en mode cin�ma)
        if (player != null)
        {
            player.gameObject.SetActive(true);
            player.canMove = false; // Contr�les toujours bloqu�s
        }

        // 5. D�marre le timer
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
            player.canMove = true; // D�bloque les contr�les

            // R�initialise l'animator si n�cessaire
            if (player.GetComponent<Animator>() != null)
            {
                var playerAnim = player.GetComponent<Animator>();
                if (playerAnim.runtimeAnimatorController != null)
                {
                    playerAnim.SetBool("IsMoving", false);
                }
            }
        }

        // 8. D�sactive la transition
        if (transitionContainer != null)
            transitionContainer.SetActive(false);
    }
}