using UnityEngine;
using System.IO;

namespace Ekkam
{
    public class BasePacket
    {
        public enum Type
        {
            None,
            Selector
        }

        public Type type;
        public PlayerData playerData;

        protected MemoryStream wms;
        protected BinaryWriter bw;

        public MemoryStream rms;
        public BinaryReader br;

        public BasePacket()
        {
            this.type = Type.None;
            this.playerData = new PlayerData();
        }

        public BasePacket(Type type, PlayerData playerData)
        {
            this.type = type;
            this.playerData = playerData;
        }

        public void BeginSerialize()
        {
            wms = new MemoryStream();
            bw = new BinaryWriter(wms);

            bw.Write((int)type);
            bw.Write(playerData.id);
            bw.Write(playerData.name);
        }

        public byte[] EndSerialize()
        {
            return wms.ToArray();
        }

        public BasePacket BaseDeserialize(byte[] data)
        {
            rms = new MemoryStream(data);
            br = new BinaryReader(rms);

            type = (Type)br.ReadInt32();
            playerData = new PlayerData(br.ReadString(), br.ReadString());

            return this;
        }

        public virtual byte[] Serialize()
        {
            BeginSerialize();
            return EndSerialize();
        }
    }
    
    public class SelectorPacket : BasePacket
    {
        public Vector2Int selectedCell;
        
        public SelectorPacket() : base(Type.Selector, new PlayerData("", ""))
        {
            this.selectedCell = Vector2Int.zero;
        }
        
        public SelectorPacket(Type type, PlayerData playerData, Vector2Int selectedCell) : base(type, playerData)
        {
            this.selectedCell = selectedCell;
        }
        
        public override byte[] Serialize()
        {
            BeginSerialize();
            bw.Write(selectedCell.x);
            bw.Write(selectedCell.y);
            return EndSerialize();
        }
        
        public SelectorPacket Deserialize(byte[] data)
        {
            SelectorPacket packet = new SelectorPacket();
            BasePacket basePacket = packet.BaseDeserialize(data);
            packet.type = basePacket.type;
            packet.playerData = basePacket.playerData;
            packet.selectedCell = new Vector2Int(basePacket.br.ReadInt32(), basePacket.br.ReadInt32());
            return packet;
        }
    }
}