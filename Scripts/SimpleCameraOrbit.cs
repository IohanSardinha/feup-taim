using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraOrbit : MonoBehaviour
{
    private Camera camera;
    public bool active;

    private int touchCount = 0;
    private Vector2 lastMousePosition;
    float ZOOM_FACTOR = 0.1f;
    float MINUMIN_DISTANCE_ZOOM = 25f;
    float Y_ROTATATION_FACTOR = 2f;
    float X_ROTATION_FACTOR = 2f;
    float Y_MOVEMENT_SPEED = 0.6f;

    float MOVEMENT_SPEED = 0.3f;
    bool isZooming = false, isRotating = false;
    

    bool move(){
       // Touch touch1 = Input.GetTouch(0), touch2 = Input.GetTouch(1);
        //Vector2 touchPos = Vector2.Lerp(touch1.position, touch2.position, 0.5f);
        Touch touch = Input.GetTouch(0);
        if(touch.phase == TouchPhase.Began){
            lastMousePosition = touch.position;
            return true;
        }

        Vector2 direction = touch.position - lastMousePosition;
        Vector3 dir = Quaternion.Euler(0,transform.localRotation.eulerAngles.y - 180, 0) * new Vector3(direction.x, 0, direction.y);
        Vector3 transaltion = transform.InverseTransformDirection(dir);

        if(transform.localEulerAngles.x < 15)
            transform.Translate(transaltion.x*MOVEMENT_SPEED*Time.deltaTime, transaltion.y*Y_MOVEMENT_SPEED*Time.deltaTime,0);            
        else if(transform.localEulerAngles.x > 345)
            transform.Translate(transaltion.x*MOVEMENT_SPEED*Time.deltaTime, -transaltion.y*Y_MOVEMENT_SPEED*Time.deltaTime,0);
        else
            transform.Translate(transaltion*MOVEMENT_SPEED*Time.deltaTime);

        lastMousePosition = touch.position;

        return true;
    }

    bool rotate(){
        Touch touch = Input.GetTouch(0);
        if(touch.phase == TouchPhase.Began){
            lastMousePosition = touch.position;
            return true;
        }
        if(touch.phase != TouchPhase.Moved)
            return false;
        
        if(Mathf.Abs(touch.position.x - lastMousePosition.x) > Mathf.Abs(touch.position.y - lastMousePosition.y))
            transform.RotateAround(camera.transform.position, Vector3.up, -(touch.position.x - lastMousePosition.x) *  Y_ROTATATION_FACTOR * Time.deltaTime);
        else
            transform.Rotate(-(touch.position.y - lastMousePosition.y) * X_ROTATION_FACTOR * Time.deltaTime, 0, 0);

        lastMousePosition = touch.position;
        
        isRotating = true;
        return true;
    }

    bool zoom(){
        Touch touch1 = Input.GetTouch(0), touch2 = Input.GetTouch(1);
        
        if(touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began){
            
            lastMousePosition = touch1.position;
            return true;
        }
        if(touch1.phase != TouchPhase.Moved || touch2.phase != TouchPhase.Moved)
            return false;
        
        Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
        Vector2 touch2PrevPos = touch2.position - touch2.deltaPosition;
        float prevMag = (touch1PrevPos - touch2PrevPos).magnitude;
        float currMag = (touch1.position - touch2.position).magnitude;

        float zoomVal = currMag - prevMag;

        Vector2 touch1Dir = touch1.position - touch1PrevPos;
        Vector2 touch2Dir = touch2.position - touch2PrevPos;

        if(Vector2.Angle(touch1Dir, touch2Dir) < 90)
            return false;

        transform.Translate(0,0, zoomVal * ZOOM_FACTOR * Time.deltaTime);
        
        isZooming = true;
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
        lastMousePosition = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {


        if(!active) return;
        touchCount = Input.touchCount;
        switch(touchCount){
            case 1:
                if(!isZooming && !isRotating)
                    move();
                break;
            case 2:
                if(zoom() && !isRotating)
                    return;
                if(!isZooming)
                    rotate();
                break;
            case 0:
                isZooming = false;
                isRotating = false;
                break;
        }     
    }
}
