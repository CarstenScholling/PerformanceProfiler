//--------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//--------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Session;
using System.Configuration;
using System.Reflection;

namespace ETWPerformanceProfiler
{
    /// <summary>
    /// Use this class to dynamically listen to the ETW events.
    /// </summary>
    internal class EtwEventDynamicProcessor : IDisposable
    {
        /// <summary>
        /// The name for local trace event session.
        /// </summary>
		private string TraceEventSessionName
		{
			get
			{
				string sessionName = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location).AppSettings.Settings["SessionName"].Value;

				return sessionName;
			}
		}

		/// <summary>
		/// The name of the provider.
		/// </summary>
		private readonly string providerName;

        /// <summary>
        /// Provider GUID generated from the provider name.
        /// </summary>
        private readonly Guid providerGuid;

        /// <summary>
        /// The callback which is called every time new event appears.
        /// </summary>
        private readonly Action<TraceEvent> traceEventHandler;

        /// <summary>
        /// Represents the stream of events that is collected from a TraceEventSession
        /// </summary>
        private ETWTraceEventSource traceEventSource;

        /// <summary>
        /// Represents a single ETW Tracing Session.
        /// </summary>
        private TraceEventSession traceEventSession;

        /// <summary>
        /// Task which processes all ETW events.
        /// Its execution finishes after <see cref="traceEventSource"/>.Close() is called.  
        /// </summary>
        private Task eventProcessingTask;

        /// <summary>
        /// <c>true</c> if event processing should be stopped.
        /// </summary>
        private bool stopProcessing = false;

        /// <summary>
        /// Has the object been disposed.
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="EtwEventDynamicProcessor"/> class.
        /// </summary>
        /// <param name="providerName">The name of the provider which events we are going to consume.</param>
        /// <param name="traceEventHandler">The callback which is called every time new event appears.</param>
        internal EtwEventDynamicProcessor(string providerName, Action<TraceEvent> traceEventHandler)
        {
            this.providerName = providerName;
            providerGuid = TraceEventProviders.GetEventSourceGuidFromName(providerName);
            this.traceEventHandler = traceEventHandler;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Start listening and processing events.
        /// </summary>
        internal void StartProcessing()
        {
            StopProcessing();

            stopProcessing = false;

            // Create new trace session.
            traceEventSession = new TraceEventSession(TraceEventSessionName, null);
            traceEventSession.StopOnDispose = true;

            // Open a ETW event source for processing. Provide the name of real time sessing to open.
            traceEventSource = new ETWTraceEventSource(TraceEventSessionName, TraceEventSourceType.Session);

            DynamicTraceEventParser parser = new DynamicTraceEventParser(traceEventSource);
            parser.All += (traceEvent) =>
            {
                if (stopProcessing)
                {
                    traceEventSource.StopProcessing();
                }

                traceEventHandler(traceEvent);
            };

            // Add an additional provider represented by providerGuid
            traceEventSession.EnableProvider(providerGuid);

            // Enqueue on the thread pool's global queue. 
            // Processing never completes by itself, but only is Close() method is called.  
            eventProcessingTask = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        traceEventSource.Process();
                    }
                    catch (NullReferenceException)
                    {
                        // There is a bug in ETWTraceEventSource it can throw NullReferenceException exception!
                    }
                });
        }
        
        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">True if managed resources should be disposed; otherwise, false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (isDisposed)
                {
                    return;
                }

                StopProcessing();

                isDisposed = true;
            }
        }

        /// <summary>
        /// Stop listening and processing events and dispose all the ETW session related objects.
        /// </summary>
        internal void StopProcessing()
        {
            try
            {
                if (traceEventSource != null)
                {
                    stopProcessing = true;
                    traceEventSource.StopProcessing();

                    eventProcessingTask.Wait();

                    traceEventSource.Dispose();
                    traceEventSource = null;
                }
            }
            finally
            {
                if (traceEventSession != null)
                {
                    traceEventSession.Dispose();
                    traceEventSource = null;
                }

                eventProcessingTask = null;
            }
        }
    }
}
