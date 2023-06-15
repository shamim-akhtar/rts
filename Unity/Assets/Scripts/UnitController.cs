using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
  public NPCMovement npc;

  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    // Check for left mouse button down
    if (Input.GetMouseButtonDown(1))
    {
      // Perform a raycast at the mouse position
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      if (Physics.Raycast(ray, out RaycastHit hit))
      {
        // Get the intersected point with the terrain plane
        Vector3 terrainPoint = hit.point;

        // Print the position of the intersected point
        //Debug.Log("Terrain Point: " + terrainPoint);

        // You can further process or use the terrain point as needed
        //npc.AddWayPoint(terrainPoint);
        npc.SetDestination(terrainPoint, App.Instance.mRectGridMap, App.Instance.mRectGridMap.pathFinder);
      }
    }

  }
}
