﻿using S7evemetry.Core.Packets.F1;
using System;

namespace S7evemetry.Tests.Core.Packets.F1.MockPackets
{
    public class CarTelemetryData : CarTelemetryDataCommon
    {
        public static CarTelemetryData? Read(Span<byte> input)
        {
            return Read<CarTelemetryData>(input);
        }
    }
}
