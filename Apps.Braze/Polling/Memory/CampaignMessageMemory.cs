namespace Apps.Braze.Polling.Memory
{
    public class CampaignMessageMemory
    {
        public DateTime LastInteractionDate { get; set; }

        /// <summary>
        /// Dictionary to store campaign messages with their last date and time of event.
        /// Key is hex string of SHA-512 hash: campaign ID, message variation ID and message content.
        /// Value is the last date and time of the triggered flight for this message version.
        /// </summary>  
        public Dictionary<string, DateTime> CampaignMessages { get; set; } = new();
    }
}
