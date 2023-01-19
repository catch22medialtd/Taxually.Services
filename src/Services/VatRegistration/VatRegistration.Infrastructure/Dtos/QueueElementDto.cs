namespace VatRegistration.Infrastructure.Dtos
{
    public abstract class QueueElementDto
    {
        public string Filename { get; set; }
    }

    public class CsvFileQueueElementDto : QueueElementDto
    {
        public byte[] Contents { get; set; }
    }

    public class XmlFileQueueElementDto : QueueElementDto
    {
        public string Contents { get; set; }
    }
}