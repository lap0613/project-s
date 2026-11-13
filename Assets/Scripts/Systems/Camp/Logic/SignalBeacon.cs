using System;

namespace Whiteout.Camp
{
    /// <summary>
    /// The placed signal beacon frame: where it stands, which parts are installed and whether rescue was requested
    /// (core-rules section 1). Changed only by <see cref="SignalBeaconSystem"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// SignalBeacon beacon = system.Place(x, y, z);
    /// bool antennaOn = beacon.IsPartInstalled(0);
    /// </code>
    /// </example>
    public sealed class SignalBeacon
    {
        private readonly bool[] _installed;

        /// <summary>Creates a beacon frame with no parts installed.</summary>
        /// <param name="partCount">Number of parts the beacon needs.</param>
        /// <param name="x">World X.</param>
        /// <param name="y">World Y (ground height).</param>
        /// <param name="z">World Z.</param>
        public SignalBeacon(int partCount, float x, float y, float z)
        {
            if (partCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(partCount), "The signal beacon needs at least one part.");
            }

            _installed = new bool[partCount];
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>World X.</summary>
        public float X { get; }

        /// <summary>World Y (ground height).</summary>
        public float Y { get; }

        /// <summary>World Z.</summary>
        public float Z { get; }

        /// <summary>Number of parts the beacon needs.</summary>
        public int PartCount => _installed.Length;

        /// <summary>Number of parts installed so far.</summary>
        public int InstalledCount { get; private set; }

        /// <summary>True when every part is installed.</summary>
        public bool IsComplete => InstalledCount == _installed.Length;

        /// <summary>True once rescue has been requested at this beacon.</summary>
        public bool RescueRequested { get; internal set; }

        /// <summary>True when the part at an index is installed.</summary>
        /// <param name="index">Part index in install order.</param>
        public bool IsPartInstalled(int index) => _installed[index];

        internal void Install(int index)
        {
            if (_installed[index])
            {
                return;
            }

            _installed[index] = true;
            InstalledCount++;
        }
    }
}
