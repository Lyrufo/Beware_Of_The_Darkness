using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("Point de respawn par défaut")]
    public Transform defaultRespawnPoint;

    [Tooltip("Prefab du joueur")]
    public GameObject playerPrefab;

    [Tooltip("Référence à DeathHandler")]
    public DeathHandler deathHandler;

    [Tooltip("Référence à CameraMovement")]
    public CameraMovement cameraMovement;

    [Header("Timings")]
    [Tooltip("Temps avant respawn après la mort")]
    public float respawnDelay = 2f;

    [Tooltip("Durée de l'animation de respawn")]
    public float respawnAnimDuration = 1.5f;

    public GameObject _currentPlayer;
    private Transform _currentRespawnPoint;

    private void Start()
    {
        _currentRespawnPoint = defaultRespawnPoint;
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
        // 1. Attendre le temps défini
        yield return new WaitForSeconds(respawnDelay);

        // 2. Jouer l'animation de transition
        if (deathHandler.deathUIAnimator != null)
        {
            deathHandler.deathUIAnimator.Play("RespawnTransitionAnim");
            yield return new WaitForSeconds(respawnAnimDuration);
        }

        // 3. Respawn le joueur
        SpawnPlayer();

        // 4. Réactiver les contrôles
        _currentPlayer.GetComponent<PlayerCharacter2D>().SetCinematicMode(false);

        // 5. Réinitialiser la caméra
        cameraMovement.ResetCamera();
    }

    private void SpawnPlayer()
    {
        if (_currentPlayer != null)
        {
            Destroy(_currentPlayer);
        }

        _currentPlayer = Instantiate(playerPrefab, _currentRespawnPoint.position, Quaternion.identity);
        cameraMovement.target = _currentPlayer.transform;
        deathHandler.playerTransform = _currentPlayer.transform;
    }
}

