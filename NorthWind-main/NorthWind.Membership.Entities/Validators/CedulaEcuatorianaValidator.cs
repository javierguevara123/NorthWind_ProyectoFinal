using System.Text.RegularExpressions;

namespace NorthWind.Membership.Entities.Validators
{
    public static class CedulaEcuatorianaValidator
    {
        public static bool IsValid(string cedula)
        {
            if (string.IsNullOrWhiteSpace(cedula))
                return false;

            // Debe tener exactamente 10 dígitos
            if (!Regex.IsMatch(cedula, @"^\d{10}$"))
                return false;

            // Los dos primeros dígitos deben estar entre 01 y 24 (provincias)
            int provincia = int.Parse(cedula.Substring(0, 2));
            if (provincia < 1 || provincia > 24)
                return false;

            // Algoritmo de validación módulo 10
            int[] coeficientes = { 2, 1, 2, 1, 2, 1, 2, 1, 2 };
            int suma = 0;

            for (int i = 0; i < 9; i++)
            {
                int digito = int.Parse(cedula[i].ToString());
                int resultado = digito * coeficientes[i];

                if (resultado >= 10)
                    resultado -= 9;

                suma += resultado;
            }

            int digitoVerificador = int.Parse(cedula[9].ToString());
            int residuo = suma % 10;
            int valorEsperado = residuo == 0 ? 0 : 10 - residuo;

            return digitoVerificador == valorEsperado;
        }
    }
}
