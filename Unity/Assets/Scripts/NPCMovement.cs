using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class NPCMovement : MonoBehaviour
{
  public CharacterController characterController;
  public float speed = 1f;
  public Animator m_Animator = null;
  public float stopDistance = 0.5f;
  public float rotationSpeed = 10f;
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

  public void SetDestination(Vector3 goal, RectGrid grid, GMAI.PathFinder pathFinder = null)
  {
    Vector2Int destination = grid.PosToIndex(goal);
    Debug.Log("Index: " + destination);
    if (pathFinder != null)
    {
      // With pathfinding.
      // 1. If pathfinder is running dont do anything.
      // 2. while pathfinder is running
      // 2.a perform one iteration of the loop.
      // 2.b continue until a success or failure is returned.
      if (pathFinder.status != GMAI.PathFinderStatus.RUNNING)
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
    while (pathFinder.status == GMAI.PathFinderStatus.RUNNING)
    {
      pathFinder.Step();
      yield return null;
    }
    // completed pathfinding.

    if (pathFinder.status == GMAI.PathFinderStatus.FAILURE)
    {
      Debug.Log("Failed finding a path. No valid path exists");
    }
    if (pathFinder.status == GMAI.PathFinderStatus.SUCCESS)
    {
      // found a valid path.
      // accumulate all the locations by traversing from goal to the start.
      List<Vector2Int> reversePathLocations = new List<Vector2Int>();
      GMAI.PathFinderNode node = pathFinder.GetCurrentNode();
      while (node != null)
      {
        reversePathLocations.Add(node.location);
        node = node.parent;
      }
      // add all these points to the waypoints.
      for (int i = reversePathLocations.Count - 1; i >= 0; i--)
      {
        Vector3 pos = grid.IndexToPos(reversePathLocations[i]);
        AddWayPoint(pos);
      }
    }
  }

  IEnumerator Coroutine_MoveTo()
  {
    while (true)
    {
      while (wayPoints.Count > 0)
      {
        Vector3 pt = wayPoints.Dequeue();
        Debug.Log("Moving to point: " + pt);
        yield return StartCoroutine(Coroutine_Move_Anim(pt));
      }
      yield return null;
    }
  }

  private IEnumerator Coroutine_Move_Anim(Vector3 targetPosition)
  {
    while (Vector3.Distance(transform.position, targetPosition) > stopDistance)
    {
      Vector3 direction = targetPosition - transform.position;
      Quaternion targetRotation = Quaternion.LookRotation(direction);
      m_Animator.SetFloat("Speed", speed, speedInterpolationTime, Time.deltaTime);
      transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
      characterController.Move(transform.forward * Time.deltaTime);
      yield return null;
    }
    yield return StartCoroutine(Coroutine_Stop_Anim());
  }

  IEnumerator Coroutine_Stop_Anim()
  {
    float dt = 0.0f;
    while (dt <= speedInterpolationTime)
    {
      m_Animator.SetFloat("Speed", 0f, speedInterpolationTime, Time.deltaTime);
      dt += Time.deltaTime;
      yield return null;
    }
    m_Animator.SetFloat("Speed", 0f);
  }
}
