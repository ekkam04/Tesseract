using UnityEngine;
using MoreMountains.Feedbacks;

namespace Ekkam
{
    public class Piece : MonoBehaviour
    {
        public GameObject piecePrefab;
        public Color pieceColor;
        public Player owner;
        
        public MMF_Player dropFeedback;
        
        private MMF_Position positionFeedback;
        private MMF_Scale scaleFeedback;
        
        public void InitializeValues(GameObject piecePrefab, Color pieceColor, Player owner)
        {
            this.piecePrefab = piecePrefab;
            this.pieceColor = pieceColor;
            this.owner = owner;
        }
        public void SpawnPrefab()
        {
            Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y + 18f, transform.position.z);
            GameObject piece = Instantiate(piecePrefab, spawnPosition, Quaternion.identity, this.transform); 
            piece.GetComponent<Renderer>().material.color = pieceColor;
            
            scaleFeedback = dropFeedback.FeedbacksList[0] as MMF_Scale;
            scaleFeedback.AnimateScaleTarget = piece.transform;
            
            positionFeedback = dropFeedback.FeedbacksList[3] as MMF_Position;
            positionFeedback.AnimatePositionTarget = piece;
            positionFeedback.DestinationPosition = transform.position;
            
            dropFeedback.PlayFeedbacks();
        }
    }
}