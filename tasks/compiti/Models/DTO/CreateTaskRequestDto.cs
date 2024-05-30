namespace compiti.Models.DTO
{
    public class CreateTaskRequestDto
    {
        //NOME | DESCRIZIONE | MATERIA
        public string Name { get; set; }
        public string Description { get; set; }
        public string Subject { get; set; }
        //COMPLETATO
        public bool IsCompleted { get; set; }
        //DATA DI SCADENZA
        public DateTime DateTime { get; set; }

    }
}
