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
            // Display a popup using the roomType name value that can be selected from(default to the current set roomNodeType) 
            int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);
            int selection = EditorGUILayout.Popup("",selected, GetRoomNodeTypeToDisplay());
            roomNodeType = roomNodeTypeList.list[selection];
            // if the room type selection has changed making child connections potentially invalid
            if(roomNodeTypeList.list[selected].isCorridor && !roomNodeTypeList.list[selection].isCorridor || !roomNodeTypeList.list[selected].isCorridor && roomNodeTypeList.list[selection].isCorridor || !roomNodeTypeList.list[selected].isBossRoom && roomNodeTypeList.list[selection].isBossRoom)
            {
                if(chillRoomNodeIDList.Count > 0)
                {
                    for(int i = chillRoomNodeIDList.Count - 1; i >= 0; i--)
                    {
                        //get child room node
                        RoomNodeSO childRoomNode = roomNodeGraph.GetRoomNode(chillRoomNodeIDList[i]);
                        // if the child room node is not null
                        if(childRoomNode != null)
                        {
                            // Remove child ID from parent room node
                            RemoveChildRoomNodeIDFromRoomNode(childRoomNode.id);
                            // Remove parent ID from child room node
                            childRoomNode.RemoveParentRoomNodeIDFromRoomNode(id);
                        }
                    }
                }
            }
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
    private void ProcessMouseDownEvent(Event currentEvents)
    {
        //left click down
        if(currentEvents.button == 0)
        {
            ProcessLeftClickDownEvent();
        }
        // right click down
        else if(currentEvents.button == 1)
        {
            ProcessRightClickDownEvent(currentEvents);
        }
    }
    private void ProcessLeftClickDownEvent()
    {
        Selection.activeObject = this;
        //toggle node selection
        if(isSelected == true)
        {
            isSelected = false;
        }
        else
        {
            isSelected = true;
        }
    }
    
    private void ProcessRightClickDownEvent(Event currentEvents)
    {
        roomNodeGraph.setNodeToDrawConnectionLineFrom(this, currentEvents.mousePosition);
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
        DragNode(currentEvents.delta);
        GUI.changed = true;
    }

    public void DragNode(Vector2 delta)
    {
        rect.position += delta;
        EditorUtility.SetDirty(this);
    }
    public bool AddChildRoomNodeIDToRoomNode(string childID)
    {
        if(IsChildRoomValid(childID))
        {
            chillRoomNodeIDList.Add(childID);
            return true;
        }
        return false;
    }

    private bool IsChildRoomValid(string childID)
    {
        bool isConnectedBossNodeAlready = false;
        foreach(RoomNodeSO roomNode in roomNodeGraph.roomNodeList)
        {
            if(roomNode.roomNodeType.isBossRoom && roomNode.parentRoomNodeIDList.Count > 0)
            {
                isConnectedBossNodeAlready = true;
            }
        }
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isBossRoom && isConnectedBossNodeAlready)
        {
            return false;
        }
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isNone)
        {
            return false;
        }
        if (chillRoomNodeIDList.Contains(childID))
        {
            return false;
        }
        if( id == childID)
        {
            return false;
        }
        if (parentRoomNodeIDList.Contains(childID))
        {
            return false;
        }
        if (roomNodeGraph.GetRoomNode(childID).parentRoomNodeIDList.Count > 0)    
        {
            return false;
        }
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && roomNodeType.isCorridor)
        {
            return false;
        }
        if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && !roomNodeType.isCorridor)
        {
            return false;
        }
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && chillRoomNodeIDList.Count >= Settings.maxChildCorridors)
        {
            return false;
        }
        if(roomNodeGraph.GetRoomNode(childID).roomNodeType.isEntrance)
        {
            return false;
        }
        if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && chillRoomNodeIDList.Count > 0)
        {
            return false;
        }
        return true;
    }

    public bool AddParentRoomNodeIDToRoomNode(string parentID)
    {
        parentRoomNodeIDList.Add(parentID);
        return true;
    }
    public bool RemoveChildRoomNodeIDFromRoomNode(string childID)
    {
        if(chillRoomNodeIDList.Contains(childID))
        {
            chillRoomNodeIDList.Remove(childID);
            return true;
        }
        return false;
    }
        public bool RemoveParentRoomNodeIDFromRoomNode(string childID)
    {
        if(parentRoomNodeIDList.Contains(childID))
        {
            parentRoomNodeIDList.Remove(childID);
            return true;
        }
        return false;
    }
#endif
}
