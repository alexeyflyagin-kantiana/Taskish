namespace Taskish.Models
{
    public class TaskItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public required string Title { get; set; }
        public string Description { get; set; } = string.Empty;
        public int StoryPoints { get; set; } = 1;
        public bool IsCompleted { get; set; } = false;
        public DateTime? Deadline { get; set; } = null;
        public DateTime? CompletedAt { get; set; } = null;
    }
}