using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    SimpleCameraOrbit cameraNavigation;

    public GameObject selectedObject = null;
    public GameObject target = null;

    float plane_height = 0;
    Plane plane;

    public bool isMouseDown = false;
    Vector2 lastMousePosition;
    float lastTouchesDistances;
    float MINUMIN_DISTANCE_MOVE_Y = 5f;

    const float MOVE_Y_FACTOR = 0.1f;

    const float ROTATION_FACTOR = 100f;

    float Y_ROTATATION_FACTOR = 2f;
    float X_ROTATION_FACTOR = 2f;

    float TOUCH_TOLERANCE = 1f;

    int rotation_axis = 0;

    Vector2 startTouchPos;

    void Start()
    {
        cameraNavigation = this.GetComponent<SimpleCameraOrbit>();
        plane = new Plane(Vector3.up, new Vector3(0,plane_height,0));
    }


    void moveTargetToIntersectionWithPlane(){
        Touch touch = Input.GetTouch(0);
        
        if(touch.phase != TouchPhase.Moved) return;
        
        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        
        float enter = 0.0f;

        if (!plane.Raycast(ray, out enter))return;

        Vector3 hitPoint = ray.GetPoint(enter);

        selectedObject.transform.position = new Vector3(hitPoint.x, plane_height, hitPoint.z);

    }

    bool moveInY(){
        Touch touch1 = Input.GetTouch(0), touch2 = Input.GetTouch(1);
        if(touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began){
            lastTouchesDistances = Vector2.Distance(touch1.position, touch2.position);
            lastMousePosition = touch1.position;
            return true;
        }
        if(touch1.phase != TouchPhase.Moved || touch2.phase != TouchPhase.Moved)
            return false;
        
        float zoomVal = lastTouchesDistances - Vector2.Distance(touch1.position, touch2.position);
        if(Mathf.Abs(zoomVal) < MINUMIN_DISTANCE_MOVE_Y)
            return false;

        float dy = -zoomVal*MOVE_Y_FACTOR*Time.deltaTime;
        selectedObject.transform.Translate(0,dy,0);
        plane_height += dy;
        plane = new Plane(Vector3.up, new Vector3(0, plane_height ,0));

        lastTouchesDistances = Vector2.Distance(touch1.position, touch2.position);
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
            selectedObject.transform.Rotate(0, -(touch.position.x - lastMousePosition.x) *  Y_ROTATATION_FACTOR * Time.deltaTime, 0);
        else
            selectedObject.transform.Rotate(-(touch.position.y - lastMousePosition.y) * X_ROTATION_FACTOR * Time.deltaTime, 0, 0);

        lastMousePosition = touch.position;
        
        return true;
    }

    void selectObject(){
        if(selectedObject != null)
            selectedObject.GetComponent<ClickableOjbect>().select();
        
    }

    void unselectObject(){
        if(selectedObject != null)
            selectedObject.GetComponent<ClickableOjbect>().unselect();
    }

    void selectObject(Touch touch){
        
        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)){
            if(hit.transform.gameObject.GetComponent<ClickableOjbect>() != null){
                selectedObject = hit.transform.gameObject;
                selectObject();
            }
            else{
                unselectObject();
                selectedObject = null;
            }
        }
        else{
            unselectObject();
            selectedObject = null;
        }
        cameraNavigation.active = selectedObject == null;
    }

    void Update(){
        int touchCount = Input.touchCount;
        if(touchCount <= 0) return;
        Touch touch = Input.GetTouch(0);
        if(touch.phase == TouchPhase.Began){
            startTouchPos = touch.position;
            return;
        }
        if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled){
            if(Vector2.Distance(touch.position, startTouchPos) < TOUCH_TOLERANCE)
            selectObject(touch);
            return;
        }
        Debug.DrawRay(Camera.main.transform.position, Camera.main.ScreenPointToRay(touch.position).direction*3, Color.red, 0);
        DrawPlane(new Vector3(0,plane_height,0) ,plane.normal);
        if(selectedObject == null) return;

        switch(touchCount){
            case 1:
                moveTargetToIntersectionWithPlane();
                break;
            case 2:
                if(moveInY()) return;
                rotate();
                break;
        }
    }


  void DrawPlane(Vector3 position, Vector3 normal) {
 
    Vector3 v3 ;
    
    if (normal.normalized != Vector3.forward)
        v3 = Vector3.Cross(normal, Vector3.forward).normalized * normal.magnitude;
    else
        v3 = Vector3.Cross(normal, Vector3.up).normalized * normal.magnitude;;
        
    var corner0 = position + v3;
    var corner2 = position - v3;
    var q = Quaternion.AngleAxis(90.0f, normal);
    v3 = q * v3;
    var corner1 = position + v3;
    var corner3 = position - v3;
    
    Debug.DrawLine(corner0, corner2, Color.green);
    Debug.DrawLine(corner1, corner3, Color.green);
    Debug.DrawLine(corner0, corner1, Color.green);
    Debug.DrawLine(corner1, corner2, Color.green);
    Debug.DrawLine(corner2, corner3, Color.green);
    Debug.DrawLine(corner3, corner0, Color.green);
    Debug.DrawRay(position, normal, Color.red);
 }


}
