using UnityEngine;

public class Splatter : MonoBehaviour
{
    public AudioManager audioManager;

    private void Start()
    {
        SendMessageUpwards("OnObjectAdded", gameObject, SendMessageOptions.DontRequireReceiver);

        int randomNumber = Random.Range(0, 3);

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
