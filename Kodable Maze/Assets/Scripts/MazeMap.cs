using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeMap : MonoBehaviour
{
    public TextAsset mazeFile = null;
    public GameObject wallPrefab, spacePrefab, errorPrefab;
    public Player player;

    private List<char> walls = new List<char>() { '-', '+', '|' };
    private List<char> spaces = new List<char>() { ' ' };
    private List<MapTile> curPath = new List<MapTile>(), potentialPaths = new List<MapTile>();
    private GameObject[,] maze;
    private MapTile startTile = null, endTile = null;
    private int mWidth, mHeight, mMaxLevel;
    private const int MinimumValidHeight = 3;

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

            List<string> mazeByLines = new List<string>();

            foreach (string line in mazeStr.Split("\n"[0]))
            {
                bool nothingFound = true;

                // check for spaces
                foreach (char spaceChar in spaces)
                {
                    foreach (char lineChar in line)
                    {
                        if (spaceChar == lineChar)
                        {
                            nothingFound = false;
                            break;
                        }
                    }
                }

                // check for walls
                foreach (char wall in walls)
                {
                    foreach (char lineChar in line)
                    {
                        if (wall == lineChar)
                        {
                            nothingFound = false;
                            break;
                        }
                    }
                }

                // only add valid lines
                if (!nothingFound)
                {
                    mazeByLines.Add(line);
                    //Debug.Log("Adding line: " + line + " length: " + line.Length);
                }
            }

            // TODO - the minimum size of a grid maze must be
            // at least 2 walls and 1 empty row of spaces to walk in
            // otherwise its invalid
            if (mazeByLines.Count < MinimumValidHeight)
            {
                ShowError(true);
                Application.Quit();
                return;
            }

            // map starts at 0,0
            // also find out the dimensions using new line sub str
            // new line for max X, and divide that from the total character count to get the number of levels, Y
            //int newlineWidth = mazeStr.IndexOf(NEWLINE) + 1;
            mWidth = mazeByLines[0].Length;
            mHeight = mazeByLines.Count;
            maze = new GameObject[mWidth, mHeight];

            int lineRow = 0;
            int lineIndex = 0;

            for (int y = mHeight - 1; y >= 0; y--)
            {
                // iterate over each tile and build the map
                for (int x = 0; x < mWidth; x++)
                {
                    // wall check
                    if (walls.Contains(mazeByLines[lineRow][lineIndex]))
                    {
                        // wall char found
                        // create a new wall
                        maze[x, y] = Instantiate(wallPrefab, new Vector3(x, y, 0f), Quaternion.identity);
                    }
                    // space check
                    else if (spaces.Contains(mazeByLines[lineRow][lineIndex]))
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
                            startTile.GetComponent<SpriteRenderer>().color = Color.blue;
                        }
                    }

                    lineIndex++;
                }

                lineIndex = 0;
                lineRow++;
            }

            // init player
            // start player movement by updating state machine
            player.transform.position = startTile.transform.position;
            player.UpdatePlayerState(Player.PlayerState.Moving);

            // Destination
            // setup the destination tile
            endTile = maze[mWidth - 1, 1].GetComponent<MapTile>();
        }
        else
        {
            ShowError(true);
            Application.Quit();
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
        //destinationTile.GetComponent<SpriteRenderer>().color = Color.yellow;
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
            endTile.GetComponent<SpriteRenderer>().color = Color.green;
        }
        else
        {
            traverseDestToStartAndParseByLevel(bestTile, bestPath, allSolutions);
        }
    }

    public List<MapTile> findAllPathsAndLevels(MapTile curTile, int level)
    {
        if (curTile == null)
            return null;

        // starting tile for search
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
                        //curTile.GetComponent<SpriteRenderer>().color = Color.red;

                        // SPECIAL EDGE CASE: only 1 empty row and 2 walls, set dest. level
                        if (mHeight == MinimumValidHeight && endTile.level == 0)
                        {
                            endTile.level = mWidth;
                        }
                    }
                }
            }
        }

        // clear the temporary storage for paths / cur
        // sort the existing paths to dead ends using level comparator
        potentialPaths.Sort(MapTile.SortByLevel);
        // TEST colors for all possible solutions

        /*foreach (MapTile tile in potentialPaths)
        {
            tile.GetComponent<SpriteRenderer>().color = Color.red;
        }*/

        if (potentialPaths.Contains(endTile))
        {
            // return the solved path, largest level
            return potentialPaths;
        }
        else
        {
            return null;
        }
    }

    public MapTile getPlayerDestination()
    {
        return endTile;
    }

    public MapTile getPlayerStart()
    {
        return startTile;
    }

    public void ShowError(bool showError)
    {
        errorPrefab.SetActive(showError);

    }
}
