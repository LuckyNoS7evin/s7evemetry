﻿using S7evemetry.Core;
using S7evemetry.Core.Packets;
using S7evemetry.F1_2020.Structures;
using System;

namespace S7evemetry.F1_2020.Observers
{
    public abstract class CarSetupDataObserver : IObserver<PacketData<PacketHeader, CarSetupData, CarSetup>>
    {
        private IDisposable? _unsubscriber;

        public void Subscribe(IObservable<PacketData<PacketHeader, CarSetupData, CarSetup>> listener)
        {
            _unsubscriber = listener.Subscribe(this);
        }

        public void Unsubscribe()
        {
            if (_unsubscriber != null)
            {
                _unsubscriber.Dispose();
                _unsubscriber = null;
            }
        }

        public abstract void OnCompleted();

        public abstract void OnError(DataException error);

        public abstract void OnError(Exception error);

        public abstract void OnNext(PacketData<PacketHeader, CarSetupData, CarSetup> value);
    }
}
