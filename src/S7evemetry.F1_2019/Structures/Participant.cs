﻿using S7evemetry.Core.Enums.F1;
using S7evemetry.Core.Structures;
using S7evemetry.F1_2019.Helpers;
using System;

namespace S7evemetry.F1_2019.Structures
{
    public class Participant : ParticipantCommon
    {
        public static new int Size { get; } = 54;
               
        public Telemetry YourTelemetry { get; set; }

        public static Participant Read(Span<byte> input)
        {
            var item = Read<Participant>(input);
            item.Driver = DriverHelper.GetDriverById(input[1]);
            item.Team = TeamHelper.GetTeamById(input[2]);
            item.Nationality = NationalityHelper.GetNationalityById(input[4]);
            item.YourTelemetry = (Telemetry)input[53];
            return item;
        }
    }
}
