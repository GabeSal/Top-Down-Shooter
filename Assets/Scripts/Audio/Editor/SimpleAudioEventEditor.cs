using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SimpleAudioEvent))]
public class SimpleAudioEventEditor : Editor
{
    #region Private Fields
    private AudioSource _previewSource;
    #endregion

    #region Standard Unity Methods
    public void OnEnable()
    {
        var audioObject = EditorUtility.CreateGameObjectWithHideFlags(
            "Audio Preview",
            HideFlags.HideAndDontSave,
            typeof(AudioSource));

        _previewSource = audioObject.GetComponent<AudioSource>();
    }

    public void OnDisable()
    {
        DestroyImmediate(_previewSource.gameObject);
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);
        if (GUILayout.Button("Preview"))
        {
            SimpleAudioEvent simpleTarget = (SimpleAudioEvent)target;
            simpleTarget.Play(_previewSource);
        }

        EditorGUI.EndDisabledGroup();
    } 
    #endregion
}
