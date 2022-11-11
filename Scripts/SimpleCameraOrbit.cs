using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraOrbit : MonoBehaviour
{
    private Camera camera;
    public bool active;

    private int touchCount = 0;
    private Vector2 lastMousePosition;
    private float lastTouchesDistances;
    float ZOOM_FACTOR = 0.1f;
    float MINUMIN_DISTANCE_ZOOM = 5f;
    float Y_ROTATATION_FACTOR = 2f;
    float X_ROTATION_FACTOR = 2f;
    float MOVEMENT_SPEED = 0.7f;

    bool move(){
       // Touch touch1 = Input.GetTouch(0), touch2 = Input.GetTouch(1);
        //Vector2 touchPos = Vector2.Lerp(touch1.position, touch2.position, 0.5f);
        Touch touch = Input.GetTouch(0);
        if(touch.phase == TouchPhase.Began){
            lastMousePosition = touch.position;
            return true;
        }

        Vector2 direction = touch.position - lastMousePosition;
        Vector3 transaltion = transform.InverseTransformDirection(new Vector3(direction.x, 0, direction.y));
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
        
        return true;
    }

    bool zoom(){
        Touch touch1 = Input.GetTouch(0), touch2 = Input.GetTouch(1);
        if(touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began){
            lastTouchesDistances = Vector2.Distance(touch1.position, touch2.position);
            lastMousePosition = touch1.position;
            return true;
        }
        if(touch1.phase != TouchPhase.Moved || touch2.phase != TouchPhase.Moved)
            return false;
        
        float zoomVal = lastTouchesDistances - Vector2.Distance(touch1.position, touch2.position);
        if(Mathf.Abs(zoomVal) < MINUMIN_DISTANCE_ZOOM)
            return false;

        camera.fieldOfView += zoomVal * ZOOM_FACTOR * Time.deltaTime;
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
                move();
                break;
            case 2:
                if(zoom())
                    return;
                rotate();
                break;
        }     
    }
}
