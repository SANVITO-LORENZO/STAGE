namespace Creatore_di_annunci.Models.Entities
{
    public class Video
    {
        public Guid Id { get; set; }
        public string Path { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }

        public string json { get; set; }
    }
}
