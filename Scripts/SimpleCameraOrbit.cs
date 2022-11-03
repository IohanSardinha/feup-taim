using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraOrbit : MonoBehaviour
{
    public bool active;
    public float speed;

    private Vector2 lastMousePosition;

    // Start is called before the first frame update
    void Start()
    {
        lastMousePosition = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if(active && Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftControl))
            transform.RotateAround(Vector3.zero, Vector3.up, (Input.mousePosition.x - lastMousePosition.x) * speed * Time.deltaTime);

        lastMousePosition = Input.mousePosition;
    }
}
