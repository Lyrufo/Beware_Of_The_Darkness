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
        if (!deathHandler ||
       !deathHandler.respawnManager ||
       !deathHandler.respawnManager.CurrentPlayer) return;

        var player = deathHandler.respawnManager.CurrentPlayer.GetComponent<PlayerCharacter2D>();
        if (player == null) return;

        if (player.canMove)
        {
            timer += Time.deltaTime;
            if (timer > maxtime)
            {
                deathHandler.HandleDeath(deathHandler.respawnManager.CurrentPlayer.transform);
                timer = 0f;
                enabled = false;
            }
        }
    }
}