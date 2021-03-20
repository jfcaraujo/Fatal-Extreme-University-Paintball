using UnityEngine;

public class Splatter : MonoBehaviour
{
    private void Start()
    {
        SendMessageUpwards("OnObjectAdded", gameObject, SendMessageOptions.DontRequireReceiver);
    }
}
