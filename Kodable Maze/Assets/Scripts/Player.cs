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
            Vector3 nextMove = mMap.calculateNextMove(transform.position);
            transform.position = nextMove;
        }
    }

    public void UpdatePlayerState(PlayerState newState)
    {
        mState = newState;
    }
}
