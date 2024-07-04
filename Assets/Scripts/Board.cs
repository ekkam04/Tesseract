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
        public void CheckForWinner(int playerNumberToCheck)
        {
            // Check rows, columns, and pillars
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (CheckLine(playerNumberToCheck, new Vector3Int(i, j, 0), Vector3Int.forward)) return;
                    if (CheckLine(playerNumberToCheck, new Vector3Int(i, 0, j), Vector3Int.up)) return;
                    if (CheckLine(playerNumberToCheck, new Vector3Int(0, i, j), Vector3Int.right)) return;
                }
            }

            // Check face diagonals
            for (int i = 0; i < 4; i++)
            {
                if (CheckLine(playerNumberToCheck, new Vector3Int(0, i, 0), new Vector3Int(1, 0, 1))) return;
                if (CheckLine(playerNumberToCheck, new Vector3Int(3, i, 0), new Vector3Int(-1, 0, 1))) return;
                if (CheckLine(playerNumberToCheck, new Vector3Int(0, 0, i), new Vector3Int(1, 1, 0))) return;
                if (CheckLine(playerNumberToCheck, new Vector3Int(3, 0, i), new Vector3Int(-1, 1, 0))) return;
                if (CheckLine(playerNumberToCheck, new Vector3Int(i, 0, 0), new Vector3Int(0, 1, 1))) return;
                if (CheckLine(playerNumberToCheck, new Vector3Int(i, 0, 3), new Vector3Int(0, 1, -1))) return;
            }

            // Check space diagonals
            if (CheckLine(playerNumberToCheck, new Vector3Int(0, 0, 0), new Vector3Int(1, 1, 1))) return;
            if (CheckLine(playerNumberToCheck, new Vector3Int(3, 0, 0), new Vector3Int(-1, 1, 1))) return;
            if (CheckLine(playerNumberToCheck, new Vector3Int(0, 0, 3), new Vector3Int(1, 1, -1))) return;
            if (CheckLine(playerNumberToCheck, new Vector3Int(3, 0, 3), new Vector3Int(-1, 1, -1))) return;
        }
        
        private bool CheckLine(int playerNumberToCheck, Vector3Int start, Vector3Int step)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector3Int pos = start + i * step;
                if (boardCells[pos.x, pos.z, pos.y] == null || boardCells[pos.x, pos.z, pos.y].ownerPlayerNumber != playerNumberToCheck)
                {
                    return false;
                }
            }
            Debug.LogWarning($"Player {playerNumberToCheck} wins!");
            return true;
        }

        private void Update()
        {
            
        }
    }
}