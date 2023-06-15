using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragUIItem :
  MonoBehaviour,
  IBeginDragHandler,
  IDragHandler,
  IEndDragHandler
{
  [SerializeField]
  GameObject PrefabToInstantiate;

  [SerializeField]
  RectTransform UIDragElement;

  [SerializeField]
  RectTransform Canvas;

  private Vector2 mOriginalLocalPointerPosition;
  private Vector3 mOriginalPanelLocalPosition;
  private Vector2 mOriginalPosition;

  private void Start()
  {
    mOriginalPosition = UIDragElement.localPosition;
  }

  public void OnBeginDrag(PointerEventData data)
  {
    mOriginalPanelLocalPosition = UIDragElement.localPosition;
    RectTransformUtility.ScreenPointToLocalPointInRectangle(
      Canvas,
      data.position,
      data.pressEventCamera,
      out mOriginalLocalPointerPosition);
  }

  public void OnDrag(PointerEventData data)
  {
    Vector2 localPointerPosition;
    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
      Canvas,
      data.position,
      data.pressEventCamera,
      out localPointerPosition))
    {
      Vector3 offsetToOriginal =
        localPointerPosition -
        mOriginalLocalPointerPosition;

      UIDragElement.localPosition =
        mOriginalPanelLocalPosition +
        offsetToOriginal;
    }
  }

  public IEnumerator Coroutine_MoveUIElement(
    RectTransform r,
    Vector2 targetPosition,
    float duration = 0.1f)
  {
    float elapsedTime = 0;
    Vector2 startingPos = r.localPosition;
    while (elapsedTime < duration)
    {
      r.localPosition =
        Vector2.Lerp(
          startingPos,
          targetPosition,
          (elapsedTime / duration));
      elapsedTime += Time.deltaTime;

      yield return new WaitForEndOfFrame();
    }
    r.localPosition = targetPosition;
  }

  public void OnEndDrag(PointerEventData eventData)
  {
    StartCoroutine(
      Coroutine_MoveUIElement(
        UIDragElement,
        mOriginalPosition,
        0.5f));

    RaycastHit hit;
    Ray ray = Camera.main.ScreenPointToRay(
      Input.mousePosition);

    if (Physics.Raycast(ray, out hit, 1000.0f))
    {
      Vector3 worldPoint = hit.point;
      Debug.Log(worldPoint);

      //Debug.Log(worldPoint);
      CreateObject(worldPoint);
    }
  }

  public void CreateObject(Vector3 position)
  {
    if (PrefabToInstantiate == null)
    {
      Debug.Log("No prefab to instantiate");
      return;
    }

    Vector3 pos = position;
    // Get the grid.
    RectGrid grid = App.Instance.mRectGridMap;
    Vector2Int index = grid.PosToIndex(pos);

    // Round the position to the nearest whole number
    //pos.x = Mathf.FloorToInt(pos.x);
    //pos.z = Mathf.FloorToInt(pos.z);
    pos.x = index.x * grid.mCellX;
    pos.z = index.y * grid.mCellY;
    pos.y = 0.0f;

    App.Instance.mRectGridMap.SetWalkable(false, index);

    GameObject obj = Instantiate(
      PrefabToInstantiate,
      pos,
      Quaternion.identity);
  }
}