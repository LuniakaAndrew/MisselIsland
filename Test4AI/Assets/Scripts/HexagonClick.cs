using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class HexagonClick : MonoBehaviour {

    private Cell cell = new Cell();
    private bool changeState = false;
    public void setCell(CellStates cState, float x, float y, int id) {
        cell.cellState = cState;
        cell.x = x;
        cell.y = y;
        cell.id = id;
    }
    public void setCellState(CellStates cState)
    {
        cell.cellState = cState;
        changeState = true;
    }
    public Vector2 getCellPosition()
    {
        return new Vector2(cell.x,cell.y);
    }
    public CellStates getCellState()
    {
        return cell.cellState;
    }
    // Use this for initialization
    void Start () {
     
	}
	
	// Update is called once per frame
	void Update () {
        if (changeState) {
            if (cell.cellState.Equals(CellStates.building))
            {
                Sprite s = Resources.Load<Sprite>("Sprites/cellBuildStart");
                if (s != null)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = s;
                    Debug.Log("Click " + s.name);
                }

                changeState = false;
            }
            if (cell.cellState.Equals(CellStates.error))
            {
                Sprite s = Resources.Load<Sprite>("Sprites/cellSelectError");
                if (s != null)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = s;
                    Debug.Log("Click " + s.name);
                }

                changeState = false;
            }
            if (cell.cellState.Equals(CellStates.prepare))
            {
                Sprite s = Resources.Load<Sprite>("Sprites/cellSelectOk");
                if (s != null)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = s;
                    Debug.Log("Click " + s.name);
                }

                changeState = false;
            }
            if (cell.cellState.Equals(CellStates.ground))
            {
                Sprite s = Resources.Load<Sprite>("Sprites/island");
                if (s != null)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = s;
                    Debug.Log("Click " + s.name);
                }

                changeState = false;
            }
            if (cell.cellState.Equals(CellStates.water))
            {
                Sprite s = Resources.Load<Sprite>("Sprites/sea");
                if (s != null)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = s;
                    Debug.Log("Click " + s.name);
                }

                changeState = false;
            }
        }
        
    }
}
