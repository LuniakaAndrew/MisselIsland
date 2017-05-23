using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System;

public class MapLoader : MonoBehaviour {

    public int mapSizesW = 20;     // Высота и ширина всей карты в ячейках
    public int mapSizesH = 10;
   
    float sectionSizeW = 0.43f;     // Высота и ширина ячеек в px
    float sectionSizeH = 0.5f;
    float sectionTopOffset = 0.25f;			// Смещение по Y каждого второго столбца
    bool sectionOffsetEverySecond = true;	// Смещение каждый второй столбик
    public static List<int[]> mapSectionsInfo = new List<int[]>();
    public static MapLoader a;
    GameObject block;
    float startPosX;
    float startPosY;
    string objName = "Hexagon";
    int id=0;
    public GameObject[,] grid = new GameObject[100, 80];
    void LoadMap(string filename) {

        XmlReader reader = new XmlTextReader(filename);
        int[] cellinfo = new int[5];
        int e = 0;
        int player=0;
        List<int[]> readMapPlayer = new List<int[]>();
        List<int[]> readMapEnemy = new List<int[]>();
        while (reader.Read())
        {
            switch (reader.NodeType)
            {
                case XmlNodeType.Element:
                    if (reader.Name=="level")
                    {
                        reader.MoveToAttribute("size_x");
                        mapSizesW = int.Parse(reader.Value);
                        reader.MoveToAttribute("size_y");
                        mapSizesH = int.Parse(reader.Value);
                    }
                    if (reader.Name == "map") {
                        reader.MoveToAttribute("player");
                        player=int.Parse(reader.Value);
                    }
                    if (reader.Name=="cell")
                        {
                            cellinfo = new int[5];
                            while (reader.MoveToNextAttribute())
                            {
                                
                                cellinfo[e] = int.Parse(reader.Value);
                                e++;
                            }
                            e = 0;
                     if (player == 1) { readMapPlayer.Add(cellinfo); }
                     if (player == 2) { readMapEnemy.Add(cellinfo); }

                    }
                     break;
             }
        }
        
        for (int i = 0; i <mapSizesW*2; i++)
        {
            for (int j = 0; j <mapSizesH; j++)
            {
                    mapSectionsInfo.Add(new int[] {0,0,0,0,0});
            }
        }
        
        for(int i=0;i<readMapPlayer.Count;i++)
        {
            mapSectionsInfo[((int)readMapPlayer[i].GetValue(0) * mapSizesH + (int)readMapPlayer[i].GetValue(1))] = readMapPlayer[i];
        }
        for (int i = 0; i < readMapEnemy.Count; i++)
        {
            mapSectionsInfo[(((int)readMapEnemy[i].GetValue(0) + mapSizesW)* mapSizesH + (int)readMapEnemy[i].GetValue(1))] = readMapEnemy[i];
        }
        readMapEnemy.Clear();
        readMapPlayer.Clear();
        reader.Close();
    }
    public void DrawMap() {
        //  Генератор карты
        // Геренируем все ячейки сначало, как воду
        LoadMap("../Test2/Assets/Resources/test_lvl.xml");
        
        startPosX = -10f;
        startPosY = 5.5f;
        float posSectionTop = 0;
        int [] temp = new int[5];
        for (int x = 0; x < mapSizesW*2; x++)    // Для каждого ряда
        {
            for (int y = 0; y < mapSizesH; y++)
            {
                // Если отступ делаем на каждой второй или первой колонке в зав-ти от заданного
                if ((!sectionOffsetEverySecond && x % 2 == 0)||(sectionOffsetEverySecond && x % 2 == 1))
                {
                    posSectionTop = sectionTopOffset;
                }
                else
                {
                    posSectionTop = 0;
                }
                // Тип местности
                 temp = mapSectionsInfo[x*mapSizesH+y];//одномерный массив в двумерный
                // Итоговый рендер
                // (Добавляется скрипт С#)
               
                id++;
                switch (temp[2])
                    {
                        case 0:
                        block = Resources.Load<GameObject>("Prefab/Water") as GameObject;
                        objName = "Water";
                        grid[x, y] = Instantiate(block, new Vector2((x * sectionSizeW) + startPosX, -1 * ((y * sectionSizeH) + posSectionTop) + startPosY), Quaternion.identity) as GameObject;
                        grid[x, y].name = objName; 
                        grid[x, y].transform.parent = transform;
                        grid[x, y].AddComponent<HexagonClick>();
                        grid[x, y].GetComponent<HexagonClick>().setCell(Assets.Scripts.CellStates.water, x, y, id);
                        //grid[x, y].GetComponent<HexagonClick>().numberOfCell = id;
                        //grid[x, y].GetComponent<HexagonClick>().x = x;
                        //grid[x, y].GetComponent<HexagonClick>().y = y;
                        break;
                        case 9999:
                        block = Resources.Load<GameObject>("Prefab/Ground") as GameObject;
                        objName = "Ground";
                        grid[x, y] = Instantiate(block, new Vector2((x * sectionSizeW) + startPosX, -1 * ((y * sectionSizeH) + posSectionTop) + startPosY), Quaternion.identity) as GameObject;
                        grid[x, y].name = objName ;
                        grid[x, y].transform.parent = transform;
                        grid[x, y].AddComponent<HexagonClick>();
                        grid[x, y].GetComponent<HexagonClick>().setCell(Assets.Scripts.CellStates.ground, x, y, id);
                        //grid[x, y].GetComponent<HexagonClick>().numberOfCell = id;
                        //grid[x, y].GetComponent<HexagonClick>().x = x;
                        //grid[x, y].GetComponent<HexagonClick>().y = y;
                        grid[x, y].gameObject.transform.GetChild(1).GetComponent<Animator>().enabled = false;
                        break;
                        case 9998:
                        block = Resources.Load<GameObject>("Prefab/FogOfWar") as GameObject;
                        objName = "Fog";
                        grid[x, y] = Instantiate(block, new Vector2((x * sectionSizeW) + startPosX, -1 * ((y * sectionSizeH) + posSectionTop) + startPosY), Quaternion.identity) as GameObject;
                        grid[x, y].name = objName;
                        grid[x, y].transform.parent = transform;
                        grid[x, y].AddComponent<HexagonClick>();
                        grid[x, y].GetComponent<HexagonClick>().setCell(Assets.Scripts.CellStates.fog, x, y, id);
                        //grid[x, y].GetComponent<HexagonClick>().numberOfCell = id;
                        //grid[x, y].GetComponent<HexagonClick>().x = x;
                        //grid[x, y].GetComponent<HexagonClick>().y = y;
                        grid[x, y].gameObject.transform.GetChild(1).GetComponent<Animator>().enabled = false;
                        break;
                }
             }
        }
        mapSectionsInfo.Clear();
    }
    public  MapLoader GetInstance()
    {
        if (a == null)
        {
            a = this;
        }

        return a;
    }
    // Use this for initialization
    void Start () {
        if (a == null)
        {
            a = this;
        }
        //DrawMap();
    }
    void Awake() {
        if (a == null)
        {
            a = this;
        }
    }
	
	// Update is called once per frame
	void Update () {
      
    }
}
