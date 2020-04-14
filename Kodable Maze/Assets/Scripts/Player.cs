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
    private List<MapTile> mMoves;

    // Start is called before the first frame update
    void Start()
    {
        mLastMove = Time.unscaledTime;

        mMap = FindObjectOfType<MazeMap>();

        // this generates the player start and end pos. too
        List<MapTile> allPossiblePaths = mMap.findAllPathsAndLevels(mMap.getPlayerStart(), 1);

        if (allPossiblePaths != null && allPossiblePaths.Count > 0)
        {
            // find the best path using existing solutions
            // create a list of paths here to be filled in by traverse()
            List<MapTile> correctPaths = new List<MapTile>();
            mMap.traverseDestToStartAndParseByLevel(mMap.getPlayerDestination(), correctPaths, allPossiblePaths);

            // assign the correct path to moves so the player
            // can get around. ALSO we need to reverse it, since we started the
            // search at the destination position at the end
            mMoves = correctPaths;
            mMoves.Reverse();
        } else
        {
            mMap.ShowError(true);
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
            if (mMoves != null && mMoves.Count > 0)
            {
                transform.position = mMoves[0].transform.position;
                mMoves.RemoveAt(0);
            }
        }
    }

    public void UpdatePlayerState(PlayerState newState)
    {
        mState = newState;
    }
}
