﻿using irsdkSharp.Serialization.Models.Data;
using irsdkSharp.Serialization.Models.Session;
using Microsoft.Extensions.Logging;
using S7evemetry.Core;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace S7evemetry.iRacing.Listeners
{
    public class IRacingListener :
        IObservable<IRacingDataModel>,
        IObservable<IRacingSessionModel>

    {
        private readonly ILogger<IRacingListener> _logger;
        private readonly irsdkSharp.Serialization.IRacingSDK _sdk;
        private readonly ICollection<IObserver<IRacingDataModel>> _dataObservers;
        private readonly ICollection<IObserver<IRacingSessionModel>> _sessionObservers;

        private bool _hasConnected;
        private int _waitTime;
        private bool _IsConnected = false;
        private readonly int _connectSleepTime = 1000;

        public IRacingListener(ILogger<IRacingListener> logger)
        {
            _logger = logger;
            _sdk = new irsdkSharp.Serialization.IRacingSDK();
            _dataObservers = new List<IObserver<IRacingDataModel>>();
            _sessionObservers = new List<IObserver<IRacingSessionModel>>();

            Task.Run(() => Loop());
        }

        public IDisposable Subscribe(IObserver<IRacingDataModel> observer)
        {
            if (!_dataObservers.Contains(observer))
            {
                _dataObservers.Add(observer);
            }
            return new Unsubscriber<IRacingDataModel>(_dataObservers, observer);
        }

        public IDisposable Subscribe(IObserver<IRacingSessionModel> observer)
        {
            if (!_sessionObservers.Contains(observer))
            {
                _sessionObservers.Add(observer);
            }
            return new Unsubscriber<IRacingSessionModel>(_sessionObservers, observer);
        }

        protected void NotifyData(IRacingSessionModel data)
        {
            foreach (var observer in _sessionObservers)
            {
                observer.OnNext(data);
            }
        }
        protected void NotifyData(IRacingDataModel data)
        {
            foreach (var observer in _dataObservers)
            {
                observer.OnNext(data);
            }
        }

        private void Loop()
        {
            int lastUpdate = -1;

            while (true)
            {
                // Check if we can find the sim
                if (_sdk.IsConnected())
                {
                    if (!_IsConnected)
                    {
                        _logger.LogInformation("Now Connected");
                        // If this is the first time, raise the Connected event
                        //this.RaiseEvent(OnConnected, EventArgs.Empty);
                    }

                    _hasConnected = true;
                    _IsConnected = true;

                    int attempts = 0;
                    const int maxAttempts = 99;

                    var sessionnum = TryGetSessionNum();
                    while (sessionnum == -1 && attempts <= maxAttempts)
                    {
                        attempts++;
                        sessionnum = TryGetSessionNum();
                    }
                    if (attempts >= maxAttempts)
                    {
                        _logger.LogWarning("Too many attempts to connect");
                        continue;
                    }

                    NotifyData(_sdk.GetData());

                    // Is the session info updated?
                    int newUpdate = _sdk.Header.SessionInfoUpdate;
                    if (newUpdate != lastUpdate)
                    {
                        lastUpdate = newUpdate;

                        // Get the session info string
                        NotifyData(_sdk.GetSessionInformation()); 
                    }

                }
                else if (_hasConnected)
                {
                    _sdk.Shutdown();
                    lastUpdate = -1;
                    _IsConnected = false;
                    _hasConnected = false;
                }
                else
                {
                    _IsConnected = false;
                    _hasConnected = false;

                    //Try to find the sim
                    _sdk.Startup();
                }

                // Sleep for a short amount of time until the next update is available
                if (_IsConnected)
                {
                    if (_waitTime <= 0 || _waitTime > 1000) _waitTime = 15;
                    Thread.Sleep(_waitTime);
                }
                else
                {
                    // Not connected yet, no need to check every 16 ms, let's try again in some time
                    Thread.Sleep(_connectSleepTime);
                }
            }

        }

        private int TryGetSessionNum()
        {
            try
            {
                var sessionnum = _sdk.GetData("SessionNum");
                return int.Parse(sessionnum.ToString());
            }
            catch
            {
                return -1;
            }
        }
    }
}