using UnityEngine;

public class TimerDieAndRespawn : MonoBehaviour
{
    public float timer;
    public float maxtime;
    public DeathHandler deathHandler;

    public void ResetTimer()
    {
        timer = 0f;
        enabled = true;
    }

    void Update()
    {
        if (!deathHandler || !deathHandler.respawnManager.CurrentPlayer) return;

        var player = deathHandler.respawnManager.CurrentPlayer.GetComponent<PlayerCharacter2D>();
        if (player.canMove && (timer += Time.deltaTime) > maxtime)
        {
            deathHandler.HandleDeath(deathHandler.respawnManager.CurrentPlayer.transform);
            enabled = false;
        }
    }
}