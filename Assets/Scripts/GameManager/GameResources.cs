using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResources : MonoBehaviour
{
    private static GameResources instance;
    public static GameResources Instance
    {
        get
        {
            if(instance == null)
            {
                instance = Resources.Load<GameResources>("GameResources");
            }
            return instance;
        }
    }
    [Space(35)]
    [Header("Dungeon")]
    [Tooltip("populate with the dungeon RoomNodeTyleListSO")]
    public RoomNodeTypeListSO roomNodeTypeList;
}
