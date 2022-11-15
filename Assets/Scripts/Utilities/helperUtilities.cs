using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class helperUtilities 
{
    public static bool ValidateCheckEmptyString(Object thisObject, string fileName, string stringToCehck)
    {
        if(stringToCehck == "")
        {
            Debug.Log($"{fileName} is empty and must contain a value in object {thisObject.name.ToString()}");
            return true;
        }
        return false;
    }
    public static bool ValidateCheckEnumerableValues(Object thisObject, string fileName, IEnumerable enumerableObjectToCheck)
    {
        bool error = false;
        int count = 0;
        foreach(var item in enumerableObjectToCheck)
        {
            if(item == null)
            {
                Debug.Log($"{fileName} has null values in object {thisObject.name.ToString()}");
                error = true;
            }
            else
            {
                count++;
            }
        }
        if (count == 0)
        {
            Debug.Log($"{fileName} has no values in object {thisObject.name.ToString()}");
            error = true;
        }
        return error;
    }
}
