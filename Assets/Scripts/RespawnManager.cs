using System.Collections;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [Header("Références")]
    public Transform defaultRespawnPoint;
    public GameObject playerPrefab;
    public DeathHandler deathHandler;
    public CameraMovement cameraMovement;

    [Header("Timings")]
    public float respawnDelay = 2f;
    public float respawnAnimDuration = 1.5f;

    public GameObject CurrentPlayer { get; private set; }
    private Transform _currentRespawnPoint;

    private void Start()
    {
        _currentRespawnPoint = defaultRespawnPoint;
        if (playerPrefab == null) Debug.LogError("Player Prefab non assigné!");
        SpawnPlayer();
    }

    public void SetRespawnPoint(Transform newPoint) => _currentRespawnPoint = newPoint;

    public void TriggerRespawn() => StartCoroutine(RespawnSequence());



    private IEnumerator RespawnSequence()
    {
        yield return new WaitForSeconds(respawnDelay);

        // Réactiver la caméra AVANT le spawn
        cameraMovement.ResetCamera();
        cameraMovement.target = _currentRespawnPoint;

        yield return new WaitForSeconds(0.5f); // Laisser le temps à la caméra

        SpawnPlayer();

        // Attendre la fin du frame pour éviter les conflits
        yield return null;

        if (CurrentPlayer != null)
        {
            var player = CurrentPlayer.GetComponent<PlayerCharacter2D>();
            if (player != null)
            {
                player.SetCinematicMode(false);
                player.playerRigidbody.gravityScale = player.InitialGravityScale; // Reset gravity
            }
        }
    }

    private void SpawnPlayer()
    {
        if (CurrentPlayer != null) Destroy(CurrentPlayer);

        CurrentPlayer = Instantiate(playerPrefab, _currentRespawnPoint.position, Quaternion.identity);
        CurrentPlayer.SetActive(false);

        cameraMovement.target = _currentRespawnPoint;
        cameraMovement.ResetCamera();
        CurrentPlayer.SetActive(true);

        cameraMovement.target = CurrentPlayer.transform;
        deathHandler.playerTransform = CurrentPlayer.transform;

        var player = CurrentPlayer.GetComponent<PlayerCharacter2D>();
        player?.ResetPlayer();

        // Ne manipuler l'animator QUE si le Canvas est actif
        if (deathHandler.DeathCanvas != null && deathHandler.DeathCanvas.gameObject.activeSelf)
        {
            deathHandler.DeathUIAnimator?.Play("EmptyState");
        }
        else
        {
            Debug.Log("DeathCanvas désactivé - skip animation reset");
        }
    }
}