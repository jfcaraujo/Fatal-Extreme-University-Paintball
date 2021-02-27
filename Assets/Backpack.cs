using UnityEngine;

public class Backpack : MonoBehaviour
{
    private Camera cam;
    
    private Vector2 mousePos;
    
    private Transform rotationCenter = null;

    // Start is called before the first frame update
    void Start()
    {
        rotationCenter = gameObject.transform.parent.Find("Center");

        cam = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    void FixedUpdate()
    {
        var centerPosition = rotationCenter.position;
        Vector2 center = new Vector2(centerPosition.x, centerPosition.y);

        Vector2 lookDir = mousePos - center;
        float lookAngle = -(Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 180f);

        Vector2 backpackPosition = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
        Vector2 backpackDir = backpackPosition - center;
        float backpackAngle = -(Mathf.Atan2(backpackDir.y, backpackDir.x) * Mathf.Rad2Deg - 180f);

        gameObject.transform.RotateAround(center, Vector3.back, lookAngle - backpackAngle);
    }
}
