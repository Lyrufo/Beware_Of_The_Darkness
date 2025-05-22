using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DeathHandler : MonoBehaviour
{
    [Header("Timings")]
    public float delayBeforeFullScreenAnim = 0.5f;
    public float deathUIAnimDuration = 4.43f;
    public float zoomDuration = 1.49f;
    public float postDeathDisplayTime = 1.5f;

    [Header("Références")]
    public Transform playerTransform;
    public Image fullScreenDeathImage;

    private RespawnManager _respawnManager;
    private CameraMovement _cameraMovement;
    private Canvas _deathCanvas;
    private Animator _deathUIAnimator;

    private void Awake()
    {
        // Désactivation initiale du Canvas
        if (DeathCanvas != null)
            DeathCanvas.gameObject.SetActive(false);
    }

    public void HandleDeath(Transform playerTransform) => StartCoroutine(DeathSequence(playerTransform));

    private IEnumerator DeathSequence(Transform playerTransform)
    {
        // Ajouter des vérifications de null partout
        if (playerTransform == null) yield break;

        var playerController = playerTransform.GetComponent<PlayerCharacter2D>();
        if (playerController != null)
        {
            playerController.SetCinematicMode(true);
            playerController.playerRigidbody.velocity = Vector2.zero; // Reset velocity
        }

        // Vérifier que la caméra existe
        if (cameraMovement == null) yield break;

        // Vérifier que le Canvas existe
        if (DeathCanvas == null)
        {
            Debug.LogError("DeathCanvas non trouvé!");
            yield break;
        }

        DeathCanvas.gameObject.SetActive(true);
        fullScreenDeathImage.gameObject.SetActive(true);

        // Vérifier l'animator
        if (DeathUIAnimator == null)
        {
            Debug.LogError("Animator de mort non trouvé!");
            yield break;
        }

        // Corriger la réinitialisation de l'animator
        DeathUIAnimator.Rebind();
        DeathUIAnimator.Update(0f);

        // Zoom caméra
        yield return StartCoroutine(cameraMovement.ZoomAndCenterCoroutine(playerTransform));
        yield return new WaitForSeconds(delayBeforeFullScreenAnim);

        // Lancer l'animation
        DeathUIAnimator.SetTrigger("StartDeath");
        yield return new WaitForSeconds(deathUIAnimDuration);

        // Pause finale
        yield return new WaitForSeconds(postDeathDisplayTime);

        // Nettoyage
        DeathCanvas.gameObject.SetActive(false);

        // Vérifier le respawn manager
        if (respawnManager != null)
        {
            respawnManager.TriggerRespawn();
        }
        else
        {
            Debug.LogError("RespawnManager non trouvé!");
        }
    }

    #region Propriétés optimisées
    public RespawnManager respawnManager => _respawnManager ??= FindObjectOfType<RespawnManager>();
    public CameraMovement cameraMovement => _cameraMovement ??= FindObjectOfType<CameraMovement>();
    public Canvas DeathCanvas => _deathCanvas ??= GetCanvasByTag("DeathCanvas");
    public Animator DeathUIAnimator => _deathUIAnimator ??= DeathCanvas?.GetComponentInChildren<Animator>();

    private Canvas GetCanvasByTag(string tag)
    {
        foreach (Canvas canvas in Resources.FindObjectsOfTypeAll<Canvas>())
            if (canvas.CompareTag(tag)) return canvas;
        return null;
    }
    #endregion
}