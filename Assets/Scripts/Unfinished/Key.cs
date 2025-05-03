using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Represents an activable lever in the scene.
/// </summary>
public class Lever : MonoBehaviour
{

    [Tooltip("The object to rotate when the lever changes its state.")]
    public Transform leverPivot;

    [Tooltip("Called when this lever is activated.")]
    public UnityEvent onActivate;

    [Tooltip("Called when this lever is deactivated.")]
    public UnityEvent onDeactivate;

    /// <summary>
    /// Is the lever in activated position? Note that this variable is "private" because it's not supposed to be editable in the inspector.
    /// </summary>
    private bool isActive;

    /// <summary>
    /// Switches the state of this lever.
    /// </summary>
    public void Activate()
    {
        // The ! symbol is the "not" operator in C#. So here, we just assign the opposite value to isActive: if it value were false before
        // activating the lever, it becomes true, and reverse.
        isActive = !isActive;

        // If the lever is now active, trigger the "on activate" event.
        if (isActive)
        {
            onActivate?.Invoke();
            leverPivot.localRotation = Quaternion.AngleAxis(-50, Vector3.forward);
        }
        // Else (meaning "in any other case", here if the lever is not active), trigger the "on deactivate" event.
        else
        {
            onDeactivate?.Invoke();
            leverPivot.localRotation = Quaternion.AngleAxis(50, Vector3.forward);
        }
    }
}

