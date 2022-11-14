using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNodeSO : ScriptableObject
{
    [HideInInspector] public string id;
    [HideInInspector] public List<string> parentRoomIDList = new List<string>();
    [HideInInspector] public List<string> chillRoomIDList = new List<string>();
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    public RoomNodeGraphSO roomNodeType;
    [HideInInspector] public RoomNodeTypeListSO roomNodeTyoeListSO;
}
