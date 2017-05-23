using UnityEngine;
using System.Collections;

public class CameraFollow2D : MonoBehaviour {

    //public Transform target;        //target for the camera to follow
    public float xOffset=0f;           //how much x-axis space should be between the camera and target
    public void MoveCam(float n) {
        //follow the target on the x-axis only
        
        transform.position = new Vector3(transform.position.x + xOffset+n, transform.position.y, transform.position.z);
        
    }
    void Update()
    {
        /*if (GameControll.controller.CameraMove == 1)
        {
            MoveCam(19.5f);
            GameControll.controller.CameraMove = 2;


        }
        if (GameControll.controller.CameraMove == 0)
        {
                MoveCam(-19.5f);
                GameControll.controller.CameraMove = 2;
        }
                
        */
    }
}
