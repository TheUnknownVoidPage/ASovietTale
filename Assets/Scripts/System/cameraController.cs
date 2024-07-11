using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    public Transform player;          // Assign the player's transform in the inspector
    public float smoothTime = 0.2f;   // Smoothing time, lower is quicker
    private Vector3 velocity = Vector3.zero;  // Reference velocity, which gets modified by the SmoothDamp function
    void Start()
    {
        Application.targetFrameRate = 60;  // Set a target frame rate if frame rate fluctuation is an issue
    }
    public void CameraUpdate()
    {
        if (player == null) return;

        // Desired camera position
        Vector3 targetPosition =  new Vector3(player.position.x, player.position.y, -10);  // Assuming you want to keep the camera 10 units back on the z-axis


        // Smoothly move the camera towards the target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime );
    }
    public static Vector3 SuperSmoothLerp(Vector3 followOld, Vector3 targetOld, Vector3 targetNew, float elapsedTime, float lerpAmount)
    {
        Vector3 f = followOld - targetOld + (targetNew - targetOld) / (lerpAmount * elapsedTime);
        return targetNew - (targetNew - targetOld) / (lerpAmount * elapsedTime) + f * Mathf.Exp(-lerpAmount * elapsedTime);
    }
}
