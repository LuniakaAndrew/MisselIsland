using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Assets.Scripts;

public class Player : MonoBehaviour {
    public List<Building> listOfBuildings = new List<Building>();
    public List<Attack> listOfAttacks = new List<Attack>();
    public static Player player;
    public int buildPoints;
    public int powerPoints;
    public int attackPoints;
   
    private GameObject[,] map = new GameObject[100, 80];
    // Use this for initialization
    void Awake()
    {
        if (player == null)
        {
            player = this;
        }
    }
    void Start() {
        if (player == null)
        {
            player = this;
        }
    }
  
    // Update is called once per frame
    void Update () {
       

    }
    public void setMap(GameObject[,] map) {
        this.map = map;
    }
    public void DrawCell(Vector2 position, int type, CellStates state)
    {
        Vector2[] posArr=new Vector2[4];
        posArr[0] = position;
        listOfBuildings[type].Draw(posArr);
        for (int i = 0; i < posArr.Length; i++) {
            if (CellStates.ground.Equals(map[(int)posArr[i].x, (int)posArr[i].y].gameObject.GetComponent<HexagonClick>().getCellState()))
            {
                map[(int)posArr[i].x, (int)posArr[i].y].gameObject.GetComponent<HexagonClick>().setCellState(state);
            }
            else {
                map[(int)posArr[i].x, (int)posArr[i].y].gameObject.GetComponent<HexagonClick>().setCellState(CellStates.error);
            }
    
        }
    }
    public void CleanCell(Vector2 position, int type)
    {
        Vector2[] posArr = new Vector2[4];
        posArr[0] = position;
        listOfBuildings[type].Draw(posArr);
        
        for (int i = 0; i < posArr.Length; i++)
        {
            if (map[(int)posArr[i].x, (int)posArr[i].y].gameObject.name.Equals("Ground"))
            {
                map[(int)posArr[i].x, (int)posArr[i].y].gameObject.GetComponent<HexagonClick>().setCellState(CellStates.ground);
            }
            if (map[(int)posArr[i].x, (int)posArr[i].y].gameObject.name.Equals("Water"))
            {
                map[(int)posArr[i].x, (int)posArr[i].y].gameObject.GetComponent<HexagonClick>().setCellState(CellStates.water);
            }
        }
    }
    public bool IsOccupied(Vector2 position, int type)
    {
        Vector2[] posArr = new Vector2[4];
        posArr[0] = position;
        listOfBuildings[type].Draw(posArr);
        for (int i = 0; i < posArr.Length; i++)
        {
            if (CellStates.ground.Equals(map[(int)posArr[i].x, (int)posArr[i].y].gameObject.GetComponent<HexagonClick>().getCellState())||
                CellStates.water.Equals(map[(int)posArr[i].x, (int)posArr[i].y].gameObject.GetComponent<HexagonClick>().getCellState()))
            {
                return true;
            }
        }
        return false;
    }
}
