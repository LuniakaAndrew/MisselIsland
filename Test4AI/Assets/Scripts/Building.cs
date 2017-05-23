using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour {

    public int type;
    public int cost;
    public int buildTime;
    public int power;
    public int[] mask;
    private Vector2 position;
    private int[] xSide;
    private int[] ySide;
    private int[] xSideEven;
    private int[] ySideEven;
    // Use this for initialization
    void Start () {
        xSide = new int[] { 0, 1, 0, -1, -1, 0, 1 };
        ySide = new int[] { -1, 1, 1, 1, 0, -1, 0 };
        xSideEven = new int[] { 0, 1, 0, -1, -1, 0, 1 };
        ySideEven = new int[] { -1, 0, 1, 0, -1, -1, -1 };
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void Draw(Vector2[] posArr) {
         for (int i = 0; i < mask.Length; i++) {
            if (posArr[0].x % 2 == 0)
            {
                posArr[i+1] = new Vector2(posArr[0].x + xSideEven[mask[i]], posArr[0].y + ySideEven[mask[i]]);
            }
            else
            {
                posArr[i+1] = new Vector2(posArr[0].x + xSide[mask[i]], posArr[0].y + ySide[mask[i]]);
            }
        }
        Debug.Log(posArr.ToString());
    }

}
