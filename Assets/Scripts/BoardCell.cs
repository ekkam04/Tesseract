using UnityEngine;

namespace Ekkam
{
    public class BoardCell : MonoBehaviour
    {
        public Vector3 index;
        public Piece occupant;
        public int ownerPlayerNumber = 0;
        
        // private void Awake()
        // {
        //     occupant = null;
        // }
    }
}