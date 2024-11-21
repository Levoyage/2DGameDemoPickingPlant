using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    private Dictionary<int, int> plantCollection = new Dictionary<int, int>();

    public void AddPlant(int plantID, string plantName)
    {
        if (plantCollection.ContainsKey(plantID))
        {
            plantCollection[plantID]++;
        }
        else
        {
            plantCollection[plantID] = 1;
        }

        Debug.Log("Added " + plantName + " (ID: " + plantID + ") to the bag. Total: " + plantCollection[plantID]);

        
    }

    public int GetPlantCount(int plantID)
    {
        return plantCollection.ContainsKey(plantID) ? plantCollection[plantID] : 0;
    }
}
