using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour

{

    [Header("Zoom sur la Porte")]

    [Tooltip("Zoom visé pour l'ouverture de porte")]
    public float doorTargetZoom = 5f;

    [Tooltip("Durée du zoom pour la porte")]
    public float doorZoomDuration = 1f;


    [Tooltip("Décalage Y pour le focus porte")]
    public float doorYOffset = 0.5f;


    [Header("Zoom sur la Mort")]

    [Tooltip("en gros la fenetre de zoom visée")]
    public float targetZoom = 3f;

    [Tooltip("durée du zoom en secondes")]
    public float zoomDuration = 2f;

    [Tooltip("la hauteur de base la cam en +")]
    public float yOffset = 1f;

    [Tooltip("vitesse de suivi de la caméra")]
    public float FollowSpeed = 2f;

    [Tooltip("Truc que suit la cam (donc player)")]
    public Transform target;

    [Tooltip("vitesse de zoom ofc")]
    public float zoomSpeed = 1f;

    [Tooltip("le temps en seconde de pause avant que le perso soit détruit après la fin du zoom")] //------------------ Go jouer anim du player ici en mode droite gauche ??? que se passe t-il oulala 
    public float deathAfterCamera = 3f;



    private float initialZoom; //ça c'est juste la fenetre de base 

    public bool isZooming = false;




    public void StartZoomBeforeDeath() 
    {
        StartCoroutine(ZoomAndCenter(target)); //truc lancé OnDeath(mis dans inspector et target aussi donc pas besoin de mettre player
    }

    private void Start()
    {
        initialZoom = Camera.main.orthographicSize; //en gros le zoom de départ c'est a partir de la position et tt de la cam de base 
    }

    void Update() //ça c'est le basic mvt de la cam quand bah y'a pas la mort et donc le zoom qui se déclenchent
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
    }

    IEnumerator ZoomAndCenter(Transform playerTransform) //choppe la coroutine et la pos et tt du player
    {

        isZooming = true; //on commence "is zooming"
        var playerControls = playerTransform.GetComponent<PlayerCharacter2D>();
        if (playerControls != null)
        {
            playerControls.canMove = false;
        }
        GameObject[] uiElements = GameObject.FindGameObjectsWithTag("UI");
        foreach (GameObject ui in uiElements)
            ui.SetActive(false); //----------------REMTTRE AU RESPAWN
        float zoomTimer = 0f; // "chrono" lancé pour pas pas dépasser le temps de zoom
        InputSystem.DisableDevice(Keyboard.current); //empeche le joueur de bouger ou faire quoi que ce soit
        //InputSystem.EnableDevice(Keyboard.current); ça pour le remettre après le respawn



        while (zoomTimer < zoomDuration)
        {

            Camera.main.orthographicSize = Mathf.Lerp(initialZoom, targetZoom, zoomTimer / zoomDuration); //ce qui zoom doucement la cam

            transform.position = Vector3.Lerp(transform.position, //et centre ce zoom sur le joueur progressivement ausis
                new Vector3(playerTransform.position.x, playerTransform.position.y, -10f),
                zoomTimer / zoomDuration);

            zoomTimer += Time.deltaTime; //on met à j le temps de elapsed
            yield return null;
        }

        yield return new WaitForSeconds(deathAfterCamera);
        //-----------------jouer anim mort (+son ?) GetComponent<Animator>().Play("DeathAnim");
        // -----------------AudioSource.PlayClipAtPoint(deathSound, transform.position);
        Destroy(playerTransform.gameObject);
        Debug.Log("Mort après zoom");

    }

    public IEnumerator DoorZoom(Transform doorTarget, System.Action onComplete = null)
    {
        isZooming = true;
        float elapsed = 0f;
        float startZoom = Camera.main.orthographicSize;
        Vector3 startPos = Camera.main.transform.position;
        Vector3 endPos = new Vector3(doorTarget.position.x, doorTarget.position.y + doorYOffset, -10f);

        // Zoom IN
        while (elapsed < doorZoomDuration)
        {
            Camera.main.orthographicSize = Mathf.Lerp(startZoom, doorTargetZoom, elapsed / doorZoomDuration);
            Camera.main.transform.position = Vector3.Lerp(startPos, endPos, elapsed / doorZoomDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        onComplete?.Invoke(); // Callback quand le zoom est fini
    }


}


