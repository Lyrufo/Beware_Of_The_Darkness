using System.Collections;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Zoom sur la Porte")] //groupe de param�tres de zoom sp�cifiques � l'ouverture de la porte qd la cam bouge

    [Tooltip("Zoom vis� pour l'ouverture de porte")]
    public float doorTargetZoom = 5f;

    [Tooltip("Dur�e du zoom pour la porte")]
    public float doorZoomDuration = 1f;

    [Tooltip("D�calage Y pour le focus porte")]
    public float doorYOffset = 0.5f;

    [Header("Zoom sur la Mort")] //groupe de param�tres pour le zoom qd on meurt 

    [Tooltip("Zoom vis� pendant la mort")]
    public float deathTargetZoom = 3f;

    [Tooltip("Dur�e du zoom de mort")]
    public float deathZoomDuration = 1.49f;

    [Tooltip("D�calage de position de la cam�ra pour la mort")]
    public Vector2 deathCamOffset = Vector2.zero;

    [Header("Mouvements Cam Habituels")]

    [Tooltip("la hauteur de base la cam en +")]
    public float yOffset = 1f;

    [Tooltip("vitesse de suivi de la cam�ra")]
    public float FollowSpeed = 2f;

    [Tooltip("Truc que suit la cam (donc player)")]
    public Transform target;

    [Header("Perso")]
    [Tooltip("le perso dont je vais bloquer les controles")]
    public PlayerCharacter2D character;

    private Camera _mainCamera;
    private float initialZoom; //�a c'est juste la fenetre de base 
    public bool isZooming = false;


    private void Start()
    {
        _mainCamera = Camera.main;
        initialZoom = _mainCamera.orthographicSize;
    }

    void Update() //�a c'est le basic mvt de la cam quand bah y'a pas la mort et donc le zoom qui se d�clenchent
    {
        if (!isZooming)
        {
            Vector3 newPos = new Vector3(target.position.x, target.position.y + yOffset, -10f);
            transform.position = Vector3.Slerp(transform.position, newPos, FollowSpeed * Time.deltaTime);
        }
    }

    public void ResetCamera()
    {
        isZooming = false;
        if (_mainCamera != null)
        {
            _mainCamera.orthographicSize = initialZoom;
        }

        // R�activation du suivi naturel
        if (target != null)
        {
            Vector3 newPos = new Vector3(target.position.x, target.position.y + yOffset, -10f);
            transform.position = newPos;
        }
    }

    public IEnumerator ZoomAndCenterCoroutine(Transform playerTransform) //choppe la coroutine et la pos et tt du player
    {
        isZooming = true;
        float zoomTimer = 0f;
        float startZoom = _mainCamera.orthographicSize;

        var playerController = playerTransform.GetComponent<PlayerCharacter2D>();
        if (playerController != null)
        {
            playerController.SetCinematicMode(true);
        }

        while (zoomTimer < deathZoomDuration)
        {
            _mainCamera.orthographicSize = Mathf.Lerp(startZoom, deathTargetZoom, zoomTimer / deathZoomDuration);
            transform.position = Vector3.Lerp(transform.position,
                new Vector3(playerTransform.position.x + deathCamOffset.x, playerTransform.position.y + deathCamOffset.y, -10f),
                zoomTimer / deathZoomDuration);

            zoomTimer += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator DoorZoomIn(Transform doorTarget)
    {
        isZooming = true;

        if (character != null)
        {
            character.SetCinematicMode(true);
        }

        float elapsed = 0f;
        float startZoom = _mainCamera.orthographicSize;
        Vector3 startPos = _mainCamera.transform.position;
        Vector3 endPos = new Vector3(doorTarget.position.x, doorTarget.position.y + doorYOffset, -10f);

        while (elapsed < doorZoomDuration)
        {
            _mainCamera.orthographicSize = Mathf.Lerp(startZoom, doorTargetZoom, elapsed / doorZoomDuration);
            _mainCamera.transform.position = Vector3.Lerp(startPos, endPos, elapsed / doorZoomDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (character != null)
        {
            character.SetCinematicMode(false);
        }

        isZooming = false;
    }

    public IEnumerator DoorZoomOut()
    {
        float elapsed = 0f;
        float startZoom = _mainCamera.orthographicSize;
        Vector3 startPos = _mainCamera.transform.position;
        Vector3 endPos = new Vector3(target.position.x, target.position.y + doorYOffset, -10f);

        while (elapsed < doorZoomDuration)
        {
            _mainCamera.orthographicSize = Mathf.Lerp(startZoom, initialZoom, elapsed / doorZoomDuration);
            _mainCamera.transform.position = Vector3.Lerp(startPos, endPos, elapsed / doorZoomDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        character.enabled = true;
        isZooming = false;
    }
}
