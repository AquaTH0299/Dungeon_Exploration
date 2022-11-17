using UnityEngine;
using UnityEditor.Callbacks;
using UnityEditor;
using System;

public class RoomNodeGraphEditor : EditorWindow 
{
    private GUIStyle roomNodeStyle;
    private GUIStyle roomNodeSelectedStyle;
    private static RoomNodeGraphSO currentRoomNodeGraph;
    private RoomNodeSO currentRoomNode = null;
    private RoomNodeTypeListSO roomNodeTypeList;
    //Node layout value
    private const float nodeWith = 180f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 13;
    // connecting line value
    private const float connectingLineWidth = 3f;
    private const float connectingLineArrowSize = 8f;
    [MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]
    private static void OpenWindow() 
    {
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }
    private void OnEnable() 
    {
        // subscribe to the inspector selection changed event
        Selection.selectionChanged += InspectorSelectionChanged;
        //define node layout style
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);
        //define selected node style
        roomNodeSelectedStyle = new GUIStyle();
        roomNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node1 no") as Texture2D;
        roomNodeSelectedStyle.normal.textColor = Color.red;
        roomNodeSelectedStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeSelectedStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);
        //load room node types
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }
    private void OnDisable() 
    {
        // Unsubscribe from the inspector selection changed event
        Selection.selectionChanged -= InspectorSelectionChanged;
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
    private void OnGUI() 
    {
        if(currentRoomNodeGraph != null)
        {
            DrawDraggedLine();
            ProcessEvents(Event.current);
            DrawRoomConnections();
            DrawRoomNode();
        }
        if(GUI.changed)
        {
            Repaint();
        }
    }
    private void DrawDraggedLine()
    {
        if(currentRoomNodeGraph.linePosition != Vector2.zero)
        {
            //draw line from node to line position
            Handles.DrawBezier(currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center,currentRoomNodeGraph.linePosition,
            currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center,currentRoomNodeGraph.linePosition, Color.cyan, null, connectingLineWidth);

            GUI.changed = true;
        }
        
    }
    private void ProcessEvents(Event currentEvent)
    {
        if (currentRoomNode == null || currentRoomNode.isLeftClickDragging == false)
        {
            currentRoomNode = IsMouseOverRoomNode(currentEvent);
        }
        if(currentRoomNode == null || currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
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
            case EventType.MouseUp:
                ProcessMouseUpEvents(currentEvent);
                break;
            case EventType.MouseDrag:
                ProcessMouseDragEvents(currentEvent);
                break;
            default:
                break;
        }
    }
    //ProcessMouseDown events on the room node graph(not over a node)
    private void ProcessMouseDownEvents(Event currentEvent)
    {
        // process right click mouse down on graph event (show content menu)
        if(currentEvent.button == 1)
        {
            ShowContextMenu(currentEvent.mousePosition);
        }
        //process left mouse down on graph event
        else if(currentEvent.button == 0)
        {
           ClearLineDrag();
           ClearAllSelectedRoomNodes();
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
        if(currentRoomNodeGraph.roomNodeList.Count == 0)
        {
            CreateRoomNode(new Vector2(250,200), roomNodeTypeList.list.Find( x => x.isEntrance));
        }
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
        // refresh graph node dictionary
        currentRoomNodeGraph.OnValidate();
    }
    //clear selection from all room node
    private void ClearAllSelectedRoomNodes()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if(roomNode.isSelected)
            {
                roomNode.isSelected = false;

                GUI.changed = true;
            }
        }
    }
    private void ProcessMouseUpEvents(Event currentEvent)
    {
        if(currentEvent.button == 1 && currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            // check if over the room node
            RoomNodeSO roomNode = IsMouseOverRoomNode(currentEvent);
            if(roomNode != null)
            {
                // if so set it as the child of the parent room node if it can be added
                if(currentRoomNodeGraph.roomNodeToDrawLineFrom.AddChildRoomNodeIDToRoomNode(roomNode.id))
                {
                    //set parent id in child room node
                    roomNode.AddParentRoomNodeIDToRoomNode(currentRoomNodeGraph.roomNodeToDrawLineFrom.id);
                }
            }
            ClearLineDrag();
        }
    }
    private void ClearLineDrag()
    {
        currentRoomNodeGraph.roomNodeToDrawLineFrom = null;
        currentRoomNodeGraph.linePosition = Vector2.zero;
        GUI.changed = true;
    }
    private void ProcessMouseDragEvents(Event currentEvent)
    {
        if(currentEvent.button == 1)
        {
            ProcessRightMouseDragEvents(currentEvent);
        }
    }
    private void ProcessRightMouseDragEvents(Event currentEvent)
    {
        if(currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            DragConnectingLine(currentEvent.delta);
            GUI.changed = true;
        }
    }
    private void DragConnectingLine(Vector2 delta)
    {
        currentRoomNodeGraph.linePosition += delta;
    }
    private void DrawRoomConnections()
    {
        // loop through all rome node
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if(roomNode.chillRoomNodeIDList.Count > 0)
            {
                // loop through child room node
                foreach( string chillRoomNodeID in roomNode.chillRoomNodeIDList)
                {
                    // get child room node from dictionary
                    if(currentRoomNodeGraph.roomNodeDictionary.ContainsKey(chillRoomNodeID))
                    {
                        DrawConnectionLine(roomNode, currentRoomNodeGraph.roomNodeDictionary[chillRoomNodeID]);
                        GUI.changed = true;
                    }
                }
            }
        } 
    }
    private void DrawConnectionLine(RoomNodeSO parentRoomNode, RoomNodeSO childRoomNode)
    {
        // get line start and end position
        Vector2 startPosition = parentRoomNode.rect.center;
        Vector2 endPosition = childRoomNode.rect.center;
        // calculate midway point
        Vector2 midPosition = (startPosition + endPosition) / 2f;
        // vector start to end position of line
        Vector2 direction = endPosition - startPosition;
        // calculate normalized perpendicular position from the mid point
        Vector2 arrowTailPoint1 = midPosition - new Vector2(-direction.y,direction.x).normalized * connectingLineArrowSize;
        Vector2 arrowTailPoint2 = midPosition + new Vector2(-direction.y,direction.x).normalized * connectingLineArrowSize;
        // calculate mid point offset position for arrowhead
        Vector2 arrowHeadPoint = midPosition + direction.normalized * connectingLineArrowSize;
        //draw arrow
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint1, arrowHeadPoint, arrowTailPoint1, Color.cyan, null, connectingLineWidth);
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint2, arrowHeadPoint, arrowTailPoint2, Color.cyan, null, connectingLineWidth);
        // draw line
        Handles.DrawBezier(startPosition, endPosition,startPosition, endPosition, Color.cyan, null, connectingLineWidth);

        GUI.changed = true;
    }
    private void DrawRoomNode()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if(roomNode.isSelected)
            {
                roomNode.Draw(roomNodeSelectedStyle);
            }
            else
            {
                roomNode.Draw(roomNodeStyle);
            }
        }
        GUI.changed = true;
    }
    private void InspectorSelectionChanged()
    {
        RoomNodeGraphSO roomNodeGraph = Selection.activeObject as RoomNodeGraphSO;
        if (roomNodeGraph != null)
        {
            currentRoomNodeGraph = roomNodeGraph;
            GUI.changed = true;
        }
    }
}