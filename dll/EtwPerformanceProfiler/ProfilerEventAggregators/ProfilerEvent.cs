//--------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//--------------------------------------------------------------------------

using System;

namespace EtwPerformanceProfiler
{
    /// <summary>
    /// Defines the various event types that are collected
    /// </summary>
    internal enum EventType
    {
        /// <summary>
        /// Method start.
        /// </summary>
        StartMethod,

        /// <summary>
        /// Method stop
        /// </summary>
        StopMethod,

        /// <summary>
        /// Statement.
        /// </summary>
        Statement,

        /// <summary>
        /// Not an event.
        /// </summary>
        None
    }

    /// <summary>
    /// Defines the various event sub types that are collected
    /// </summary>
    internal enum EventSubType
    {
        /// <summary>
        /// Sql event.
        /// </summary>
        SqlEvent,

        /// <summary>
        /// C/AL event.
        /// </summary>
        AlEvent,

        /// <summary>
        /// Statement.
        /// </summary>
        SystemEvent,

        /// <summary>
        /// Not an event.
        /// </summary>
        None
    }

    /// <summary>
    /// Defines the data structure for an ETW event issued by the NAV server
    /// </summary>
    internal struct ProfilerEvent
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        internal EventType Type { get; set; }

        /// <summary>
        /// Gets or sets the sub type.
        /// </summary>
        internal EventSubType SubType { get; set; }

        /// <summary>
        /// Gets or sets the session id.
        /// </summary>
        internal int SessionId { get; set; }

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
		internal string ObjectType { get; set; }

        /// <summary>
        /// Gets or sets the object id.
        /// </summary>
        internal int ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the line no.
        /// </summary>
        internal int LineNumber { get; set; }

        /// <summary>
        /// Gets or sets the name of the Statement.
        /// </summary>
        internal string StatementName { get; set; }

        /// <summary>
        /// Gets or sets the time stamp in 100ns.
        /// </summary>
        internal double TimeStampRelativeMSec { get; set; }

        /// <summary>
        /// Returns true if this is the none AL event.
        /// </summary>
        internal bool IsNonAlEvent
        {
            get { return !IsAlEvent; }
        }

        /// <summary>
        /// Returns true if this is the AL event.
        /// </summary>
        internal bool IsAlEvent
        {
            get { return ObjectId != 0; }
        }

        /// <summary>
        /// Determines if the current object is equal to the <paramref name="obj"/>
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>true, if the two instances are equal</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is ProfilerEvent))
            {
                return false;
            }

            ProfilerEvent other = (ProfilerEvent)obj;

            return Type.Equals(other.Type)
                   && SessionId.Equals(other.SessionId)
                   && ObjectType.Equals(other.ObjectType)
                   && ObjectId.Equals(other.ObjectId) && LineNumber.Equals(other.LineNumber)
                   && StatementName.Equals(other.StatementName) &&
                   TimeStampRelativeMSec.Equals(other.TimeStampRelativeMSec);
        }

        /// <summary>
        /// Determines if the two instances of <see cref="ProfilerEvent"/> are equal.
        /// </summary>
        /// <param name="lfh">The first instance</param>
        /// <param name="rhs">The second instance</param>
        /// <returns>true, if the two instances are equal</returns>
        public static bool operator ==(ProfilerEvent lfh, ProfilerEvent rhs)
        {
            return lfh.Equals(rhs);
        }

        /// <summary>
        /// Determines if the two instances of <see cref="ProfilerEvent"/> are not equal.
        /// </summary>
        /// <param name="lhs">The first instance</param>
        /// <param name="rhs">The second instance</param>
        /// <returns>true, if the two instances are not equal</returns>
        public static bool operator !=(ProfilerEvent lhs, ProfilerEvent rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// Generates a hash code for the  object.
        /// </summary>
        /// <returns>A hash code</returns>
        public override int GetHashCode()
        {
            return Type.GetHashCode() ^ SessionId.GetHashCode() ^ ObjectId.GetHashCode() ^ ObjectType.GetHashCode()
                   ^ StatementName.GetHashCode() ^ LineNumber.GetHashCode() ^ TimeStampRelativeMSec.GetHashCode();
        }
    }
}
