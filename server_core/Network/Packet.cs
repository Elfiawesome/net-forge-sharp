using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace NetForge.Network
{
	// Help with packet class here
	public enum PacketType : ushort
	{
		HelloWorldPacket = 0,
		CHandshake,
		SHandshake,

	}
	
	public static class PacketFactory // NOTE: Idk if I want a factory or a registry
	{
		private static readonly Dictionary<PacketType, Func<BasePacket>> _packetConstructors = new();

		public static void Initialize()
        {
            RegisterPacketType<HelloWorldPacket>(PacketType.HelloWorldPacket);
        }

		private static void RegisterPacketType<T>(PacketType type) where T : BasePacket, new()
        {
            if (_packetConstructors.ContainsKey(type))
            {
                return;
            }
            _packetConstructors[type] = () => new T();
        }

		public static BasePacket? CreatePacket(PacketType type)
        {
            if (_packetConstructors.TryGetValue(type, out var constructor))
            {
                return constructor();
            }
            return null;
        }
	}

	public abstract class BasePacket
	{
		public abstract PacketType PacketId { get; }
		
		public abstract void SerializePayload(BinaryWriter writer);
        public abstract void DeserializePayload(BinaryReader reader);
	}

	public class HelloWorldPacket : BasePacket
	{
		public override PacketType PacketId => PacketType.HelloWorldPacket;
		public string Message { get; set; } = string.Empty;
		public HelloWorldPacket() { }
		public HelloWorldPacket(string message)
		{
			Message = message;
		}
		public override void SerializePayload(BinaryWriter writer)
		{
			writer.Write(Message);
		}
        public override void DeserializePayload(BinaryReader reader)
		{
			Message = reader.ReadString();
		}
	}
}