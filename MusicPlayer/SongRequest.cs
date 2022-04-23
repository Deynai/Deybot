namespace Deybot.Music
{
    public enum Platform
    {
        Local,
        Youtube,
    }

    public struct SongRequest
    {
        public Platform Platform { get; init; }
        public string ID { get; init; }
    }
}