using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace PacketGenerator
{
	class Program
	{
		static StringBuilder packetProtocolScripts = new StringBuilder();
		static StringBuilder packetIdScripts = new StringBuilder();

		static StringBuilder clientHandlerScripts = new StringBuilder();
		static StringBuilder serverHandlerScripts = new StringBuilder();

		static ushort currentPacketId = 0;
		
		static void Main(string[] args)
		{
			var pdlPaths = new List<string>(args);
			
			foreach(var pdlPath in pdlPaths)
			{
				XmlReaderSettings settings = new XmlReaderSettings()
				{
					IgnoreComments = true,
					IgnoreWhitespace = true
				};

				string prefix = GetFilePrefix(pdlPath);

				using (XmlReader r = XmlReader.Create(pdlPath, settings))
				{
					r.MoveToContent();

					while (r.Read())
					{
						if (r.Depth == 1 && r.NodeType == XmlNodeType.Element)
						{
							ParsePacket(r, prefix);
						}
					}

					string fileText = string.Format(PacketFormat.ScriptFormat, packetIdScripts, packetProtocolScripts, prefix);
					File.WriteAllText($"{prefix}GeneratedPacket.cs", fileText);

					string clientManagerText = string.Format(PacketFormat.ManagerScriptFormat, clientHandlerScripts, prefix);
					File.WriteAllText($"{prefix}ClientPacketManager.cs", clientManagerText);
					
					string serverManagerText = string.Format(PacketFormat.ManagerScriptFormat, serverHandlerScripts, prefix);
					File.WriteAllText($"{prefix}ServerPacketManager.cs", serverManagerText);
				}

				ClearPacketScript();
			}
		}

		private static string GetFilePrefix(string pdlPath)
		{
			if (pdlPath.Contains("INGAME"))
				return "InGame";
			else if (pdlPath.Contains("ROOM"))
				return "Room";

			return string.Empty;		
		}

		public static void ParsePacket(XmlReader r, string prefix)
		{
			if (r.NodeType == XmlNodeType.EndElement)
				return;

			if (r.Name.ToLower() != "packet")
			{
				Console.WriteLine("Invalid packet node");
				return;
			}	

			string packetName = r["name"];
			if (string.IsNullOrEmpty(packetName))
			{
				Console.WriteLine("Packet without name");
				return;
			}

			Tuple<string, string, string> t = ParseMembers(r);

			string packetProtocol = string.Format(PacketFormat.PacketProtocolFormat, packetName, t.Item1, t.Item2, t.Item3, prefix);
			packetProtocolScripts.Append(packetProtocol);

			string packetEnum = string.Format(PacketFormat.packetEnumFormat, packetName, ++currentPacketId) + Environment.NewLine + "\t";
			packetIdScripts.Append(packetEnum);

			if (packetName.StartsWith("RES_"))
			{
				string HandlerFormat = string.Format(PacketFormat.HandlerFormat, prefix, packetName) + Environment.NewLine;
				clientHandlerScripts.Append(HandlerFormat);
			}
			else if (packetName.StartsWith("REQ_"))
			{
				string HandlerFormat = string.Format(PacketFormat.HandlerFormat, prefix, packetName) + Environment.NewLine;
				serverHandlerScripts.Append(HandlerFormat);
			}
		}

		private static void ClearPacketScript()
		{
			packetProtocolScripts.Clear();
			packetIdScripts.Clear();
			clientHandlerScripts.Clear();
			serverHandlerScripts.Clear();
		}

		// {1} 멤버 변수들
		// {2} 멤버 변수 Read
		// {3} 멤버 변수 Write
		public static Tuple<string, string, string> ParseMembers(XmlReader r)
		{
			string memberCode = "";
			string readCode = "";
			string writeCode = "";

			int depth = r.Depth + 1;
			while (r.Read())
			{
				if (r.Depth != depth)
					break;

				string memberName = r["name"];
				if (string.IsNullOrEmpty(memberName))
				{
					Console.WriteLine("Member without name");
					return null;
				}

				if (string.IsNullOrEmpty(memberCode) == false)
					memberCode += Environment.NewLine;
				if (string.IsNullOrEmpty(readCode) == false)
					readCode += Environment.NewLine;
				if (string.IsNullOrEmpty(writeCode) == false)
					writeCode += Environment.NewLine;

				string memberType = r.Name.ToLower();
				switch (memberType)
				{
					case "byte":
					case "sbyte":
						memberCode += string.Format(PacketFormat.MemberFormat, memberType, memberName);
						readCode += string.Format(PacketFormat.ReadByteFormat, memberName, memberType);
						writeCode += string.Format(PacketFormat.WriteByteFormat, memberName, memberType);
						break;
					case "bool":
					case "short":
					case "ushort":
					case "int":
					case "long":
					case "float":
					case "double":
						memberCode += string.Format(PacketFormat.MemberFormat, memberType, memberName);
						readCode += string.Format(PacketFormat.ReadFunctionFormat, memberName, ToMemberType(memberType), memberType);
						writeCode += string.Format(PacketFormat.WriteFunctionFormat, memberName, memberType);
						break;
					case "string":
						memberCode += string.Format(PacketFormat.MemberFormat, memberType, memberName);
						readCode += string.Format(PacketFormat.ReadStringFormat, memberName);
						writeCode += string.Format(PacketFormat.WriteStringFormat, memberName);
						break;
					case "list":
						Tuple<string, string, string> t = ParseList(r);
						memberCode += t.Item1;
						readCode += t.Item2;
						writeCode += t.Item3;
						break;
					default:
						break;
				}
			}

			memberCode = memberCode.Replace("\n", "\n\t");
			readCode = readCode.Replace("\n", "\n\t\t");
			writeCode = writeCode.Replace("\n", "\n\t\t");

			return new Tuple<string, string, string>(memberCode, readCode, writeCode);
		}
		
		public static Tuple<string, string, string> ParseList(XmlReader r)
		{
			string listName = r["name"];
			if (string.IsNullOrEmpty(listName))
			{
				Console.WriteLine("List without name");
				return null;
			}

			Tuple<string, string, string> t = ParseMembers(r);

			string memberCode = string.Format(PacketFormat.FieldFormat,
				FirstCharToUpper(listName),
				FirstCharToLower(listName),
				t.Item1,
				t.Item2,
				t.Item3);

			string readCode = string.Format(PacketFormat.ReadListFormat,
				FirstCharToUpper(listName),
				FirstCharToLower(listName));

			string writeCode = string.Format(PacketFormat.WriteListFormat,
				FirstCharToUpper(listName),
				FirstCharToLower(listName));

			return new Tuple<string, string, string>(memberCode, readCode, writeCode);
		}

		public static string ToMemberType(string memberType)
		{
			switch (memberType)
			{
				case "bool":
					return "ToBoolean";
				case "short":
					return "ToInt16";
				case "ushort":
					return "ToUInt16";
				case "int":
					return "ToInt32";
				case "long":
					return "ToInt64";
				case "float":
					return "ToSingle";
				case "double":
					return "ToDouble";
				default:
					return "";
			}
		}

		public static string FirstCharToUpper(string input)
		{
			if (string.IsNullOrEmpty(input))
				return "";
			return input[0].ToString().ToUpper() + input.Substring(1);
		}

		public static string FirstCharToLower(string input)
		{
			if (string.IsNullOrEmpty(input))
				return "";
			return input[0].ToString().ToLower() + input.Substring(1);
		}
	}
}
