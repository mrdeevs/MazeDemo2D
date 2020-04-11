using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeMap : MonoBehaviour
{
    public TextAsset mazeFile = null;
    public GameObject wallPrefab, spacePrefab;
    public Player player;

    private List<char> walls = new List<char>() { '-', '+', '|' };
    private List<char> spaces = new List<char>() { ' ' };
    private GameObject[,] maze;
    private const string NEWLINE = "\n";
    private Vector3 startingSpace = Vector3.zero;
    private int mWidth, mHeight;

    // Start is called before the first frame update
    void Start()
    {
        // INIT
        // Generates a 2D Map given the input Maze.txt data
        // [----]
        if (mazeFile != null && mazeFile.bytes.Length > 0)
        {
            // convert the map data text file into a string
            string mazeStr = mazeFile.text;

            // map starts at 0,0
            // also find out the dimensions using new line sub str
            // new line for max X, and divide that from the total character count to get the number of levels, Y
            int newlineWidth = mazeStr.IndexOf(NEWLINE) + 1;
            mWidth = newlineWidth - 1; // -1 for the extra newline character
            mHeight = mazeStr.Length / newlineWidth;
            int x = 0;
            int y = mHeight - 1;
            maze = new GameObject[mWidth, mHeight];

            // now that we already have dimensions, let's strip out new line
            // characters because it's impacting parsing
            mazeStr = mazeStr.Replace("\n\n", NEWLINE);

            // iterate over each tile and build the map
            for (int i = 0; i < mazeStr.Length; i++)
            {
                if (walls.Contains(mazeStr[i]))
                {
                    // wall char found
                    // create a new wall
                    maze[x, y] = Instantiate(wallPrefab, new Vector3(x, y, 0f), Quaternion.identity);
                }
                else if (spaces.Contains(mazeStr[i]))
                {
                    // space found, open
                    // create a new space
                    Vector3 spaceLoc = new Vector3(x, y, 0f);

                    maze[x, y] = Instantiate(spacePrefab, spaceLoc, Quaternion.identity);

                    // store and remember where we should place the player
                    if (startingSpace == Vector3.zero)
                    {
                        startingSpace = spaceLoc;
                    }
                }

                x++;

                if (x == newlineWidth)
                {
                    // lower y down one column, level
                    // reset x to the beginning again
                    // remember, converting a 1D str to a 2D array, but 0,0 starts at the bottom left in the editor
                    y--;
                    x = 0;
                }
            }

            // init player
            // start player movement by updating state machine
            player.transform.position = startingSpace;
            player.UpdatePlayerState(Player.PlayerState.Moving);
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

    public GameObject calculateNextMove(Vector3 startingPos)
    {
        // find the associated game objects in our mapping
        // start with the starting position game object, then its easy to navigate 2d
        for (int x = 0; x < mWidth; x++)
        {
            for (int y = 0; y < mHeight; y++)
            {
                Vector3 curTilePos = maze[x, y].transform.position;
                if (curTilePos.x == startingPos.x && curTilePos.y == startingPos.y)
                {
                    // current starting pos / player pos
                    maze[x, y].GetComponent<SpriteRenderer>().color = Color.magenta;

                    // right
                    int xRight = x + 1;
                    MapTile rightTile = maze[xRight, y].GetComponent<MapTile>();
                    if (x + 1 < mWidth && rightTile.type == MapTile.TileType.Space && !rightTile.alreadyVisited)
                    {
                        Debug.Log("right");
                        return maze[xRight, y];
                    }

                    // down
                    int yDown = y - 1;
                    MapTile downTile = maze[x, yDown].GetComponent<MapTile>();
                    if (y - 1 > 0 && downTile.type == MapTile.TileType.Space && !downTile.alreadyVisited)
                    {
                        Debug.Log("down");
                        return maze[x, yDown];
                    }

                    // left
                    int xLeft = x - 1;
                    MapTile leftTile = maze[xLeft, y].GetComponent<MapTile>();
                    if (x - 1 > 0 && leftTile.type == MapTile.TileType.Space && !leftTile.alreadyVisited)
                    {
                        Debug.Log("left");
                        return maze[xLeft, y];
                    }

                    // top
                    int yUp = y + 1;
                    MapTile upTile = maze[x, yUp].GetComponent<MapTile>();
                    if (y + 1 < mHeight && upTile.type == MapTile.TileType.Space && !upTile.alreadyVisited)
                    {
                        Debug.Log("up");
                        return maze[x, yUp];
                    }
                }
            }
        }

        return null;
    }
}
