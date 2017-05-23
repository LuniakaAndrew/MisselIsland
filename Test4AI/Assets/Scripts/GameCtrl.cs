using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameCtrl : MonoBehaviour {

    public string CurrentLayer = "BuildBarPanel";
    public bool sceneStarting;
    public Image image;
    public float fadeSpeed = 1.5f;
    private GameStates gameSate;
    private Building lastBuilding;
    private Attack lastAttack;
    private Vector2 lastPosition=new Vector2();
    // Use this for initialization
    void Start () {
        MapLoader.a.DrawMap();
        Player.player.setMap(MapLoader.a.grid);
        gameSate = GameStates.Start;
        gameSate = GameStates.PalyerBuild;
        //lastBuilding = new Building();
        //lastAttack = new Attack();
    }
	
	// Update is called once per frame
	void Update () {
        if (sceneStarting)
        {
            StartScene();
            sceneStarting = false;
            gameSate = GameStates.PalyerBuild;
        }

        if (Input.GetMouseButtonDown(0))
        {
           CastRay(CurrentLayer);
           // CastRay("Background");
        }
    }
    void StartScene()
    {
        image.enabled = true;
        image.color = Color.Lerp(image.color, Color.clear, fadeSpeed * Time.deltaTime);

        if (image.color.a <= 0.01f)
        {
            image.color = Color.clear;
            image.enabled = false;
            sceneStarting = false;
            Cursor.visible = true;

        }

    }
    void CastRay(string layer)
    {
        LayerMask mask = LayerMask.GetMask(layer);
        RaycastHit2D hit = new RaycastHit2D();
        hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1, mask.value);

        if (hit.collider != null && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            if (gameSate.Equals(GameStates.PalyerBuild))
            {
                if (!lastPosition.Equals(new Vector2())) {
                    Player.player.CleanCell(lastPosition, lastBuilding.type);
                }
               
                Player.player.DrawCell(hit.collider.gameObject.GetComponent<HexagonClick>().getCellPosition(), lastBuilding.type,CellStates.prepare);
                lastPosition = hit.collider.gameObject.GetComponent<HexagonClick>().getCellPosition();
                return;
            }

            if (gameSate.Equals(GameStates.PlayerAttack))
            {

                return;
            }
         }
    }
    public void ClickPass()
    {        
        if (gameSate.Equals(GameStates.PalyerBuild)) {
            gameSate = GameStates.PlayerAttack;
            Debug.Log(gameSate.ToString());
            return;
        }

        if (gameSate.Equals(GameStates.PlayerAttack))
        {
            gameSate = GameStates.EnemyBuild;
            Debug.Log(gameSate.ToString());
            return;
        }

        if (gameSate.Equals(GameStates.EnemyBuild))
        {
            gameSate = GameStates.EnemyAttack;
            Debug.Log(gameSate.ToString());
            return;
        }

        if (gameSate.Equals(GameStates.EnemyAttack))
        {
            gameSate = GameStates.PalyerBuild;
            Debug.Log(gameSate.ToString());
            return;
        }
    }
    public void ClickBuild(GameObject hitray)
    {

        if (gameSate.Equals(GameStates.PalyerBuild))
        {
            Building tempBuilding = hitray.gameObject.GetComponent<Building>();
            lastBuilding = tempBuilding;
            Player.player.buildPoints -= tempBuilding.cost;
            Player.player.powerPoints -= tempBuilding.power;
            UICtrl.uiCtrl.hideConfirm(true);
            UICtrl.uiCtrl.hideDenide(true);
            CurrentLayer = "Background";
        }
        if (gameSate.Equals(GameStates.PlayerAttack))
        {
            Attack tempBuilding = hitray.gameObject.GetComponent<Attack>();
            lastAttack = tempBuilding;
            Player.player.attackPoints -= tempBuilding.cost;
            CurrentLayer = "Background";
        }
    }

}
