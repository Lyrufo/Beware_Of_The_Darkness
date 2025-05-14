using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class CameraMovement : MonoBehaviour

{

    [Header("Zoom sur la Porte")] //groupe de paramètres de zoom spécifiques à l'ouverture de la porte qd la cam bouge

    [Tooltip("Zoom visé pour l'ouverture de porte")]
    public float doorTargetZoom = 5f;

    [Tooltip("Durée du zoom pour la porte")]
    public float doorZoomDuration = 1f;


    [Tooltip("Décalage Y pour le focus porte")]
    public float doorYOffset = 0.5f;


    [Header("Zoom sur la Mort")] //groupe de paramètres pour le zoom qd on meurt 

    [Tooltip("en gros la fenetre de zoom visée")]
    public float targetZoom = 3f;

    [Tooltip("durée du zoom en secondes")]
    public float zoomDuration = 2f;

    [Tooltip("le temps en seconde de pause avant que le perso soit détruit après la fin du zoom")]
    public float deathAfterCamera = 3f; //------------------ Go jouer anim du player ici en mode droite gauche ??? que se passe t-il oulala 


    [Header("Mouvements Cam Habituels")]

    [Tooltip("la hauteur de base la cam en +")]
    public float yOffset = 1f;

    [Tooltip("vitesse de suivi de la caméra")]
    public float FollowSpeed = 2f;

    [Tooltip("Truc que suit la cam (donc player)")]
    public Transform target;

    [Header("Perso")]
    [Tooltip("le perso dont je vais bloquer les controles")]
    public PlayerCharacter2D character;




    private float initialZoom; //ça c'est juste la fenetre de base 

    public bool isZooming = false;

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
        } //donc le retour à la pos de base avec la vitesse et tt de paramétré ? 
       
    }

    public void ResetCamera()
    {
       isZooming = false; //donc fais ref au truc d'au dessus ig ? (en mode quand on appellle reset cam, ça nous met !isZooming donc remet à position initiale avec les paramètres d'en haut 
    }

    public IEnumerator ZoomAndCenterCoroutine(Transform playerTransform) //choppe la coroutine et la pos et tt du player
    {
        isZooming = true;
        float zoomTimer = 0f;
        float startZoom = Camera.main.orthographicSize;

        var playerController = playerTransform.GetComponent<PlayerCharacter2D>();
        if (playerController != null)
        {
            playerController.SetCinematicMode(true); //truc cinématique avec paramètres pour figer perso etc
        }

        while (zoomTimer < zoomDuration)
        {

            Camera.main.orthographicSize = Mathf.Lerp(initialZoom, targetZoom, zoomTimer / zoomDuration); //ce qui zoom doucement la cam
            transform.position = Vector3.Lerp(transform.position, //et centre ce zoom sur le joueur progressivement ausis
                new Vector3(playerTransform.position.x, playerTransform.position.y, -10f),zoomTimer / zoomDuration);
                

            zoomTimer += Time.deltaTime; //on met à j le temps de elapsed
            yield return null;
        }

        yield return new WaitForSeconds(deathAfterCamera);
        //-----------------jouer anim mort (+son ?) GetComponent<Animator>().Play("DeathAnim");
        // -----------------AudioSource.PlayClipAtPoint(deathSound, transform.position);
        Destroy(playerTransform.gameObject);
        Debug.Log("Mort après zoom");

    }

    public IEnumerator DoorZoomIn(Transform doorTarget) //donc ici paf on zoom sur ouverture porte
    {
        isZooming = true;

        if (character != null)
        {
            character.SetCinematicMode(true); // Active le mode cinématique
            character.playerRigidbody.velocity = Vector2.zero; //bloque la velocité 
            character.GetComponent<Rigidbody2D>().velocity = Vector2.zero; // Stoppe tout mouvement
        }
        float elapsed = 0f;
        float startZoom = Camera.main.orthographicSize; //ofc on appelle donc la size (genre à quel point c'est zoomé) de base de la cam "start zoom" pour y revenir 
        Vector3 startPos = Camera.main.transform.position; //et ça c'est la position 
        Vector3 endPos = new Vector3(doorTarget.position.x, doorTarget.position.y + doorYOffset, -10f); //et ça c'est la target ici la porte pour genre la fin du zoom in


        while (elapsed < doorZoomDuration)
        {
           
            Camera.main.orthographicSize = Mathf.Lerp(startZoom, doorTargetZoom, elapsed / doorZoomDuration);
            Camera.main.transform.position = Vector3.Lerp(startPos, endPos, elapsed / doorZoomDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (character != null)
        {
            character.SetCinematicMode(false); // Désactive le mode cinématique
        }

        isZooming = false;
    }

    public IEnumerator DoorZoomOut() //et paf le dezoom
    { 
        float elapsed = 0f;
        float startZoom = Camera.main.orthographicSize;//ofc on appelle donc la size (genre à quel point c'est zoomé) de base de la cam "start zoom" pour y revenir 
        Vector3 playerPos = new Vector3(target.position.x, target.position.y + yOffset, -10f);
        Vector3 startPos = Camera.main.transform.position; //et ça c'est la position 
        Vector3 endPos = new Vector3(target.position.x, target.position.y + doorYOffset, -10f); //et ça c'est la target donc le perso pour la fin du zoom out


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


