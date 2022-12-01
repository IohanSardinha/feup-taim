using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ClickableOjbect : MonoBehaviour{
    
    public GameObject target;
    public Material selectedMaterial;
    public Material originalMaterial;
    
    public List<GameObject> objects = new List<GameObject>();


    public void select(){
        foreach(GameObject gameObject in objects){
            gameObject.GetComponent<MeshRenderer>().material = selectedMaterial;
        }
    }

    public void unselect(){
        foreach(GameObject gameObject in objects){
            gameObject.GetComponent<MeshRenderer>().material = originalMaterial;
        }
    }

}
