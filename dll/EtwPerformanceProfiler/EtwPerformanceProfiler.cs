//--------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//--------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace EtwPerformanceProfiler
{
    /// <summary>
    /// Represents the performance profiler class to be used in AL.
    /// </summary>
    public class EtwPerformanceProfiler : IDisposable
    {
        /// <summary>
        /// The associated event processor.
        /// </summary>
        private DynamicProfilerEventProcessor dynamicProfilerEventProcessor;

        /// <summary>
        /// The call tree of all the aggregated method and SQL statement calls parsed from the ETW events
        /// </summary>
        private IEnumerator<AggregatedEventNode> callTree;

        /// <summary>
        /// Max relative time stamp for the currect profiling session.
        /// </summary>
        private double maxRelativeTimeStamp;

        public int CurrentSessionId
        {
            get 
            {
                return callTree.Current.SessionId;
            }
        }

        public string CurrentTenant
        {
            get 
            {
                return callTree.Current.Tenant;
            }
        }

		public string CurrentUserName
		{
			get
			{
				return callTree.Current.UserName;
			}
		}
		public Guid CurrentAppId
		{
			get
			{
				return callTree.Current.AppId;
			}
		}
		public string CurrentAppInfo
		{
			get
			{
				return callTree.Current.AppInfo;
			}
		}

		/// <summary>
		/// Gets the call tree's current statement's owning object id.
		/// </summary>
		public int CurrentOwningObjectId
        {
            get
            {
                return callTree.Current.ObjectId;
            }
        }

        /// <summary>
        /// Gets the call tree's current statement.
        /// </summary>
        public string CurrentStatement
        {
            get
            {
                return callTree.Current.StatementName;
            }
        }

        /// <summary>
        /// Gets the current line number on the call tree.
        /// </summary>
        public int CurrentLineNumber
        {
            get
            {
                return callTree.Current.LineNo;
            }
        }

        /// <summary>
        /// Gets call tree's current statements duration in miliseconds
        /// </summary>
        public long CurrentDurationMs
        {
            get
            {
                return (long)callTree.Current.DurationMSec;
            }
        }

        /// <summary>
        /// Gets call tree's current statements min duration in miliseconds
        /// </summary>
        public long CurrentMinDurationMs
        {
            get
            {
                return (long)callTree.Current.MinDurationMSec;
            }
        }

        /// <summary>
        /// Gets call tree's current statements max duration in miliseconds
        /// </summary>
        public long CurrentMaxDurationMs
        {
            get
            {
                return (long)callTree.Current.MaxDurationMSec;
            }
        }

        /// <summary>
        /// Gets call tree's current statements duration from the end of profiling.
        /// </summary>
        public long CurrentLastActiveMs
        {
            get
            {
                return (long)(maxRelativeTimeStamp - (long)callTree.Current.MaxRelativeTimeStampMSec);
            }
        }

        /// <summary>
        /// Gets the call tree' current current statement's depth.
        /// </summary>
        public int CurrentIndentation
        {
            get
            {
                return callTree.Current.Depth;
            }
        }

		/// <summary>
		/// Gets the current object type on the call tree. 
		/// </summary>
		public string CurrentOwningObjectTypeName
		{
			get
			{
				return callTree.Current.ObjectType;
			}
		}

        /// <summary>
        /// Gets the call tree' current current statement's hit count.
        /// </summary>
        public int CurrentHitCount
        {
            get
            {
                return callTree.Current.HitCount;
            }
        }

        /// <summary>
        /// Starts ETW profiling.
        /// </summary>
        /// <param name="sessionId">The session unique identifier.</param>
        /// <param name="threshold">The filter value in milliseconds. Values greater then this will only be shown.</param>
        public void Start(int sessionId, int threshold = 0)
        {
            dynamicProfilerEventProcessor = new DynamicProfilerEventProcessor(sessionId, threshold);

            dynamicProfilerEventProcessor.Start();
        }

        /// <summary>
        /// Stops profiling and aggregates the events
        /// </summary>
        public void Stop()
        {
            dynamicProfilerEventProcessor.Stop();

            callTree = dynamicProfilerEventProcessor.FlattenCallTree().GetEnumerator();

            maxRelativeTimeStamp = dynamicProfilerEventProcessor.MaxRelativeTimeStamp();
        }

        /// <summary>
        /// Analyzes events from the ETL file and aggregates events from the multiple sessions.
        /// </summary>
        /// <param name="etlFilePath">ETL file to be analyzed.</param>
        /// <param name="threshold">The filter value in milliseconds. Values greater then this will only be shown.</param>
        public void AnalyzeETLFile(string etlFilePath, int threshold = 0)
        {
            if (dynamicProfilerEventProcessor != null)
            {
                dynamicProfilerEventProcessor.Dispose();
                dynamicProfilerEventProcessor = null;
            }

            using (ProfilerEventEtlFileProcessor profilerEventEtlFileProcessor = new ProfilerEventEtlFileProcessor(etlFilePath, threshold))
            {
                profilerEventEtlFileProcessor.ProcessEtlFile();

                callTree = profilerEventEtlFileProcessor.FlattenCallTree().GetEnumerator();

                maxRelativeTimeStamp = profilerEventEtlFileProcessor.MaxRelativeTimeStamp();
            }
        }

        /// <summary>
        /// Calls the tree move next.
        /// </summary>
        /// <returns></returns>
        public bool CallTreeMoveNext()
        {
            return callTree.MoveNext();
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
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">True if managed resources should be disposed; otherwise, false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dynamicProfilerEventProcessor != null)
                {
                    dynamicProfilerEventProcessor.Dispose();
                }

                callTree = null;
            }
        }
    }
}
