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

    [Tooltip("Durée avant respawn après la fin de l'anim")]
    public float postDeathDisplayTime = 1.5f;

    [Tooltip("Transform du joueur")]
    public Transform playerTransform;

    [Tooltip("Image pour l'animation de mort plein écran")]
    [System.NonSerialized] public Image fullScreenDeathImage;

    [Tooltip("Animator de l'UI de mort")]
    [System.NonSerialized] public Animator deathUIAnimator;

    //[Tooltip("L'objet représentant l'effet du monstre")]
    //[System.NonSerialized] public GameObject monsterEffect;


    private RespawnManager _respawnManager;
    public RespawnManager respawnManager => _respawnManager ??= FindObjectOfType<RespawnManager>(true);

    private CameraMovement _cameraMovement;
    public CameraMovement cameraMovement => _cameraMovement ??= FindObjectOfType<CameraMovement>(true);

    private Canvas _deathCanvas;
    public Canvas DeathCanvas => _deathCanvas ??= FindObjectOfType<Canvas>(true);

    private Image _fullScreenDeathImage;
    public Image FullScreenDeathImage => _fullScreenDeathImage ??= DeathCanvas?.GetComponentInChildren<Image>(true);

    private Animator _deathUIAnimator;
    public Animator DeathUIAnimator => _deathUIAnimator ??= DeathCanvas?.GetComponentInChildren<Animator>(true);

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

        if (DeathCanvas != null)
        {
            DeathCanvas.gameObject.SetActive(true);
            DeathCanvas.enabled = true;
            LayoutRebuilder.ForceRebuildLayoutImmediate(DeathCanvas.GetComponent<RectTransform>());
        }
        else if (DeathCanvas == null)
        {
            Debug.Log("deathcanvanull");
        }

        //if (monsterEffect != null)
        //{
        //Animator monsterEffectAnimator = monsterEffect.GetComponent<Animator>();
        //if (monsterEffectAnimator != null && !string.IsNullOrEmpty(monsterEffectAnimationName))
        //{
        //monsterEffectAnimator.Play(monsterEffectAnimationName);
        //}
        //}

        if (fullScreenDeathImage != null)
        {

            FullScreenDeathImage.gameObject.SetActive(true);
            FullScreenDeathImage.SetAllDirty();
        }
        else if (DeathCanvas == null)
        {
            Debug.Log("fullscreendeathimage null");
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

        if (DeathCanvas != null) DeathCanvas.gameObject.SetActive(false);
    }



}
