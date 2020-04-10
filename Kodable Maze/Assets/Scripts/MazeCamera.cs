using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCamera : MonoBehaviour
{
    public Player player;

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
            transform.position.x, 
            Mathf.Lerp(transform.position.y, player.transform.position.y, 1.5f * Time.deltaTime), 
            transform.position.z);
    }
}
