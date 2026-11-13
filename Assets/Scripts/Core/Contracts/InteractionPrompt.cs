namespace Whiteout.Core.Contracts
{
    /// <summary>
    /// What to show above an E-action target (design/ux/ui-plan.md section 8). The text itself lives in the UI text
    /// table; this carries only its key, one argument and where to show it.
    /// </summary>
    /// <example>
    /// <code>
    /// var prompt = new InteractionPrompt("prompt.gather", "나무", true, x, y, z);
    /// </code>
    /// </example>
    public readonly struct InteractionPrompt
    {
        /// <summary>Creates the prompt.</summary>
        /// <param name="textKey">Key in the UI text table.</param>
        /// <param name="argument">Value for the text's {0} placeholder; may be empty.</param>
        /// <param name="isAvailable">False when the action cannot be done now (shown greyed).</param>
        /// <param name="x">World X of the target.</param>
        /// <param name="y">World Y of the target.</param>
        /// <param name="z">World Z of the target.</param>
        public InteractionPrompt(string textKey, string argument, bool isAvailable, float x, float y, float z)
        {
            TextKey = textKey;
            Argument = argument ?? string.Empty;
            IsAvailable = isAvailable;
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>Key in the UI text table.</summary>
        public string TextKey { get; }

        /// <summary>Value for the text's {0} placeholder.</summary>
        public string Argument { get; }

        /// <summary>False when the action cannot be done now (shown greyed).</summary>
        public bool IsAvailable { get; }

        /// <summary>World X of the target.</summary>
        public float X { get; }

        /// <summary>World Y of the target.</summary>
        public float Y { get; }

        /// <summary>World Z of the target.</summary>
        public float Z { get; }
    }
}
