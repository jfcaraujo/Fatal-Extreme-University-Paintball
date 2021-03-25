using UnityEngine;

/// <summary>
/// Checks entry and exit of objects into the height level.
/// </summary>
public class HeightCheck : MonoBehaviour
{
    private DepthLevelManager playerDepthManager;
    private Player_Controller player_Controller;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        playerDepthManager = player.GetComponent<DepthLevelManager>();
        player_Controller = player.GetComponent<Player_Controller>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // This function is used to fix bugs with objects that contain multiple colliders
        // Without this, some of the colliders exited, even tho the entire object did not
        // And the sorting layers weren't correctly changed

        if (other.attachedRigidbody != null && other.attachedRigidbody.velocity.y == 0)
        {
            DepthLevelManager cc;
            bool isPlayer = false;

            if (other.gameObject.CompareTag("Player"))
            {
                isPlayer = true;
                cc = playerDepthManager;
            }
            else
            {
                cc = other.gameObject.GetComponent<DepthLevelManager>();
            }

            if (cc == null)
                return;

            if (isPlayer)
            {
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("UpperGround"), false);
                player_Controller.ToggleCollisions("UpperObstacles", true);
            }

            cc.SwitchSortingLayer(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // If the object that entered is coming from above, it'll be on the level
        if (other.attachedRigidbody != null && other.attachedRigidbody.velocity.y <= -0.01)
        {

            DepthLevelManager cc;
            bool isPlayer = false;

            if (other.gameObject.CompareTag("Player"))
            {
                isPlayer = true;
                cc = playerDepthManager;
            }
            else
            {
                cc = other.gameObject.GetComponent<DepthLevelManager>();
            }

            if (cc == null)
                return;

            if (isPlayer && Input.GetButton("Drop"))
            {
                // However, if the object is the player and they want to drop, it'll be off the level
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("UpperGround"), true);
                player_Controller.ToggleCollisions("UpperObstacles", false);
                cc.SwitchSortingLayer(true);
            }
            else
            {
                if (isPlayer)
                {
                    Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("UpperGround"), false);
                    player_Controller.ToggleCollisions("UpperObstacles", true);
                }

                cc.SwitchSortingLayer(false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.attachedRigidbody == null) return;

        DepthLevelManager cc;
        bool isPlayer = false;

        if (other.gameObject.CompareTag("Player"))
        {
            isPlayer = true;
            cc = playerDepthManager;
        }
        else
        {
            cc = other.gameObject.GetComponent<DepthLevelManager>();
        }

        if (cc == null)
            return;

        // If the object exited to above, it'll be on the level
        if (other.attachedRigidbody.velocity.y >= 0)
        {
            if (isPlayer)
            {
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("UpperGround"), false);
                player_Controller.ToggleCollisions("UpperObstacles", true);
            }

            cc.SwitchSortingLayer(false);
        }
        // If the object exited to below, it'll be off the level
        else
        {
            if (isPlayer)
            {
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("UpperGround"), true);
                player_Controller.ToggleCollisions("UpperObstacles", false);
            }

            cc.SwitchSortingLayer(true);
        }
    }
}
