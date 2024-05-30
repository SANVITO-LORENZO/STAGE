namespace compiti.Models
{
    public class AddTasksViewModel
    {
        //NOME | DESCRIZIONE | MATERIA
        public string Name { get; set; }
        public string Description { get; set; }
        public string subject { get; set; }
        //COMPLETATO
        public bool IsCompleted { get; set; }
        //DATA DI SCADENZA
        public DateTime DateTime { get; set; }
    }
}
