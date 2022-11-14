using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeType",menuName = "Scpirtable Objects/Dungeon/Room Node Type")]
public class RoomNodeTypeSO : ScriptableObject
{
    public string roomNodeTypeName;

    [Header("Only flag the RoomNodeTypes that should be visible in the editor")]
    public bool displayInNodeGraphEditor = true;
    [Header("One Type Should Be A Corridor")]
    public bool isCorridor;
    [Header("One Type Should Be A CorridorNS")]
    public bool isCorridorNS;
    [Header("One Type Should Be A CorriderEW")]
    public bool isCorridorEW;
    [Header("One Type Should Be An Entrance")]
    public bool isEntrance;
    [Header("One Type Should Be A Boss Room")]
    public bool isBossRoom;
    [Header("One Type Should Be None(Unassigned)")]
    public bool isNone;
#if UNITY_EDITOR
    private void OnValidate() 
    {
        helperUtilities.ValidateCheckEmptyString(this, nameof(roomNodeTypeName), roomNodeTypeName);
    }
#endif
}
