//--------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//--------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.Diagnostics.Tracing;

namespace EtwPerformanceProfiler
{
    /// <summary>
    /// This class is responsible for aggregating events and building the call tree.
    /// It processes event from different sessions.
    /// </summary>
    internal class MultipleSessionsEventAggregator : EventAggregator, IEventAggregator
    {
        private Dictionary<int, SingleSessionEventAggregator> sessionAggregators;

        /// <summary>
        /// The threshold value. The aggregated call three will be filtered on values greater than the threshold.
        /// </summary>
        private readonly long threshold;

        /// <summary>
        /// <c>true</c> if event processing is suspended.
        /// </summary>
        private bool suspended = false;

        /// <summary>
        /// Creates a new instance of the <see cref="MultipleSessionsEventAggregator"/> class.
        /// </summary>
        /// <param name="threshold">The threshold value. The aggregated call tree will only show events greater than this.</param>
        internal MultipleSessionsEventAggregator(long threshold)
        {
            this.threshold = threshold;

            Initialize();
        }

        /// <summary>
        /// The callback which is called every time new event appears.
        /// </summary>
        /// <param name="traceEvent">The trace event.</param>
        public void AddEtwEventToAggregatedCallTree(TraceEvent traceEvent)
        {
            if (suspended)
            {
                return;
            }

            if (!TraceEventToCollect(traceEvent))
            {
                return;
            }

            int sessionId = GetSessionId(traceEvent);

            SingleSessionEventAggregator sessionAggregator;
            if (!sessionAggregators.TryGetValue(sessionId, out sessionAggregator))
            {
                sessionAggregator = new SingleSessionEventAggregator(sessionId, threshold);
                sessionAggregator.Initialize();
                sessionAggregators[sessionId] = sessionAggregator;
            }

            sessionAggregator.AddEtwEventToAggregatedCallTree(traceEvent);
        }

        /// <summary>
        /// Calculates maximum relative time stamp.
        /// </summary>
        /// <returns>Maximum relative time stamp.</returns>
        public double MaxRelativeTimeStamp()
        {
            return sessionAggregators.Max(a => a.Value.MaxRelativeTimeStamp());
        }

        /// <summary>
        /// Initializes state of the <see cref="MultipleSessionsEventAggregator"/>
        /// </summary>
        public void Initialize()
        {
            sessionAggregators = new Dictionary<int, SingleSessionEventAggregator>();
        }

        /// <summary>
        /// Finishes aggregation.
        /// </summary>
        /// <param name="buildAggregatedCallTree">true if the aggregated call is to be built.</param>
        public void FinishAggregation(bool buildAggregatedCallTree = true)
        {
            foreach (var singleSessionEventAggregator in sessionAggregators)
            {
                singleSessionEventAggregator.Value.FinishAggregation(buildAggregatedCallTree);
            }
        }

        /// <summary>
        /// Traverses the call stack tree.
        /// </summary>
        /// <returns>Flatten call tree.</returns>
        public IEnumerable<AggregatedEventNode> FlattenCallTree()
        {
            return sessionAggregators.SelectMany(singleSessionEventAggregator => singleSessionEventAggregator.Value.FlattenCallTree());
        }

        /// <summary>
        /// Suspend event processing.
        /// </summary>
        public void Suspend()
        {
            suspended = true;
        }

        /// <summary>
        /// Resume event processing.
        /// </summary>
        public void Resume()
        {
            suspended = false;
        }
    }
}
