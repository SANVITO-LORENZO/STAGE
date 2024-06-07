namespace Creatore_di_annunci.Models.Entities
{
    public class House
    {
        public Guid Id { get; set; }
        public int Piani { get; set; }
        public int Bagni { get; set; }
        public int MQuadri { get; set; }
        public bool ascensore { get; set; }
        public bool piscina {  get; set; }
        public bool giardino { get; set; }
        public bool terrazza {  get; set; }
        public int prezzo { get; set; }
    }
}
