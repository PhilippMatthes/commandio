using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverAnimation : MonoBehaviour {

    public float horizontalSpeed = 0f;
    public float verticalSpeed = 0f;
    public float amplitude = 0f;

    private Vector3 position;
    private Vector3 initialPosition;

	// Use this for initialization
	void Start () {
        position = transform.position;
        initialPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        position.x += horizontalSpeed;
        position.y = Mathf.Sin(Time.realtimeSinceStartup * verticalSpeed) * amplitude + initialPosition.y;
        transform.position = position;
	}
}
