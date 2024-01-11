using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ResizeController))]
public class ResizeControllerEditor : Editor
{
    SerializedProperty currentResizeTypeProp;
    SerializedProperty percentageOfScreenProp;
    SerializedProperty originalSizeProp;

    private void OnEnable()
    {
        currentResizeTypeProp = serializedObject.FindProperty("currentResizeType");
        percentageOfScreenProp = serializedObject.FindProperty("percentageOfScreen");
        originalSizeProp = serializedObject.FindProperty("maxSize");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        ResizeController resizeController = (ResizeController)target;

        // Show or hide the percentageOfScreen property based on the currentResizeType
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(currentResizeTypeProp);

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            // Force a repaint to update the inspector GUI
            SceneView.RepaintAll();
        }

        if (resizeController.currentResizeType == ResizeController.ResizeType.Percentage)
        {
            EditorGUILayout.PropertyField(percentageOfScreenProp);
        }
        else if (resizeController.currentResizeType == ResizeController.ResizeType.Size)
        {
            EditorGUILayout.PropertyField(originalSizeProp);
        }

        serializedObject.ApplyModifiedProperties();
    }
}