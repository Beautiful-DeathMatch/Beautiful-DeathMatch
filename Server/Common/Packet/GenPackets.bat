START ../../PacketGenerator/bin/PacketGenerator.exe ../../PacketGenerator/INGAME_PDL.xml
XCOPY /Y InGameGenPackets.cs "../../../Client/Assets/Scripts/Network/Packet"
XCOPY /Y InGameGenPackets.cs "../../InGameServer/Packet"
XCOPY /Y InGameClientPacketManager.cs "../../../Client/Assets/Scripts/Network/Packet"
XCOPY /Y InGameServerPacketManager.cs "../../InGameServer/Packet"

START ../../PacketGenerator/bin/PacketGenerator.exe ../../PacketGenerator/ROOM_PDL.xml
XCOPY /Y RoomGenPackets.cs "../../../Client/Assets/Scripts/Network/Packet"
XCOPY /Y RoomGenPackets.cs "../../RoomServer/Packet"
XCOPY /Y RoomClientPacketManager.cs "../../../Client/Assets/Scripts/Network/Packet"
XCOPY /Y RoomServerPacketManager.cs "../../RoomServer/Packet"