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

        // this generates the player start and end pos. too
        List<MapTile> allPossiblePaths = mMap.findAllPathsAndLevels(mMap.getPlayerStart(), 1);

        // find the best path using existing solutions
        // create a list of paths here to be filled in by traverse()
        List<MapTile> correctPath = new List<MapTile>();
        mMap.traverseDestToStartAndParseByLevel(mMap.getPlayerDestination(), correctPath, allPossiblePaths);

        // now that we have all possible dead ends in the map
        // using a breadth first level search, we need to start
        // at the lowest level and traverse back to the player
        foreach(MapTile finalPathTile in correctPath)
        {

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
