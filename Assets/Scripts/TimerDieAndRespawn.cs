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

        // Vérifie si le joueur existe ET est actif
        if (deathHandler.respawnManager._currentPlayer == null ||
            !deathHandler.respawnManager._currentPlayer.activeInHierarchy) return;

        var player = deathHandler.respawnManager._currentPlayer.GetComponent<PlayerCharacter2D>();
        if (player == null || !player.canMove) return;

        timer += Time.deltaTime;

        if (timer > maxtime)
        {
            deathHandler.HandleDeath(player.transform); // Passer la transform du joueur actuel
            enabled = false;
        }
    }
}
