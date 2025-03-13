using System.Collections.Generic;
using System.Linq;

public enum StoryNodeStatus
{
    Locked,
    New,        // Newly available, shows an exclamation marker
    Unlocked,   // Already visited or in progress
    Completed   // Finished gameplay for that branch
}

public class StoryNode
{
    public string Id { get; set; }
    public string Title { get; set; }
    public List<StoryNode> Children { get; set; } = new List<StoryNode>();
    public StoryNodeStatus Status { get; set; } = StoryNodeStatus.Locked;
    public List<string> RequiredEvents { get; set; } = new List<string>();

    // Checks if all required events are captured
    public bool IsAvailable(Dictionary<string, bool> capturedEvents)
    {
        return RequiredEvents.All(e => capturedEvents.ContainsKey(e) && capturedEvents[e]);
    }
}