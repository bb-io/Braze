namespace Apps.Braze.Models.Content
{
    public class ContentFileData
    {
        public string Text { get; }
        public string Extension { get; }

        public ContentFileData(string text, string extension)
        {
            Text = text;
            Extension = extension;
        }
    }
}
