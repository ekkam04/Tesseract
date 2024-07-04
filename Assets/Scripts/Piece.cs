using UnityEngine;

namespace Ekkam
{
    public class Piece : MonoBehaviour
    {
        public GameObject piecePrefab;
        public Color pieceColor;
        public Player owner;
        
        public void InitializeValues(GameObject piecePrefab, Color pieceColor, Player owner)
        {
            this.piecePrefab = piecePrefab;
            this.pieceColor = pieceColor;
            this.owner = owner;
        }
        public void SpawnPrefab()
        {
            GameObject piece = Instantiate(piecePrefab, transform.position, Quaternion.identity, this.transform); 
            piece.GetComponent<Renderer>().material.color = pieceColor;
        }
    }
}