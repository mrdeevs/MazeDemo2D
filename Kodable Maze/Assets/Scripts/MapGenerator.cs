using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public TextAsset mazeFile = null;
    public GameObject wallPrefab, spacePrefab;
    public Player player;

    private List<char> walls = new List<char>() { '-', '+', '|' };
    private List<char> spaces = new List<char>() { ' ' };
    private const string NEWLINE = "\n";

    // Start is called before the first frame update
    void Start()
    {
        if (mazeFile != null && mazeFile.bytes.Length > 0)
        {
            // convert the map data text file into a string
            string mazeStr = mazeFile.text;
            Debug.Log("Original maze length: " + mazeStr.Length);

            // map starts at 0,0
            // also find out the dimensions using new line sub str
            // new line for max X, and divide that from the total character count to get the number of levels, Y
            int width = mazeStr.IndexOf(NEWLINE) + 1;
            int height = mazeStr.Length / width;
            int x = 0;
            int y = height - 1;

            Debug.Log("map width : " + width + " map height : " + height + " input data length : " + mazeStr.Length);

            // now that we already have dimensions, let's strip out new line
            // characters because it's impacting parsing
            mazeStr = mazeStr.Replace("\n\n", NEWLINE);
            Debug.Log("Formatted maze length: " + mazeStr.Length);

            // iterate over each tile and build the map
            for (int i = 0; i < mazeStr.Length; i++)
            {
                if (walls.Contains(mazeStr[i]))
                {
                    Debug.Log("Wall Found.");
                    // wall char found
                    // create a new wall
                    Instantiate(wallPrefab, new Vector3(x, y, 0f), Quaternion.identity);
                }
                else if (spaces.Contains(mazeStr[i]))
                {
                    Debug.Log("Space Found.");
                    // space found, open
                    // create a new space
                    Instantiate(spacePrefab, new Vector3(x, y, 0f), Quaternion.identity);
                }
                else
                {
                    if (mazeStr[i] == '\n') Debug.Log("New Line (n) Found.");
                    if (mazeStr[i] == '\r') Debug.Log("New Line (r) Found.");
                }

                // increment the x marker 
                x++;

                if (x == width)
                {
                    // lower y down one column, level
                    // reset x to the beginning again
                    // remember, converting a 1D str to a 2D array, but 0,0 starts at the bottom left in the editor
                    y--;
                    x = 0;
                }
            }

            // INIT PLAYER
            // initialize the player at the top left (0, height)
            player.transform.position = new Vector3(0f, height - 1, 0f);
        }
        else
        {
            Debug.Log("Invalid File input for maze data!");
            // todo show a dialogue in game view
            //
            //
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
