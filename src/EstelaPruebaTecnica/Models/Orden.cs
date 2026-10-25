namespace EstelaPruebaTecnica.Models
{
    public class Orden
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public ICollection<OrdenDetalle> Detalles { get; set; }
    }
}
