using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script to handle the Backpack power-up.
/// </summary>
public class Backpack : MonoBehaviour
{
    // Used to get data to aim the Backpack
    private Camera cam;
    private Vector2 mousePos;

    public Transform rotationCenter;

    private void Start()
    {
        cam = FindObjectOfType<Camera>();
    }

    private void Update()
    {
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    private void FixedUpdate()
    {
        // Here, the mouse position is checked so the backpack can be pointed in its direction
        // The backpack rotates around the rotationCenter's position

        var centerPosition = rotationCenter.position;
        Vector2 center = new Vector2(centerPosition.x, centerPosition.y);

        Vector2 lookDir = mousePos - center;
        float lookAngle = -(Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 180f);

        var position = gameObject.transform.position;
        Vector2 backpackPosition = new Vector2(position.x, position.y);
        Vector2 backpackDir = backpackPosition - center;
        float backpackAngle = -(Mathf.Atan2(backpackDir.y, backpackDir.x) * Mathf.Rad2Deg - 180f);

        gameObject.transform.RotateAround(center, Vector3.back, lookAngle - backpackAngle);
    }

    private void OnDisable()
    {
        // When disabling the backpack, splatters added to it have to be removed
        List<Transform> splattersToRemove = new List<Transform>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform splatter = transform.GetChild(i);

            if (splatter.CompareTag("Splatter"))
            {
                splattersToRemove.Add(splatter);
            }
        }

        foreach (Transform item in splattersToRemove)
        {
            Destroy(item.gameObject);
        }
    }
}
