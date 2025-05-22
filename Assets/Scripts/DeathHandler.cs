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

    [Tooltip("L'objet repr�sentant l'effet du monstre")]
    public GameObject monsterEffect;

    [Tooltip("Canvas pour l'�cran de mort")]
    public Canvas deathCanvas;

    [Tooltip("Image pour l'animation de mort plein �cran")]
    public Image fullScreenDeathImage;

    [Tooltip("Animator de l'UI de mort")]
    public Animator deathUIAnimator;

    [Tooltip("Dur�e avant respawn apr�s la fin de l'anim")]
    public float postDeathDisplayTime = 1.5f;

    [Tooltip("Transform du joueur")]
    public Transform playerTransform;

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

        if (deathCanvas != null)
        {
            deathCanvas.gameObject.SetActive(true);
            deathCanvas.sortingOrder = 999; // Pour �tre s�r qu'il soit au premier plan
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

        // Zoom cam�ra
        Coroutine zoomCoroutine = StartCoroutine(cameraMovement.ZoomAndCenterCoroutine(playerTransform));

        // Attends 0.5s avant de lancer l'anim plein �cran
        yield return new WaitForSeconds(0.5f);

        // S�quence propre d'activation du canvas + anim pour �viter les premi�res frames doubl�es
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
