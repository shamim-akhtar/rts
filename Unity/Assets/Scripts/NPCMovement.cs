using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
  public float speed = 1f;

  //  A queue of waypoints.
  public Queue<Vector3> wayPoints = new Queue<Vector3>();

  // Start is called before the first frame update
  void Start()
  {
    StartCoroutine(Coroutine_MoveTo());
  }

  // Update is called once per frame
  void Update()
  {

  }

  public void AddWayPoint(Vector3 point)
  {
    wayPoints.Enqueue(point);
  }

  public void SetDestination(Vector3 goal, RectGrid grid, GMAI.PathFinder pathFinder=null)
  {
    Vector2Int destination = grid.PosToIndex(goal);
    Debug.Log("Index: " + destination);
    if (pathFinder!=null)
    {
      // With pathfinding.
      // 1. If pathfinder is running dont do anything.
      // 2. while pathfinder is running
      // 2.a perform one iteration of the loop.
      // 2.b continue until a success or failure is returned.
      if(pathFinder.status != GMAI.PathFinderStatus.RUNNING)
      {
        // clear all the waypoints.
        wayPoints.Clear();
        Vector2Int start = grid.PosToIndex(transform.position);

        pathFinder.Init(start, destination);
        //grid.ResetCellColors();
        // Start a coroutine to do go to loop the pathfinding steps.
        StartCoroutine(Coroutine_PathFinding(pathFinder, grid));
      }
    }
    else
    {
      // Without pathfinding.
      // we do not have pathfinding yet.
      // For now we just add the destination point to the waypoint queue.
      AddWayPoint(goal);
    }
  }

  IEnumerator Coroutine_PathFinding(GMAI.PathFinder pathFinder, RectGrid grid)
  {
    while(pathFinder.status == GMAI.PathFinderStatus.RUNNING)
    {
      pathFinder.Step();
      yield return null;
    }
    // completed pathfinding.

    if(pathFinder.status == GMAI.PathFinderStatus.FAILURE)
    {
      Debug.Log("Failed finding a path. No valid path exists");
    }
    if(pathFinder.status == GMAI.PathFinderStatus.SUCCESS)
    {
      // found a valid path.
      // accumulate all the locations by traversing from goal to the start.
      List<Vector2Int> reversePathLocations = new List<Vector2Int>();
      GMAI.PathFinderNode node = pathFinder.GetCurrentNode();
      while(node != null)
      {
        reversePathLocations.Add(node.location);
        node = node.parent;
      }
      // add all these points to the waypoints.
      for(int i = reversePathLocations.Count-1; i >= 0; i--)
      {
        Vector3 pos = grid.IndexToPos(reversePathLocations[i]);
        AddWayPoint(pos);
      }
      //AddWayPoint(pathFinder.goal);
    }
  }

  IEnumerator Coroutine_MoveTo()
  {
    while(true)
    {
      while(wayPoints.Count > 0)
      {
        yield return StartCoroutine(Coroutine_MoveToPoint(wayPoints.Dequeue(), speed));
      }
      yield return null;
    }
  }

  IEnumerator Coroutine_MoveToPoint(Vector3 p, float speed)
  {
    //Vector3 endP = new Vector3(p.x, p.y, 0.0f);
    //Vector2 startP = new Vector2(transform.position.x, transform.position.y);
    Vector3 startP = transform.position;

    float distance = Vector3.Distance(p, startP);
    float totalTime = distance/speed;
    float elaspedTime = 0f;

    while(elaspedTime < totalTime)
    {
      float t = Mathf.Clamp01(elaspedTime/totalTime);
      Vector3 pos = Vector3.Lerp(startP, p, t);
      transform.position = pos;// new Vector3(pos.x, pos.y, transform.position.z);
      //Debug.Log(elaspedTime);
      elaspedTime += Time.deltaTime;
      yield return new WaitForEndOfFrame();
    }
    transform.position = p;// new Vector3(p.x, p.y, transform.position.z);
  }
}
