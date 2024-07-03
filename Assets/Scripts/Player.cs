using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Ekkam
{
    public class Player : MonoBehaviour
    {
        public Vector2 touchPosition;
        public bool allowTap = true;
        
        [SerializeField] GameObject piecePrefab;
        
        public GameObject selectedPiecePrefab;
        public Color selectedPieceColor;
        
        public Board board;
        
        [Header("Player References")]
        
        public Color player1Color;
        public Color player2Color;
        
        public GameObject player1PiecePrefab;
        public GameObject player2PiecePrefab;
        
        public int playerTurn = 1;
        public TMP_Text turnText;

        private void Start()
        {
            ChangeTurn(1);
        }

        public void OnTapPerformed(InputAction.CallbackContext context)
        {
            if (!allowTap) return;
            
            Ray ray = Camera.main.ScreenPointToRay(touchPosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                GameObject tappedObject = hit.collider.gameObject;
                if (tappedObject.CompareTag("Selector"))
                {
                    allowTap = false;
                    var selector = tappedObject.GetComponent<PieceSelector>();
                    if (selector != null)
                    {
                        print("selector tapped, index: " + selector.index);
                        DropPiece(selector.index);
                        Invoke(nameof(AllowTapping), 0.25f);
                    }
                }
            }
        }

        private void ChangeTurn(int player)
        {
            playerTurn = player;
            selectedPieceColor = player == 1 ? player1Color : player2Color;
            selectedPiecePrefab = player == 1 ? player1PiecePrefab : player2PiecePrefab;
            turnText.color = player == 1 ? player1Color : player2Color;
            turnText.text = $"Player {player}'s turn";
        }
        
        private void AllowTapping()
        {
            allowTap = true;
        }
        
        public void OnPositionChanged(InputAction.CallbackContext context)
        {
            touchPosition = context.ReadValue<Vector2>();
        }

        private void DropPiece(Vector2 index)
        {
            BoardCell lowestEmptyCell = null;
            float lowestZ = 4f;
            
            foreach (BoardCell cell in board.boardCells)
            {
                if (cell != null)
                {
                    if (new Vector2(cell.index.x, cell.index.y) == index)
                    {
                        print("found cell at index: " + cell.index + " with occupant: " + cell.occupant);
                        if (cell.occupant == null && cell.index.z < lowestZ)
                        {
                            lowestEmptyCell = cell;
                            lowestZ = cell.index.z;
                        }
                    }
                }
            }

            if (lowestEmptyCell != null)
            {
                print("lowest empty cell: " + lowestEmptyCell.index);
                SpawnPiece(lowestEmptyCell.transform.position);
                lowestEmptyCell.occupant = piecePrefab.GetComponent<Piece>();
                lowestEmptyCell.ownerPlayerNumber = playerTurn;

                board.CheckForWinner(playerTurn);
                ChangeTurn(playerTurn == 1 ? 2 : 1);
            }
            else
            {
                print("no empty cell found");
            }
        }

        private void SpawnPiece(Vector3 piecePosition)
        {
            GameObject piece = Instantiate(piecePrefab, piecePosition, Quaternion.identity);
            piece.GetComponent<Piece>().InitializeValues(selectedPiecePrefab, selectedPieceColor, this, playerTurn);
            piece.GetComponent<Piece>().SpawnPrefab();
        }
    }
}