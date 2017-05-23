using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICtrl : MonoBehaviour {
    public GameObject[] UIElements;
    public static UICtrl uiCtrl;
	// Use this for initialization

	void Start () {
        if (uiCtrl == null)
        {
            uiCtrl = this;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void hideBuildBar(bool hide) {
        UIElements[0].gameObject.SetActive(hide);
    }
    public void hideAttackBar(bool hide) {
        UIElements[1].gameObject.SetActive(hide);
    }
    public void hideConfirm(bool hide) {
        UIElements[2].gameObject.SetActive(hide);
    }
    public void hideDenide(bool hide) {
        UIElements[3].gameObject.SetActive(hide);
    }
    public void hidePass(bool hide) {
        UIElements[4].gameObject.SetActive(hide);
    }
    public void hideButtons(bool hide) {
        foreach (Button temp in UIElements[0].gameObject.GetComponentsInChildren<Button>())
            temp.interactable = hide;
    }
}
