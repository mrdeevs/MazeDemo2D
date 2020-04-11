using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float MovementInterval = 3.0f; // make 0 to move in real time each time

    public enum PlayerState
    {
        Idle, Moving
    }

    private PlayerState mState = PlayerState.Idle;
    private float mLastMove;
    private MazeMap mMap;
    private List<MapTile> mPaths;

    // Start is called before the first frame update
    void Start()
    {
        mLastMove = Time.unscaledTime;
        mMap = FindObjectOfType<MazeMap>();
        mPaths = mMap.findPathsAndLevels(mMap.getPlayerStart(), 1);
        mPaths.Sort(MapTile.SortByLevel);

        List<int> removeIndicies = new List<int>();
        foreach (MapTile tile in mPaths)
        {
            tile.GetComponent<SpriteRenderer>().color = Color.green;
            int index = mPaths.IndexOf(tile);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Allow player movement to automatically happen
        // every MovementInterval seconds
        if (mState == PlayerState.Moving && Time.unscaledTime - mLastMove > MovementInterval)
        {
            mLastMove = Time.unscaledTime;

            // make sure its valid
            // otherwise we won't move til the next frame
            if (mPaths != null && mPaths.Count > 0)
            {
                mPaths[0].alreadyVisited = true;
                transform.position = mPaths[0].transform.position;
                mPaths.RemoveAt(0);
            }
        }
    }

    public void UpdatePlayerState(PlayerState newState)
    {
        mState = newState;
    }
}
