using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public void StartGame()
    {
        GameManager.Instance.StartGameplay();
        Debug.Log("Jeu démarré !");
    }

    public void QuitGame()
    {
        Debug.Log("Quitter le jeu");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
