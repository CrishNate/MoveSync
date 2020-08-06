using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
 
public class Spectator : MonoBehaviour {
 
    //initial speed
    [SerializeField] private int _speed = 10;
    [SerializeField] private int _sensitivity = 2;

    // Update is called once per frame
    void Update ()
    {
        if (EventSystem.current.currentSelectedGameObject != null) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;

        
        if (Input.GetMouseButtonDown(1))
            Cursor.lockState = CursorLockMode.Locked;
        if (Input.GetMouseButtonUp(1))
            Cursor.lockState = CursorLockMode.None;

        if (Input.GetMouseButton(1))
        {
            Vector3 currentRotation = Camera.main.transform.rotation.eulerAngles;
            float pitch = currentRotation.x - Input.GetAxis("Mouse Y") * _sensitivity;
            if (pitch > 90 && pitch < 180) pitch = 90.0f;
            if (pitch < 270 && pitch > 90) pitch = 270.0f;
            
            currentRotation.x = pitch;
            currentRotation.y += Input.GetAxis("Mouse X") * _sensitivity;
            Camera.main.transform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0);
        }
        
        //For the following 'if statements' don't include 'else if', so that the user can press multiple buttons at the same time
        //move camera to the left
        if(Input.GetKey(KeyCode.A))
        {
            transform.position = transform.position + Camera.main.transform.right *-1 * _speed * Time.deltaTime;
        }
 
        //move camera backwards
        if(Input.GetKey(KeyCode.S))
        {
            transform.position = transform.position + Camera.main.transform.forward *-1 * _speed * Time.deltaTime;
  
        }
        //move camera to the right
        if(Input.GetKey(KeyCode.D))
        {
            transform.position = transform.position + Camera.main.transform.right * _speed * Time.deltaTime;
  
        }
        //move camera forward
        if(Input.GetKey(KeyCode.W))
        {
  
            transform.position = transform.position + Camera.main.transform.forward * _speed * Time.deltaTime;
        }
        //move camera upwards
        if(Input.GetKey(KeyCode.E))
        {
            transform.position = transform.position + Camera.main.transform.up * _speed * Time.deltaTime;
        }
        //move camera downwards
        if(Input.GetKey(KeyCode.Q))
        {
            transform.position = transform.position + Camera.main.transform.up * -1 *_speed * Time.deltaTime;
        }
 
    }
}