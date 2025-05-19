using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDebug : MonoBehaviour
{
    [Header("Debug Settings")]
    public bool showMovementDebug = true;
    public bool showJumpDebug = true;
    public bool showGroundDebug = true;
    public bool showAnimatorDebug = true;

    private PlayerCharacter2D playerController;
    private Animator animator;
    private Rigidbody2D rb;

    private void Start()
    {
        playerController = GetComponent<PlayerCharacter2D>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        Debug.Log("=== Player Debug Initialized ===");
        Debug.Log($"PlayerController found: {playerController != null}");
        Debug.Log($"Animator found: {animator != null}");
        Debug.Log($"Rigidbody2D found: {rb != null}");
    }

    private void Update()
    {
        if (showMovementDebug)
        {
            Debug.Log($"Movement - Horizontal Input: {Input.GetAxisRaw("Horizontal")}");
            Debug.Log($"Movement - Current Velocity: X={rb.velocity.x.ToString("F2")}, Y={rb.velocity.y.ToString("F2")}");
            Debug.Log($"Movement - CanMove: {playerController.canMove}");
        }

        if (showJumpDebug)
        {
            Debug.Log($"Jump - Space Pressed: {Input.GetKeyDown(KeyCode.Space)}");
            Debug.Log($"Jump - IsGrounded: {animator.GetBool("isGrounded")}");
            Debug.Log($"Jump - Velocity Y: {rb.velocity.y.ToString("F2")}");
        }

        if (showGroundDebug)
        {
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.2f, Vector2.down, playerController.groundDistance, playerController.groundLayer);
            Debug.Log($"Ground - Raycast Hit: {hit.collider != null} ({hit.collider?.name})");
            Debug.Log($"Ground - Distance: {playerController.groundDistance}");
            Debug.Log($"Ground - Layer Mask: {playerController.groundLayer.value}");
        }

        if (showAnimatorDebug)
        {
            Debug.Log($"Animator - xVelocity: {animator.GetFloat("xVelocity").ToString("F2")}");
            Debug.Log($"Animator - yVelocity: {animator.GetFloat("yVelocity").ToString("F2")}");
            Debug.Log($"Animator - isGrounded: {animator.GetBool("isGrounded")}");
            Debug.Log($"Animator - Current State: {GetCurrentAnimatorState()}");
        }
    }

    private string GetCurrentAnimatorState()
    {
        if (animator == null) return "No Animator";

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName("Base Layer.Idle") ? "Idle" :
               stateInfo.IsName("Base Layer.Run") ? "Run" :
               stateInfo.IsName("Base Layer.Jump") ? "Jump" :
               stateInfo.IsName("Base Layer.Fall") ? "Fall" :
               "Unknown State";
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // Draw ground check
        Gizmos.color = playerController.isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.down * playerController.groundDistance, 0.2f);

        // Draw velocity
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)rb.velocity.normalized);
    }
}
