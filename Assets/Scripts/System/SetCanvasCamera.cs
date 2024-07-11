using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCanvasCamera : MonoBehaviour
{
    void Start()
    {
        // Set the main camera as the render camera for the Canvas
        var canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.worldCamera = Camera.main;
            canvas.planeDistance = 1; // You can adjust this value as needed
        }
    }
}

