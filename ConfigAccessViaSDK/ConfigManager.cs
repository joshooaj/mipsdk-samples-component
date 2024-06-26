﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using VideoOS.Platform;
using VideoOS.Platform.Messaging;

namespace ConfigAccessViaSDK
{
    /// <summary>
    /// This class helps getting the configuration updated for a single site, 
    /// it is not intended to be used in an MFA setup.
    /// </summary>
    public class ConfigManager
    {
        private MessageCommunication _messageCommunication;
        private object _systemConfigurationChangedIndicationReference;
        private Timer _catchUpTimer;

        public void Init()
        {
            _catchUpTimer = new Timer(CatchUpTimerHandler);

            try
            {
                MessageCommunicationManager.Start(EnvironmentManager.Instance.MasterSite.ServerId);
                _messageCommunication = MessageCommunicationManager.Get(EnvironmentManager.Instance.MasterSite.ServerId);

                _systemConfigurationChangedIndicationReference = _messageCommunication.RegisterCommunicationFilter(SystemConfigChangedHandler,
                    new VideoOS.Platform.Messaging.CommunicationIdFilter(MessageId.System.SystemConfigurationChangedIndication));

            }
            catch (MIPException ex)
            {
                Trace.WriteLine("Message Communcation not supported:" + ex.Message);
            }
        }

        public void Close()
        {
            _messageCommunication.UnRegisterCommunicationFilter(_systemConfigurationChangedIndicationReference);
            MessageCommunicationManager.Stop(EnvironmentManager.Instance.MasterSite.ServerId);
        }

        /// <summary>
        /// This method is called, when the EventServer did not report the detailed configuration message.
        /// Can happen for older systems, or because the EventServer is very busy.
        /// 
        /// As a fail-safe we reload the entire configuration after 60 seconds
        /// </summary>
        /// <param name="o"></param>
        private void CatchUpTimerHandler(object o)
        {
            // Disable timer
            _catchUpTimer.Change(Timeout.Infinite, Timeout.Infinite);
            // Reload everything
            VideoOS.Platform.SDK.Environment.ReloadConfiguration(Configuration.Instance.ServerFQID);
        }

        /// <summary>
        /// Message comming from EventServer with information about what is changed
        /// </summary>
        /// <param name="message"></param>
        /// <param name="dest"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        private object SystemConfigChangedHandler(VideoOS.Platform.Messaging.Message message, FQID dest, FQID source)
        {
            List<FQID> fqids = message.Data as List<FQID>;
            if (fqids == null)
            {
                // Start timer, when the detailed info is not available.  
                _catchUpTimer.Change(TimeSpan.FromSeconds(90), TimeSpan.FromSeconds(90));
                return null;
            }

            // Detailed info received - stop catchup timer
            _catchUpTimer.Change(Timeout.Infinite, Timeout.Infinite);     // Disable timer, as we now have the detailed changes

            HashSet<FQID> serverFQIDList = new HashSet<FQID>();
            foreach (FQID fqid in fqids)
            {
                Item item = Configuration.Instance.GetItem(fqid);
                if (item != null)
                {
                    Trace.WriteLine("SystemConfigurationChangedIndication - received -- for: " + item.Name);
                    FQID recorderFQID;
                    if (fqid.Kind == Kind.Server)
                        recorderFQID = fqid;
                    else
                        recorderFQID = fqid.GetParent();
                    if (recorderFQID != null)
                        serverFQIDList.Add(recorderFQID);
                }
                else
                {
                    Trace.WriteLine("SystemConfigurationChangedIndication - received -- for: Unknown Item");
                    // unknown item - we will reload entire configuration
                    serverFQIDList.Clear();
                    serverFQIDList.Add(Configuration.Instance.ServerFQID);
                    break;
                }
            }

            Thread reloadThread = new Thread(new ParameterizedThreadStart(ReloadConfigurationThread));
            reloadThread.Start(serverFQIDList);
            
            return null;
        }

        /// <summary>
        /// Make sure we reload configuration from another thread
        /// </summary>
        /// <param name="obj"></param>
        private void ReloadConfigurationThread(object obj)
        {
            HashSet<FQID> serverFQIDList = obj as HashSet<FQID>;
            if (serverFQIDList != null)
            {
                // Now ask SDK to reload configuration from server, this will issue the "LocalConfigurationChangedIndication"
                foreach (FQID serverFQID in serverFQIDList)
                    VideoOS.Platform.SDK.Environment.ReloadConfiguration(serverFQID);                
            }

        }
    }
}
