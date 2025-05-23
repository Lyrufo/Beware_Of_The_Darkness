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

    [Header("R�f�rences")]
    public Transform playerTransform;
    public Canvas fullScreenDeathImage;

    private RespawnManager _respawnManager;
    private CameraMovement _cameraMovement;
    private Canvas _deathCanvas;
    private Animator _deathUIAnimator;

    [Header("Custom timing")]
    public float cameraMoveDelay = 2.2f; // le moment exact o� l'�cran est noir, � config dans l'Inspector



    private void Awake()
    {
        // D�sactivation initiale du Canvas
        if (DeathCanvas != null)
            DeathCanvas.gameObject.SetActive(false);
    }

    public void HandleDeath(Transform playerTransform) => StartCoroutine(DeathSequence(playerTransform));

    private IEnumerator DeathSequence(Transform playerTransform)
    {
        if (playerTransform == null || cameraMovement == null || respawnManager == null)
        {
            Debug.LogError("R�f�rences manquantes dans DeathHandler!");
            yield break;
        }

        if (DeathUIAnimator == null)
        {
            Debug.LogError("Animator de mort non trouv�!");
            yield break;
        }

        // 1. Zoom sur le joueur
        yield return StartCoroutine(cameraMovement.ZoomAndCenterCoroutine(playerTransform));
        yield return new WaitForSeconds(delayBeforeFullScreenAnim);

        DeathCanvas.gameObject.SetActive(true);
        DeathUIAnimator.SetTrigger("StartDeath");

        // 2. D�placement anticip� de la cam�ra vers le point de respawn PENDANT l'animation de mort
        // 2. Attendre le moment exact avant de d�placer la cam�ra
        yield return new WaitForSeconds(cameraMoveDelay);

        // Puis d�placer la cam�ra au point de respawn
        Transform respawnPoint = respawnManager.defaultRespawnPoint;
        cameraMovement.target = null; // d�sactiver le suivi temporaire
        cameraMovement.transform.position = new Vector3(
            respawnPoint.position.x,
            respawnPoint.position.y + cameraMovement.yOffset,
            -10f
        );
        cameraMovement.ResetCamera();

        // 3. Attendre la fin de l�animation de mort
        yield return new WaitForSeconds(deathUIAnimDuration);

        //yield return new WaitForSeconds(postDeathDisplayTime);

        DeathCanvas.gameObject.SetActive(false);

        // 4. Respawn (camera est d�j� plac�e au bon endroit)
        respawnManager.TriggerRespawn();
    }

    #region Propri�t�s optimis�es
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