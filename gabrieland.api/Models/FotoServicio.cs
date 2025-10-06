namespace gabrieland.api.Models
{
    public class FotoServicio
    {
        public int ID_foto { get; set; }
        public string foto { get; set; }
        public string esPrincipal { get; set; } = "Y"; // Default to 'Y'
        public int SAL_ID_Servicio { get; set; }
    }
}
