using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance; 

    public List<AllItems> _inventoryItems = new List<AllItems>(); //les objets actuellement en inventaire

    private void Awake()
    {
        Instance = this;
    }

    public void AddItem(AllItems item) //pour l'ajouter à l'inv
    {
        if(!_inventoryItems.Contains(item)) //on vérifie qu'on l'a pas déjà
        {
            _inventoryItems.Add(item);
        }
    }

    public void RemoveItem(AllItems item) //pour supprimer de l'inv
    {
        if (_inventoryItems.Contains(item)) //on vérifie qu'on l'a 
        {
            _inventoryItems.Remove(item);
        }
    }

    public enum AllItems //la liste des objets qu'on peut prendre dans tout le jeu 
    {
        Key1, //donc ici on peut faire une liste d'objets 
        Key2, 
        Key3,
    }
}
