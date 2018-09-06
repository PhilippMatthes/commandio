/*
  Copyright (c) 2018, Marko Viitanen (Fador), Philipp Matthes
  
  Permission to use, copy, modify, and/or distribute this software for any purpose 
  with or without fee is hereby granted, provided that the above copyright notice 
  and this permission notice appear in all copies.
  
  THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH 
  REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF MERCHANTABILITY 
  AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT, 
  INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES WHATSOEVER RESULTING FROM 
  LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE 
  OR OTHER TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR 
  PERFORMANCE OF THIS SOFTWARE.
*/

using UnityEngine;
using System.Collections;
using System;

public class ObjectPlacement : MonoBehaviour
{
    [Tooltip("Any prefab will block this dedicated area (x, z)")]
    public Vector2 blockingArea;

    [Tooltip("Grid resolution, e.g. for 4x4 place grid 2 would allow 2x2 object to be placed")]
    public float grid = 2.0f;

    // Mask of the ground to hit
    [Tooltip("Which layer to use for raycast target")]
    public LayerMask mask = -1;

    // Store which spaces are in use
    int[,] usedSpace;

    private GameObject objectToPlace = null;
    private GameObject instantiatedObjectToPlace = null;

    private Bounds placementBounds;

    bool mouseClick = false;
    Vector3 lastPos;

    // Use this for initialization
    void Start()
    {

        // Check terrain first because it has usually no Renderer
        if (GetComponent<Terrain>() != null)
        {
            placementBounds = GetComponent<Terrain>().terrainData.bounds;
        }
        else if (GetComponent<Renderer>() != null)
        {
            placementBounds = GetComponent<Renderer>().bounds;
        }

        Vector3 slots = placementBounds.size / grid;
        usedSpace = new int[Mathf.CeilToInt(slots.x), Mathf.CeilToInt(slots.z)];
        for (var x = 0; x < Mathf.CeilToInt(slots.x); x++)
        {
            for (var z = 0; z < Mathf.CeilToInt(slots.z); z++)
            {
                usedSpace[x, z] = 0;
            }
        }
    }

    public void SetObjectToPlace(GameObject objectToPlace)
    {
        this.objectToPlace = objectToPlace;
    }

    // Update is called once per frame
    void Update()
    {

        if (!objectToPlace) return;

        Vector3 point;

        // Check for mouse ray collision with this object
        if (GetTargetLocation(out point))
        {
            //I'm lazy and use the object size from the renderer..
            Vector3 halfSlots = placementBounds.size / 2.0f;

            // Transform position is the center point of this object, x and z are grid slots from 0..slots-1
            int x = (int)Math.Round(Math.Round(point.x - transform.position.x + halfSlots.x - grid / 2.0f) / grid);
            int z = (int)Math.Round(Math.Round(point.z - transform.position.z + halfSlots.z - grid / 2.0f) / grid);

            // Calculate the quantized world coordinates on where to actually place the object
            point.x = (float)(x) * grid - halfSlots.x + transform.position.x + grid / 2.0f;
            point.z = (float)(z) * grid - halfSlots.z + transform.position.z + grid / 2.0f;

            if (!instantiatedObjectToPlace)
            {
                instantiatedObjectToPlace = Instantiate(objectToPlace, point, Quaternion.identity);
            }
            else
            {
                instantiatedObjectToPlace.transform.position = point;
            }
            


            // On left click, insert the object to the area and mark it as "used"
            if (Input.GetMouseButtonDown(0) && mouseClick == false)
            {
                mouseClick = true;
                // Place the object
                if (usedSpace[x, z] == 0)
                {

                    objectToPlace = null;
                    instantiatedObjectToPlace = null;

                    int areaX = (int)Math.Ceiling(blockingArea.x);
                    int areaZ = (int)Math.Ceiling(blockingArea.y);

                    int sizeX = x + areaX;
                    int sizeZ = z + areaZ;

                    for (var xIterator = x - areaX; xIterator <= sizeX; xIterator++)
                    {
                        for (var zIterator = z - areaZ; zIterator <= sizeZ; zIterator++)
                        {
                            usedSpace[xIterator, zIterator] = 1;
                        }
                    }
                }
            }
            else if (!Input.GetMouseButtonDown(0))
            {
                mouseClick = false;
            }

        }
    }

    bool GetTargetLocation(out Vector3 point)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hitInfo = new RaycastHit();
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, mask))
        {
            if (hitInfo.collider == GetComponent<Collider>())
            {
                point = hitInfo.point;
                return true;
            }
        }
        point = Vector3.zero;
        return false;
    }
}