﻿using System.Collections;
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
            int width = mazeStr.IndexOf(NEWLINE) + 1;
            int height = mazeStr.Length / width;
            int x = 0;
            int y = height - 1;
            maze = new GameObject[width - 1, height];

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

                if (x == width)
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

    public static Vector3 calculateShortestPath(Player player)
    {
        Vector3 nextMove = Vector3.one;
        return nextMove;
    }
}