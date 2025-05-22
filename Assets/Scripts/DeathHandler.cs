using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DeathHandler : MonoBehaviour
{

    [Tooltip("Temps d'attente avant le fullscreen anim")]
    public float delayBeforeFullScreenAnim = 0.5f;

    [Tooltip("Durée totale de l'animation UI de mort")]
    public float deathUIAnimDuration = 4.43f;

    [Tooltip("Durée du zoom (doit matcher avec cameraMovement.deathZoomDuration)")]
    public float zoomDuration = 1.49f;

    [Tooltip("Son de mort")]
    public AudioClip deathSound;

    [Tooltip("Nom de l'animation d'arrivée du monstre")]
    public string monsterEffectAnimationName = "MonsterEffectArrival";

    [Tooltip("L'objet représentant l'effet du monstre")]
    public GameObject monsterEffect;

    [Tooltip("Canvas pour l'écran de mort")]
    public Canvas deathCanvas;

    [Tooltip("Image pour l'animation de mort plein écran")]
    public Image fullScreenDeathImage;

    [Tooltip("Animator de l'UI de mort")]
    public Animator deathUIAnimator;

    [Tooltip("Durée avant respawn après la fin de l'anim")]
    public float postDeathDisplayTime = 1.5f;

    [Tooltip("Transform du joueur")]
    public Transform playerTransform;

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
            playerController.playerRigidbody.velocity = Vector2.zero;
            playerController.playerRigidbody.gravityScale = 0f;
            playerController.enabled = false;
        }

        foreach (GameObject ui in GameObject.FindGameObjectsWithTag("UI"))
        {
            ui.SetActive(false);
        }

        if (deathCanvas != null)
        {
            deathCanvas.gameObject.SetActive(true);
            deathCanvas.sortingOrder = 999; // Pour être sûr qu'il soit au premier plan
        }

        if (monsterEffect != null)
        {
            Animator monsterEffectAnimator = monsterEffect.GetComponent<Animator>();
            if (monsterEffectAnimator != null && !string.IsNullOrEmpty(monsterEffectAnimationName))
            {
                monsterEffectAnimator.Play(monsterEffectAnimationName);
            }
        }

        if (fullScreenDeathImage != null)
        {
 
            if (deathUIAnimator != null)
                deathUIAnimator.enabled = false;

            fullScreenDeathImage.gameObject.SetActive(true);
        }

        // Zoom caméra
        Coroutine zoomCoroutine = StartCoroutine(cameraMovement.ZoomAndCenterCoroutine(playerTransform));

        // Attends 0.5s avant de lancer l'anim plein écran
        yield return new WaitForSeconds(0.5f);

        // Séquence propre d'activation du canvas + anim pour éviter les premières frames doublées
        if (deathUIAnimator != null)
        {
            deathUIAnimator.enabled = true;
            deathUIAnimator.Play("FullScreenDeathAnim", 0, 0f);
        }

        float remainingDuration = deathUIAnimDuration - (delayBeforeFullScreenAnim + zoomDuration);
        yield return new WaitForSeconds(Mathf.Max(remainingDuration, 0f));

        if (deathUIAnimator != null)
        {
            deathUIAnimator.enabled = false;
        }

        //AudioSource.PlayClipAtPoint(deathSound, playerTransform.position);
        yield return new WaitForSeconds(postDeathDisplayTime);
        respawnManager.TriggerRespawn();

        if (deathCanvas != null) deathCanvas.gameObject.SetActive(false);
    }

    public RespawnManager respawnManager => FindObjectOfType<RespawnManager>(true);
    public CameraMovement cameraMovement => FindObjectOfType<CameraMovement>(true);

}
