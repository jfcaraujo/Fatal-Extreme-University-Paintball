using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to switch an object between two depth levels. Each depth level is a sorting layer.
/// This component is rendered in the inspector using <see cref="DepthLevelManagerEditor"/>.
/// </summary>
public class DepthLevelManager : MonoBehaviour
{
    private List<SpriteRenderer> sprites;

    public int frontLayerId;
    public int backLayerId;

    private void Start()
    {
        sprites = new List<SpriteRenderer>();

        // Gets every sprite renderer from object (children included)
        Queue<Transform> objectsToCheck = new Queue<Transform>();
        objectsToCheck.Enqueue(transform);

        while (objectsToCheck.Count > 0)
        {
            Transform obj = objectsToCheck.Dequeue();

            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();

            if (sr != null)
                sprites.Add(sr);

            for (int i = 0; i < obj.childCount; i++)
            {
                objectsToCheck.Enqueue(obj.GetChild(i));
            }
        }
    }

    /// <summary>
    /// Switches the depth the object is in.
    /// </summary>
    /// <param name="front">If the object should be placed in the front layer (<c>false</c> for back layer).</param>
    public void SwitchSortingLayer(bool front)
    {
        int layerID = front ? frontLayerId : backLayerId;

        foreach (SpriteRenderer item in sprites)
        {
            item.sortingLayerID = layerID;
        }
    }
}
