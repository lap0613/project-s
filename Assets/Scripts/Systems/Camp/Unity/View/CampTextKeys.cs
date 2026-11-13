namespace Whiteout.Camp.Unity
{
    /// <summary>UI text table keys used by the Camp views. The texts themselves live in the UiTextTable asset.</summary>
    /// <example>
    /// <code>
    /// bus.Publish(new NoticePosted(CampTextKeys.MissingMaterialsNotice, "나무 1/3"));
    /// </code>
    /// </example>
    public static class CampTextKeys
    {
        /// <summary>Prompt on a campfire that takes fuel; {0} = fuel item name.</summary>
        public const string FuelPrompt = "prompt.fuel";

        /// <summary>Prompt on a burned-out campfire; {0} = relight cost.</summary>
        /// <summary>Prompt for lighting a new fire.</summary>
        public const string IgnitePrompt = "prompt.ignite";

        public const string RelightPrompt = "prompt.relight";

        /// <summary>Prompt inside a shelter.</summary>
        public const string RestPrompt = "prompt.rest";

        /// <summary>Notice when a required item (flint) is missing; {0} = item name.</summary>
        public const string MissingRequiredNotice = "notice.missing_required";

        /// <summary>Notice when materials are missing; {0} = the materials with have/need.</summary>
        public const string MissingMaterialsNotice = "notice.missing_materials";

        /// <summary>Notice when the campfire cannot hold more fuel.</summary>
        public const string FireFullNotice = "notice.fire_full";

        /// <summary>Notice when the preview overlaps an obstacle.</summary>
        public const string PlacementBlockedNotice = "notice.placement_blocked";

        /// <summary>Notice when a structure's per-run limit is reached; {0} = structure name.</summary>
        public const string PlacementLimitNotice = "notice.placement_limit";

        /// <summary>Prompt on the beacon when the bag holds a part; {0} = part name.</summary>
        public const string BeaconInstallPrompt = "prompt.beacon_install";

        /// <summary>Greyed prompt on the beacon when the bag holds no part; {0} = installed/required, e.g. 1/3.</summary>
        public const string BeaconMissingPrompt = "prompt.beacon_missing";

        /// <summary>Prompt on the complete beacon.</summary>
        public const string BeaconRequestPrompt = "prompt.beacon_request";

        /// <summary>Notice when E is pressed on the beacon without a part.</summary>
        public const string BeaconMissingPartNotice = "notice.beacon_missing_part";

        /// <summary>Notice when the last part is installed.</summary>
        public const string BeaconCompletedNotice = "notice.beacon_completed";

        /// <summary>Notice when rescue is requested.</summary>
        public const string RescueRequestedNotice = "notice.rescue_requested";
    }
}
