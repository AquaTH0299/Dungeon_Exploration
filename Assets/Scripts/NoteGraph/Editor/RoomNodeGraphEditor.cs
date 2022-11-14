using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class RoomNodeGraphEditor : EditorWindow 
{
    private GUIStyle roomNodeStyle;
    //Node layout value
    private const float nodeWith = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;
    [MenuItem("Room Node Graph Editor",menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]
    private static void OpenWindow() 
    {
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }

    private void OnEnable() 
    {
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node 1") as Texture2D;
        roomNodeStyle.normal.textColor = Color.yellow;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);
    }
    private void OnGUI() 
    {
        GUILayout.BeginArea(new Rect(new Vector2(100, 100),new Vector2(nodeWith,nodeHeight)), roomNodeStyle);
        EditorGUILayout.LabelField("Node 1");
        GUILayout.EndArea();
        GUILayout.BeginArea(new Rect(new Vector2(500, 500),new Vector2(nodeWith,nodeHeight)), roomNodeStyle);
        EditorGUILayout.LabelField("Node 2");
        GUILayout.EndArea();
    }
}


