using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Tooltip("vitesse de suivi de la caméra")]
    public float FollowSpeed = 2f;

    [Tooltip("Truc que suit la cam (donc player)")]
    public Transform target;

    [Tooltip("la hauteur de base la cam en +")]
    public float yOffset = 1f;

    private void Update()
    {
        Vector3 newPos = new Vector3(target.position.x, target.position.y+ yOffset, -10f);
        transform.position = Vector3.Slerp(transform.position, newPos, FollowSpeed*Time.deltaTime);
    }
}
