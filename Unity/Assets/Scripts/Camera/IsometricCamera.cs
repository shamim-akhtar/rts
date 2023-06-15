using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsometricCamera : MonoBehaviour
{
  public float height = 10f; // Camera height above the scene
  public float angle = 45f; // Camera angle in degrees

  void Start()
  {
    // Set the camera's position and rotation
    transform.position = new Vector3(0f, height, 0f);
    transform.rotation = Quaternion.Euler(angle, 0f, 0f);

    // Enable orthographic camera mode
    //Camera.main.orthographic = true;

    // Adjust the orthographic size based on the screen height
    float screenHeight = Screen.height;
    Camera.main.orthographicSize = screenHeight / (2f * height);
  }
}