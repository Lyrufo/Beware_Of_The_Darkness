using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [Header("R�f�rences")]
    [Tooltip("Point de respawn par d�faut")]
    public Transform defaultRespawnPoint;

    [Tooltip("Prefab du joueur")]
    public GameObject playerPrefab;

    [Tooltip("R�f�rence � DeathHandler")]
    public DeathHandler deathHandler;

    [Tooltip("R�f�rence � CameraMovement")]
    public CameraMovement cameraMovement;

    [Header("Timings")]
    [Tooltip("Temps avant respawn apr�s la mort")]
    public float respawnDelay = 2f;

    [Tooltip("Dur�e de l'animation de respawn")]
    public float respawnAnimDuration = 1.5f;

    public GameObject CurrentPlayer { get; private set; }
    public Transform _currentRespawnPoint;

    private void Start()
    {
        _currentRespawnPoint = defaultRespawnPoint;
        if (playerPrefab == null)
        {
            Debug.LogError("Player Prefab non assign� dans RespawnManager!");
            return;
        }
        SpawnPlayer();
    }

    public void SetRespawnPoint(Transform newPoint)
    {
        _currentRespawnPoint = newPoint;
    }

    public void TriggerRespawn()
    {
        StartCoroutine(RespawnSequence());
    }

    private IEnumerator RespawnSequence()
    {
        // 1. Attendre le temps d�fini
        yield return new WaitForSeconds(respawnDelay);

        // 2. Jouer l'animation de transition
        if (deathHandler.deathUIAnimator != null)
        {
            deathHandler.deathUIAnimator.Play("RespawnTransitionAnim");
            yield return new WaitForSeconds(respawnAnimDuration);
        }

        // 3. Respawn le joueur
        SpawnPlayer();

        // 4. R�activer les contr�les
        CurrentPlayer.GetComponent<PlayerCharacter2D>().SetCinematicMode(false);

        // 5. R�initialiser la cam�ra
        cameraMovement.ResetCamera();
    }

    private void SpawnPlayer()
    {
        if (CurrentPlayer != null) Destroy(CurrentPlayer);

        CurrentPlayer = Instantiate(playerPrefab, _currentRespawnPoint.position, Quaternion.identity);


        // Force l'assignation des r�f�rences
        if (cameraMovement != null) cameraMovement.target = CurrentPlayer.transform;
        if (deathHandler != null) deathHandler.playerTransform = CurrentPlayer.transform;

        // R�initialise le composant
        var player = CurrentPlayer.GetComponent<PlayerCharacter2D>();
        if (player != null)
        {
            player.ResetPlayer(); // R�active le player + anim
        }
    }


}

