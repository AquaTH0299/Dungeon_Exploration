using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoomNodeSO : ScriptableObject
{
    [HideInInspector] public string id;
    [HideInInspector] public List<string> parentRoomNodeIDList = new List<string>();
    [HideInInspector] public List<string> chillRoomNodeIDList = new List<string>();
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    public RoomNodeTypeSO roomNodeType;
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;
#if UNITY_EDITOR
    [HideInInspector] public Rect rect;
    [HideInInspector] public bool isLeftClickDragging = false;
    [HideInInspector] public bool isSelected = false;
    public void Initialise(Rect rect, RoomNodeGraphSO nodeGraph, RoomNodeTypeSO roomNodeType)
    {
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.name = "roomNode";
        this.roomNodeGraph = nodeGraph;
        this.roomNodeType = roomNodeType;
        //load room node type list
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }
    public void Draw(GUIStyle nodeStyle)
    {
        // draw node box using begin Area
        GUILayout.BeginArea(rect, nodeStyle);
        // Start Region to detect popup selection changes
        EditorGUI.BeginChangeCheck();
        // display a popup using the room node type name values that can be selected from< default to the currently set room node type>
        if(parentRoomNodeIDList.Count > 0 || roomNodeType.isEntrance)
        {
            //display the label that can't be changed
            EditorGUILayout.LabelField(roomNodeType.roomNodeTypeName);
        }
        else
        {
            int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);
            int selection = EditorGUILayout.Popup("",selected, GetRoomNodeTypeToDisplay());
            roomNodeType = roomNodeTypeList.list[selection];
        }
        if(EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(this);
        }
        GUILayout.EndArea();
    }
    public string[] GetRoomNodeTypeToDisplay()
    {
        string[] roomArray = new string[roomNodeTypeList.list.Count];
        for ( int i = 0; i < roomNodeTypeList.list.Count; i++)
        {
            if(roomNodeTypeList.list[i].displayInNodeGraphEditor)
            {
                roomArray[i] = roomNodeTypeList.list[i].roomNodeTypeName;
            }
            
        }
        return roomArray;
    }
    public void ProcessEvents(Event currentEvents)
    {
        switch (currentEvents.type)
        {
            case EventType.MouseDown://process mouses down event
                ProcessMouseDownEvent(currentEvents);
                break;
            case EventType.MouseUp://process mouses up event
                ProcessMouseUpEvent(currentEvents);
                break;
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvents);
                break;
            default:
                break;
        }
    }

    private void ProcessMouseDragEvent(Event currentEvents)
    {
        if(currentEvents.button == 0)
        {
            ProcessLeftClickDragEvent(currentEvents);
        }
    }

    private void ProcessLeftClickDragEvent(Event currentEvents)
    {
        isLeftClickDragging = true;
        dragNode(currentEvents.delta);
        GUI.changed = true;
    }

    public void dragNode(Vector2 delta)
    {
        rect.position += delta;
        EditorUtility.SetDirty(this);
    }

    private void ProcessMouseUpEvent(Event currentEvents)
    {
        //if left click up
        if(currentEvents.button == 0)
        {
            ProcessLeftClickUpEvent();
        }
    }

    private void ProcessLeftClickUpEvent()
    {
        if(isLeftClickDragging)
        {
            isLeftClickDragging = false;
        }
    }

    private void ProcessMouseDownEvent(Event currentEvents)
    {
        //left click down
        if(currentEvents.button == 0)
        {
            ProcessLeftClickDownEvent();
        }
        else if(currentEvents.button == 1)
        {
            ProcessRightClickDownEvent(currentEvents);
        }
    }

    private void ProcessRightClickDownEvent(Event currentEvents)
    {
        roomNodeGraph.setNodeToDrawConnectionLineFrom(this, currentEvents.mousePosition);
    }

    private void ProcessLeftClickDownEvent()
    {
        Selection.activeObject = this;
        
        // if(isSelected == true)
        // {
        //     isSelected = false;
        // }
        // else
        // {
        //     isSelected = true;
        // }
    }
    public bool AddChildRoomNodeIDToRoomNode(string childID)
    {
        chillRoomNodeIDList.Add(childID);
        return true;
    }
    public bool AddParentRoomNodeIDToRoomNode(string parentID)
    {
        parentRoomNodeIDList.Add(parentID);
        return true;
    }
#endif
}
