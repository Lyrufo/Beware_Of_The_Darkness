using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TimerDieAndRespawn timer;
    public PlayerCharacter2D player;
    public GameObject StartCanvas;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Démarrage manuel plus tard
        player.enabled = false;
        timer.enabled = false;
    }

    public void StartGameplay()
    {
        if (StartCanvas != null)
            StartCanvas.SetActive(false); // Cache le menu de démarrage

        player.enabled = true;
        timer.enabled = true;
        timer.StartTimer();
    }
}
