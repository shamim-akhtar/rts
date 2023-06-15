using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The objective of this script is to be able to change the 
// visualisation of the cell.
public class RectGridCell : MonoBehaviour
{
  [SerializeField]
  GameObject walkableObj;
  [SerializeField]
  GameObject nonWalkableObj;

  public Vector2Int index = Vector2Int.zero;
  public bool isWalkable = true;
  // Start is called before the first frame update
  void Start()
  {
    SetWalkable(isWalkable);
  }

  // Update is called once per frame
  void Update()
  {

  }

  public void SetWalkable(bool flag)
  {
    if(flag)
    {
      walkableObj.SetActive(true);
      nonWalkableObj.SetActive(false);
    }
    else
    {
      walkableObj.SetActive(false);
      nonWalkableObj.SetActive(true);
    }
  }
}
