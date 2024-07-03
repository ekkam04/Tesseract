using System;
using UnityEngine;

namespace Ekkam
{
    public class Board : MonoBehaviour
    {
        public BoardCell[,,] boardCells = new BoardCell[4, 4, 4];
        [SerializeField] private Vector3 boardOrigin;
        [SerializeField] private float cellSpacing;
    
        void Start()
        {
            GameObject[] cellObjects = GameObject.FindGameObjectsWithTag("BoardCell");

            foreach (GameObject cellObject in cellObjects)
            {
                BoardCell cell = cellObject.GetComponent<BoardCell>();
                if (cell != null)
                {
                    Vector3 relativePosition = cellObject.transform.position - boardOrigin;
            
                    // Calculate the indices considering the spacing
                    int x = Mathf.RoundToInt(relativePosition.x / cellSpacing);
                    int y = Mathf.RoundToInt(relativePosition.y / cellSpacing);
                    int z = Mathf.RoundToInt(relativePosition.z / cellSpacing);

                    // Ensure the indices are within the array bounds
                    if (x >= 0 && x < 4 && y >= 0 && y < 4 && z >= 0 && z < 4)
                    {
                        boardCells[x, z, y] = cell;
                        cell.index = new Vector3(x, z, y);
                    }
                    else
                    {
                        Debug.LogError($"Cell at {cellObject.transform.position} is out of bounds");
                    }
                }
            }
        }
        
        // Check for a win if the player has a piece in all 4 cells of a row, column, or diagonal, anywhere on the board
        // boardCells[,,].ownerPlayerNumber is the player number of the piece in that cell
        public void CheckForWinner(int playerNumberToCheck)
        {
            // Check rows
            for (int z = 0; z < 4; z++)
            {
                for (int y = 0; y < 4; y++)
                {
                    if (boardCells[0, z, y].ownerPlayerNumber == playerNumberToCheck &&
                        boardCells[1, z, y].ownerPlayerNumber == playerNumberToCheck &&
                        boardCells[2, z, y].ownerPlayerNumber == playerNumberToCheck &&
                        boardCells[3, z, y].ownerPlayerNumber == playerNumberToCheck)
                    {
                        Debug.Log($"Player {playerNumberToCheck} wins!");
                        return;
                    }
                }
            }
            
            // Check columns
            for (int z = 0; z < 4; z++)
            {
                for (int x = 0; x < 4; x++)
                {
                    if (boardCells[x, z, 0].ownerPlayerNumber == playerNumberToCheck &&
                        boardCells[x, z, 1].ownerPlayerNumber == playerNumberToCheck &&
                        boardCells[x, z, 2].ownerPlayerNumber == playerNumberToCheck &&
                        boardCells[x, z, 3].ownerPlayerNumber == playerNumberToCheck)
                    {
                        Debug.Log($"Player {playerNumberToCheck} wins!");
                        return;
                    }
                }
            }
            
            // Check diagonals
                
        }

        private void Update()
        {
            
        }
    }
}