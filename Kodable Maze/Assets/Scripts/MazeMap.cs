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
    private List<MapTile> curPath = new List<MapTile>(), potentialPaths = new List<MapTile>();
    private GameObject[,] maze;
    private MapTile startTile = null, destinationTile = null;
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

    public void traverseDestToStartAndParseByLevel(MapTile destinationTile, List<MapTile> bestPath, List<MapTile> allSolutions)
    {
        // add the next block
        // create a list for the 4 possible next moves
        // x, y maps directly from world space to our 2D array
        bestPath.Add(destinationTile);
        destinationTile.GetComponent<SpriteRenderer>().color = Color.yellow;
        List<MapTile> adjSpaces = new List<MapTile>(4);
        int x = (int)destinationTile.transform.position.x;
        int y = (int)destinationTile.transform.position.y;

        // left
        int xLeft = x - 1;
        if (xLeft > 0)
        {
            MapTile leftTile = maze[xLeft, y].GetComponent<MapTile>();
            if (leftTile.level < destinationTile.level && allSolutions.Contains(leftTile) && leftTile.type == MapTile.TileType.Space)
            {
                leftTile.GetComponent<SpriteRenderer>().color = Color.magenta;
                adjSpaces.Add(leftTile);
            }
        }

        // right
        int xRight = x + 1;
        if (xRight < mWidth)
        {
            MapTile rightTile = maze[xRight, y].GetComponent<MapTile>();
            if (rightTile.level < destinationTile.level && allSolutions.Contains(rightTile) && rightTile.type == MapTile.TileType.Space)
            {
                rightTile.GetComponent<SpriteRenderer>().color = Color.magenta;
                adjSpaces.Add(rightTile);
            }
        }

        // top
        int yUp = y + 1;
        if (yUp < mHeight)
        {
            MapTile upTile = maze[x, yUp].GetComponent<MapTile>();
            if (upTile.level < destinationTile.level && allSolutions.Contains(upTile) && upTile.type == MapTile.TileType.Space)
            {
                upTile.GetComponent<SpriteRenderer>().color = Color.magenta;
                adjSpaces.Add(upTile);
            }
        }

        // down
        int yDown = y - 1;
        if (yDown > 0)
        {
            MapTile downTile = maze[x, yDown].GetComponent<MapTile>();
            if (downTile.level < destinationTile.level && allSolutions.Contains(downTile) && downTile.type == MapTile.TileType.Space)
            {
                downTile.GetComponent<SpriteRenderer>().color = Color.magenta;
                adjSpaces.Add(downTile);
            }
        }

        // now sort the adjacent valid spaces by ascending order
        // and then only keep the first element (the smallest level adj. space)
        float lowestLevelFound = Mathf.Infinity;
        MapTile bestTile = null;
        foreach (MapTile adjTile in adjSpaces)
        {
            if (adjTile.level < lowestLevelFound)
            {
                bestTile = adjTile;
                lowestLevelFound = adjTile.level;
            }
        }

        // todo - call the next recursive function
        // with the best adjacent tile
        if (bestTile == startTile || bestTile == null)
        {
            Debug.Log("Found the starting tile, we're done!");
        }
        else
        {
            traverseDestToStartAndParseByLevel(bestTile, bestPath, allSolutions);
        }
    }

    public List<MapTile> findAllPathsAndLevels(MapTile curTile, int level)
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
                            findAllPathsAndLevels(rightTile, level + 1);
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
                            findAllPathsAndLevels(downTile, level + 1);
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
                            findAllPathsAndLevels(leftTile, level + 1);
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
                            findAllPathsAndLevels(upTile, level + 1);
                        }
                    }

                    // found a dead end / possible exit
                    if (endReached && level > mMaxLevel)
                    {
                        // iterate over solved path and find any
                        // tiles that are already in the cur path. dont remove those
                        potentialPaths.AddRange(curPath);
                        mMaxLevel = level;
                    }
                }
            }
        }

        // clear the temporary storage for paths / cur
        // sort the existing paths to dead ends using level comparator
        potentialPaths.Sort(MapTile.SortByLevel);

        // test colors
        foreach (MapTile tile in potentialPaths)
        {
            tile.GetComponent<SpriteRenderer>().color = Color.green;
        }

        // reverse from the destination by level
        // color the destination block
        // store the destination / end tile here
        destinationTile = potentialPaths[potentialPaths.Count - 1];

        // return the solved path, largest level
        return potentialPaths;
    }

    public MapTile getPlayerDestination()
    {
        return destinationTile;
    }

    public MapTile getPlayerStart()
    {
        return startTile;
    }
}
