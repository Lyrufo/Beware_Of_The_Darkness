using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "InteractiveObjects", menuName = "Object")] //crée un raccourci dans les assets avec ce chemin pour avoir un objet avec ce script
public class InteractiveObjectsScriptable : ScriptableObject
{
    public string interactiveObjectName; // ici on stock toutes les données génériques qui vont se retrouver de partout juste avec des variable différentes
    public string description;
    public GameObject objectModel;
    public Sprite minimodel;
}
