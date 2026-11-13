namespace Whiteout.Core.Contracts
{
    /// <summary>Published by Camp when the player pays for and places a structure.</summary>
    /// <example>
    /// <code>
    /// using var sub = bus.Subscribe&lt;StructurePlaced&gt;(e =&gt; Log(e.StructureId));
    /// </code>
    /// </example>
    public readonly struct StructurePlaced
    {
        /// <summary>Creates the event.</summary>
        /// <param name="structureId">Id of the structure definition.</param>
        /// <param name="x">World X.</param>
        /// <param name="z">World Z.</param>
        public StructurePlaced(string structureId, float x, float z)
        {
            StructureId = structureId;
            X = x;
            Z = z;
        }

        /// <summary>Id of the structure definition.</summary>
        public string StructureId { get; }

        /// <summary>World X.</summary>
        public float X { get; }

        /// <summary>World Z.</summary>
        public float Z { get; }
    }
}
