using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Multiplayer.Playmode;
using System.Linq;

namespace Ekkam
{
    public class Player : MonoBehaviour
    {
        public Vector2 touchPosition;
        public bool allowTap = false;
        
        [SerializeField] GameObject piecePrefab;
        
        public GameObject selectedPiecePrefab;
        public Color selectedPieceColor;
        
        public Board board;
        public GameObject selector;
        
        public int playerNumber;
        
        [Header("Player References")]
        
        public Player opponent;
        
        public Color player1Color;
        public Color player2Color;
        
        public GameObject player1PiecePrefab;
        public GameObject player2PiecePrefab;
        
        public int playerTurn = 1;
        public TMP_Text turnText;

        private void Start()
        {
            // var mppmTag = CurrentPlayer.ReadOnlyTags();
            // if (mppmTag.Contains("P1"))
            // {
            //     playerNumber = 1;
            //     Debug.LogWarning("Player 1");
            // }
            // else if (mppmTag.Contains("P2"))
            // {
            //     playerNumber = 2;
            //     Debug.LogWarning("Player 2");
            // }
            
            // ChangeTurn(1);
            AllowTapping(false);
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
                    AllowTapping(false);
                    var selector = tappedObject.GetComponent<PieceSelector>();
                    if (selector != null)
                    {
                        print("selector tapped, index: " + selector.index);
                        DropPiece(selector.index, true);
                        // Invoke(nameof(AllowTapping), 0.25f);
                    }
                }
            }
        }
        
        public void Initialize(int player)
        {
            playerNumber = player;
            selectedPieceColor = player == 1 ? player1Color : player2Color;
            selectedPiecePrefab = player == 1 ? player1PiecePrefab : player2PiecePrefab;
            
            opponent.playerNumber = player == 1 ? 2 : 1;
            opponent.selectedPieceColor = player == 1 ? player2Color : player1Color;
            opponent.selectedPiecePrefab = player == 1 ? player2PiecePrefab : player1PiecePrefab;
        }
        
        public void AllowTapping(bool allow)
        {
            if (selector != null) selector.SetActive(allow);
            allowTap = allow;
            turnText.text = allow ? "Your turn!" : "Waiting for opponent...";
        }
        
        public void OnPositionChanged(InputAction.CallbackContext context)
        {
            touchPosition = context.ReadValue<Vector2>();
        }

        public void DropPiece(Vector2Int index, bool isOwner)
        {
            BoardCell lowestEmptyCell = null;
            float lowestZ = 4f;
            
            foreach (BoardCell cell in board.boardCells)
            {
                if (cell != null)
                {
                    if (new Vector2(cell.index.x, cell.index.y) == index)
                    {
                        // print("found cell at index: " + cell.index + " with occupant: " + cell.occupant);
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
                lowestEmptyCell.ownerPlayerNumber = playerNumber;
                
                if (isOwner) Client.instance.SendSelector(index);

                board.CheckForWinner(playerNumber);
                // ChangeTurn(playerTurn == 1 ? 2 : 1);
            }
            else
            {
                print("no empty cell found");
            }
        }

        private void SpawnPiece(Vector3 piecePosition)
        {
            GameObject piece = Instantiate(piecePrefab, piecePosition, Quaternion.identity);
            piece.GetComponent<Piece>().InitializeValues(selectedPiecePrefab, selectedPieceColor, this);
            piece.GetComponent<Piece>().SpawnPrefab();
        }
    }
}