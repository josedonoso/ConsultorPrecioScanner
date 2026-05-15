using System.ComponentModel.DataAnnotations;

namespace ConsultorPrecio.Models {
    public class Producto {
        public int Id { get; set; }
        [Required]
        public string CodigoBarra { get; set; }
        [Required]
        public string Nombre { get; set; }
        public int Precio { get; set; }

    }
}
