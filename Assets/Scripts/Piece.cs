using UnityEngine;

namespace Ekkam
{
    public class Piece : MonoBehaviour
    {
        public GameObject piecePrefab;
        public Color pieceColor;
        public Player owner;
        public int ownerPlayerNumber = 0;
        
        public void InitializeValues(GameObject piecePrefab, Color pieceColor, Player owner, int ownerPlayerNumber)
        {
            this.piecePrefab = piecePrefab;
            this.pieceColor = pieceColor;
            this.owner = owner;
            this.ownerPlayerNumber = ownerPlayerNumber;
        }
        public void SpawnPrefab()
        {
            GameObject piece = Instantiate(piecePrefab, transform.position, Quaternion.identity, this.transform); 
            piece.GetComponent<Renderer>().material.color = pieceColor;
        }
    }
}