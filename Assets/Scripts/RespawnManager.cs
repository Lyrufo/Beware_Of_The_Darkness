using System.Collections;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [Header("R�f�rences")]
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
        if (playerPrefab == null) Debug.LogError("Player Prefab non assign�!");
        SpawnPlayer();
    }

    public void SetRespawnPoint(Transform newPoint) => _currentRespawnPoint = newPoint;

    public void TriggerRespawn() => StartCoroutine(RespawnSequence());

    private IEnumerator RespawnSequence()
    {
        yield return new WaitForSeconds(respawnDelay);

        // D�sactivation du Canvas de mort avant le respawn
        if (deathHandler.DeathCanvas != null)
            deathHandler.DeathCanvas.gameObject.SetActive(false);

        cameraMovement.target = _currentRespawnPoint;
        yield return new WaitForSeconds(0.5f);
        cameraMovement.ResetCamera();

        SpawnPlayer();

        yield return new WaitForSeconds(0.1f);
        CurrentPlayer.GetComponent<PlayerCharacter2D>().SetCinematicMode(false);
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
            Debug.Log("DeathCanvas d�sactiv� - skip animation reset");
        }
    }
}