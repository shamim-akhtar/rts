using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class NPCMovement1 : MonoBehaviour
{
  public float speed = 1f;
  public Animator m_Animator = null;
  public float stopDistance = 0.5f;
  public float rotationSpeed = 10f;

  const float maxDirection = 1.0f;
  const float directionInterpolationTime = .5f;
  const float speedInterpolationTime = 0.25f;

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
    //Vector2Int destination = grid.PosToIndex(goal);
    //Debug.Log("Index: " + destination);
    //if (pathFinder!=null)
    //{
    //  // With pathfinding.
    //  // 1. If pathfinder is running dont do anything.
    //  // 2. while pathfinder is running
    //  // 2.a perform one iteration of the loop.
    //  // 2.b continue until a success or failure is returned.
    //  if(pathFinder.status != GMAI.PathFinderStatus.RUNNING)
    //  {
    //    // clear all the waypoints.
    //    wayPoints.Clear();
    //    Vector2Int start = grid.PosToIndex(transform.position);

    //    pathFinder.Init(start, destination);
    //    //grid.ResetCellColors();
    //    // Start a coroutine to do go to loop the pathfinding steps.
    //    StartCoroutine(Coroutine_PathFinding(pathFinder, grid));
    //  }
    //}
    //else
    //{
    //  // Without pathfinding.
    //  // we do not have pathfinding yet.
    //  // For now we just add the destination point to the waypoint queue.
    //  AddWayPoint(goal);
    //}
    AddWayPoint(goal);
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
        Vector3 pt = wayPoints.Dequeue();
        Debug.Log("Moving to point: " + pt);
        yield return StartCoroutine(Coroutine_MoveToPoint_Anim(pt, speed));
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

  ////float stopDistance = 0.5f;
  ////Vector3 endP = new Vector3(p.x, p.y, 0.0f);
  ////Vector2 startP = new Vector2(transform.position.x, transform.position.y);
  //Vector3 startP = transform.position;
  ////Transform target = WayPoints[m_WayPointIndex];
  //if (Vector3.Distance(p, transform.position) > stopDistance)
  //{
  //  m_Animator.SetFloat("Speed", speed, speedInterpolationTime, Time.deltaTime);
  //  Vector3 currentDir = transform.forward;
  //  Vector3 wantedDir = (p - transform.position);
  //  wantedDir = wantedDir.normalized;
  //  Vector3 cross = Vector3.Cross(currentDir, wantedDir);
  //  //m_Animator.SetFloat("Direction", cross.y * maxDirection, directionInterpolationTime, Time.deltaTime);
  //  Debug.Log("Moving");
  //  yield return null;
  //}
  ////yield return Coroutine_Stop_Anim();
  /// <summary>
  /// 
  /// </summary>
  /// <param name="p"></param>
  /// <param name="speed"></param>
  /// <returns></returns>
  IEnumerator Coroutine_MoveToPoint_Anim(Vector3 p, float speed)
  {

    if (Vector3.Distance(transform.position, p) > stopDistance)
    {
      // Calculate the direction from current position to target position
      Vector3 direction = (p - transform.position).normalized;

      // Rotate the NPC to face the movement direction
      Quaternion targetRotation = Quaternion.LookRotation(direction);
      transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

      // Move the NPC towards the target position
      transform.Translate(direction * speed * Time.deltaTime, Space.World);
      yield return null;
    }
    //m_Animator.SetFloat("Speed", 0f);
  }

  IEnumerator Coroutine_Stop_Anim()
  {
    float dt = 0.0f;
    while(dt <= speedInterpolationTime)
    {
      m_Animator.SetFloat("Speed", 0f, speedInterpolationTime, Time.deltaTime);
      dt += Time.deltaTime;
      Debug.Log("Stopping");
      yield return null;
    }
    m_Animator.SetFloat("Speed", 0f);
    Debug.Log("Stopped");
  }
}
