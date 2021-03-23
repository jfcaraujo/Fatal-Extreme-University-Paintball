using UnityEngine;
using UnityEditor;
using System.Linq;

/// <summary>
/// Inspector UI for <see cref="DepthLevelManager"/>.
/// </summary>
[CustomEditor(typeof(DepthLevelManager))]
public class DepthLevelManagerEditor : Editor
{
    private SerializedProperty frontLayerId;
    private SerializedProperty backLayerId;
    private SerializedProperty changeMask;

    private void OnEnable()
    {
        frontLayerId = serializedObject.FindProperty("frontLayerId");
        backLayerId = serializedObject.FindProperty("backLayerId");
        changeMask = serializedObject.FindProperty("changeMask");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        int newId = DrawSortingLayersPopup("Front Layer", frontLayerId.intValue);
        if (EditorGUI.EndChangeCheck())
        {
            frontLayerId.intValue = newId;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        newId = DrawSortingLayersPopup("Back Layer", backLayerId.intValue);
        if (EditorGUI.EndChangeCheck())
        {
            backLayerId.intValue = newId;
        }
        EditorGUILayout.EndHorizontal();

        changeMask.boolValue = EditorGUILayout.Toggle("Change Mask", changeMask.boolValue);

        if (GUI.changed)
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

    int DrawSortingLayersPopup(string fieldName, int layerID)
    {
        var layers = SortingLayer.layers;
        var names = layers.Select(l => l.name).ToArray();
        if (!SortingLayer.IsValid(layerID))
        {
            layerID = layers[0].id;
        }
        var layerValue = SortingLayer.GetLayerValueFromID(layerID);
        var newLayerValue = EditorGUILayout.Popup(fieldName, layerValue, names);
        return layers[newLayerValue].id;
    }
}
