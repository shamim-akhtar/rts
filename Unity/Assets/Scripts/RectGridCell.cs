using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The objective of this script is to be able to change the 
// visualisation of the cell.
public class RectGridCell : MonoBehaviour
{
  [SerializeField]
  SpriteRenderer innerSprite;
  [SerializeField]
  SpriteRenderer outerSprite;

  public Vector2Int index = Vector2Int.zero;
  public bool isWalkable = true;
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  public void SetInnerColor(Color col)
  {
    innerSprite.color = col;
  }

  public void SetOuterColor(Color col)
  {
    outerSprite.color = col;
  }
}
