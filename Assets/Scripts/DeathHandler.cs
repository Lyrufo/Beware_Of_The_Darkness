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
        var playerController = playerTransform.GetComponent<PlayerCharacter2D>();
        playerController?.SetCinematicMode(true);

        // Désactivation UI
        foreach (GameObject ui in GameObject.FindGameObjectsWithTag("UI"))
            ui.SetActive(false);

        // Activation contrôlée du Canvas
        DeathCanvas.gameObject.SetActive(true);
        DeathCanvas.enabled = true;
        fullScreenDeathImage.gameObject.SetActive(true);

        // Réinitialisation de l'animator
        _deathUIAnimator.Rebind();
        _deathUIAnimator.Update(0f);

        // Zoom caméra
        yield return StartCoroutine(_cameraMovement.ZoomAndCenterCoroutine(playerTransform));
        yield return new WaitForSeconds(delayBeforeFullScreenAnim);

        // Lancement animation
        _deathUIAnimator.SetTrigger("StartDeath");
        yield return new WaitForSeconds(deathUIAnimDuration);

        // Pause sur dernière frame
        _deathUIAnimator.enabled = false;
        yield return new WaitForSeconds(postDeathDisplayTime);

        // Nettoyage
        DeathCanvas.gameObject.SetActive(false);
        _respawnManager.TriggerRespawn();
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