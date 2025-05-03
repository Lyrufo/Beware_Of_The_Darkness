using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents an object able to interact with <see cref="Lever"/> objects in the scene.
/// </summary>
public class LeverInteraction : MonoBehaviour
{

    [Tooltip("The maximum distance (in units) for the object to detect levers.")]
    public float radius;

    /// <summary>
    /// Called once per frame.
    /// </summary>
    void Update()
    {
        // We use the E key as the "activate" input.
        if (Input.GetKeyDown(KeyCode.E))
        {
            // We can use "overlap" functions to detect the levers in a given range. The OverlapSphere() detect all the colliders in a
            // radius, which is just a distance check to the physical objects. It returns an array of colliders, in other words, a list of
            // all the colliders found in the defined range.
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

            // For reach collider in that list...
            foreach (Collider collider in colliders)
            {
                // We try to get the lever component on the detected object.
                Lever leverComponent = collider.GetComponent<Lever>();

                // GetComponent() will return "null" if the expected component is not on the object. So here, this condition is valid only
                // if the object that has the current collider also have a Lever component.
                if (leverComponent != null)
                {
                    // If a Lever component has been found, activate it.
                    leverComponent.Activate();

                    // The "return" keyword stops the function, and so stops the loop. This means that even if there's more than 1 lever in
                    // the range, we will only the first detected one.
                    // This demo keeps things simple. But if our level design allow to have several levers close to each other, we could
                    // implement additional checks, like always trigger the closest one, or the one that is "the most visible", the one that
                    // is the most aligned with the player's rotation.
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Called when the scene is drawn in the editor. This function is never called in a build.
    /// </summary>
    private void OnDrawGizmos()
    {
        // Gizmos are "editor drawings", only visible in the editor, used to feedback things for the development team.
        // In our case, we use them to represent the detection range of the player to activate levers.
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}


