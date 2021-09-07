using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager))]  
public class DebugScript : Editor
{

    override public void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameManager myScript = (GameManager)target;
        if (GUILayout.Button("Add 50 Gold"))
        {
            myScript.AddScore(50);
        }
        if (GUILayout.Button("Blue"))
        {
            myScript.AddKey(KeyColor.BLUE);
        }
        if (GUILayout.Button("Red"))
        {
            myScript.AddKey(KeyColor.RED);
        }
        if (GUILayout.Button("Yellow"))
        {
            myScript.AddKey(KeyColor.YELLOW);
        }
        if (GUILayout.Button("Gray"))
        {
            myScript.AddKey(KeyColor.GRAY);
        }
    }

    private void GiveKey()
    {
        Debug.Log("key");
    }
}
