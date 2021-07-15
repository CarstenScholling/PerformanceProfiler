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
    /// This class should be used to aggregate event from the multiple session.
    /// </summary>
    internal class ProfilerEventEtlFileProcessor : IDisposable
    {
        /// <summary>
        /// Has the object been disposed.
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// The associated event processor.
        /// </summary>
        private readonly EtwEventFileProcessor etwEventFileProcessor;

        /// <summary>
        /// The associated event aggregator.
        /// </summary>
        private readonly MultipleSessionsEventAggregator multipleSessionsEventAggregator;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="etlFilePath"></param>
        /// <param name="threshold">The filter value in milliseconds. Values greater then this will only be shown.</param>
        public ProfilerEventEtlFileProcessor(string etlFilePath, int threshold)
        {
            multipleSessionsEventAggregator = new MultipleSessionsEventAggregator(threshold);

            etwEventFileProcessor = new EtwEventFileProcessor(etlFilePath, multipleSessionsEventAggregator.AddEtwEventToAggregatedCallTree);
        }

        /// <summary>
        /// Analyzes events from the ETL file and aggregates events from the multiple sessions.
        /// </summary>
        internal void ProcessEtlFile()
        {
            etwEventFileProcessor.ProcessEtlFile();

            multipleSessionsEventAggregator.FinishAggregation();
        }

        /// <summary>
        /// Traverses the call stack tree.
        /// </summary>
        /// <returns>Flatten call tree.</returns>
        internal IEnumerable<AggregatedEventNode> FlattenCallTree()
        {
            return multipleSessionsEventAggregator.FlattenCallTree();
        }

        /// <summary>
        /// Calculates maximum relative time stamp.
        /// </summary>
        /// <returns>Maximum relative time stamp.</returns>
        public double MaxRelativeTimeStamp()
        {
            return multipleSessionsEventAggregator.MaxRelativeTimeStamp();
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
                if (isDisposed)
                {
                    return;
                }

                isDisposed = true;
            }
        }
    }
}
