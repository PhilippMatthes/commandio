using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float panSpeed = 20f;
    public float panBorderThickness = 30f;
    public Vector2 panLimitRectangle;
    public float scrollSpeed = 20f;
    public float minY = 20f;
    public float maxY = 120f;
    public float smoothingSpeed = 0.01f;

    float smoothing = 0.0f;

    private bool onHandheld;

    void Start()
    {
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
        
	}

    private void UpdateHandheldMovement()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            transform.Translate(-touchDeltaPosition.x * panSpeed, -touchDeltaPosition.y * panSpeed, 0);
        } else if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            Camera.main.orthographicSize += deltaMagnitudeDiff * 0.1f;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 5, 20);
        }
    }

    private void UpdateMovement()
    {
        Vector3 pos = transform.position;
        Vector2 mouse = Input.mousePosition;
        float screenHeight = Screen.height;
        float screenWidth = Screen.width;

        float deltaUpperScreenEdge = Mathf.Clamp(screenHeight - mouse.y, 0, screenHeight);
        float deltaLowerScreenEdge = Mathf.Clamp(mouse.y, 0, screenHeight);
        float deltaRightScreenEdge = Mathf.Clamp(screenWidth - mouse.x, 0, screenWidth);
        float deltaLeftScreenEdge = Mathf.Clamp(mouse.x, 0, screenWidth);

        if (deltaUpperScreenEdge < panBorderThickness)
        {
            pos.z += panSpeed * (panBorderThickness - deltaUpperScreenEdge) * Time.deltaTime;
        }

        if (deltaLowerScreenEdge < panBorderThickness)
        {
            pos.z -= panSpeed * (panBorderThickness - deltaLowerScreenEdge) * Time.deltaTime;
        }

        if (deltaRightScreenEdge < panBorderThickness)
        {
            pos.x += panSpeed * (panBorderThickness - deltaRightScreenEdge) * Time.deltaTime;
        }

        if (deltaLeftScreenEdge <= panBorderThickness)
        {
            pos.x -= panSpeed * (panBorderThickness - deltaLeftScreenEdge) * Time.deltaTime;
        }

        pos.x = Mathf.Clamp(pos.x, -panLimitRectangle.x, panLimitRectangle.x);
        pos.z = Mathf.Clamp(pos.z, -panLimitRectangle.y, panLimitRectangle.y);

        /*
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        pos.y -= scroll * scrollSpeed * 100f * Time.deltaTime;

        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        */

        if (transform.position != pos)
        {
            smoothing += smoothingSpeed;
            Vector3 smoothed = Vector3.Lerp(transform.position, pos, smoothing);
            transform.position = smoothed;

        }
        else
        {
            smoothing = 0f;
        }

        smoothing = Mathf.Clamp(smoothing, 0.0f, 1.0f);
    }


}
