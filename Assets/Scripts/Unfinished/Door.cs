using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component used to represent a door in the scene, which can be opened or closed.
/// </summary>
public class Door : MonoBehaviour
{

    [Tooltip("The object to rotate when the door is open.")]
    public Transform doorPivot;

    [Tooltip("The collider to disable when the door is open.")]
    public Collider doorCollider;

    /// <summary>
    /// Opens this door, by rotating the door pivot.
    /// </summary>
    public void Open()
    {
        // In Unity, rotations are represented by Quaternions. The function AngleAxis() allow us to create a Quaternion that represents a
        // rotation of a given angle (in degrees) around a given axis. In our case, we want a rotation of 90 degrees around the Y (up) axis
        // to emulate an opened door.
        doorPivot.localRotation = Quaternion.AngleAxis(90, Vector3.up);
        // When the door is open, we can disable the collider that blocks the way
        doorCollider.enabled = false;
    }

    /// <summary>
    /// Opens this door, by resetting the door pivot's rotation.
    /// </summary>
    public void Close()
    {
        // Quaternion.identity is a "zero" rotation.
        doorPivot.localRotation = Quaternion.identity;
        // When the door is open, we can enable the collider that blocks the way
        doorCollider.enabled = true;
    }

}
