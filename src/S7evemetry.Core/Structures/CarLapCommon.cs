﻿
using S7evemetry.Core.Enums.F1;
using System;

namespace S7evemetry.Core.Structures
{
	/// <summary>
	/// CarLap data for each Car
	/// </summary>
	public abstract class CarLapCommon
	{
		/// <summary>
		/// Last lap time in seconds
		/// </summary>
		public float LastLapTime { get; set; }

		/// <summary>
		/// Current time around the lap in seconds
		/// </summary>
		public float CurrentLapTime { get; set; }

		/// <summary>
		/// Distance vehicle is around current lap in metres – could be negative if line hasn’t been crossed yet
		/// </summary>
		public float LapDistance { get; set; }

		/// <summary>
		/// Total distance travelled in session in metres – could be negative if line hasn’t been crossed yet
		/// </summary>
		public float TotalDistance { get; set; }

		/// <summary>
		/// Delta in seconds for safety car
		/// </summary>
		public float SafetyCarDelta { get; set; }

		/// <summary>
		/// Car race position
		/// </summary>
		public byte CarPosition { get; set; }

		/// <summary>
		/// Current lap number
		/// </summary>
		public byte CurrentLapNum { get; set; }

		/// <summary>
		/// The current Pit Status - converted to enum from 0 = none, 1 = pitting, 2 = in pit area
		/// </summary>
		public PitStatus PitStatus { get; set; }

		/// <summary>
		/// The current sector - 0 = sector1, 1 = sector2, 2 = sector3
		/// </summary>
		public Sector Sector { get; set; }

		/// <summary>
		/// Current lap invalid - 0 = valid, 1 = invalid
		/// </summary>
		public bool CurrentLapInvalid { get; set; }

		/// <summary>
		/// Accumulated time penalties in seconds to be added
		/// </summary>
		public byte Penalties { get; set; }

		/// <summary>
		/// Grid position the vehicle started the race in
		/// </summary>
		public byte GridPosition { get; set; }

		/// <summary>
		/// Status of driver - 0 = in garage, 1 = flying lap, 
		/// 2 = in lap, 3 = out lap, 4 = on track
		/// </summary>
		public DriverStatus DriverStatus { get; set; }

		/// <summary>
		/// Result status - 0 = invalid, 1 = inactive, 
		/// 2 = active,3 = finished, 4 = disqualified, 
		/// 5 = not classified, 6 = retired
		/// </summary>
		public ResultStatus ResultStatus { get; set; }

		/// <summary>
		/// Reads the common data for CarLap.
		/// </summary>
		/// <typeparam name="T">An inherited Type of CarLapCommon</typeparam>
		/// <param name="input">
		/// The Span of byte which contain the common CarLap data packet
		/// </param>
		/// <param name="gapSize">There is a gap where extra data should be placed, instead of 2 spans we supply a gap</param>
		/// <returns>Instance of T with deserialized data from input</returns>
		protected static T Read<T>(Span<byte> input, int gapSize) where T : CarLapCommon, new()
		{
            var lap = new T
            {
                LastLapTime = BitConverter.ToSingle(input.Slice(0, 4)), // 4 byte
                CurrentLapTime = BitConverter.ToSingle(input.Slice(4, 4)), // 4 byte

                LapDistance = BitConverter.ToSingle(input.Slice(gapSize, 4)), // 4 byte
                TotalDistance = BitConverter.ToSingle(input.Slice(gapSize + 4, 4)), // 4 byte
                SafetyCarDelta = BitConverter.ToSingle(input.Slice(gapSize + 8, 4)), // 4 byte
                CarPosition = input[gapSize + 12],
                CurrentLapNum = input[gapSize + 13],
                PitStatus = (PitStatus)input[gapSize + 14],
                Sector = (Sector)input[gapSize + 15],
                CurrentLapInvalid = Convert.ToBoolean(input[gapSize + 16]),
                Penalties = input[gapSize + 17],
                GridPosition = input[gapSize + 18],
                DriverStatus = (DriverStatus)input[gapSize + 19],
                ResultStatus = (ResultStatus)input[gapSize + 20]
            };

            return lap;
		}
	}
}
