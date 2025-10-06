using gabrieland.Client.Models;

namespace gabrieland.Client.Services
{
    public class ReservaAppState
    {
        public Reserva? ReservaActual { get; set; }
        public List<ReservaService>? ReservedServicesList { get; set; }

        public void CleanGlobalReservation()
        {
            ReservaActual = null;
            ReservedServicesList = null;
        }
    }
}
