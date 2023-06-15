using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Rendering;

namespace GMAI
{
  public enum PathFinderStatus
  {
    NOT_INITIALISED = 0,
    SUCCESS,
    FAILURE,
    RUNNING,
  }

  public class PathFinderNode
  {
    public PathFinderNode parent = null;
    public Vector2Int location;
    private float gcost;
    private float hcost;
    private float fcost;

    public PathFinderNode(PathFinderNode parent, Vector2Int location, float gcost, float hcost)
    {
      this.parent = parent;
      this.location = location;
      this.gcost = gcost;
      this.hcost = hcost;
      SetGCost(gcost);
    }

    public void SetGCost(float gcost)
    {
      this.gcost = gcost;
      fcost = gcost + hcost;
    }

    public float GetFCost() 
    { 
      return fcost;
    }
    public float GetGCost()
    {
      return gcost;
    }
  }

  public class PathFinder
  {
    public PathFinderStatus status = PathFinderStatus.NOT_INITIALISED;
    public Vector2Int start;
    public Vector2Int goal;

    public delegate float CostFunction(Vector2Int a, Vector2Int b);
    public CostFunction heuristicCost; // H cost
    public CostFunction traversalCost; // G cost.

    public delegate List<Vector2Int> GetNeighbours(Vector2Int index);
    public GetNeighbours getNeighbours;

    PathFinderNode currentNode = null;

    // OpenList and Closed list.
    List<PathFinderNode> openList = new List<PathFinderNode>();
    List<PathFinderNode> closedList = new List<PathFinderNode>();

    public PathFinderNode GetCurrentNode()
    {
      return currentNode;
    }

    // Helper functions.
    protected PathFinderNode GetLeastCostNode(List<PathFinderNode> myList)
    {
      int index = 0;
      float least_cost = myList[0].GetFCost();

      for(int i = 1; i < myList.Count; i++)
      {
        if(least_cost > myList[i].GetFCost())
        {
          least_cost= myList[i].GetFCost();
          index = i;
        }
      }
      return myList[index];
    }

    protected int IsInList(Vector2Int location, List<PathFinderNode> myList)
    {
      for(int i = 0; i < myList.Count; ++i)
      {
        if(location.x == myList[i].location.x && location.y == myList[i].location.y)
          return i;
      }
      return -1;
    }

    public PathFinder()
    {

    }

    public bool Init(Vector2Int start, Vector2Int goal)
    {
      // First we check is the path finder is already running.
      // If so then we do not initialise a new pathfinding.
      if(status == PathFinderStatus.RUNNING)
      {
        Debug.Log("Pathfinder already running. Cannot initialise.");
        return false;
      }

      Reset();

      this.start = start;
      this.goal = goal;

      // First calculate the heuristic cost from the start to the end.
      // We do so by calling the delegate to calculate the heuristic cost.
      float hcost = heuristicCost(start, goal);
      float gcost = 0f; // There is no traversal cost yet.

      // Create the root node of the traversal tree.
      PathFinderNode rootNode = new PathFinderNode(null, start, gcost, hcost);
      currentNode= rootNode;
      openList.Add(rootNode);

      status= PathFinderStatus.RUNNING;

      return true;
    }


    public PathFinderStatus Step()
    {
      closedList.Add(currentNode);
      if(openList.Count == 0)
      {
        // There are no more nodes to search. We have exhausted our search.
        status = PathFinderStatus.FAILURE;
        return status;
      }

      // Get the lowest cost node to traverse and make it the current node.
      currentNode = GetLeastCostNode(openList);
      // now remove the node from the list because we are already exploring it now.
      openList.Remove(currentNode);

      // check if the location of this node is same as the goal.
      if(currentNode.location.x == goal.x && currentNode.location.y == goal.y)
      {
        // we have found our destination.
        status = PathFinderStatus.SUCCESS;
        return status;
      }
      // we have not found the goal yet. Need to traverse this node.
      // get the neighbours of the location of the current node.
      List<Vector2Int> neighbours = getNeighbours(currentNode.location);
      foreach(Vector2Int neighbour in neighbours)
      {
        if(IsInList(neighbour, closedList) == -1)
        {
          // The cell does not exist in the closed list.
          // So lets calculate the cost of the traversal
          // from the current node to this neighbour node.
          // Also remember that the traversal cost is the cost of traversal
          // from the start to this neighbour node. We will will need to 
          // get the gcost from the currentNode and add the new traversal cost from
          // currentNode to this neighbour node.
          float gcost = currentNode.GetGCost() + traversalCost(currentNode.location, neighbour);
          float hcost = heuristicCost(neighbour, goal);

          // Now check if the node is already in the openlist.
          int idOpenList = IsInList(neighbour, openList);
          if(idOpenList == -1)
          {
            // its not in openlist.
            // so we will create a new PathFinderNode and add it to the openList so that
            // we can explore it further when needed.
            PathFinderNode node = new PathFinderNode(currentNode, neighbour, gcost, hcost);
            openList.Add(node);
          }
          else
          {
            // there is already a node in the openList with the same location as the neighbour.
            // So we will just update the cost with the new gcost is the current gcost is
            // lesser than the one that is already in the open list.
            float oldGCost = openList[idOpenList].GetGCost();
            if(gcost < oldGCost)
            {
              // set the parent to currentNode because this branch is better.
              openList[idOpenList].parent = currentNode;
              openList[idOpenList].SetGCost(gcost);
            }
          }
        }
      }

      return status;
    }

    void Reset()
    {
      currentNode= null;
      openList.Clear();
      closedList.Clear();
      status = PathFinderStatus.NOT_INITIALISED;
    }
  }
}
