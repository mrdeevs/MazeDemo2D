using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour
{
    public static int SortByLevel (MapTile t1, MapTile t2)
    {
        return t1.level.CompareTo(t2.level);
    }

    public TileType type;
    public int level;

    public enum TileType { Wall, Space }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
