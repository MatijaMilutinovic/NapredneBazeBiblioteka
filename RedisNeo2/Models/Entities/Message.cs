namespace RedisNeo2.Models.Entities
{
    public class Message
    {
        public string? Id { get; set; }
        public string MessageText { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime VremeSlanja { get; set; }
        public string RoomId { get; set; } = string.Empty;
    }
}
