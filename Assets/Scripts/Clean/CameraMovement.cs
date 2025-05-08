using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour

{
    [Tooltip("vitesse de suivi de la caméra")]
    public float FollowSpeed = 2f;

    [Tooltip("Truc que suit la cam (donc player)")]
    public Transform target;

    [Tooltip("la hauteur de base la cam en +")]
    public float yOffset = 1f;



    //ce qui concerne le zoom à la mort
    [Tooltip("vitesse de zoom ofc")]
    public float zoomSpeed = 1f;

    [Tooltip("en gros la fenetre de zoom visée")]
    public float targetZoom = 3f;

    [Tooltip("durée du zoom en secondes")]
    public float zoomDuration = 2f;

    [Tooltip("le temps en seconde de pause avant que le perso soit détruit après la fin du zoom")] //------------------ Go jouer anim du player ici en mode droite gauche ??? que se passe t-il oulala 
    public float deathAfterCamera = 3f;

    private float initialZoom; //ça c'est juste la fenetre de base 

    private bool isZooming = false;




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


}


