using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

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

    [Tooltip("en gros la fenetre de zoom vis�e")]
    public float targetZoom = 3f;

    [Tooltip("dur�e du zoom en secondes")]
    public float zoomDuration = 2f;

    [Tooltip("le temps en seconde de pause avant que le perso soit d�truit apr�s la fin du zoom")]
    public float deathAfterCamera = 3f; //------------------ Go jouer anim du player ici en mode droite gauche ??? que se passe t-il oulala 


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




    private float initialZoom; //�a c'est juste la fenetre de base 

    public bool isZooming = false;

    private void Start()
    {
        initialZoom = Camera.main.orthographicSize; //en gros le zoom de d�part c'est a partir de la position et tt de la cam de base 
    }

    void Update() //�a c'est le basic mvt de la cam quand bah y'a pas la mort et donc le zoom qui se d�clenchent
    {
        if (!isZooming)
        {
            Vector3 newPos = new Vector3(target.position.x, target.position.y + yOffset, -10f);
            transform.position = Vector3.Slerp(transform.position, newPos, FollowSpeed * Time.deltaTime);
        } //donc le retour � la pos de base avec la vitesse et tt de param�tr� ? 
       
    }

    public void ResetCamera()
    {
       isZooming = false; //donc fais ref au truc d'au dessus ig ? (en mode quand on appellle reset cam, �a nous met !isZooming donc remet � position initiale avec les param�tres d'en haut 
    }

    public IEnumerator ZoomAndCenterCoroutine(Transform playerTransform) //choppe la coroutine et la pos et tt du player
    {
        isZooming = true;
        float zoomTimer = 0f;
        float startZoom = Camera.main.orthographicSize;

        var playerController = playerTransform.GetComponent<PlayerCharacter2D>();
        if (playerController != null)
        {
            playerController.SetCinematicMode(true); //truc cin�matique avec param�tres pour figer perso etc
        }

        while (zoomTimer < zoomDuration)
        {

            Camera.main.orthographicSize = Mathf.Lerp(initialZoom, targetZoom, zoomTimer / zoomDuration); //ce qui zoom doucement la cam
            transform.position = Vector3.Lerp(transform.position, //et centre ce zoom sur le joueur progressivement ausis
                new Vector3(playerTransform.position.x, playerTransform.position.y, -10f),zoomTimer / zoomDuration);
                

            zoomTimer += Time.deltaTime; //on met � j le temps de elapsed
            yield return null;
        }

        yield return new WaitForSeconds(deathAfterCamera);
        //-----------------jouer anim mort (+son ?) GetComponent<Animator>().Play("DeathAnim");
        // -----------------AudioSource.PlayClipAtPoint(deathSound, transform.position);
        Destroy(playerTransform.gameObject);
        Debug.Log("Mort apr�s zoom");

    }

    public IEnumerator DoorZoomIn(Transform doorTarget) //donc ici paf on zoom sur ouverture porte
    {
        isZooming = true;

        if (character != null)
        {
            character.SetCinematicMode(true); // Active le mode cin�matique
            character.playerRigidbody.velocity = Vector2.zero; //bloque la velocit� 
            character.GetComponent<Rigidbody2D>().velocity = Vector2.zero; // Stoppe tout mouvement
        }
        float elapsed = 0f;
        float startZoom = Camera.main.orthographicSize; //ofc on appelle donc la size (genre � quel point c'est zoom�) de base de la cam "start zoom" pour y revenir 
        Vector3 startPos = Camera.main.transform.position; //et �a c'est la position 
        Vector3 endPos = new Vector3(doorTarget.position.x, doorTarget.position.y + doorYOffset, -10f); //et �a c'est la target ici la porte pour genre la fin du zoom in


        while (elapsed < doorZoomDuration)
        {
           
            Camera.main.orthographicSize = Mathf.Lerp(startZoom, doorTargetZoom, elapsed / doorZoomDuration);
            Camera.main.transform.position = Vector3.Lerp(startPos, endPos, elapsed / doorZoomDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (character != null)
        {
            character.SetCinematicMode(false); // D�sactive le mode cin�matique
        }

        isZooming = false;
    }

    public IEnumerator DoorZoomOut() //et paf le dezoom
    { 
        float elapsed = 0f;
        float startZoom = Camera.main.orthographicSize;//ofc on appelle donc la size (genre � quel point c'est zoom�) de base de la cam "start zoom" pour y revenir 
        Vector3 playerPos = new Vector3(target.position.x, target.position.y + yOffset, -10f);
        Vector3 startPos = Camera.main.transform.position; //et �a c'est la position 
        Vector3 endPos = new Vector3(target.position.x, target.position.y + doorYOffset, -10f); //et �a c'est la target donc le perso pour la fin du zoom out


        while (elapsed < doorZoomDuration)
        {Camera.main.orthographicSize = Mathf.Lerp(startZoom, initialZoom, elapsed / doorZoomDuration);
        Camera.main.transform.position = Vector3.Lerp(startPos, endPos, elapsed / doorZoomDuration);
        elapsed += Time.deltaTime;
            
            yield return null;
        }
        
        character.enabled = true;

        isZooming = false;
    }


}


