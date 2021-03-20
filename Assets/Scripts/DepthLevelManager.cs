using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to switch an object between two depth levels. Each depth level is a sorting layer.
/// This component is rendered in the inspector using <see cref="DepthLevelManagerEditor"/>.
/// </summary>
public class DepthLevelManager : MonoBehaviour
{
    private List<SpriteRenderer> sprites;
    private List<SpriteMask> spriteMasks;

    public int frontLayerId;
    public int backLayerId;

    public bool changeMask;

    private void Start()
    {
        sprites = new List<SpriteRenderer>();
        spriteMasks = new List<SpriteMask>();

        GetAllSpriteComponents(transform, false);
    }

    /// <summary>
    /// Gets all SpriteMasks and SpriteRenderers in the object's hierarchy.
    /// </summary>
    /// <param name="transf">Root object's transform.</param>
    /// <param name="changeLayer">If the object's hierarchy should have its layer updated.</param>
    private void GetAllSpriteComponents(Transform transf, bool changeLayer)
    {
        if (sprites.Count == 0)
            changeLayer = false;

        int newLayerId = changeLayer ? sprites[0].sortingLayerID : -1;

        // Gets every sprite renderer from object (children included)
        Queue<Transform> objectsToCheck = new Queue<Transform>();
        objectsToCheck.Enqueue(transf);

        while (objectsToCheck.Count > 0)
        {
            Transform obj = objectsToCheck.Dequeue();

            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            SpriteMask sm = obj.GetComponent<SpriteMask>();

            if (sr != null)
            {
                sprites.Add(sr);

                if (changeLayer)
                    sr.sortingLayerID = newLayerId;
            }

            if (sm != null)
            {
                spriteMasks.Add(sm);

                if (changeLayer)
                    sm.sortingLayerID = newLayerId;
            }

            for (int i = 0; i < obj.childCount; i++)
            {
                objectsToCheck.Enqueue(obj.GetChild(i));
            }
        }
    }

    /// <summary>
    /// Receives a <c>OnObjectAdded message</c> when a new object is added to the hierarchy, usually sent from a child (or child of child).
    /// </summary>
    /// <param name="addedObject">Added object.</param>
    private void OnObjectAdded(GameObject addedObject)
    {
        GetAllSpriteComponents(addedObject.transform, true);
    }

    /// <summary>
    /// Switches the depth the object is in.
    /// </summary>
    /// <param name="front">If the object should be placed in the front layer (<c>false</c> for back layer).</param>
    public void SwitchSortingLayer(bool front)
    {
        List<Renderer> objectsToRemove = new List<Renderer>();

        int layerID = front ? frontLayerId : backLayerId;

        foreach (SpriteRenderer item in sprites)
        {
            if (item == null)
            {
                objectsToRemove.Add(item);
            }
            else
            {
                item.sortingLayerID = layerID;
            }
        }

        if (changeMask)
        {
            foreach (SpriteMask item in spriteMasks)
            {
                if (item == null)
                {
                    objectsToRemove.Add(item);
                }
                else
                {
                    item.frontSortingLayerID = layerID;
                    item.backSortingLayerID = layerID;
                }
            }
        }

        sprites.RemoveAll(x => objectsToRemove.Contains(x));
        spriteMasks.RemoveAll(x => objectsToRemove.Contains(x));
    }
}
