using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TimerDieAndRespawn : MonoBehaviour
{
    //ok au final pas respawn sur ce script mais peur de tout foutre en l'air si je renomme pas tout bien
    public float timer;
    public float maxtime;

    public Transform playerTransform;

    public DeathHandler deathHandler;

    public void ResetTimer()
    {
        timer = 0f;
        enabled = true;
    }

    void Update()
    {
        if (deathHandler == null || deathHandler.respawnManager == null) return;

        if (deathHandler.respawnManager._currentPlayer == null) return;

        var player = deathHandler.respawnManager._currentPlayer.GetComponent<PlayerCharacter2D>();
        if (!player.canMove) return;

        timer += Time.deltaTime;

        if (timer > maxtime)
        {
            // Utilise la transform du joueur courant
            deathHandler.HandleDeath(deathHandler.respawnManager._currentPlayer.transform);
            enabled = false;
        }
    }
}
