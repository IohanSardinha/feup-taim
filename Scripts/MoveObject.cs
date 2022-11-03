using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public GameObject refPlane;
    public enum MovementState{
        NO_TARGET,
        CLICK_IN_PLANE,
        MOVE_IN_Y,
        ROTATE
    };

    public GameObject selectedObject = null;
    public GameObject target = null;

    float plane_height = 0;
    Plane plane;

    public MovementState state = MovementState.NO_TARGET;

    public bool isMouseDown = false;
    Vector2 lastMousePosition;

    const float MOVE_Y_FACTOR = 1f;

    const float ROTATION_FACTOR = 100f;

    int rotation_axis = 0;

    void Start()
    {
        plane = new Plane(Vector3.up, new Vector3(0,plane_height,0));
    }

    
    public void setState(string newState){
        if((target != null) && (selectedObject != null))
            Debug.Log(Vector3.Distance(selectedObject.transform.position, target.transform.position));

        switch(newState){
            case "MXZ":
                state = MovementState.CLICK_IN_PLANE;
                break;
            case "MY":
                state = MovementState.MOVE_IN_Y;
                break;
            case "RTX":
                state = MovementState.ROTATE;
                rotation_axis = 0;
                break;
            case "RTY":
                state = MovementState.ROTATE;
                rotation_axis = 1;
                break;
            case "RT|":
                state = MovementState.ROTATE;
                rotation_axis = 2;
                break;
        }
    }

    void moveTargetToIntersectionWithPlane(){
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        float enter = 0.0f;

        if (!plane.Raycast(ray, out enter))return;

        Vector3 hitPoint = ray.GetPoint(enter);

        refPlane.transform.position = hitPoint;

        if(Input.GetMouseButtonDown(0)) isMouseDown = true;
        else if(isMouseDown && Input.GetMouseButtonUp(0)){ 
            isMouseDown = false;
            state = MovementState.MOVE_IN_Y;
        }
        
        if(isMouseDown)
            selectedObject.transform.position = new Vector3(hitPoint.x, plane_height, hitPoint.z);

    }

    void moveInY(){
        if(Input.GetMouseButtonDown(0)){
            isMouseDown = true;
            lastMousePosition = Input.mousePosition;
        }
        else if(isMouseDown && Input.GetMouseButtonUp(0)){ 
            isMouseDown = false;
            plane = new Plane(Vector3.up, new Vector3(0, plane_height ,0));
            state = MovementState.ROTATE;
        }
        if(!isMouseDown) return;
        float dy = (Input.mousePosition.y - lastMousePosition.y)*MOVE_Y_FACTOR*Time.deltaTime;
        selectedObject.transform.Translate(0,dy,0);
        plane_height += dy;

        lastMousePosition = Input.mousePosition;
    }

    void rotate(){

        if(Input.GetMouseButtonDown(2)) rotation_axis = (rotation_axis + 1) % 3;

        float rotation = Input.mouseScrollDelta.y * ROTATION_FACTOR * Time.deltaTime * 2 * Mathf.PI;

        selectedObject.transform.Rotate(rotation*(rotation_axis == 0 ? 1 : 0),
                                rotation*(rotation_axis == 1 ? 1 : 0),
                                rotation*(rotation_axis == 2 ? 1 : 0));
    }

    void Update(){
        Debug.DrawRay(Camera.main.transform.position, Camera.main.ScreenPointToRay(Input.mousePosition).direction*3, Color.red, 0);
        DrawPlane(new Vector3(0,plane_height,0) ,plane.normal);
        switch(state){
            case MovementState.NO_TARGET:
                if(selectedObject != null)
                    state = MovementState.CLICK_IN_PLANE;
                break;
            case MovementState.CLICK_IN_PLANE:
                moveTargetToIntersectionWithPlane();
                break;
            case MovementState.MOVE_IN_Y:
                moveInY();
                break;
            case MovementState.ROTATE:
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
