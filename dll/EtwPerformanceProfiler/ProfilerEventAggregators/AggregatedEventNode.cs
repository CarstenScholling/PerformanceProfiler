//--------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//--------------------------------------------------------------------------

using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System;

namespace EtwPerformanceProfiler
{
    /// <summary>
    /// Represents the aggregated call tree.
    /// </summary>
    public class AggregatedEventNode
    {
        /// <summary>
        /// Gets or sets the Session id.
        /// </summary>
        public int SessionId { get; set; }

		/// <summary>
		/// Gets or sets the user name.
		/// </summary>
		internal string UserName { get; set; }

		/// <summary>
		/// Gets or sets the tenant id this event originates from.
		/// </summary>
		internal string Tenant { get; set; }

		/// <summary>
		/// Gets or sets the app id this event originates from.
		/// </summary>
		internal Guid AppId { get; set; }

		/// <summary>
		/// Gets or sets the app info this event originates from.
		/// </summary>
		internal string AppInfo { get; set; }

		/// <summary>
		/// Gets or sets the type of the object.
		/// </summary>
		public string ObjectType { get; private set; }
        /// <summary>
        /// Gets or sets the object id.
        /// </summary>

        public int ObjectId { get; private set; }

        /// <summary>
        /// Gets or sets the line no.
        /// </summary>
        public int LineNo { get; private set; }

        /// <summary>
        /// Gets or sets the name of the Statement.
        /// </summary>
        public string StatementName { get; set; }

        /// <summary>
        /// Gets or sets the duration in MS.
        /// </summary>
        public double DurationMSec { get; set; }

        /// <summary>
        /// Gets or sets the min duration in MS.
        /// </summary>
        public double MinDurationMSec { get; set; }

        /// <summary>
        /// Gets or sets the max duration in MS.
        /// </summary>
        public double MaxDurationMSec { get; set; }

        /// <summary>
        /// Gets the children of the current node.
        /// </summary>
        public List<AggregatedEventNode> Children { get; private set; }

        /// <summary>
        /// Gets or sets the number times we executed this statement.
        /// </summary>
        public int HitCount { get; private set; }

        /// <summary>
        /// Gets or sets the time stamp in MS.
        /// </summary>
        public double TimeStampRelativeMSec { get; private set; }

        /// <summary>
        /// Gets or sets the min time stamp in MS.
        /// </summary>
        public double MinRelativeTimeStampMSec { get; private set; }

        /// <summary>
        /// Gets or sets the max time stamp in MS.
        /// </summary>
        public double MaxRelativeTimeStampMSec { get; private set; }

        /// <summary>
        /// Gets or sets the parent node.
        /// </summary>
        public AggregatedEventNode Parent { get; private set; }

        /// <summary>
        /// Gets or sets the original type of the node.
        /// </summary>
        internal EventType OriginalType { get; private set; }

        /// <summary>
        /// Gets or sets the evaluated type. The statement type can change to a start type 
        /// during aggregation if the statement is a function call.
        /// </summary>
        internal EventType EvaluatedType { get; set; }

        /// <summary>
        /// Gets or sets the sub type of the node.
        /// </summary>
        internal EventSubType SubType { get; set; }

        /// <summary>
        /// Depth of the current element in the tree.
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// Returns true if this is the none AL event.
        /// </summary>
        internal bool IsNoneAlEvent
        {
            get { return !IsAlEvent; }
        }

        /// <summary>
        /// Returns true if this is the AL event.
        /// </summary>
        internal bool IsAlEvent
        {
            get
            {
                Debug.Assert((SubType == EventSubType.AlEvent) == (ObjectId != 0));
                return SubType == EventSubType.AlEvent;
            }
        }
             
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregatedEventNode"/> class.
        /// </summary>
        /// <param name="parent">Parent <see cref="AggregatedEventNode"/>.</param>
        internal AggregatedEventNode(AggregatedEventNode parent = null)
        {
            Children = new List<AggregatedEventNode>();
            Parent = parent;
            Depth = parent != null ? parent.Depth + 1 : 0;
        }

        /// <summary>
        /// Pushes event into call stack. It might get or create aggregated event.
        /// </summary>
        /// <param name="profilerEvent">The profiler event.</param>
        /// <returns>The aggregated event node.</returns>
        internal AggregatedEventNode PushEventIntoCallStack(ProfilerEvent profilerEvent)
        {
            Debug.Assert(profilerEvent.Type == EventType.Statement || profilerEvent.Type == EventType.StartMethod);

            AggregatedEventNode res = Children.Find(e =>
                e.SessionId == profilerEvent.SessionId &&
                e.ObjectType == profilerEvent.ObjectType &&
                e.ObjectId == profilerEvent.ObjectId &&
                e.LineNo == profilerEvent.LineNumber &&
                e.StatementName == profilerEvent.StatementName);

            if (res != null)
            {
                // We need to initialize state of the AggregatedEventNode.
                // Otherwise duration will not be calculated correctly or we can get broken tree structure.
                res.EvaluatedType = profilerEvent.Type;
                res.TimeStampRelativeMSec = profilerEvent.TimeStampRelativeMSec;

                ++res.HitCount;
                return res;
            }

            res = new AggregatedEventNode(this)
                {
                    SessionId = profilerEvent.SessionId,
					Tenant = profilerEvent.Tenant,
					UserName = profilerEvent.UserName,
					AppId = profilerEvent.AppId,
					AppInfo = profilerEvent.AppInfo,
                    ObjectType = profilerEvent.ObjectType,
                    ObjectId = profilerEvent.ObjectId,
                    LineNo = profilerEvent.LineNumber,
                    StatementName = profilerEvent.StatementName,
                    TimeStampRelativeMSec = profilerEvent.TimeStampRelativeMSec,
                    OriginalType = profilerEvent.Type,
                    EvaluatedType = profilerEvent.Type,
                    SubType = profilerEvent.SubType,
                };

            Children.Add(res);

            ++res.HitCount;
            return res;
        }

        /// <summary>
        /// Pops event from the call stack.
        /// </summary>
        /// <param name="endTimeStampRelativeMSec">End time stamp for the aggregated event node on the top of call stack.</param>
        /// <returns>The aggregated event node.</returns>
        internal AggregatedEventNode PopEventFromCallStackAndCalculateDuration(double endTimeStampRelativeMSec)
        {
            double lastDuration = endTimeStampRelativeMSec - TimeStampRelativeMSec;

            if (MinDurationMSec <= 0 || MinDurationMSec > lastDuration)
            {
                MinDurationMSec = lastDuration;
            }

            if (MaxDurationMSec < lastDuration)
            {
                MaxDurationMSec = lastDuration;
            }

            DurationMSec += lastDuration;

            TimeStampRelativeMSec = endTimeStampRelativeMSec;

            return Parent;
        }

        /// <summary>
        /// Calculates min max time stamp.
        /// </summary>
        internal void CalcMinMaxRelativeTimeStampMSec()
        {
            foreach (AggregatedEventNode child in Children)
            {
                child.CalcMinMaxRelativeTimeStampMSec();
            }

            if (Children.Count > 0)
            {
                MinRelativeTimeStampMSec = Children.Min(n => n.MinRelativeTimeStampMSec);

                MaxRelativeTimeStampMSec = Children.Max(n => n.MaxRelativeTimeStampMSec);
            }
            else
            {
                MinRelativeTimeStampMSec = TimeStampRelativeMSec;

                MaxRelativeTimeStampMSec = TimeStampRelativeMSec;
            }
        }
    }
}
