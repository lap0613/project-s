namespace Whiteout.Gathering.Unity
{
    /// <summary>UI text table keys used by the Resources views. The texts themselves live in the UiTextTable asset.</summary>
    /// <example>
    /// <code>
    /// bus.Publish(new NoticePosted(ResourceTextKeys.BagFullNotice, "나무"));
    /// </code>
    /// </example>
    public static class ResourceTextKeys
    {
        /// <summary>Prompt above a pickup; {0} = item name.</summary>
        public const string GatherPrompt = "prompt.gather";

        /// <summary>Prompt for cooking at a lit fire; {0} = raw item name.</summary>
        public const string CookPrompt = "prompt.cook";

        /// <summary>Notice when an item does not fit in the bag; {0} = item name.</summary>
        public const string BagFullNotice = "notice.bag_full";

        /// <summary>Bag slot menu entry: use a consumable.</summary>
        public const string UseAction = "bag.action.use";

        /// <summary>Bag slot menu entry: drop the stack.</summary>
        public const string DropAction = "bag.action.drop";
    }
}
