public class JournalEntry
{
    public string Mood;
    public string EventTags;
    public string Notes;
    public string Emoji;

    // A constructor to make it easy to create a new entry
    public JournalEntry(string mood, string tags, string notes, string emoji)
    {
        this.Mood = mood;
        this.EventTags = tags;
        this.Notes = notes;
        this.Emoji = emoji;
    }
}