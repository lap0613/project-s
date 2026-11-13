namespace Whiteout.Core.Contracts
{
    /// <summary>
    /// Asks the HUD to show a short notice (bag full, missing flint, missing materials; design/ux/ui-plan.md
    /// section 7.3 [E]). Carries a UI text table key, never the text itself.
    /// </summary>
    /// <example>
    /// <code>
    /// bus.Publish(new NoticePosted("notice.bag_full", "나무"));
    /// </code>
    /// </example>
    public readonly struct NoticePosted
    {
        /// <summary>Creates the event.</summary>
        /// <param name="textKey">Key in the UI text table.</param>
        /// <param name="argument">Value for the text's {0} placeholder; may be empty.</param>
        public NoticePosted(string textKey, string argument)
        {
            TextKey = textKey;
            Argument = argument ?? string.Empty;
        }

        /// <summary>Key in the UI text table.</summary>
        public string TextKey { get; }

        /// <summary>Value for the text's {0} placeholder.</summary>
        public string Argument { get; }
    }
}
