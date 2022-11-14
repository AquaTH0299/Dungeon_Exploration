using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNodeTypeListSO : ScriptableObject
{
    [Space(10)]
    [Header("Room Node Type List")]
    [Tooltip("This list should be populated with all the RoomBadeTypeSO for the game - it is used instead of the enum")]
    public List<RoomNodeTypeSO> list;
#if UNITY_EDITOR
    private void OnValidate()
    {
        helperUtilities.ValidateCheckEnumerableValues(this, nameof(list), list);
    }
#endif
}
