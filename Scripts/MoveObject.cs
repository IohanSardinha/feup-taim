using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveObject : MonoBehaviour
{
    public TMPro.TextMeshProUGUI ui; 
    SimpleCameraOrbit cameraNavigation;

    HashSet<string> set = new HashSet<string>();

    public GameObject bloom;
    public GameObject selectedObject = null;
    public GameObject target = null;

    float plane_height = 0;
    Plane plane;

    public bool isMouseDown = false;
    Vector2 lastMousePosition;

    const float MOVE_Y_FACTOR = 0.05f;

    const float ROTATION_FACTOR = 100f;

    float Y_ROTATATION_FACTOR = 0.5f;
    float X_ROTATION_FACTOR = 0.5f;

    float TOUCH_TOLERANCE = 1f;

    int rotation_axis = 0;
    public GameObject rotationAxis;

    Vector2 startTouchPos;

    bool isMoving = false, isZooming = false, isRotating = false;

    void Start()
    {
        cameraNavigation = this.GetComponent<SimpleCameraOrbit>();
        plane = new Plane(Vector3.up, new Vector3(0,plane_height,0));
    }


    void moveTargetToIntersectionWithPlane(){
        Touch touch = Input.GetTouch(0);
        
        if(touch.phase == TouchPhase.Began){
            lastMousePosition = touch.position;
            return;
        }

        if(touch.phase != TouchPhase.Moved) return;

        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        
        float enter = 0.0f;

        rotationAxis.transform.position = selectedObject.transform.position;
        if(transform.localEulerAngles.x < 15 || transform.localEulerAngles.x > 345){
            Plane p = new Plane(transform.forward, selectedObject.transform.position);
            if (!p.Raycast(ray, out enter))return;
            Vector3 hitP = ray.GetPoint(enter);
            selectedObject.transform.position = hitP;
            return;
        }


        if (!plane.Raycast(ray, out enter))return;

        Vector3 hitPoint = ray.GetPoint(enter);

        selectedObject.transform.position = new Vector3(hitPoint.x, plane_height, hitPoint.z);

        isMoving = true;
        
    }

    bool moveInY(){
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

        Debug.Log(Vector2.Angle(touch1Dir, touch2Dir));

        if(Vector2.Angle(touch1Dir, touch2Dir) < 90)
            return false;

        float dy = zoomVal*MOVE_Y_FACTOR*Time.deltaTime;

        if(transform.localEulerAngles.x < 15 || transform.localEulerAngles.x > 345){
            selectedObject.transform.Translate(transform.forward*dy);
            rotationAxis.transform.position = selectedObject.transform.position;
            return true;
        }

        selectedObject.transform.position += Vector3.up*dy;
        plane_height += dy;
        plane = new Plane(Vector3.up, new Vector3(0, plane_height ,0));

        isZooming = true;
        rotationAxis.transform.position = selectedObject.transform.position;
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
        
        if(Mathf.Abs(touch.position.x - lastMousePosition.x) > Mathf.Abs(touch.position.y - lastMousePosition.y)){
            rotationAxis.transform.RotateAround(selectedObject.transform.position,Vector3.up ,-(touch.position.x - lastMousePosition.x) *  Y_ROTATATION_FACTOR * Time.deltaTime);
            selectedObject.transform.RotateAround(selectedObject.transform.position,Vector3.up ,-(touch.position.x - lastMousePosition.x) *  Y_ROTATATION_FACTOR * Time.deltaTime);
        }
        else{
            if(rotation_axis == 0){
                selectedObject.transform.Rotate((touch.position.y - lastMousePosition.y) * X_ROTATION_FACTOR * Time.deltaTime,0,0);
            }
            else{
                selectedObject.transform.Rotate(0,0,(touch.position.y - lastMousePosition.y) * X_ROTATION_FACTOR * Time.deltaTime);
            }
        }
            

        lastMousePosition = touch.position;
        
        isRotating = true;
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
                if(hit.transform.gameObject == selectedObject){
                    rotation_axis = (rotation_axis+1)%2;
                    rotationAxis.transform.RotateAround(rotationAxis.transform.position, selectedObject.transform.up, 90);
                    return;
                }
                unselectObject();
                selectedObject = hit.transform.gameObject;
                rotationAxis.transform.position = selectedObject.transform.position;
                target = hit.transform.gameObject.GetComponent<ClickableOjbect>().target;
                plane_height = selectedObject.transform.position.y;
                plane = new Plane(Vector3.up, new Vector3(0, plane_height ,0));
                rotationAxis.transform.eulerAngles = new Vector3(selectedObject.transform.eulerAngles.x, selectedObject.transform.eulerAngles.y+90*rotation_axis, selectedObject.transform.eulerAngles.z+90);
                selectObject();
            }
            else
            {
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
        if(selectedObject != null){
            rotationAxis.SetActive(true);
            
            ui.text = "distance: "+Mathf.Round(Vector3.Distance(target.transform.position, selectedObject.transform.position)*10000)/100+"cm, "+Mathf.Ceil(Vector3.Angle(selectedObject.transform.forward, target.transform.forward))+"º";
            if(!set.Contains(selectedObject.name) && Vector3.Angle(selectedObject.transform.forward, target.transform.forward) < 2 && (Vector3.Distance(target.transform.position, selectedObject.transform.position)*100) <= 2){
                Instantiate(bloom, selectedObject.transform.position, selectedObject.transform.rotation);
                set.Add(selectedObject.name);
            }           
        }else{
            rotationAxis.SetActive(false);
            ui.text = "moving camera";
        }
        int touchCount = Input.touchCount;
        if(touchCount <= 0){
            isRotating = false;
            isZooming = false;
            isMoving = false;
            return;
        } 
        Touch touch = Input.GetTouch(0);
        if(touch.phase == TouchPhase.Began){
            startTouchPos = touch.position;
        }
        if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled){
            if(Vector2.Distance(touch.position, startTouchPos) < TOUCH_TOLERANCE)
            selectObject(touch);
            return;
        }
        if(selectedObject == null) return;

        switch(touchCount){
            case 1:
                if(!isRotating && !isZooming)
                    moveTargetToIntersectionWithPlane();
                break;
            case 2:
                if(!isRotating && !isMoving && moveInY()) return;
                if(!isZooming && !isMoving)
                    rotate();
                break;
        }
    }

}
