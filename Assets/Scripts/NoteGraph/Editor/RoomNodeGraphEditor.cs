using UnityEngine;
using UnityEditor.Callbacks;
using UnityEditor;
using System;

public class RoomNodeGraphEditor : EditorWindow 
{
    private GUIStyle roomNodeStyle;
    private static RoomNodeGraphSO currentRoomNodeGraph;
    private RoomNodeSO currentRoomNode = null;
    private RoomNodeTypeListSO roomNodeTypeList;
    //Node layout value
    private const float nodeWith = 180f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 13;
    [MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]
    private static void OpenWindow() 
    {
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }
    [OnOpenAsset(0)] //need the namespace UnityEditor.callback
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;
        if(roomNodeGraph != null)
        {
            OpenWindow();
            currentRoomNodeGraph = roomNodeGraph;
            return true;
        }
        return false;
    }
    private void OnEnable() 
    {
        //define node layout style
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.normal.textColor = Color.green;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        //load room node types
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }
    private void OnGUI() 
    {
        if(currentRoomNodeGraph != null)
        {
            ProcessEvents(Event.current);
            DrawRoomNode();
        }
        if(GUI.changed)
        {
            Repaint();
        }
    }

    private void ProcessEvents(Event currentEvent)
    {
        if (currentRoomNode == null || currentRoomNode.isLeftClickDragging == false)
        {
            currentRoomNode = IsMouseOverRoomNode(currentEvent);
        }
        if(currentRoomNode == null)
        {
        ProcessRoomNodeGraphEvents(currentEvent);
        }
        else
        {
            currentRoomNode.ProcessEvents(currentEvent);
        }
    }

    private RoomNodeSO IsMouseOverRoomNode(Event currentEvent)
    {
        for (int i = currentRoomNodeGraph.roomNodeList.Count - 1; i >= 0; i--)
        {
            if(currentRoomNodeGraph.roomNodeList[i].rect.Contains(currentEvent.mousePosition))
            {
                return currentRoomNodeGraph.roomNodeList[i];
            }
        }
        return null;
    }

    //ProcessRoomGraphEvents
    private void ProcessRoomNodeGraphEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            case EventType.MouseDown:
                ProcessMouseDownEvents(currentEvent);
                break;
            default:
                break;
        }
    }
    //ProcessMouseDown events on the room node graph(not over a node)
    private void ProcessMouseDownEvents(Event currentEvent)
    {
        if(currentEvent.button == 1)
        {
            ShowContextMenu(currentEvent.mousePosition);
        }
    }

    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Create room node"), false, CreateRoomNode, mousePosition);
        genericMenu.ShowAsContext();
    }

    private void CreateRoomNode(object mousePositionObject)
    {
        CreateRoomNode(mousePositionObject, roomNodeTypeList.list.Find( x => x.isNone));
    }
    private void CreateRoomNode(object mousePositionObject,RoomNodeTypeSO roomNodeType)
    {
        Vector2 mousePosition = (Vector2)mousePositionObject;
        RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();// add roomNode scripTable Object asset
        currentRoomNodeGraph.roomNodeList.Add(roomNode); // add room node to current room node graph room node List
        roomNode.Initialise(new Rect(mousePosition, new Vector2(nodeWith,nodeHeight)), currentRoomNodeGraph, roomNodeType); //set room node value
        AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);//add room node to room node graph scripTable Object asset database
        AssetDatabase.SaveAssets();
    }
    private void DrawRoomNode()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            roomNode.Draw(roomNodeStyle);
        }
        GUI.changed = true;
    }
}