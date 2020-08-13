﻿using System;

namespace S7evemetry.Udp
{
	public abstract class UdpStreamObserver : IObserver<byte[]>
	{
		private IDisposable? _unsubscriber;

		public void Subscribe(IObservable<byte[]> listener)
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

		public abstract void OnError(Exception error);

		public abstract void OnNext(byte[] value);
	}
}
