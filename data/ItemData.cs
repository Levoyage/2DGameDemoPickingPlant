using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
None,
rafflesia_arnoldii,
ginger,
orchid_ground
}

[CreateAssetMenu()]
public class ItemData:ScriptableObject
{
    public ItemType type=ItemType.None;
    public Sprite sprite;
    public GameObject prefab;
    public int maxCount=1;
}