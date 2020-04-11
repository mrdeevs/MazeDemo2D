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
    private List<MapTile> curPath = new List<MapTile>(), solvedPath = new List<MapTile>();
    private GameObject[,] maze;
    private MapTile startTile = null, endTile = null;
    private const string NEWLINE = "\n";
    private int mWidth, mHeight, mMaxLevel;

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

                    // player start
                    // store and remember where we should place the player
                    if (startTile == null)
                    {
                        startTile = maze[x, y].GetComponent<MapTile>();
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
            player.transform.position = startTile.transform.position;
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

    public List<MapTile> findPathsAndLevels(MapTile curTile, int level)
    {
        // find the associated game objects in our mapping
        // start with the starting position game object, then its easy to navigate 2d

        // current starting pos / player pos
        Vector3 curTilePos = curTile.transform.position;

        for (int x = 0; x < mWidth; x++)
        {
            for (int y = 0; y < mHeight; y++)
            {
                // found the location in the 2d array for the current
                // tile. now we need to recursively increment level and go right, down, left top
                if (maze[x, y].GetComponent<MapTile>() == curTile)
                {
                    curTile.level = level;
                    bool endReached = true;
                    curPath.Add(curTile);

                    // right
                    int xRight = x + 1;
                    if (xRight < mWidth)
                    {
                        MapTile rightTile = maze[xRight, y].GetComponent<MapTile>();
                        if (rightTile.type == MapTile.TileType.Space && rightTile.level == 0)
                        {
                            endReached = false;
                            findPathsAndLevels(rightTile, level + 1);
                        }
                    }

                    // down
                    int yDown = y - 1;
                    if (yDown > 0)
                    {
                        MapTile downTile = maze[x, yDown].GetComponent<MapTile>();
                        if (downTile.type == MapTile.TileType.Space && downTile.level == 0)
                        {
                            endReached = false;
                            findPathsAndLevels(downTile, level + 1);
                        }
                    }

                    // left
                    int xLeft = x - 1;
                    if (xLeft > 0)
                    {
                        MapTile leftTile = maze[xLeft, y].GetComponent<MapTile>();
                        if (leftTile.type == MapTile.TileType.Space && leftTile.level == 0)
                        {
                            endReached = false;
                            findPathsAndLevels(leftTile, level + 1);
                        }
                    }

                    // top
                    int yUp = y + 1;
                    if (yUp < mHeight)
                    {
                        MapTile upTile = maze[x, yUp].GetComponent<MapTile>();
                        if (upTile.type == MapTile.TileType.Space && upTile.level == 0)
                        {
                            endReached = false;
                            findPathsAndLevels(upTile, level + 1);
                        }
                    }

                    if(endReached)
                    {
                        Debug.Log("End reached! Level: " + level);
                        curTile.GetComponent<SpriteRenderer>().color = Color.red;
                        
                        if(level > mMaxLevel)
                        {
                            Debug.Log("New maximum found. Level: " + level);
                            solvedPath = new List<MapTile>(curPath);
                            curPath.Clear();
                            mMaxLevel = level;
                        }
                    }
                }
            }
        }

        // return the solved path, longest
        return solvedPath;
    }

    public MapTile getPlayerStart()
    {
        return startTile;
    }
}
