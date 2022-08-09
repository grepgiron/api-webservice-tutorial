using AuthServiceDavivienda.Context;
using AuthServiceDavivienda.Models;

namespace AuthServiceDavivienda.Helper
{
    public class TokenValidation
    {
        public static Boolean tokenValidation(UserToken uT)
        {
            DateTime ahora = Convert.ToDateTime(DateTime.Now);
            DateTime lastToken = Convert.ToDateTime(uT.LastToken);
            var horas = (ahora - lastToken).TotalHours;
            int t = Convert.ToInt32(horas);

            if(t < 24)
            {
                return true;
            }
            return false;
        }
    }
}
