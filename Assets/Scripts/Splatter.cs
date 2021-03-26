using UnityEngine;

/// <summary>
/// Manages a splatter 
/// </summary>
public class Splatter : MonoBehaviour
{
    public AudioManager audioManager;

    public bool isGrenade;

    private void Start()
    {
        //for the depth level manager of a parent object to add the splatter sprite to the objects that will change layer 
        SendMessageUpwards("OnObjectAdded", gameObject, SendMessageOptions.DontRequireReceiver);
        
        if (isGrenade)
            audioManager.PlaySound("GlassBreak");

        switch (Random.Range(0, 3))
        {
            case 0:
                audioManager.PlaySound("Splatter1");
                break;
            case 1:
                audioManager.PlaySound("Splatter2");
                break;
            case 2:
                audioManager.PlaySound("Splatter3");
                break;
            default:
                break;
        }
    }
}
