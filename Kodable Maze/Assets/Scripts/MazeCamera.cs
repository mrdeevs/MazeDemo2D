using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCamera : MonoBehaviour
{
    public Player player;
    bool firstTimeCentered;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // animate the camera towards centering on
        // the player while its not there yet
        transform.position = new Vector3(
            Mathf.Lerp(transform.position.x, player.transform.position.x, 3f * Time.deltaTime),
            Mathf.Lerp(transform.position.y, player.transform.position.y, 3f * Time.deltaTime),
            transform.position.z);

        int roundedX = Mathf.RoundToInt(transform.position.x);
        int roundedY = Mathf.RoundToInt(transform.position.y);

        if (!firstTimeCentered
            && roundedX == (int)player.transform.position.x
            && roundedY == (int)player.transform.position.y)
        {
            firstTimeCentered = true;
            player.UpdatePlayerState(Player.PlayerState.Moving);
        }
    }
}
