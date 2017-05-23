using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class GameControll : MonoBehaviour {

    public int[] stateOfGame = new int[4];//чей ход игрок 1 или 2; стадия строительства или атаки; тип постройки;номер хода


    /// <summary>
    /// Player settings
    /// </summary>
    public int PlayerAttackPoints;
    public float PlayerEnergyPoints;
    public int PlayerBuildPoints;
    /// <summary>
    /// Enemy settings
    /// </summary>
    public int EnemyAttackPoints;
    public float EnemyEnergyPoints;
    public int EnemyBuildPoints;

    public GameObject[] ActiveDeactiveElements;
    public Button[] DisableButtons;
    public Text[] OutputFields;
    public GameObject[] MyAttacks;
    public GameObject[] HisAttacks;
    public GameObject[] HisBuildings;
    public static GameControll controller;
    public string CurrentLayer="BuildBarPanel";
    public int CameraMove=2;
    public Animator[] Progress;

    bool sceneStarting;
    public float fadeSpeed = 1.5f;
    public Image _image;

    float BankEnergy;
    float EnemyBankEnergy;

    LastBuildingPos lastAction = new LastBuildingPos();
    /// <summary>
    /// Player 
    /// </summary>
    OccupiedCells PlayerOccupiedBuildingCells = new OccupiedCells();
    OccupiedCells PlayerOccupiedAttackCells = new OccupiedCells();

    ConstructedBuildings PlayerListOfBuildings = new ConstructedBuildings();//список из 7 зданий, в строке:х,у,кол-во построек 
    ProducedAttack PlayerListOfAttacks = new ProducedAttack();//список из 7 зданий, в строке:х,у,кол-во построек 
    /// <summary>
    /// Enemy 
    /// </summary>
    OccupiedCells EnemyOccupiedBuildingCells = new OccupiedCells();
    OccupiedCells EnemyOccupiedAttackCells = new OccupiedCells();

    ConstructedBuildings EnemyListOfBuildings = new ConstructedBuildings();//список из 7 зданий, в строке:х,у,кол-во построек 
    ProducedAttack EnemyListOfAttacks = new ProducedAttack();//список из 7 зданий, в строке:х,у,кол-во построек 

    private class Building
    {
        public int idOfBuilding;
        public int costOfBuilding;
        public int timeOfBuilding;
        public Vector2 coordinates=new Vector2();
        public Building()
        {

        }
        public Building(int idOfBuilding, int timeOfBuilding, Vector2 coordinates)
        {
            this.idOfBuilding = idOfBuilding;
            this.timeOfBuilding = timeOfBuilding;
            this.coordinates = coordinates;
        }
        public Building(int idOfBuilding, int timeOfBuilding,Vector2 coordinates,int costOfBuilding)
        {
            this.idOfBuilding = idOfBuilding;
            this.timeOfBuilding = timeOfBuilding;
            this.coordinates = coordinates;
            this.costOfBuilding = costOfBuilding;
        }

    }
    private class Attack
    {
        public int idOfAttack;
        public int costOfAttack;
        public int timeOfAttack;
        public int countOfAttack;
        public Vector2 coordinates = new Vector2();
        public Attack()
        {

        }
        public Attack(int idOfAttack, int timeOfAttack, Vector2 coordinates)
        {
            this.idOfAttack = idOfAttack;
            this.timeOfAttack = timeOfAttack;
            this.coordinates = coordinates;
        }
        public Attack(int idOfAttack, int timeOfAttack, Vector2 coordinates, int costOfAttack)
        {
            this.idOfAttack = idOfAttack;
            this.timeOfAttack = timeOfAttack;
            this.coordinates = coordinates;
            this.costOfAttack = costOfAttack;
        }
        public Attack(int idOfAttack, int timeOfAttack, Vector2 coordinates, int costOfAttack,int countOfAttack)
        {
            this.idOfAttack = idOfAttack;
            this.timeOfAttack = timeOfAttack;
            this.coordinates = coordinates;
            this.costOfAttack = costOfAttack;
            this.countOfAttack = countOfAttack;
        }

    }
    
    private class ConstructedBuildings
    {
        public int[] CountOfBuildingById=new int[7];
        public List<Building> listOfBuildings=new List<Building>();
        public void AddBuilding(int typeB,Vector2 xy,int timeB)
        {
            listOfBuildings.Add(new Building(typeB, timeB, xy));
            switch (typeB)
            {
                case 0: CountOfBuildingById[typeB] += 1;
                        break;
                case 1: CountOfBuildingById[typeB] += 1;
                        break;
                case 2: CountOfBuildingById[typeB] += 1;
                        break;
                case 3: CountOfBuildingById[typeB] += 1;
                        break;
                case 4: CountOfBuildingById[typeB] += 1;
                        break;
                case 5: CountOfBuildingById[typeB] += 1;
                        break;
                case 6: CountOfBuildingById[typeB] += 1;
                        break;
            }
        }
        public int CountOfDoneBuild(int idOfConstraction)
        {
            int counter = 0;
            for (int i = 0; i < listOfBuildings.Count; i++)
            {
                if (listOfBuildings[i].timeOfBuilding <= 0 && listOfBuildings[i].idOfBuilding == idOfConstraction)
                {
                    counter++;   
                }
               
            }
            return counter;
        } 
        public Building[] TimeToBuild( float Energy)
        {
            Building[] val = new Building[20];
            for (int i = 0; i < listOfBuildings.Count; i++)
            {
                if(Energy<=0.0 && listOfBuildings[i].idOfBuilding==1)
                    listOfBuildings[i].timeOfBuilding -= 1;

                if(Energy > 0.0)
                    listOfBuildings[i].timeOfBuilding -= 1;

                if (listOfBuildings[i].timeOfBuilding == 0)
                {
                    val[i] = listOfBuildings[i];
                }
                else
                {
                    if (listOfBuildings[i].timeOfBuilding < 0)
                        listOfBuildings[i].timeOfBuilding = 0;

                    val[i] = null;
                } 
            }
            return val;
        }
    }
    private class ProducedAttack
    {
        public int[] CountOfAttackById = new int[7];
        public List<Attack> listOfAttacks = new List<Attack>();
        public bool[] listOfAttacksCD = new bool[6];
        public bool[] AttacksTimeChange = new bool[6];
        public Attack[] Attacks = new Attack[6];

        public void GoAttack(int typeB, Vector2 xy, int timeB, int countA)
        {
            Attacks[typeB] = new Attack(typeB, timeB, xy, countA);
            AttacksTimeChange[typeB] = false;
        }
        public void AddAttack(int typeB, Vector2 xy, int timeB,int countA)
        {
            listOfAttacks.Add(new Attack(typeB, timeB, xy, countA));
            
  
            switch (typeB)
            {
                case 0:
                    CountOfAttackById[typeB] += 1;
                    break;
                case 1:
                    CountOfAttackById[typeB] += 1;
                    break;
                case 2:
                    CountOfAttackById[typeB] += 1;
                    break;
                case 3:
                    CountOfAttackById[typeB] += 1;
                    break;
                case 4:
                    CountOfAttackById[typeB] += 1;
                    break;
                case 5:
                    CountOfAttackById[typeB] += 1;
                    break;
                case 6:
                    CountOfAttackById[typeB] += 1;
                    break;
            }
        }

       public bool CountOfReadyAttack(int idOfConstraction)
        {
            bool counter = false;
            for (int i = 0; i < listOfAttacksCD.Length; i++)
            {
                if (listOfAttacksCD[i].timeOfAttack <= 0 && listOfAttacksCD[i].timeOfAttack == idOfConstraction)
                {
                    counter=true;
                }
            }
            return counter;
        }
        public Attack[] TimeToAttack(float Energy)
        {
          
            if (Energy < 0.0)
            {
                for (int i = 0; i < Attacks.Length; i++)
                {
                    if (Attacks[i] != null && !AttacksTimeChange[i])
                    {
                        Attacks[i].timeOfAttack += (int)Math.Floor((double)(Attacks[i].timeOfAttack / 2));
                        AttacksTimeChange[i] = true;
                        Debug.Log("Low");
                    }
                }
               
            }
            if(Energy > 0.0)
            {
                for (int i = 0; i < Attacks.Length; i++)
                {
                    if (Attacks[i] != null && AttacksTimeChange[i])
                    {
                        Attacks[i].timeOfAttack -= (int)Math.Floor((double)(Attacks[i].timeOfAttack / 3));
                        AttacksTimeChange[i] = false;
                        Debug.Log("UP");
                    }
                }

            }
            
         Attack[] val = new Attack[20];
            for (int i = 0; i < Attacks.Length; i++)
            {
                if (Attacks[i] != null)
                {
                    if (listOfAttacksCD[Attacks[i].idOfAttack])
                    {
                        Attacks[i].timeOfAttack -= 1;
                    }


                    if (Attacks[i].timeOfAttack == 0)
                    {
                        val[i] = Attacks[i];
                        listOfAttacksCD[Attacks[i].idOfAttack] = false;
                    }
                    else
                    {
                        if (Attacks[i].timeOfAttack < 0)
                            Attacks[i].timeOfAttack = 0;

                        val[i] = null;
                    }
                }
                
            }
            return val;
        }
    }

    public class OccupiedCells
    {
        List<int[,]> AnActiveCell = new List<int[,]>();
        public void setCoordinates( int[,] coordinates)
        {
            for (int i = 0; i < coordinates.GetLength(1); i++)
            {
              AnActiveCell.Add(new int[,] { { coordinates[0,i] }, { coordinates[1,i] } });
            }
        }
        public void getCoordinates(int[,] coordinates)
        {

            for (int i = 0; i < coordinates.GetLength(1); i++)
            {
                AnActiveCell.Add(new int[,] { { coordinates[0, i] }, { coordinates[1, i] } });
            }
        }
        public bool IsOccupied(int[,] x)
        {
            int[,] temp = AnActiveCell.Find(delegate (int[,] bk) 
            {    for (int i = 0; i < x.GetLength(0); i++)
                {
                    for (int j = 0; j < x.GetLength(1); j++)
                    {
                        if (bk[i, j] != x[i, j])
                        {
                            return false;  // If one is not equal, the two arrays differ
                        }
                    }
                }
                return true;
            });
            if (temp != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    private class LastBuildingPos
    {
        public int x;
        public int y;
        public int numberOfCell;
        public int idOfBuilding;
        public int idOfAttack;
        public int costOfBuilding;
        public int costOfAttack;
        public int timeOfBuilding;
        public int timeOfAttack;
        public int countOfAttack;
        public int[,] maskOfBuilding;
        public LastBuildingPos()
        {
            this.x=0;
            this.y =0;
            this.numberOfCell =0;
            this.idOfBuilding =0;
            this.idOfAttack =0;
            this.costOfBuilding =0;
            this.costOfAttack =0;
            this.timeOfBuilding =0;
            this.timeOfAttack =0;
            this.countOfAttack =0;
        }
        public void ClearData()
        {
            x = 0;
            y = 0;
            numberOfCell = 0;
            idOfBuilding = 0;
            idOfAttack = 0;
            costOfBuilding = 0;
            costOfAttack = 0;
            timeOfBuilding = 0;
            maskOfBuilding=new int[0,0];
        }
    }

    void Awake()
    {
        if (controller == null)
        {
            controller = this;
        }
        sceneStarting = true;
    }
    // Use this for initialization
    void Start () {
        if (controller == null)
        {
            controller = this;
        }
        MapLoader.a.DrawMap();
        OutputFields[1].text = "AttackPoints:" + PlayerAttackPoints;
        OutputFields[0].text = "BuildPoints:" + PlayerBuildPoints;
        lastAction = new LastBuildingPos();
        PlayerOccupiedAttackCells = new OccupiedCells();
        PlayerListOfBuildings = new ConstructedBuildings();
        PlayerListOfAttacks = new ProducedAttack();
        EnemyOccupiedAttackCells = new OccupiedCells();
        EnemyListOfBuildings = new ConstructedBuildings();
        EnemyListOfAttacks = new ProducedAttack();
    }
	
	// Update is called once per frame
	void Update () {
        Debug.Log("RayClickUpdate");
        if (sceneStarting) StartScene();
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("RayClick");
            if (CameraMove == 2)
            {

                CastRay(CurrentLayer);
            }
        }
    }
    void FixedUpdate() {
        Debug.Log("RayClick");
         if (Input.GetMouseButtonDown(0))
         {
             Debug.Log("RayClick");
             if (CameraMove == 2)
             {

                 CastRay(CurrentLayer);
             }
         }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }

    }
    void CastRay(string layer)
    {
        Debug.Log("Ray");
        LayerMask mask =LayerMask.GetMask(layer);
        RaycastHit2D hit=new RaycastHit2D();
        hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1, mask.value);

        if (hit.collider != null && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            switch (hit.collider.gameObject.name)
            {
                 case "Water":
                    if (stateOfGame[0] == 1 && stateOfGame[1] == 1)
                    {
                        ClickGroundAttack(stateOfGame[2], hit);
                    }
                    break;
                case "Ground":
                    if (stateOfGame[1] == 0)
                    {
                        ClickGroundBuild(stateOfGame[2], hit);
                    }
                    if (stateOfGame[1] == 1)
                    {
                        ClickGroundAttack(stateOfGame[2], hit);
                    }
                    break;
            }
        }
 
    }

    public void AbleDisableButtons(Button[] Btn,bool AbleDisAble)
    {
        foreach (Button temp in Btn)
            temp.interactable = AbleDisAble;
    }
    public void DisableButton(Button Btn)
    {
        
        Btn.interactable = false;
    }
    public void AbleButton(Button Btn)
    {
        Btn.interactable = true;
    }
    // check possible of attack
    public bool CheckEnableAttackPlayer(int idOfAttack)
    {
        bool result=false;
        switch (idOfAttack)
        {
            case 0:
                if (PlayerListOfBuildings.CountOfBuildingById[0] >= 1 && PlayerListOfBuildings.CountOfDoneBuild(0) >= 1)
                {
                    result = true;
                }
                    
                break;
            case 1:
                if (PlayerListOfBuildings.CountOfBuildingById[2] >= 1 && PlayerListOfBuildings.CountOfDoneBuild(2) >= 1)
                {
                    result = true;
                }
                    
                break;
            case 2:
                if (PlayerListOfBuildings.CountOfBuildingById[3] >= 1 && PlayerListOfBuildings.CountOfDoneBuild(3) >= 1)
                {
                    result = true;
                }
                    
                break;
            case 3:
                if (PlayerListOfBuildings.CountOfBuildingById[0] >= 1 && PlayerListOfBuildings.CountOfBuildingById[4] >= 1 && PlayerListOfBuildings.CountOfDoneBuild(0) >= 1 && PlayerListOfBuildings.CountOfDoneBuild(4) >= 1)
                {
                    result = true;
                }
                   
                break;
            case 4:
                if (PlayerListOfBuildings.CountOfBuildingById[3] >= 1 && PlayerListOfBuildings.CountOfBuildingById[2] >= 1 && PlayerListOfBuildings.CountOfBuildingById[4] >= 1
                    && PlayerListOfBuildings.CountOfDoneBuild(3) >= 1 && PlayerListOfBuildings.CountOfDoneBuild(2) >= 1 && PlayerListOfBuildings.CountOfDoneBuild(4) >= 1)
                {
                    result = true;
                }
                   
                break;
        }
        return result;
    }
    public bool CheckEnableAttackEnemy(int idOfAttack)
    {
        bool result = false;
        switch (idOfAttack)
        {
            case 0:
                if (EnemyListOfBuildings.CountOfBuildingById[0] >= 1 && EnemyListOfBuildings.CountOfDoneBuild(0) >= 1)
                {
                    result = true;
                }

                break;
            case 1:
                if (EnemyListOfBuildings.CountOfBuildingById[2] >= 1 && EnemyListOfBuildings.CountOfDoneBuild(2) >= 1)
                {
                    result = true;
                }

                break;
            case 2:
                if (EnemyListOfBuildings.CountOfBuildingById[3] >= 1 && EnemyListOfBuildings.CountOfDoneBuild(3) >= 1)
                {
                    result = true;
                }

                break;
            case 3:
                if (EnemyListOfBuildings.CountOfBuildingById[0] >= 1 && EnemyListOfBuildings.CountOfBuildingById[4] >= 1 && EnemyListOfBuildings.CountOfDoneBuild(0) >= 1 && EnemyListOfBuildings.CountOfDoneBuild(4) >= 1)
                {
                    result = true;
                }

                break;
            case 4:
                if (EnemyListOfBuildings.CountOfBuildingById[3] >= 1 && EnemyListOfBuildings.CountOfBuildingById[2] >= 1 && EnemyListOfBuildings.CountOfBuildingById[4] >= 1
                    && EnemyListOfBuildings.CountOfDoneBuild(3) >= 1 && EnemyListOfBuildings.CountOfDoneBuild(2) >= 1 && EnemyListOfBuildings.CountOfDoneBuild(4) >= 1)
                {
                    result = true;
                }

                break;
        }
        return result;
    }
    // add count of attack
    public bool AddCountAttackPlayer(GameObject[] Attacker)
    {
        bool result = false;
        for (int i = 0; i < Attacker.Length; i++)
        {
            switch (Attacker[i].GetComponent<FireButton>().idOfAttack)
            {
                case 0:
                    if (PlayerListOfBuildings.CountOfBuildingById[0] >= 1 && PlayerListOfBuildings.CountOfDoneBuild(0) >= 1)
                    {
                        Attacker[i].GetComponent<FireButton>().countOfAttack += PlayerListOfBuildings.CountOfDoneBuild(0);
                        OutputFields[3].text = Attacker[i].GetComponent<FireButton>().countOfAttack.ToString();
                        Attacker[i].GetComponent<FireButton>().attackBar = 90;
                        result = true;
                        Progress[0].SetInteger("Percent", 90);
                        OutputFields[8].text = Attacker[i].GetComponent<FireButton>().timeOfAttack.ToString();
                    }

                    break;
                case 1:
                    if (PlayerListOfBuildings.CountOfBuildingById[2] >= 1 && PlayerListOfBuildings.CountOfDoneBuild(2) >= 1)
                    {
                        Attacker[i].GetComponent<FireButton>().countOfAttack += PlayerListOfBuildings.CountOfDoneBuild(2);
                        OutputFields[4].text = Attacker[i].GetComponent<FireButton>().countOfAttack.ToString();
                        Attacker[i].GetComponent<FireButton>().attackBar = 90;
                        result = true;
                        Progress[1].SetInteger("Percent", 90);
                        OutputFields[9].text = Attacker[i].GetComponent<FireButton>().timeOfAttack.ToString();
                    }

                    break;
                case 2:
                    if (PlayerListOfBuildings.CountOfBuildingById[3] >= 1 && PlayerListOfBuildings.CountOfDoneBuild(3) >= 1)
                    {
                        Attacker[i].GetComponent<FireButton>().countOfAttack += PlayerListOfBuildings.CountOfDoneBuild(3);
                        OutputFields[5].text = Attacker[i].GetComponent<FireButton>().countOfAttack.ToString();
                        Attacker[i].GetComponent<FireButton>().attackBar = 90;
                        result = true;
                        Progress[2].SetInteger("Percent", 90);
                        OutputFields[10].text = Attacker[i].GetComponent<FireButton>().timeOfAttack.ToString();
                    }

                    break;
                case 3:
                    if (PlayerListOfBuildings.CountOfBuildingById[0] >= 1 && PlayerListOfBuildings.CountOfBuildingById[4] >= 1 && PlayerListOfBuildings.CountOfDoneBuild(0) >= 1 && PlayerListOfBuildings.CountOfDoneBuild(4) >= 1)
                    {
                        float stepcounts;
                        int attackbar = Attacker[i].GetComponent<FireButton>().attackBar;
                        int tempPercent = Attacker[i].GetComponent<FireButton>().percentOfAttack;
                        tempPercent += PlayerListOfBuildings.CountOfBuildingById[0] * 40;
                        tempPercent += PlayerListOfBuildings.CountOfBuildingById[4] * 60;
                        tempPercent = (int)Math.Ceiling((double)(tempPercent / Attacker[i].GetComponent<FireButton>().timeOfAttack));
                        attackbar = tempPercent;
                        stepcounts = (float)((float)(tempPercent * Attacker[i].GetComponent<FireButton>().timeOfAttack)/100.0f) - (float)Attacker[i].GetComponent<FireButton>().timeOfAttack;
                        OutputFields[11].text = stepcounts.ToString();
                        if (tempPercent >= 100)
                        {
                            int countofa= (int)Math.Floor((double)(tempPercent / 100));
                            Attacker[i].GetComponent<FireButton>().countOfAttack += countofa;
                            OutputFields[6].text = Attacker[i].GetComponent<FireButton>().countOfAttack.ToString();
                            attackbar -= 100 * countofa;
                           // tempPercent -= 100 * countofa;
                        }
                        Attacker[i].GetComponent<FireButton>().attackBar = attackbar;
                       // Attacker[i].GetComponent<FireButton>().percentOfAttack = tempPercent;
                        Progress[3].SetInteger("Percent",attackbar);
                        
                        result = true;
                    }

                    break;
                case 4:
                    if (PlayerListOfBuildings.CountOfBuildingById[3] >= 1 && PlayerListOfBuildings.CountOfBuildingById[2] >= 1 && PlayerListOfBuildings.CountOfBuildingById[4] >= 1
                        && PlayerListOfBuildings.CountOfDoneBuild(3) >= 1 && PlayerListOfBuildings.CountOfDoneBuild(2) >= 1 && PlayerListOfBuildings.CountOfDoneBuild(4) >= 1)
                    {
                        float stepcounts;
                        int attackbar = Attacker[i].GetComponent<FireButton>().attackBar;
                        int tempPercent = Attacker[i].GetComponent<FireButton>().percentOfAttack;
                        tempPercent += PlayerListOfBuildings.CountOfBuildingById[3] * 20;
                        tempPercent += PlayerListOfBuildings.CountOfBuildingById[4] * 50;
                        tempPercent += PlayerListOfBuildings.CountOfBuildingById[2] * 30;
                        tempPercent = (int)Math.Ceiling((double)(tempPercent / Attacker[i].GetComponent<FireButton>().timeOfAttack));
                        attackbar = tempPercent;
                        stepcounts = (float)((float)(tempPercent * Attacker[i].GetComponent<FireButton>().timeOfAttack) / 100.0f) - (float)Attacker[i].GetComponent<FireButton>().timeOfAttack;
                        OutputFields[12].text = stepcounts.ToString();
                        if (tempPercent >= 100)
                        {
                            int countofa = (int)Math.Floor((double)(tempPercent / 100));
                            Attacker[i].GetComponent<FireButton>().countOfAttack += countofa;
                            OutputFields[7].text = Attacker[i].GetComponent<FireButton>().countOfAttack.ToString();
                            attackbar -= 100 * countofa;
                         //   tempPercent -= 100 * countofa;
                        }
                        Attacker[i].GetComponent<FireButton>().attackBar = attackbar;
                       // Attacker[i].GetComponent<FireButton>().percentOfAttack = tempPercent;
                        Progress[4].SetInteger("Percent", attackbar);
                        result = true;
                    }

                    break;
            }
        }
        return result;
    }
    public bool AddCountAttackEnemy(GameObject[] Attacker)
    {
        bool result = false;
        for (int i = 0; i < Attacker.Length; i++)
        {
            switch (Attacker[i].GetComponent<FireButton>().idOfAttack)
            {
                case 0:
                    if (EnemyListOfBuildings.CountOfBuildingById[0] >= 1 && EnemyListOfBuildings.CountOfDoneBuild(0) >= 1)
                    {
                        Attacker[i].GetComponent<FireButton>().countOfAttack += EnemyListOfBuildings.CountOfDoneBuild(0);
                        //OutputFields[3].text = Attacker[i].GetComponent<FireButton>().countOfAttack.ToString();
                        Attacker[i].GetComponent<FireButton>().attackBar = 90;
                        result = true;
                        //Progress[0].SetInteger("Percent", 90);
                        //OutputFields[8].text = Attacker[i].GetComponent<FireButton>().timeOfAttack.ToString();
                    }

                    break;
                case 1:
                    if (EnemyListOfBuildings.CountOfBuildingById[2] >= 1 && EnemyListOfBuildings.CountOfDoneBuild(2) >= 1)
                    {
                        Attacker[i].GetComponent<FireButton>().countOfAttack += EnemyListOfBuildings.CountOfDoneBuild(2);
                        //OutputFields[4].text = Attacker[i].GetComponent<FireButton>().countOfAttack.ToString();
                        Attacker[i].GetComponent<FireButton>().attackBar = 90;
                        result = true;
                        //Progress[1].SetInteger("Percent", 90);
                        //OutputFields[9].text = Attacker[i].GetComponent<FireButton>().timeOfAttack.ToString();
                    }

                    break;
                case 2:
                    if (EnemyListOfBuildings.CountOfBuildingById[3] >= 1 && EnemyListOfBuildings.CountOfDoneBuild(3) >= 1)
                    {
                        Attacker[i].GetComponent<FireButton>().countOfAttack += EnemyListOfBuildings.CountOfDoneBuild(3);
                        //OutputFields[5].text = Attacker[i].GetComponent<FireButton>().countOfAttack.ToString();
                        Attacker[i].GetComponent<FireButton>().attackBar = 90;
                        result = true;
                        //Progress[2].SetInteger("Percent", 90);
                        //OutputFields[10].text = Attacker[i].GetComponent<FireButton>().timeOfAttack.ToString();
                    }

                    break;
                case 3:
                    if (EnemyListOfBuildings.CountOfBuildingById[0] >= 1 && EnemyListOfBuildings.CountOfBuildingById[4] >= 1 && EnemyListOfBuildings.CountOfDoneBuild(0) >= 1 && EnemyListOfBuildings.CountOfDoneBuild(4) >= 1)
                    {
                        float stepcounts;
                        int attackbar = Attacker[i].GetComponent<FireButton>().attackBar;
                        int tempPercent = Attacker[i].GetComponent<FireButton>().percentOfAttack;
                        tempPercent += EnemyListOfBuildings.CountOfBuildingById[0] * 40;
                        tempPercent += EnemyListOfBuildings.CountOfBuildingById[4] * 60;
                        tempPercent = (int)Math.Ceiling((double)(tempPercent / Attacker[i].GetComponent<FireButton>().timeOfAttack));
                        attackbar = tempPercent;
                        stepcounts = (float)((float)(tempPercent * Attacker[i].GetComponent<FireButton>().timeOfAttack) / 100.0f) - (float)Attacker[i].GetComponent<FireButton>().timeOfAttack;
                        //OutputFields[11].text = stepcounts.ToString();
                        if (tempPercent >= 100)
                        {
                            int countofa = (int)Math.Floor((double)(tempPercent / 100));
                            Attacker[i].GetComponent<FireButton>().countOfAttack += countofa;
                            //OutputFields[6].text = Attacker[i].GetComponent<FireButton>().countOfAttack.ToString();
                            attackbar -= 100 * countofa;
                            // tempPercent -= 100 * countofa;
                        }
                        Attacker[i].GetComponent<FireButton>().attackBar = attackbar;
                        // Attacker[i].GetComponent<FireButton>().percentOfAttack = tempPercent;
                        //Progress[3].SetInteger("Percent", attackbar);

                        result = true;
                    }

                    break;
                case 4:
                    if (EnemyListOfBuildings.CountOfBuildingById[3] >= 1 && EnemyListOfBuildings.CountOfBuildingById[2] >= 1 && EnemyListOfBuildings.CountOfBuildingById[4] >= 1
                        && EnemyListOfBuildings.CountOfDoneBuild(3) >= 1 && EnemyListOfBuildings.CountOfDoneBuild(2) >= 1 && EnemyListOfBuildings.CountOfDoneBuild(4) >= 1)
                    {
                        float stepcounts;
                        int attackbar = Attacker[i].GetComponent<FireButton>().attackBar;
                        int tempPercent = Attacker[i].GetComponent<FireButton>().percentOfAttack;
                        tempPercent += EnemyListOfBuildings.CountOfBuildingById[3] * 20;
                        tempPercent += EnemyListOfBuildings.CountOfBuildingById[4] * 50;
                        tempPercent += EnemyListOfBuildings.CountOfBuildingById[2] * 30;
                        tempPercent = (int)Math.Ceiling((double)(tempPercent / Attacker[i].GetComponent<FireButton>().timeOfAttack));
                        attackbar = tempPercent;
                        stepcounts = (float)((float)(tempPercent * Attacker[i].GetComponent<FireButton>().timeOfAttack) / 100.0f) - (float)Attacker[i].GetComponent<FireButton>().timeOfAttack;
                        //OutputFields[12].text = stepcounts.ToString();
                        if (tempPercent >= 100)
                        {
                            int countofa = (int)Math.Floor((double)(tempPercent / 100));
                            Attacker[i].GetComponent<FireButton>().countOfAttack += countofa;
                            //OutputFields[7].text = Attacker[i].GetComponent<FireButton>().countOfAttack.ToString();
                            attackbar -= 100 * countofa;
                            //   tempPercent -= 100 * countofa;
                        }
                        Attacker[i].GetComponent<FireButton>().attackBar = attackbar;
                        // Attacker[i].GetComponent<FireButton>().percentOfAttack = tempPercent;
                        //Progress[4].SetInteger("Percent", attackbar);
                        result = true;
                    }

                    break;
            }
        }
        return result;
    }

    public void ClickDenide()
    {
        if (stateOfGame[0] == 1 && stateOfGame[1] == 0)
        {
            CleanDrawGround(lastAction.x, lastAction.y, lastAction.idOfBuilding, 17);
            PlayerBuildPoints += lastAction.costOfBuilding;
            lastAction.ClearData();
            CurrentLayer = "BuildBarPanel";
            OutputFields[0].text = "BuildPoints:" + PlayerBuildPoints;
            ActiveDeactiveElements[0].gameObject.SetActive(true);
            ActiveDeactiveElements[2].gameObject.SetActive(false);
            ActiveDeactiveElements[3].gameObject.SetActive(false);
            ActiveDeactiveElements[4].gameObject.SetActive(true);
        }
        if (stateOfGame[0] == 1 && stateOfGame[1] == 1)
        {
            CleanDrawGroundAttack(lastAction.x, lastAction.y, lastAction.idOfAttack, 20);
            PlayerAttackPoints += lastAction.costOfAttack;
            MyAttacks[lastAction.idOfAttack].gameObject.GetComponent<FireButton>().countOfAttack = lastAction.countOfAttack+1;
            lastAction.ClearData();
            CurrentLayer = "AttackBarPanel";
            OutputFields[1].text = "AttackPoints:" + PlayerAttackPoints;
            ActiveDeactiveElements[1].gameObject.SetActive(true);
            ActiveDeactiveElements[2].gameObject.SetActive(false);
            ActiveDeactiveElements[3].gameObject.SetActive(false);
            ActiveDeactiveElements[4].gameObject.SetActive(true);
        }
    }
    public void ClickConfirm()
    {
        if (stateOfGame[0] == 1 && stateOfGame[1] == 0)
        {
            if (PlayerBuildPoints < 0)
            {
                Debug.Log("Building is not possible");
                OutputFields[0].text = "Building is not possible";
            }
            else
            {
                PlayerListOfBuildings.AddBuilding(lastAction.idOfBuilding, new Vector2(lastAction.x, lastAction.y), lastAction.timeOfBuilding);
                PlayerOccupiedBuildingCells.setCoordinates(lastAction.maskOfBuilding);
                CurrentLayer = "BuildBarPanel";
                CleanDrawGround(lastAction.x, lastAction.y, lastAction.idOfAttack, 23);
                lastAction.ClearData();
                OutputFields[0].text = "PBuildPoints:"+ PlayerBuildPoints;
                ActiveDeactiveElements[0].gameObject.SetActive(true);
                ActiveDeactiveElements[2].gameObject.SetActive(false);
                ActiveDeactiveElements[3].gameObject.SetActive(false);
                ActiveDeactiveElements[4].gameObject.SetActive(true);
            }
        }
        if (stateOfGame[0] == 2 && stateOfGame[1] == 0)
        {
            if (EnemyBuildPoints < 0)
            {
                Debug.Log("Building is not possible");
                OutputFields[0].text = "Building is not possible";
            }
            else
            {
                EnemyListOfBuildings.AddBuilding(lastAction.idOfBuilding, new Vector2(lastAction.x, lastAction.y), lastAction.timeOfBuilding);
                EnemyOccupiedBuildingCells.setCoordinates(lastAction.maskOfBuilding);
                CurrentLayer = "BuildBarPanel";
                lastAction.ClearData();
                OutputFields[0].text = "EBuildPoints:" + EnemyBuildPoints;
                ActiveDeactiveElements[0].gameObject.SetActive(true);
                ActiveDeactiveElements[2].gameObject.SetActive(false);
                ActiveDeactiveElements[3].gameObject.SetActive(false);
                ActiveDeactiveElements[4].gameObject.SetActive(true);
            }
        }
        if (stateOfGame[0] == 1 && stateOfGame[1] == 1)
        {
            if (PlayerAttackPoints < 0)
            {
                Debug.Log("Attack is not possible");
                OutputFields[1].text = "Attack is not possible";
            }
            else
            {
                PlayerListOfAttacks.AddAttack(lastAction.idOfAttack, new Vector2(lastAction.x, lastAction.y), lastAction.timeOfAttack, lastAction.countOfAttack);
                PlayerOccupiedAttackCells.setCoordinates(lastAction.maskOfBuilding);
                CurrentLayer = "AttackBarPanel";
                MyAttacks[lastAction.idOfAttack].gameObject.GetComponent<FireButton>().countOfAttack = lastAction.countOfAttack;
                DrawAttack(lastAction.x, lastAction.y, lastAction.idOfAttack,19,EnemyOccupiedBuildingCells);
                lastAction.ClearData();
                OutputFields[1].text = "PAttackPoints:" + PlayerAttackPoints;
                ActiveDeactiveElements[1].gameObject.SetActive(true);
                ActiveDeactiveElements[2].gameObject.SetActive(false);
                ActiveDeactiveElements[3].gameObject.SetActive(false);
                ActiveDeactiveElements[4].gameObject.SetActive(true);
                foreach (GameObject a in MyAttacks)
                {
                    Progress[a.GetComponent<FireButton>().idOfAttack].SetInteger("Percent", a.GetComponent<FireButton>().attackBar);
                }
            }
        }
        if (stateOfGame[0] == 2 && stateOfGame[1] == 1)
        {
            if (EnemyAttackPoints < 0)
            {
                Debug.Log("Attack is not possible");
                OutputFields[1].text = "Attack is not possible";
            }
            else
            {
                EnemyListOfAttacks.AddAttack(lastAction.idOfAttack, new Vector2(lastAction.x, lastAction.y), lastAction.timeOfAttack, lastAction.countOfAttack);
                EnemyOccupiedAttackCells.setCoordinates(lastAction.maskOfBuilding);
                CurrentLayer = "AttackBarPanel";
                HisAttacks[lastAction.idOfAttack].gameObject.GetComponent<FireButton>().countOfAttack = lastAction.countOfAttack;
                DrawAttack(lastAction.x, lastAction.y, lastAction.idOfAttack, 19,PlayerOccupiedBuildingCells);
                lastAction.ClearData();
                OutputFields[1].text = "EAttackPoints:" + EnemyAttackPoints;
                ActiveDeactiveElements[1].gameObject.SetActive(true);
                ActiveDeactiveElements[2].gameObject.SetActive(false);
                ActiveDeactiveElements[3].gameObject.SetActive(false);
                ActiveDeactiveElements[4].gameObject.SetActive(true);
                foreach (GameObject a in HisAttacks)
                {
                    Progress[a.GetComponent<FireButton>().idOfAttack].SetInteger("Percent", a.GetComponent<FireButton>().attackBar);
                }
            }
        }
    }
    public void EnemyTurn()
    {
        if (stateOfGame[0] == 2)
        {
            ClickBuild(HisBuildings[0]);
            lastAction.maskOfBuilding = DrawGround(67, 13, 0, 16);
            lastAction.x = 67;
            lastAction.y = 13;
            ClickConfirm();
        }
       
    }
    public void ClickPass()
    {

        if (stateOfGame[1] == 0)
        {
            CameraMove = 1;
            CurrentLayer = "AttackBarPanel";
            stateOfGame[1] = 1;
            if (stateOfGame[3] % 2 == 1)//Enemy Turn
            {
//                ActiveDeactiveElements[0].SetActive(false);
//                ActiveDeactiveElements[1].SetActive(false);
                AddCountAttackForEnemy(HisAttacks);
                //stateOfGame[0] = 2;
                for (int i = 0; i < 5; i++)
                    if (CheckEnableAttackEnemy(i) && !EnemyListOfAttacks.listOfAttacksCD[i])
                    {
                        AbleButton(DisableButtons[i]);
                    }
                    else
                    {
                        DisableButton(DisableButtons[i]);
                    }
               Debug.Log("Pass buildg");
            }
            else//Your Turn
            {
                Debug.Log("buildg");
                ActiveDeactiveElements[0].SetActive(false);
                ActiveDeactiveElements[1].SetActive(true);
                AddCountAttack(MyAttacks);
               // stateOfGame[0] = 1;
                for (int i = 0; i < 5; i++)
                    if (CheckEnableAttackEnemy(i) && !PlayerListOfAttacks.listOfAttacksCD[i])
                    {
                        AbleButton(DisableButtons[i]);
                    }
                    else
                    {
                        DisableButton(DisableButtons[i]);
                    }
            }

        }
        else
        {
            if (stateOfGame[1] == 1)
            {
                CameraMove = 0;
                CurrentLayer = "BuildBarPanel";
                
                stateOfGame[1] = 0;
                stateOfGame[3] += 1;

                if (stateOfGame[3] % 2 == 1)//Enemy Turn
                {
                    OutputFields[2].text = "Enemy Turn";
                    stateOfGame[0] = 2;
                    ActiveDeactiveElements[1].SetActive(false);
                    ActiveDeactiveElements[0].SetActive(false);
                    EnemyAttackPoints++;
                    EnemyBuildPoints++;
                    EnemyTurn();
                    Building[] toB = EnemyListOfBuildings.TimeToBuild(EnemyEnergyPoints);
                    for (int i = 0; i < toB.Length; i++)
                    {
                        Debug.Log("Building");
                        if (toB[i] != null)
                        {
                            Debug.Log("Building is");
                            DrawBuildings((int)toB[i].coordinates.x, (int)toB[i].coordinates.y, toB[i].idOfBuilding);
                            if (toB[i].idOfBuilding == 1)
                            {
                                EnemyEnergyPoints += EnemyBankEnergy;
                                //Debug.Log("Energy" + EnemyBankEnergy);
                                EnemyBankEnergy = 0;
                            }
                        }
                    }

                    Attack[] toB1 = EnemyListOfAttacks.TimeToAttack(EnemyEnergyPoints);
                    for (int i = 0; i < toB1.Length; i++)
                        if (toB1[i] != null)
                        {
                            AbleButton(DisableButtons[toB1[i].idOfAttack]);
                        }
                   Debug.Log("Pass attack");
                }
                else//Your Turn
                {
                    Debug.Log("attack");
                    ActiveDeactiveElements[0].SetActive(true);
                    ActiveDeactiveElements[1].SetActive(false);
                    OutputFields[2].text = "Your Turn";
                    stateOfGame[0] = 1;
                    PlayerAttackPoints++;
                    PlayerBuildPoints++;
                    
                        Building[] toB = PlayerListOfBuildings.TimeToBuild(PlayerEnergyPoints);
                        for (int i = 0; i < toB.Length; i++)
                        {
                            if (toB[i] != null)
                            {
                                DrawBuildings((int)toB[i].coordinates.x, (int)toB[i].coordinates.y, toB[i].idOfBuilding);
                                if (toB[i].idOfBuilding == 1)
                                {
                                    PlayerEnergyPoints += BankEnergy;
                                    Debug.Log("Energy"+BankEnergy);
                                    BankEnergy = 0;
                                }
                            }
                         }

                    Attack[] toB1 = PlayerListOfAttacks.TimeToAttack(PlayerEnergyPoints);
                    for (int i = 0; i < toB1.Length; i++)
                        if (toB1[i] != null)
                        {
                            AbleButton(DisableButtons[toB1[i].idOfAttack]);
                        }
                }
                
            }
        }
        OutputFields[1].text = "AttackPoints:" + PlayerAttackPoints;
        OutputFields[0].text = "BuildPoints:" + PlayerBuildPoints;

    }
    public void ClickPass()
    {
        if (stateOfGame[3] % 2 == 1)//Enemy Turn
        {
            OutputFields[2].text = "Enemy Turn";
            OutputFields[1].text = "EAttackPoints:" + EnemyAttackPoints;
            OutputFields[0].text = "EBuildPoints:" + EnemyBuildPoints;

            if (stateOfGame[1] == 0)
            {
                Debug.Log("Enemy Attack");
                CameraMove = 0;

                CurrentLayer = "AttackBarPanel";
                stateOfGame[1] = 1;
                stateOfGame[0] = 2;

                ActiveDeactiveElements[0].SetActive(false);
                ActiveDeactiveElements[1].SetActive(true);
                AddCountAttackEnemy(HisAttacks);

                for (int i = 0; i < 5; i++)
                    if (CheckEnableAttackEnemy(i) && !EnemyListOfAttacks.listOfAttacksCD[i])
                    {
                        AbleButton(DisableButtons[i]);
                    }
                    else
                    {
                        DisableButton(DisableButtons[i]);
                    }
            }
            else
            {
                if (stateOfGame[1] == 1)
                {
                    OutputFields[2].text = "Your Turn";
                    OutputFields[1].text = "PAttackPoints:" + PlayerAttackPoints;
                    OutputFields[0].text = "PBuildPoints:" + PlayerBuildPoints;

                    Debug.Log("Enemy End Turn");
                    CameraMove = 2;

                    CurrentLayer = "BuildBarPanel";

                    stateOfGame[1] = 0;
                    stateOfGame[3] += 1;
                    stateOfGame[0] = 1;

                    ActiveDeactiveElements[0].SetActive(true);
                    ActiveDeactiveElements[1].SetActive(false);

                    EnemyAttackPoints++;
                    EnemyBuildPoints++;

                    Building[] toB = EnemyListOfBuildings.TimeToBuild(EnemyEnergyPoints);
                    for (int i = 0; i < toB.Length; i++)
                    {
                        if (toB[i] != null)
                        {
                            DrawBuildings((int)toB[i].coordinates.x, (int)toB[i].coordinates.y, toB[i].idOfBuilding);
                            if (toB[i].idOfBuilding == 1)
                            {
                                EnemyEnergyPoints += EnemyBankEnergy;
                                EnemyBankEnergy = 0;
                            }
                        }
                    }

                    Attack[] toB1 = EnemyListOfAttacks.TimeToAttack(EnemyEnergyPoints);
                    for (int i = 0; i < toB1.Length; i++)
                        if (toB1[i] != null)
                        {
                            AbleButton(DisableButtons[toB1[i].idOfAttack]);
                        }
                }
            }

        }
        else//Your Turn
        {
            OutputFields[2].text = "Your Turn";
            OutputFields[1].text = "PAttackPoints:" + PlayerAttackPoints;
            OutputFields[0].text = "PBuildPoints:" + PlayerBuildPoints;

            if (stateOfGame[1] == 0)
            {
                Debug.Log("Player Attack");
                CameraMove = 1;

                CurrentLayer = "AttackBarPanel";

                stateOfGame[1] = 1;
                stateOfGame[0] = 1;

                ActiveDeactiveElements[0].SetActive(false);
                ActiveDeactiveElements[1].SetActive(true);

                AddCountAttackPlayer(MyAttacks);

                for (int i = 0; i < 5; i++)
                    if (CheckEnableAttackPlayer(i) && !PlayerListOfAttacks.listOfAttacksCD[i])
                    {
                        AbleButton(DisableButtons[i]);
                    }
                    else
                    {
                        DisableButton(DisableButtons[i]);
                    }
            }
            else
            {
                if (stateOfGame[1] == 1)
                {
                    OutputFields[2].text = "Enemy Turn";
                    OutputFields[1].text = "EAttackPoints:" + EnemyAttackPoints;
                    OutputFields[0].text = "EBuildPoints:" + EnemyBuildPoints;
                    Debug.Log("Player End Turn");
                    CameraMove = 2;

                    CurrentLayer = "BuildBarPanel";

                    stateOfGame[1] = 0;
                    stateOfGame[3] += 1;
                    stateOfGame[0] = 2;

                    ActiveDeactiveElements[0].SetActive(true);
                    ActiveDeactiveElements[1].SetActive(false);

                    PlayerAttackPoints++;
                    PlayerBuildPoints++;

                    Building[] toB = PlayerListOfBuildings.TimeToBuild(PlayerEnergyPoints);
                    for (int i = 0; i < toB.Length; i++)
                    {
                        if (toB[i] != null)
                        {
                            DrawBuildings((int)toB[i].coordinates.x, (int)toB[i].coordinates.y, toB[i].idOfBuilding);
                            if (toB[i].idOfBuilding == 1)
                            {
                                PlayerEnergyPoints += BankEnergy;
                                BankEnergy = 0;
                            }
                        }
                    }

                    Attack[] toB1 = PlayerListOfAttacks.TimeToAttack(PlayerEnergyPoints);
                    for (int i = 0; i < toB1.Length; i++)
                        if (toB1[i] != null)
                        {
                            AbleButton(DisableButtons[toB1[i].idOfAttack]);
                        }
                }
            }
        }
    }

    public void ClickGroundBuild(int BuildingId, RaycastHit2D hitray)
    {
        int textureId = 0;
        if (stateOfGame[0] == 1)
            textureId = 17;
        else
            textureId = 20;

        if (lastAction.x == 0 && lastAction.y == 0)
        {
            lastAction.maskOfBuilding=DrawGround(hitray.collider.gameObject.GetComponent<HexagonClick>().x, hitray.collider.gameObject.GetComponent<HexagonClick>().y, BuildingId,16);
            lastAction.x = hitray.collider.gameObject.GetComponent<HexagonClick>().x;
            lastAction.y = hitray.collider.gameObject.GetComponent<HexagonClick>().y;
        }
        else
        {
            CleanDrawGround(lastAction.x, lastAction.y, BuildingId, textureId);
            lastAction.maskOfBuilding = DrawGround(hitray.collider.gameObject.GetComponent<HexagonClick>().x, hitray.collider.gameObject.GetComponent<HexagonClick>().y, BuildingId,16);
            lastAction.x = hitray.collider.gameObject.GetComponent<HexagonClick>().x;
            lastAction.y = hitray.collider.gameObject.GetComponent<HexagonClick>().y;
        }
    }
    public void ClickGroundAttack(int BuildingId, RaycastHit2D hitray)
    {
        int textureId = 0;
        if (stateOfGame[0] == 1)
            textureId = 20;
        else
            textureId = 17;

        if (lastAction.x == 0 && lastAction.y == 0)
        {
            lastAction.maskOfBuilding = DrawGroundAttack(hitray.collider.gameObject.GetComponent<HexagonClick>().x, hitray.collider.gameObject.GetComponent<HexagonClick>().y, BuildingId, 21);
            lastAction.x = hitray.collider.gameObject.GetComponent<HexagonClick>().x;
            lastAction.y = hitray.collider.gameObject.GetComponent<HexagonClick>().y;
        }
        else
        {
            CleanDrawGroundAttack(lastAction.x, lastAction.y, BuildingId, textureId);
            lastAction.maskOfBuilding = DrawGroundAttack(hitray.collider.gameObject.GetComponent<HexagonClick>().x, hitray.collider.gameObject.GetComponent<HexagonClick>().y, BuildingId, 21);
            lastAction.x = hitray.collider.gameObject.GetComponent<HexagonClick>().x;
            lastAction.y = hitray.collider.gameObject.GetComponent<HexagonClick>().y;
        }
    }
    public void ClickBuild(GameObject hitray)
    {
        
        if(stateOfGame[0] == 1)
        {
            PlayerBuildPoints -= hitray.gameObject.GetComponent<BuildButton>().costOfBuilding;
            stateOfGame[1] = 0;
            stateOfGame[2] = hitray.gameObject.GetComponent<BuildButton>().idOfBuilding;
            if (stateOfGame[2] == 1)
                BankEnergy += hitray.gameObject.GetComponent<BuildButton>().powerOfBuilding;
            else
                PlayerEnergyPoints += hitray.gameObject.GetComponent<BuildButton>().powerOfBuilding;

            lastAction.costOfBuilding = hitray.gameObject.GetComponent<BuildButton>().costOfBuilding;
            lastAction.timeOfBuilding = hitray.gameObject.GetComponent<BuildButton>().timeOfBuilding;
            lastAction.idOfBuilding = hitray.gameObject.GetComponent<BuildButton>().idOfBuilding;

            CurrentLayer = "Background";
            ActiveDeactiveElements[0].gameObject.SetActive(false);
            ActiveDeactiveElements[2].gameObject.SetActive(true);
            ActiveDeactiveElements[3].gameObject.SetActive(true);
            ActiveDeactiveElements[4].gameObject.SetActive(false);
        }
        if(stateOfGame[0] == 2)
        {
            EnemyBuildPoints -= hitray.gameObject.GetComponent<BuildButton>().costOfBuilding;
            stateOfGame[2] = hitray.gameObject.GetComponent<BuildButton>().idOfBuilding;
            if (stateOfGame[2] == 1)
                EnemyBankEnergy += hitray.gameObject.GetComponent<BuildButton>().powerOfBuilding;
            else
                EnemyEnergyPoints += hitray.gameObject.GetComponent<BuildButton>().powerOfBuilding;

            lastAction.costOfBuilding = hitray.gameObject.GetComponent<BuildButton>().costOfBuilding;
            lastAction.timeOfBuilding = hitray.gameObject.GetComponent<BuildButton>().timeOfBuilding;
            lastAction.idOfBuilding = hitray.gameObject.GetComponent<BuildButton>().idOfBuilding;

            CurrentLayer = "Background";
            ActiveDeactiveElements[0].gameObject.SetActive(false);
            ActiveDeactiveElements[2].gameObject.SetActive(true);
            ActiveDeactiveElements[3].gameObject.SetActive(true);
            ActiveDeactiveElements[4].gameObject.SetActive(false);
        }
    }
    public void ClickAttack(GameObject hitray)
    {
        if(stateOfGame[0] == 1)
        {
            PlayerAttackPoints -= hitray.gameObject.GetComponent<FireButton>().costOfAttack;
            stateOfGame[1] = 1;
            stateOfGame[2] = hitray.gameObject.GetComponent<FireButton>().idOfAttack;
            lastAction.costOfAttack = hitray.gameObject.GetComponent<FireButton>().costOfAttack;
            lastAction.timeOfAttack = hitray.gameObject.GetComponent<FireButton>().timeOfAttack;
            lastAction.idOfAttack = hitray.gameObject.GetComponent<FireButton>().idOfAttack;
            lastAction.countOfAttack = hitray.gameObject.GetComponent<FireButton>().countOfAttack;
            lastAction.countOfAttack -= 1;
            OutputFields[lastAction.idOfAttack + 3].text = lastAction.countOfAttack.ToString();

            if ((lastAction.countOfAttack - 1) < 0)
            {
                DisableButton(DisableButtons[lastAction.idOfAttack]);
                PlayerListOfAttacks.listOfAttacksCD[lastAction.idOfAttack] = true;
                PlayerListOfAttacks.GoAttack(lastAction.idOfAttack, new Vector2(lastAction.x, lastAction.y), lastAction.timeOfAttack, lastAction.countOfAttack);
            }

            CurrentLayer = "Background";
            ActiveDeactiveElements[1].gameObject.SetActive(false);
            ActiveDeactiveElements[2].gameObject.SetActive(true);
            ActiveDeactiveElements[3].gameObject.SetActive(true);
            ActiveDeactiveElements[4].gameObject.SetActive(false);
        }
        if (stateOfGame[0] == 2)
        {
            EnemyAttackPoints -= hitray.gameObject.GetComponent<FireButton>().costOfAttack;
            stateOfGame[1] = 1;
            stateOfGame[2] = hitray.gameObject.GetComponent<FireButton>().idOfAttack;
            lastAction.costOfAttack = hitray.gameObject.GetComponent<FireButton>().costOfAttack;
            lastAction.timeOfAttack = hitray.gameObject.GetComponent<FireButton>().timeOfAttack;
            lastAction.idOfAttack = hitray.gameObject.GetComponent<FireButton>().idOfAttack;
            lastAction.countOfAttack = hitray.gameObject.GetComponent<FireButton>().countOfAttack;
            lastAction.countOfAttack -= 1;
            OutputFields[lastAction.idOfAttack + 3].text = lastAction.countOfAttack.ToString();

            if ((lastAction.countOfAttack - 1) < 0)
            {
                DisableButton(DisableButtons[lastAction.idOfAttack]);
                EnemyListOfAttacks.listOfAttacksCD[lastAction.idOfAttack] = true;
                EnemyListOfAttacks.GoAttack(lastAction.idOfAttack, new Vector2(lastAction.x, lastAction.y), lastAction.timeOfAttack, lastAction.countOfAttack);
            }

            CurrentLayer = "Background";
            ActiveDeactiveElements[1].gameObject.SetActive(false);
            ActiveDeactiveElements[2].gameObject.SetActive(true);
            ActiveDeactiveElements[3].gameObject.SetActive(true);
            ActiveDeactiveElements[4].gameObject.SetActive(false);
        }
    }

    public void CleanDrawGround(int x, int y, int idBuilding, int ground)
    {
        int[] xSide = new int[] { 0, 1, 0, -1, -1, 0, 1 };
        int[] ySide = new int[] { -1, 1, 1, 1, 0, -1, 0 };
        int[] xSideEven = new int[] { 0, 1, 0, -1, -1, 0, 1 };
        int[] ySideEven = new int[] { -1, 0, 1, 0, -1, -1, -1 };
        int[,] position = new int[,] { { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 } };

        switch (idBuilding)
        {
            case 0:
                position[0, 0] = x;
                position[1, 0] = y;
                break;
            case 1:
                position[0, 0] = x;
                position[1, 0] = y;
                if (x % 2 == 0)
                {
                    position[0, 1] = x + xSideEven[6];
                    position[1, 1] = y + ySideEven[6];
                }
                else
                {
                    position[0, 1] = x + xSide[6];
                    position[1, 1] = y + ySide[6];
                }
                break;
            case 2:
                position[0, 0] = x;
                position[1, 0] = y;
                if (x % 2 == 0)
                {
                    position[0, 1] = x + xSideEven[0];
                    position[1, 1] = y + ySideEven[0];
                    position[0, 2] = x + xSideEven[4];
                    position[1, 2] = y + ySideEven[4];
                    position[0, 3] = x + xSideEven[6];
                    position[1, 3] = y + ySideEven[6];
                }
                else
                {
                    position[0, 1] = x + xSide[0];
                    position[1, 1] = y + ySide[0];
                    position[0, 2] = x + xSide[4];
                    position[1, 2] = y + ySide[4];
                    position[0, 3] = x + xSide[6];
                    position[1, 3] = y + ySide[6];
                }
                break;
            case 3:
                position[0, 0] = x;
                position[1, 0] = y;
                if (x % 2 == 0)
                {
                    position[0, 1] = x + xSideEven[4];
                    position[1, 1] = y + ySideEven[4];
                    position[0, 2] = x + xSideEven[1];
                    position[1, 2] = y + ySideEven[1];
                }
                else
                {
                    position[0, 1] = x + xSide[4];
                    position[1, 1] = y + ySide[4];
                    position[0, 2] = x + xSide[1];
                    position[1, 2] = y + ySide[1];
                }
                break;
            case 4:
                position[0, 0] = x;
                position[1, 0] = y;
                if (x % 2 == 0)
                {
                    position[0, 1] = x + xSideEven[4];
                    position[1, 1] = y + ySideEven[4];
                }
                else
                {
                    position[0, 1] = x + xSide[4];
                    position[1, 1] = y + ySide[4];
                }
                break;
            case 5:
                position[0, 0] = x;
                position[1, 0] = y;
                break;
            case 6:
                position[0, 0] = x;
                position[1, 0] = y;
                break;
            case 7:
                position = new int[2, 100];
                position = DrawHexagonCircle(x, y, 3);
                break;
        }
        for (int i = 0; i < position.GetLength(1); i++)
        {
            //Debug.Log(i);
            if (MapLoader.a.grid[position[0, i], position[1, i]].gameObject.name.Equals("Water"))
            {
                MapLoader.a.grid[position[0, i], position[1, i]].gameObject.GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[18];
            }
            else
            {
                if (PlayerOccupiedBuildingCells.IsOccupied(new int[,] { { position[0, i] }, { position[1, i] } })|| EnemyOccupiedBuildingCells.IsOccupied(new int[,] { { position[0, i] }, { position[1, i] } }))
                {
                    MapLoader.a.grid[position[0, i], position[1, i]].gameObject.GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[ground];
                }
                else
                    MapLoader.a.grid[position[0, i], position[1, i]].gameObject.GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[ground];
            }

        }
        
    }
    public int[,] DrawGround(int x, int y, int idBuilding, int ground)
    {
        int[] xSide = new int[] { 0, 1, 0, -1, -1, 0, 1 };
        int[] ySide = new int[] { -1, 1, 1, 1, 0, -1, 0 };
        int[] xSideEven = new int[] { 0, 1, 0, -1, -1, 0, 1 };
        int[] ySideEven = new int[] { -1, 0, 1, 0, -1, -1, -1 };
        int[,] position= new int[,] {{ 0, 0, 0, 0},{ 0, 0, 0, 0 } };
          

        switch (idBuilding)
        {
            case 0:
                position[0, 0] = x;
                position[1, 0] = y;
                break;
            case 1:
                position[0, 0] = x;
                position[1, 0] = y;
                if (x % 2 == 0)
                {
                    position[0, 1] = x+ xSideEven[6];
                    position[1, 1] = y+ ySideEven[6];
                }
                else
                {
                    position[0, 1] = x+ xSide[6];
                    position[1, 1] = y+ ySide[6];
                }
                break;
            case 2:
                position[0, 0] = x;
                position[1, 0] = y;
                if (x % 2 == 0)
                {
                    position[0, 1] = x + xSideEven[0];
                    position[1, 1] = y + ySideEven[0];
                    position[0, 2] = x + xSideEven[4];
                    position[1, 2] = y + ySideEven[4];
                    position[0, 3] = x + xSideEven[6];
                    position[1, 3] = y + ySideEven[6];
                }
                else
                {
                    position[0, 1] = x + xSide[0];
                    position[1, 1] = y + ySide[0];
                    position[0, 2] = x + xSide[4];
                    position[1, 2] = y + ySide[4];
                    position[0, 3] = x + xSide[6];
                    position[1, 3] = y + ySide[6];
                }
                break;
            case 3:
                position[0, 0] = x;
                position[1, 0] = y;
                if (x % 2 == 0)
                {
                    position[0, 1] = x + xSideEven[4];
                    position[1, 1] = y + ySideEven[4];
                    position[0, 2] = x + xSideEven[1];
                    position[1, 2] = y + ySideEven[1];
                }
                else
                {
                    position[0, 1] = x + xSide[4];
                    position[1, 1] = y + ySide[4];
                    position[0, 2] = x + xSide[1];
                    position[1, 2] = y + ySide[1];
                }
                break;
            case 4:
                position[0, 0] = x;
                position[1, 0] = y;
                if (x % 2 == 0)
                {
                    position[0, 1] = x + xSideEven[4];
                    position[1, 1] = y + ySideEven[4];
                }
                else
                {
                    position[0, 1] = x + xSide[4];
                    position[1, 1] = y + ySide[4];
                }
                break;
            case 5:
                position[0, 0] = x;
                position[1, 0] = y;
                break;
            case 6:
                position[0, 0] = x;
                position[1, 0] = y;
                break;
            case 7:
                position = new int[2, 100];
                position = DrawHexagonCircle(x, y, 1);
                break;
        }
        bool err = false;
        for (int i = 0; i < position.GetLength(1); i++)
        {
           if (MapLoader.a.grid[position[0, i], position[1, i]].gameObject.name.Equals("Water") || PlayerOccupiedBuildingCells.IsOccupied(new int[,] { { position[0, i] }, { position[1, i] } }) || EnemyOccupiedBuildingCells.IsOccupied(new int[,] { { position[0, i] }, { position[1, i] } }))
            {
                if (position[0, i] != 0 && position[1, i] != 0 && idBuilding != 7)
                {
                    err = true;
                    MapLoader.a.grid[position[0, i], position[1, i]].gameObject.GetComponent<SpriteRenderer>().sprite = MapLoader.a.Buildings[15];
                }
                if (idBuilding == 7)
                {
                    MapLoader.a.grid[position[0, i], position[1, i]].gameObject.GetComponent<HexagonClick>().shield = 1;
                    position[0, i]=0;
                    position[1, i]=0;
                }
            }
            else
            {
                MapLoader.a.grid[position[0, i], position[1, i]].gameObject.GetComponent<SpriteRenderer>().sprite = MapLoader.a.Buildings[ground];
                if (idBuilding == 7)
                {
                    MapLoader.a.grid[position[0, i], position[1, i]].gameObject.GetComponent<HexagonClick>().shield = 1;
                    position[0, i] = 0;
                    position[1, i] = 0;
                }
            }
        }
        if(err)
            ActiveDeactiveElements[2].gameObject.SetActive(false);
        else
            ActiveDeactiveElements[2].gameObject.SetActive(true);

       // if(stateOfGame[0] == 2)
         //   ActiveDeactiveElements[2].gameObject.SetActive(false);
        return position;
    }
    public void DrawBuildings(int x, int y, int idBuilding) {
        int[] xSide = new int[] { 0, 1, 0, -1, -1, 0, 1 };
        int[] ySide = new int[] { -1, 1, 1, 1, 0, -1, 0 };
        int[] xSideEven = new int[] { 0, 1, 0, -1, -1, 0, 1 };
        int[] ySideEven = new int[] { -1, 0, 1, 0, -1, -1, -1 };

        
        switch (idBuilding) {
            case 0:if (MapLoader.a.grid[x, y].gameObject.transform.childCount > 0)
                        MapLoader.a.grid[x, y].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[0];
                    break;
            case 1:
                if (MapLoader.a.grid[x, y].gameObject.transform.childCount > 0)
                {
                    MapLoader.a.grid[x, y].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[1];
                
                if (x % 2 == 0)
                    {
                        MapLoader.a.grid[x + xSideEven[6], y + ySideEven[6]].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[7];
                    }
                    else
                    {
                        MapLoader.a.grid[x + xSide[6], y + ySide[6]].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[7];
                    }
                }
                   break;
            case 2:
                if (MapLoader.a.grid[x, y].gameObject.transform.childCount > 0)
                {
                   MapLoader.a.grid[x, y].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[13];
                    MapLoader.a.grid[x, y].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sortingOrder = 2;
                    if (x % 2 == 0)
                    {
                        MapLoader.a.grid[x + xSideEven[0], y + ySideEven[0]].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[8];
                        MapLoader.a.grid[x + xSideEven[4], y + ySideEven[4]].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[2];
                        MapLoader.a.grid[x + xSideEven[6], y + ySideEven[6]].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[11];
                        MapLoader.a.grid[x + xSideEven[6], y + ySideEven[6]].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sortingOrder = 2;
                    }
                    else
                    {
                        MapLoader.a.grid[x + xSide[0], y + ySide[0]].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[8];
                        MapLoader.a.grid[x + xSide[4], y + ySide[4]].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[2];
                        MapLoader.a.grid[x + xSide[6], y + ySide[6]].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[11];
                        MapLoader.a.grid[x + xSide[6], y + ySide[6]].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sortingOrder = 2;
                    }
                }
 
                    break; 
            case 3:
                if (MapLoader.a.grid[x, y].gameObject.transform.childCount > 0)
                {
                    MapLoader.a.grid[x, y].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[9];
                    if (x % 2 == 0)
                    {
                        MapLoader.a.grid[x + xSideEven[4], y + ySideEven[4]].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[3];
                        MapLoader.a.grid[x + xSideEven[1], y + ySideEven[1]].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[12];
                    }
                    else
                    {
                        MapLoader.a.grid[x + xSide[4], y + ySide[4]].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[3];
                        MapLoader.a.grid[x + xSide[1], y + ySide[1]].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[12];
                    }
                }
                    break;
            case 4:
                if (MapLoader.a.grid[x, y].gameObject.transform.childCount > 0)
                {
                    MapLoader.a.grid[x, y].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[10];
                    if (x % 2 == 0)
                    {
                        MapLoader.a.grid[x + xSideEven[4], y + ySideEven[4]].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[4];

                    }
                   else
                    {
                        MapLoader.a.grid[x + xSide[4], y + ySide[4]].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[4];
                    }
                }
                    break;
            case 5:
                if (MapLoader.a.grid[x, y].gameObject.transform.childCount > 0) MapLoader.a.grid[x, y].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[5];
                break;
            case 6:
                if (MapLoader.a.grid[x, y].gameObject.transform.childCount > 0) MapLoader.a.grid[x, y].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[6];
                break;
        }

    }
    public void DestroyBuildings(int x, int y, int idBuilding)
    {
        int[] xSide = new int[] { 0, 1, 0, -1, -1, 0, 1 };
        int[] ySide = new int[] { -1, 1, 1, 1, 0, -1, 0 };
        int[] xSideEven = new int[] { 0, 1, 0, -1, -1, 0, 1 };
        int[] ySideEven = new int[] { -1, 0, 1, 0, -1, -1, -1 };

        switch (idBuilding)
        {
            case 0:
                if (MapLoader.a.grid[x, y].gameObject.transform.childCount > 0)
                          MapLoader.a.grid[x, y].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = null;
                break;
            case 1:
                if (MapLoader.a.grid[x, y].gameObject.transform.childCount > 0)
                {
                    MapLoader.a.grid[x, y].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = null;
                    if (x % 2 == 0)
                    {
                        MapLoader.a.grid[x + xSideEven[6], y + ySideEven[6]].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = null;
                    }
                    else
                    {
                        MapLoader.a.grid[x + xSide[6], y + ySide[6]].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = null;
                    }

                }
               
                break;
            case 2:
                if (MapLoader.a.grid[x, y].gameObject.transform.childCount > 0)
                {
                    MapLoader.a.grid[x, y].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = null;
                
                    if (x % 2 == 0)
                    {
                        MapLoader.a.grid[x + xSideEven[0], y + ySideEven[0]].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = null;
                        MapLoader.a.grid[x + xSideEven[4], y + ySideEven[4]].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = null;
                        MapLoader.a.grid[x + xSideEven[6], y + ySideEven[6]].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = null;
                    }
                    else
                    {
                        MapLoader.a.grid[x + xSide[0], y + ySide[0]].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = null;
                        MapLoader.a.grid[x + xSide[4], y + ySide[4]].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = null;
                        MapLoader.a.grid[x + xSide[6], y + ySide[6]].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = null;
                    }
                }
     
                break;
            case 3:
                if (MapLoader.a.grid[x, y].gameObject.transform.childCount > 0)
                {
                    MapLoader.a.grid[x, y].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = null;
                    if (x % 2 == 0)
                    {
                        MapLoader.a.grid[x + xSideEven[4], y + ySideEven[4]].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = null;
                        MapLoader.a.grid[x + xSideEven[1], y + ySideEven[1]].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = null;
                    }
                    else
                    {
                        MapLoader.a.grid[x + xSide[4], y + ySide[4]].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = null;
                        MapLoader.a.grid[x + xSide[1], y + ySide[1]].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = null;
                    }
                }

                break;
            case 4:
                if (MapLoader.a.grid[x, y].gameObject.transform.childCount > 0)
                {
                    MapLoader.a.grid[x, y].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = null;
                    if (x % 2 == 0)
                    {
                        MapLoader.a.grid[x + xSideEven[4], y + ySideEven[4]].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = null;
                    }
                    else
                    {
                        MapLoader.a.grid[x + xSide[4], y + ySide[4]].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = null;
                    }
                }
 
                break;
            case 5: if (MapLoader.a.grid[x, y].gameObject.transform.childCount > 0) MapLoader.a.grid[x, y].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = null; break;
            case 6: if (MapLoader.a.grid[x, y].gameObject.transform.childCount > 0) MapLoader.a.grid[x, y].gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = null; break;
        }
    }

    public void CleanDrawGroundAttack(int x, int y, int dAttack, int ground)
    {
        int[] xSide = new int[] { 0, 1, 0, -1, -1, 0, 1 };
        int[] ySide = new int[] { -1, 1, 1, 1, 0, -1, 0 };
        int[] xSideEven = new int[] { 0, 1, 0, -1, -1, 0, 1 };
        int[] ySideEven = new int[] { -1, 0, 1, 0, -1, -1, -1 };
        int[,] position = new int[,] { { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };


        switch (dAttack)
        {
            case 0:
                position[0, 0] = x;
                position[1, 0] = y;
                break;
            case 1:
                position[0, 0] = x;
                position[1, 0] = y;
                if (x % 2 == 0)
                {
                    position[0, 1] = x + xSideEven[0];
                    position[1, 1] = y + ySideEven[0];
                    position[0, 2] = x + xSideEven[4];
                    position[1, 2] = y + ySideEven[4];
                    position[0, 3] = x + xSideEven[6];
                    position[1, 3] = y + ySideEven[6];
                }
                else
                {
                    position[0, 1] = x + xSide[0];
                    position[1, 1] = y + ySide[0];
                    position[0, 2] = x + xSide[4];
                    position[1, 2] = y + ySide[4];
                    position[0, 3] = x + xSide[6];
                    position[1, 3] = y + ySide[6];

                }
                break;
            case 2:
                position[0, 0] = x;
                position[1, 0] = y;
                if (x % 2 == 0)
                {
                    position[0, 1] = x + xSideEven[6];
                    position[1, 1] = y + ySideEven[6];
                    position[0, 2] = x + xSideEven[3];
                    position[1, 2] = y + ySideEven[3];
                }
                else
                {
                    position[0, 1] = x + xSide[6];
                    position[1, 1] = y + ySide[6];
                    position[0, 2] = x + xSide[3];
                    position[1, 2] = y + ySide[3];
                }
                break;

            case 3:
                position = new int[2, 100];
                position = DrawHexagonCircle(x, y, 2);
                break;
            case 4:
                position = new int[2, 100];
                position = DrawHexagonCircle(x, y, 3);
                break;
            case 5:
                position = new int[2, 100];
                position = DrawHexagonCircle(x, y, 1);
                break;

        }
        for (int i = 0; i < position.GetLength(1); i++)
        {
            if (MapLoader.a.grid[position[0, i], position[1, i]].gameObject.name.Equals("Water"))
            {
               // MapLoader.a.grid[position[0, i], position[1, i]].gameObject.GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[18];
            }
            else
            {
                if (PlayerOccupiedAttackCells.IsOccupied(new int[,] { { position[0, i] }, { position[1, i] } }) || EnemyOccupiedBuildingCells.IsOccupied(new int[,] { { position[0, i] }, { position[1, i] } }))
                {
                    //  MapLoader.a.grid[position[0, i], position[1, i]].gameObject.GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[16];
                }
                else
                {
                    MapLoader.a.grid[position[0, i], position[1, i]].gameObject.GetComponent<SpriteRenderer>().sprite = MapLoader.a.Buildings[ground];
                }
                    
            }
        }

    }
    public int[,] DrawGroundAttack(int x, int y, int dAttack, int ground)
    {
        int[] xSide = new int[] { 0, 1, 0, -1, -1, 0, 1 };
        int[] ySide = new int[] { -1, 1, 1, 1, 0, -1, 0 };
        int[] xSideEven = new int[] { 0, 1, 0, -1, -1, 0, 1 };
        int[] ySideEven = new int[] { -1, 0, 1, 0, -1, -1, -1 };
        int[,] position = new int[,] { { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };


        switch (dAttack)
        {
            case 0:
                position[0, 0] = x;
                position[1, 0] = y;
                break;
            case 1:
                position[0, 0] = x;
                position[1, 0] = y;
                if (x % 2 == 0)
                {
                    position[0, 1] = x + xSideEven[0];
                    position[1, 1] = y + ySideEven[0];
                    position[0, 2] = x + xSideEven[4];
                    position[1, 2] = y + ySideEven[4];
                    position[0, 3] = x + xSideEven[6];
                    position[1, 3] = y + ySideEven[6];
                }
                else
                {
                    position[0, 1] = x + xSide[0];
                    position[1, 1] = y + ySide[0];
                    position[0, 2] = x + xSide[4];
                    position[1, 2] = y + ySide[4];
                    position[0, 3] = x + xSide[6];
                    position[1, 3] = y + ySide[6];

                }
                break;
            case 2:
                position[0, 0] = x;
                position[1, 0] = y;
                if (x % 2 == 0)
                {
                    position[0, 1] = x + xSideEven[6];
                    position[1, 1] = y + ySideEven[6];
                    position[0, 2] = x + xSideEven[3];
                    position[1, 2] = y + ySideEven[3];
                }
                else
                {
                    position[0, 1] = x + xSide[6];
                    position[1, 1] = y + ySide[6];
                    position[0, 2] = x + xSide[3];
                    position[1, 2] = y + ySide[3];
                }
                break;

            case 3:
                position = new int[2, 100];
                position = DrawHexagonCircle(x, y, 2);
                break;
            case 4:
                position = new int[2, 100];
                position = DrawHexagonCircle(x, y, 3);
                break;
            

        }
        for (int i = 0; i < position.GetLength(1); i++)
        {
                if (MapLoader.a.grid[position[0, i], position[1, i]].gameObject.name.Equals("Water") || PlayerOccupiedAttackCells.IsOccupied(new int[,] { { position[0, i] }, { position[1, i] } }) || EnemyOccupiedAttackCells.IsOccupied(new int[,] { { position[0, i] }, { position[1, i] } }))
                {
             //       MapLoader.a.grid[position[0, i], position[1, i]].gameObject.GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[15];
                }
                else
                {
                    MapLoader.a.grid[position[0, i], position[1, i]].gameObject.GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[ground];
                }
        }

        return position;
    }
    public void DrawAttack(int x, int y, int idAttack)
     {
         int[] xSide = new int[] { 0, 1, 0, -1, -1, 0, 1 };
         int[] ySide = new int[] { -1, 1, 1, 1, 0, -1, 0 };
         int[] xSideEven = new int[] { 0, 1, 0, -1, -1, 0, 1 };
         int[] ySideEven = new int[] { -1, 0, 1, 0, -1, -1, -1 };

         switch (idAttack)
         {
             case 0:
                 MapLoader.a.grid[x, y].gameObject.GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[19];
                  break;
             case 1:
                 MapLoader.a.grid[x, y].gameObject.GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[19];
                 if (x % 2 == 0)
                 {
                     MapLoader.a.grid[x + xSideEven[0], y + ySideEven[0]].gameObject.GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[19];
                     MapLoader.a.grid[x + xSideEven[4], y + ySideEven[4]].gameObject.GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[19];
                     MapLoader.a.grid[x + xSideEven[6], y + ySideEven[6]].gameObject.GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[19];
                 }
                 else
                 {
                     MapLoader.a.grid[x + xSide[0], y + ySide[0]].gameObject.GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[19];
                     MapLoader.a.grid[x + xSide[4], y + ySide[4]].gameObject.GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[19];
                     MapLoader.a.grid[x + xSide[6], y + ySide[6]].gameObject.GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[19];

                 }
                 break;
             case 2:
                 MapLoader.a.grid[x, y].gameObject.GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[19];
                 if (x % 2 == 0)
                 {
                     MapLoader.a.grid[x + xSideEven[6], y + ySideEven[6]].gameObject.GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[19];
                     MapLoader.a.grid[x + xSideEven[3], y + ySideEven[3]].gameObject.GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[19];
                 }
                 else
                 {
                     MapLoader.a.grid[x + xSide[6], y + ySide[6]].gameObject.GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[19];
                     MapLoader.a.grid[x + xSide[3], y + ySide[3]].gameObject.GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[19];
                 }
                 break;
             case 3:

                 DrawHexagonCircle(x, y, 2);

                 break;
             case 4:
                 DrawHexagonCircle(x, y, 3);
                 break;
         }

     }

   public void DrawAttack(int x, int y, int dAttack, int ground, OccupiedCells Target)
    {
        int[] xSide = new int[] { 0, 1, 0, -1, -1, 0, 1 };
        int[] ySide = new int[] { -1, 1, 1, 1, 0, -1, 0 };
        int[] xSideEven = new int[] { 0, 1, 0, -1, -1, 0, 1 };
        int[] ySideEven = new int[] { -1, 0, 1, 0, -1, -1, -1 };
        int[,] position = new int[,] { { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };


        switch (dAttack)
        {
            case 0:
                position[0, 0] = x;
                position[1, 0] = y;
                break;
            case 1:
                position[0, 0] = x;
                position[1, 0] = y;
                if (x % 2 == 0)
                {
                    position[0, 1] = x + xSideEven[0];
                    position[1, 1] = y + ySideEven[0];
                    position[0, 2] = x + xSideEven[4];
                    position[1, 2] = y + ySideEven[4];
                    position[0, 3] = x + xSideEven[6];
                    position[1, 3] = y + ySideEven[6];
                }
                else
                {
                    position[0, 1] = x + xSide[0];
                    position[1, 1] = y + ySide[0];
                    position[0, 2] = x + xSide[4];
                    position[1, 2] = y + ySide[4];
                    position[0, 3] = x + xSide[6];
                    position[1, 3] = y + ySide[6];

                }
                break;
            case 2:
                position[0, 0] = x;
                position[1, 0] = y;
                if (x % 2 == 0)
                {
                    position[0, 1] = x + xSideEven[6];
                    position[1, 1] = y + ySideEven[6];
                    position[0, 2] = x + xSideEven[3];
                    position[1, 2] = y + ySideEven[3];
                }
                else
                {
                    position[0, 1] = x + xSide[6];
                    position[1, 1] = y + ySide[6];
                    position[0, 2] = x + xSide[3];
                    position[1, 2] = y + ySide[3];
                }
                break;

            case 3:
                position = new int[2, 100];
                position = DrawHexagonCircle(x, y, 2);
                break;
            case 4:
                position = new int[2, 100];
                position = DrawHexagonCircle(x, y, 3);
                break;


        }
        for (int i = 0; i < position.GetLength(1); i++)
        {
            if (MapLoader.a.grid[position[0, i], position[1, i]].gameObject.name.Equals("Water"))
            {
               // MapLoader.a.grid[position[0, i], position[1, i]].gameObject.GetComponentInChildren<SpriteRenderer>().sprite = MapLoader.a.Buildings[15];
            }

            if (Target.IsOccupied(new int[,] { { position[0, i] }, { position[1, i] } }))
            {
                if (dAttack == 3)
                {
                    MapLoader.a.grid[position[0, i], position[1, i]].gameObject.GetComponent<SpriteRenderer>().sprite = MapLoader.a.Buildings[17];
                    if (MapLoader.a.grid[position[0, i], position[1, i]].gameObject.transform.childCount > 0)
                    {
                        // GameObject temp = MapLoader.a.grid[position[0, i], position[1, i]].gameObject.transform.GetChild(0).gameObject;
                        // temp.GetComponent<SpriteRenderer>().enabled = true;
                        MapLoader.a.grid[position[0, i], position[1, i]].gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 1;
                    }
                }
                else
                {
                    if (MapLoader.a.grid[position[0, i], position[1, i]].gameObject.transform.childCount > 0)
                    {
                        MapLoader.a.grid[position[0, i], position[1, i]].gameObject.transform.GetChild(1).GetComponent<Animator>().enabled = true;
                        MapLoader.a.grid[position[0, i], position[1, i]].gameObject.transform.GetChild(1).GetComponent<Animator>().Play("Idle");
                    }

                    if (MapLoader.a.grid[position[0, i], position[1, i]].gameObject.GetComponent<HexagonClick>().shield <= 0)
                    {
                        MapLoader.a.grid[position[0, i], position[1, i]].gameObject.GetComponent<SpriteRenderer>().sprite = MapLoader.a.Buildings[22];
                        DestroyBuildings(position[0, i], position[1, i], dAttack);
                    } 
                    else
                        MapLoader.a.grid[position[0, i], position[1, i]].gameObject.GetComponent<HexagonClick>().shield = 0;
                }
                    
            }
            else
            {
                if (dAttack == 3)
                {
                    MapLoader.a.grid[position[0, i], position[1, i]].gameObject.GetComponent<SpriteRenderer>().sprite = MapLoader.a.Buildings[17];
                    if (MapLoader.a.grid[position[0, i], position[1, i]].gameObject.transform.childCount > 0)
                    {
                        //GameObject temp = MapLoader.a.grid[position[0, i], position[1, i]].gameObject.transform.GetChild(0).gameObject;
                        //temp.GetComponent<SpriteRenderer>().enabled = true;
                        MapLoader.a.grid[position[0, i], position[1, i]].gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 1;
                    }
                }
                else
                {
                    if (MapLoader.a.grid[position[0, i], position[1, i]].gameObject.transform.childCount > 0)
                    {
                        MapLoader.a.grid[position[0, i], position[1, i]].gameObject.transform.GetChild(1).GetComponent<Animator>().enabled = true;
                        MapLoader.a.grid[position[0, i], position[1, i]].gameObject.transform.GetChild(1).GetComponent<Animator>().Play("Idle");
                    }
                    if (MapLoader.a.grid[position[0, i], position[1, i]].gameObject.GetComponent<HexagonClick>().shield <= 0)
                    {
                        MapLoader.a.grid[position[0, i], position[1, i]].gameObject.GetComponent<SpriteRenderer>().sprite = MapLoader.a.Buildings[ground];
                    }
                    else
                    {
                        MapLoader.a.grid[position[0, i], position[1, i]].gameObject.GetComponent<HexagonClick>().shield = 0;
                    }
                }
            }
                
        }
    }

    public int[,] DrawHexagonCircle(int x, int y, int radius) {
        int[] xSide = new int[] { 0, 1, 0, -1, -1, 0, 1 };
        int[] ySide = new int[] { -1, 1, 1, 1, 0, -1, 0 };
        int[] xSideEven = new int[] { 0, 1, 0, -1, -1, 0, 1 };
        int[] ySideEven = new int[] { -1, 0, 1, 0, -1, -1, -1 };
        int[,] position = new int[2,100];
        int counter = 1;
        // MapLoader.a.grid[x, y].gameObject.GetComponentInChildren<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 0.8f);
        position[0, 0] = x;
        position[1, 0] = y;
        int target = 0;
        Vector2 pos = new Vector2(x,y);
        for (int a = 0; a < radius; a++)
        {
            if (radius == target) break;
            else
            {
                target++;
                x=(int)pos.x;
                y=(int)pos.y;
            }
             for (int j = 0; j < 7; j++)
            {
                for (int i = 0; i < target; i++)
                {
                    if (x % 2 == 0)
                    {
                       // MapLoader.a.grid[x + xSideEven[j], y + ySideEven[j]].gameObject.GetComponentInChildren<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 0.8f);
                        x = x + xSideEven[j];
                        y = y + ySideEven[j];
                        position[0, counter] = x;
                        position[1, counter] = y;
                        counter++;
                    }
                    else
                    {
                       // MapLoader.a.grid[x + xSide[j], y + ySide[j]].gameObject.GetComponentInChildren<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 0.8f);
                        x = x + xSide[j];
                        y = y + ySide[j];
                        position[0, counter] = x;
                        position[1, counter] = y;
                        counter++;
                    }
                }
            }
        }

        return position;
    }
    public int[] sectionCoords(int i)
    {
        int x = i % MapLoader.a.mapSizesH;
        if (x == 0)
        {
            x = MapLoader.a.mapSizesH;
        }
        int y = ((i - x) / MapLoader.a.mapSizesH);
        x -= 1;
        return new int[] { x, y };
    }
    void StartScene()
    {
        _image.enabled = true;
        _image.color = Color.Lerp(_image.color, Color.clear, fadeSpeed * Time.deltaTime);

        if (_image.color.a <= 0.01f)
        {
            _image.color = Color.clear;
            _image.enabled = false;
            sceneStarting = false;
            Cursor.visible = true;

        }

    }
}
