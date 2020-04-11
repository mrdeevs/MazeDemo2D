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

    // Start is called before the first frame update
    void Start()
    {
        mLastMove = Time.unscaledTime;
        mMap = FindObjectOfType<MazeMap>();
    }

    // Update is called once per frame
    void Update()
    {
        // Allow player movement to automatically happen
        // every MovementInterval seconds
        if (mState == PlayerState.Moving && Time.unscaledTime - mLastMove > MovementInterval)
        {
            mLastMove = Time.unscaledTime;
            GameObject nextTile = mMap.calculateNextMove(mMap.getPlayerStart(), 1);

            // make sure its valid
            // otherwise we won't move til the next frame
            if (nextTile != null)
            {
                transform.position = nextTile.transform.position;
                nextTile.GetComponent<MapTile>().alreadyVisited = true;
            }
        }
    }

    public void UpdatePlayerState(PlayerState newState)
    {
        mState = newState;
    }
}
