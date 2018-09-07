using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Camera))]
public class CameraController : MonoBehaviour {


    public float panSpeed = 0.2f;
    public float panBorderThickness = 200f;
    public float scrollSpeed = 100f;
    public float rotationSpeed = 200f;
    public float rotationEaseDuration = 1f;
    public GameObject target;

    float halfEaseDuration = 1f;
    float currentRotationEaseRight = 0f;
    float currentRotationEaseLeft = 0f;

    bool shouldRotateRight = false;
    bool shouldRotateLeft = false;

    bool onHandheld;

    void Start()
    {
        halfEaseDuration = rotationEaseDuration / 2;
        onHandheld = SystemInfo.deviceType == DeviceType.Handheld;
    }

    void LateUpdate () {
        if (onHandheld)
        {
            UpdateHandheldMovement();
        }
        else
        {
            UpdateMovement();
        }
        UpdateRotation();
        
	}

    public void rotateLeft()
    {
        shouldRotateLeft = true;
    }

    public void rotateRight()
    {
        shouldRotateRight = true;
    }

    private void UpdateRotation()
    {
        if (shouldRotateLeft)
        {
            currentRotationEaseLeft += Time.deltaTime;
            float ease = currentRotationEaseLeft > halfEaseDuration ? rotationEaseDuration - currentRotationEaseLeft : currentRotationEaseLeft;
            transform.RotateAround(target.transform.position, Vector3.up, rotationSpeed * ease * Time.deltaTime);
            if (currentRotationEaseLeft > rotationEaseDuration) {
                currentRotationEaseLeft = 0;
                shouldRotateLeft = false;
            }
        }
        if (shouldRotateRight)
        {
            currentRotationEaseRight += Time.deltaTime;
            float ease = currentRotationEaseRight > halfEaseDuration ? rotationEaseDuration - currentRotationEaseRight : currentRotationEaseRight;
            transform.RotateAround(target.transform.position, Vector3.down, rotationSpeed * ease * Time.deltaTime);
            if (currentRotationEaseRight > rotationEaseDuration)
            {
                currentRotationEaseRight = 0;
                shouldRotateRight = false;
            }
        }
    }

    private void UpdateHandheldMovement()
    {
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            Vector3 translation = new Vector3(0, 0, deltaMagnitudeDiff * 0.1f);
            transform.Translate(translation, Space.Self);
        }
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            Vector3 translation = new Vector3(-touchDeltaPosition.x * panSpeed * 0.01f, -touchDeltaPosition.y * panSpeed * 0.01f, 0);

            transform.Translate(translation, Space.Self);
        }
    }

    private void UpdateMovement()
    {

        Vector3 pos = new Vector3(0, 0, 0);
        Vector2 mouse = Input.mousePosition;
        float screenHeight = Screen.height;
        float screenWidth = Screen.width;

        float deltaUpperScreenEdge = Mathf.Clamp(screenHeight - mouse.y, 0, screenHeight);
        float deltaLowerScreenEdge = Mathf.Clamp(mouse.y, 0, screenHeight);
        float deltaRightScreenEdge = Mathf.Clamp(screenWidth - mouse.x, 0, screenWidth);
        float deltaLeftScreenEdge = Mathf.Clamp(mouse.x, 0, screenWidth);

        if (deltaUpperScreenEdge < panBorderThickness)
        {
            pos.y += panSpeed * (panBorderThickness - deltaUpperScreenEdge) * Time.deltaTime;
        }

        if (deltaLowerScreenEdge < panBorderThickness)
        {
            pos.y -= panSpeed * (panBorderThickness - deltaLowerScreenEdge) * Time.deltaTime;
        }

        if (deltaRightScreenEdge < panBorderThickness)
        {
            pos.x += panSpeed * (panBorderThickness - deltaRightScreenEdge) * Time.deltaTime;
        }

        if (deltaLeftScreenEdge <= panBorderThickness)
        {
            pos.x -= panSpeed * (panBorderThickness - deltaLeftScreenEdge) * Time.deltaTime;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        pos.z = scroll * scrollSpeed * 100f * Time.deltaTime;

        if (transform.position != pos)
        {
            transform.Translate(pos, Space.Self);
        }
    }

    /*

    //
    // VARIABLES
    //

    public float turnSpeed = 4.0f;      // Speed of camera turning when mouse moves in along an axis
    public float panSpeed = 4.0f;       // Speed of the camera when being panned
    public float zoomSpeed = 4.0f;      // Speed of the camera going back and forth

    private Vector3 mouseOrigin;    // Position of cursor when mouse dragging starts
    private bool isPanning;     // Is the camera being panned?
    private bool isRotating;    // Is the camera being rotated?
    private bool isZooming;     // Is the camera zooming?

    //
    // UPDATE
    //

    void Update()
    {
        // Get the left mouse button
        if (Input.GetMouseButtonDown(0))
        {
            // Get mouse origin
            mouseOrigin = Input.mousePosition;
            isRotating = true;
        }

        // Get the right mouse button
        if (Input.GetMouseButtonDown(1))
        {
            // Get mouse origin
            mouseOrigin = Input.mousePosition;
            isPanning = true;
        }

        // Get the middle mouse button
        if (Input.GetMouseButtonDown(2))
        {
            // Get mouse origin
            mouseOrigin = Input.mousePosition;
            isZooming = true;
        }

        // Disable movements on button release
        if (!Input.GetMouseButton(0)) isRotating = false;
        if (!Input.GetMouseButton(1)) isPanning = false;
        if (!Input.GetMouseButton(2)) isZooming = false;

        // Rotate camera along X and Y axis
        if (isRotating)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

            transform.RotateAround(transform.position, transform.right, -pos.y * turnSpeed);
            transform.RotateAround(transform.position, Vector3.up, pos.x * turnSpeed);
        }

        // Move the camera on it's XY plane
        if (isPanning)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

            Vector3 move = new Vector3(pos.x * panSpeed, pos.y * panSpeed, 0);
            transform.Translate(move, Space.Self);
        }

        // Move the camera linearly along Z axis
        if (isZooming)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

            Vector3 move = pos.y * zoomSpeed * transform.forward;
            transform.Translate(move, Space.World);
        }
    }

    */

}
