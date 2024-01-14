START ../../PacketGenerator/bin/PacketGenerator.exe ../../PacketGenerator/INGAME_PDL.xml ../../PacketGenerator/ROOM_PDL.xml

XCOPY /Y InGameGeneratedPacket.cs "../../../Client/Assets/Scripts/Network/Packet"
XCOPY /Y InGameGeneratedPacket.cs "../../InGameServer/Packet"

XCOPY /Y RoomGeneratedPacket.cs "../../../Client/Assets/Scripts/Network/Packet"
XCOPY /Y RoomGeneratedPacket.cs "../../RoomServer/Packet"

XCOPY /Y InGameClientPacketManager.cs "../../../Client/Assets/Scripts/Network/Packet"
XCOPY /Y InGameServerPacketManager.cs "../../InGameServer/Packet"

XCOPY /Y RoomClientPacketManager.cs "../../../Client/Assets/Scripts/Network/Packet"
XCOPY /Y RoomServerPacketManager.cs "../../RoomServer/Packet"