using UnityEngine;

public class Splatter : MonoBehaviour
{
    void Start()
    {
        SendMessageUpwards("OnObjectAdded", gameObject, SendMessageOptions.DontRequireReceiver);
    }
}
