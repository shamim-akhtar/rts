using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
  public float panSpeed = 20f; // Speed at which the camera pans
  public float zoomSpeed = 20f; // Speed at which the camera zooms
  public float minZoomDistance = 5f; // Minimum distance for zooming in
  public float maxZoomDistance = 50f; // Maximum distance for zooming out

  // Example boundaries (modify based on your game's needs)
  public float minX = -50f;
  public float maxX = 50f;
  public float minZ = -50f;
  public float maxZ = 50f;
  public float minY = 5f;
  public float maxY = 50f;

  private Vector3 lastMousePosition; // Last recorded mouse position during the drag

  void Update()
  {
    // Check for left mouse button down
    if (Input.GetMouseButtonDown(0))
    {
      // Perform a raycast at the mouse position
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      if (Physics.Raycast(ray, out RaycastHit hit))
      {
        // Get the intersected point with the terrain plane
        Vector3 terrainPoint = hit.point;

        // Print the position of the intersected point
        Debug.Log("Terrain Point: " + terrainPoint);

        // You can further process or use the terrain point as needed
      }
    }

    if (Input.GetMouseButtonDown(2))
    {
      // Store the current mouse position
      lastMousePosition = Input.mousePosition;
    }


    if (Input.GetMouseButton(2))
    {
      // Calculate the mouse movement delta
      Vector3 mouseDelta = Input.mousePosition - lastMousePosition;

      // Calculate the desired panning movement based on the mouse movement
      Vector3 panMovement = new Vector3(-mouseDelta.x, 0f, -mouseDelta.y) * panSpeed * Time.deltaTime;

      // Apply the panning movement to the camera's position
      transform.Translate(panMovement, Space.World);

      // Update the last mouse position
      lastMousePosition = Input.mousePosition;

      // Clamp the camera's position to the valid boundaries
      transform.position = ClampCameraPosition(transform.position);
    }

    // Check for mouse scroll wheel movement
    float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
    if (scrollWheel != 0f)
    {
      // Calculate the desired zoom movement based on the scroll wheel input
      Vector3 zoomMovement = transform.forward * scrollWheel * zoomSpeed * Time.deltaTime;

      // Apply the zoom movement to the camera's position
      transform.Translate(zoomMovement, Space.World);

      // Clamp the camera's position to the valid boundaries
      transform.position = ClampCameraPosition(transform.position);
    }
  }

  private Vector3 ClampCameraPosition(Vector3 position)
  {
    // Perform your own custom bounds checking logic here
    // Replace the following lines with your actual bounds checking code

    position.x = Mathf.Clamp(position.x, minX, maxX);
    position.y = Mathf.Clamp(position.y, minY, maxY);
    position.z = Mathf.Clamp(position.z, minZ, maxZ);

    return position;
  }
}
