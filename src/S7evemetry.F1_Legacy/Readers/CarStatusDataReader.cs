﻿using S7evemetry.Core.Enums.F1;
using S7evemetry.Core.Packets.F1;
using S7evemetry.F1_Legacy.Structures;
using System;

namespace S7evemetry.F1_Legacy.Readers
{
    public class CarStatusDataReader
    {
        public PacketData<PacketHeader, CarStatusData, CarStatus>? Read(Span<byte> input, PacketHeader packetHeader)
        {
            if (packetHeader == null)
            {
                throw new ArgumentNullException(nameof(packetHeader), $"{nameof(packetHeader)} is null.");
            }

            if (packetHeader.PacketId != PacketType.CarStatus) return null;

            if (input.Length != (packetHeader.GridSize * CarStatus.Size) + CarStatusData.Size)
            {
                return null;
            }

            var packet = new PacketData<PacketHeader, CarStatusData, CarStatus>()
            {
                Header = packetHeader
            };

            for (var i = 0; i < packet.Header.GridSize; i++)
            {
                packet.CarData.Add(CarStatus.Read(input.Slice((i * CarStatus.Size), CarStatus.Size)));
            }

            packet.Data = new CarStatusData();

            return packet;
        }
    }
}
