using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using KingdomData;

public class WindowManager : MonoBehaviour {

    public string nameWindow;
    public Text header;
    public List<GameObject> hiddenHud;
    public List<GameObject> gridObjects;
    public GridLayoutGroup grid;
    public Vector2 cellSize;
    public int columnCount;
    public bool isMinimalize;

    private RectTransform rectTranformGrid;
    private Vector3 startPosition;
    private Vector2 startSize;

    void Start () {

        if (grid != null)
        {
            rectTranformGrid = grid.GetComponent<RectTransform>();
            startPosition = rectTranformGrid.position;
            startSize = rectTranformGrid.sizeDelta;

            ChangeGrid();
        }
        if (header == null)
            header = transform.Find("Header").GetComponent<Text>();
    }

	void Update () {
        if (header != null && header.text != nameWindow)
            header.text = nameWindow;
	}

    public void ChangeSize()
    {

    }
    public void ChangeGrid()
    {
        if (grid == null)
            return;

        grid.cellSize = cellSize;

        if (columnCount == 0)
            columnCount = 1;

        grid.constraintCount = columnCount;
       
        rectTranformGrid.sizeDelta = new Vector3(grid.cellSize.x * columnCount + grid.spacing.x * (columnCount-1), 0);
    }
    public void AddToGrid(GameObject obj)
    {
        if (grid == null)
            return;

        if (rectTranformGrid == null)
            rectTranformGrid = grid.GetComponent<RectTransform>();

        float layer = (float)gridObjects.Count / (float)grid.constraintCount;

        obj.transform.SetParent(grid.transform);
        gridObjects.Add(obj);

        float layerAfter = (float)gridObjects.Count / (float)grid.constraintCount;
        Debug.Log("Add to grid (Parent) " + obj.transform.parent.name);
        if ((layer % 1) == 0 && (layerAfter%1)> 0)
        {
            rectTranformGrid.sizeDelta = new Vector3(rectTranformGrid.sizeDelta.x, rectTranformGrid.sizeDelta.y + grid.cellSize.y + grid.spacing.y);
            if(gridObjects.Count > 1)
                grid.transform.position -= new Vector3(0, (grid.cellSize.y+grid.spacing.y) / 2, 0);
        }
    }
    public void ClearGrid()
    {
        foreach (GameObject coor in gridObjects)
            if (coor != null)
                Destroy(coor);
    }
    public void ResetGrid()
    {
        ClearGrid();
        rectTranformGrid.position = startPosition;
        rectTranformGrid.sizeDelta = startSize;
    }
    public void MinimizeAndMaximilizeWindow()
    {
        isMinimalize = !isMinimalize;
        if (isMinimalize)
            MinimizeWindow();
        else
            MaximilizeWindow();

    }
    public void MinimizeWindow()
    {
        foreach (GameObject obj in hiddenHud)
            if (obj != null)
                obj.SetActive(false);
    }
    public void MaximilizeWindow()
    {
        foreach (GameObject obj in hiddenHud)
            if (obj != null)
                obj.SetActive(true);
    }
    public void CloseWindow()
    {
        Destroy(gameObject);
    }
}
