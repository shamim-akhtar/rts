using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

public class Cell
{
  public Vector2Int index;
  public int weight = 1;// int.MaxValue;

  public Cell(Vector2Int ind)
  {
    index = ind;
  }

  public bool IsWalkable()
  {
    return weight != int.MaxValue;
  }
}

public class RectGrid : MonoBehaviour
{
  public int mX = 20; // maximum number of columns
  public int mY = 20; // maximum number of rows.
  public int mCellX = 1;
  public int mCellY = 1;

  public Vector2Int startPt = Vector2Int.zero;

  //public GameObject rectGridCellPrefab;

  //GameObject[,] cells = null;
  Cell[,] cells = null;

  //public Color COLOR_WALKABLE = Color.cyan;
  //public Color COLOR_NON_WALKABLE = Color.black;
  //public Color COLOR_PATH = Color.green;

  public bool noDiagonalMovement = false;

  public Vector3 IndexToPos(Vector2Int index)
  {
    Vector3 pos = new Vector3(index.x * mCellX - startPt.x, 0.0f, index.y * mCellY - startPt.y);
    Debug.Log("Position: " + pos.ToString());
    return pos;
  }

  public Vector2Int PosToIndex(Vector3 pos)
  {
    int xIndex = Mathf.FloorToInt((pos.x - startPt.x) / mCellX);
    int yIndex = Mathf.FloorToInt((pos.z - startPt.y) / mCellY);

    Debug.Log("Index: " + xIndex + ", " + yIndex);
    return new Vector2Int(xIndex, yIndex);
  }


  // The NPC
  //public NPCMovement npc;
  public GMAI.PathFinder pathFinder = new GMAI.PathFinder();

  // Start is called before the first frame update
  void Start()
  {
    //cells = new GameObject[mX, mY];
    cells = new Cell[mY, mY];
    for(int i = 0; i < mX; ++i)
    {
      for(int j = 0; j < mY; ++j)
      {
        Vector2Int index = new Vector2Int(i, j);
        cells[i, j] = new Cell(index);
      }
    }

    pathFinder.heuristicCost = ManhattanCost;
    pathFinder.traversalCost = EuclideanCost;
    pathFinder.getNeighbours = GetNeighbours;
  }

  public void SetWalkable(bool flag, Vector3 pos)
  {
    Vector2Int index = PosToIndex(pos);
    SetWalkable(flag, index);
  }

  public void SetWalkable(bool flag, Vector2Int index)
  {
    if(flag)
    {
      cells[index.x, index.y].weight = 1;
    }
    else
    {
      cells[index.x, index.y].weight = int.MaxValue;
    }
  }

  // Update is called once per frame
  void Update()
  {
  }

  // Delegate implementation
  public float ManhattanCost(Vector2Int a, Vector2Int b)
  {
    return Mathf.Abs(a.x-b.x) * mCellX + Mathf.Abs(a.y-b.y) * mCellY;
  }
  public float EuclideanCost(Vector2Int a, Vector2Int b)
  {
    Vector2Int x = a;
    Vector2Int y = b;

    x.x = a.x * mCellX;
    x.y = a.y * mCellY;
    y.x = b.x * mCellX;
    y.y = b.y * mCellY;
    return Vector2Int.Distance(x, y);
  }

  public List<Vector2Int> GetNeighbours(Vector2Int a)
  {
    List<Vector2Int> neighbours = new List<Vector2Int>();

    int x = a.x;
    int y = a.y;

    // Check up
    if(y < mY - 1)
    {
      int i = x;
      int j = y + 1;

      //RectGridCell gridCell = cells[i, j].GetComponent<RectGridCell>();
      Cell cell = cells[i, j];
      if (cell.IsWalkable())
      {
        // add to the list of neighbours.
        neighbours.Add(cell.index);
      }
    }
    if (!noDiagonalMovement)
    {
      // Check top-right
      if (y < mY - 1 && x < mX - 1)
      {
        int i = x + 1;
        int j = y + 1;

        //RectGridCell gridCell = cells[i, j].GetComponent<RectGridCell>();
        Cell cell = cells[i, j];
        if (cell.IsWalkable())
        {
          // add to the list of neighbours.
          neighbours.Add(cell.index);
        }
      }
    }
    // Check right
    if (x < mX - 1)
    {
      int i = x + 1;
      int j = y;

      //RectGridCell gridCell = cells[i, j].GetComponent<RectGridCell>();
      Cell cell = cells[i, j];
      if (cell.IsWalkable())
      {
        // add to the list of neighbours.
        neighbours.Add(cell.index);
      }
    }
    if (!noDiagonalMovement)
    {
      // Check right-down
      if (x < mX - 1 && y > 0)
      {
        int i = x + 1;
        int j = y - 1;

        //RectGridCell gridCell = cells[i, j].GetComponent<RectGridCell>();
        Cell cell = cells[i, j];
        if (cell.IsWalkable())
        {
          // add to the list of neighbours.
          neighbours.Add(cell.index);
        }
      }
    }
    // Check down
    if ( y > 0)
    {
      int i = x;
      int j = y - 1;

      //RectGridCell gridCell = cells[i, j].GetComponent<RectGridCell>();
      Cell cell = cells[i, j];
      if (cell.IsWalkable())
      {
        // add to the list of neighbours.
        neighbours.Add(cell.index);
      }
    }
    if (!noDiagonalMovement)
    {
      // Check down-left
      if (y > 0 && x > 0)
      {
        int i = x - 1;
        int j = y - 1;

        //RectGridCell gridCell = cells[i, j].GetComponent<RectGridCell>();
        Cell cell = cells[i, j];
        if (cell.IsWalkable())
        {
          // add to the list of neighbours.
          neighbours.Add(cell.index);
        }
      }
    }
    // Check left
    if (x > 0)
    {
      int i = x - 1;
      int j = y;

      //RectGridCell gridCell = cells[i, j].GetComponent<RectGridCell>();
      Cell cell = cells[i, j];
      if (cell.IsWalkable())
      {
        // add to the list of neighbours.
        neighbours.Add(cell.index);
      }
    }
    if (!noDiagonalMovement)
    {
      // Check left-top
      if (x > 0 && y < mY - 1)
      {
        int i = x - 1;
        int j = y + 1;

        //RectGridCell gridCell = cells[i, j].GetComponent<RectGridCell>();
        Cell cell = cells[i, j];
        if (cell.IsWalkable())
        {
          // add to the list of neighbours.
          neighbours.Add(cell.index);
        }
      }
    }
    return neighbours;
  }
}
