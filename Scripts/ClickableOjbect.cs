using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableOjbect : MonoBehaviour
{

    MoveObject manager;
    public GameObject target;

    void Start(){
        manager = Camera.main.GetComponent<MoveObject>();
    }

    void OnMouseDown(){
        if(manager.selectedObject != this.gameObject){
            manager.selectedObject = this.gameObject;
            manager.target = target;
            manager.setState("MXZ");
        }
    }
}
