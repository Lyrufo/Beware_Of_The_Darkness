using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DeathHandler : MonoBehaviour
{
    [Tooltip("Temps d'attente avant le fullscreen anim")]
    public float delayBeforeFullScreenAnim = 0.5f;

    [Tooltip("Dur�e totale de l'animation UI de mort")]
    public float deathUIAnimDuration = 4.43f;

    [Tooltip("Dur�e du zoom (doit matcher avec cameraMovement.deathZoomDuration)")]
    public float zoomDuration = 1.49f;

    [Tooltip("Son de mort")]
    public AudioClip deathSound;

    [Tooltip("Nom de l'animation d'arriv�e du monstre")]
    public string monsterEffectAnimationName = "MonsterEffectArrival";

    [Tooltip("Dur�e avant respawn apr�s la fin de l'anim")]
    public float postDeathDisplayTime = 1.5f;

    [Tooltip("Transform du joueur")]
    public Transform playerTransform;

    [Tooltip("Image pour l'animation de mort plein �cran")]
    public Image fullScreenDeathImage; // Retir� System.NonSerialized

    private RespawnManager _respawnManager;
    public RespawnManager respawnManager => _respawnManager ??= FindObjectOfType<RespawnManager>(true);

    private CameraMovement _cameraMovement;
    public CameraMovement cameraMovement => _cameraMovement ??= FindObjectOfType<CameraMovement>(true);

    private Canvas _deathCanvas;
    public Canvas DeathCanvas
    {
        get
        {
            if (_deathCanvas == null)
            {
                // Nouvelle m�thode de recherche incluant les objets inactifs
                Canvas[] allCanvases = Resources.FindObjectsOfTypeAll<Canvas>();
                foreach (Canvas canvas in allCanvases)
                {
                    if (canvas.CompareTag("DeathCanvas"))
                    {
                        _deathCanvas = canvas;
                        break;
                    }
                }
            }
            return _deathCanvas;
        }
    }

    private Image _fullScreenDeathImage;
    public Image FullScreenDeathImage => _fullScreenDeathImage ??= DeathCanvas?.GetComponentInChildren<Image>(true);

    private Animator _deathUIAnimator;
    public Animator DeathUIAnimator
    {
        get
        {
            if (_deathUIAnimator == null && DeathCanvas != null)
            {
                _deathUIAnimator = DeathCanvas.GetComponentInChildren<Animator>(true);

                // Debug critique pour v�rification
                if (_deathUIAnimator == null)
                {
                    Debug.LogError("Animator manquant sur le FullScreenDeathImage ! V�rifiez : " +
                                  FullScreenDeathImage?.gameObject.name);
                }
            }
            return _deathUIAnimator;
        }
    }

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
            playerController.playerRigidbody.velocity = Vector2.zero;
            playerController.playerRigidbody.gravityScale = 0f;
            playerController.enabled = false;
        }

        foreach (GameObject ui in GameObject.FindGameObjectsWithTag("UI"))
        {
            ui.SetActive(false);
        }

        if (DeathCanvas != null)
        {
            // Activation FORC�E dans l'ordre correct
            DeathCanvas.gameObject.SetActive(true);
            DeathCanvas.enabled = true;

            Debug.Log($"DeathCanvas activ� : {DeathCanvas.gameObject.activeSelf}"); // Debug 1

            if (FullScreenDeathImage != null)
            {
                FullScreenDeathImage.gameObject.SetActive(true);
                FullScreenDeathImage.enabled = true;
            }

            // V�rification finale de l'Animator
            if (DeathUIAnimator == null)
            {
                Debug.LogError("ERREUR CRITIQUE - Animator non trouv� apr�s activation !");
                yield break;
            }

            DeathUIAnimator.Rebind();
            DeathUIAnimator.Update(0f);
            DeathCanvas.GetComponent<CanvasGroup>().alpha = 1;
        }
        else
        {
            Debug.LogError("DeathCanvas non trouv� ! V�rifiez le tag 'DeathCanvas'");
            yield break;
        }

        // D�but de la s�quence d'animation
        Coroutine zoomCoroutine = StartCoroutine(cameraMovement.ZoomAndCenterCoroutine(playerTransform));
        yield return new WaitForSeconds(0.5f);

        // CORRECTION MAJEURE : Utilisation de la propri�t� avec majuscule
        if (DeathUIAnimator != null)
        {
            Debug.Log($"Lancement animation sur : {DeathUIAnimator.gameObject.name}"); // Debug 2
            DeathUIAnimator.enabled = true;
            DeathUIAnimator.SetTrigger("StartDeath");

            // V�rification du d�clenchement
            Debug.Log($"Animation en cours : {DeathUIAnimator.GetCurrentAnimatorStateInfo(0).IsName("FullScreenDeathAnim")}");
        }
        else
        {
            Debug.LogError("Animator non disponible au moment du d�clenchement !");
        }

        float remainingDuration = deathUIAnimDuration - (delayBeforeFullScreenAnim + zoomDuration);
        yield return new WaitForSeconds(Mathf.Max(remainingDuration, 0f));

        if (DeathUIAnimator != null)
        {
            DeathUIAnimator.enabled = false;
        }

        yield return new WaitForSeconds(postDeathDisplayTime);
        respawnManager.TriggerRespawn();

        if (DeathCanvas != null) DeathCanvas.gameObject.SetActive(false);
    }
}