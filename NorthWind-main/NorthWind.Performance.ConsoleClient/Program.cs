using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

class Program
{
    // ⚙️ CONFIGURACIÓN: Ajusta el puerto al de tu API (mira launchSettings.json)
    private const string BaseUrl = "https://localhost:7176";

    // Credenciales de un ADMINISTRADOR válido en tu Base de Datos
    private const string AdminEmail = "superuser@northwind.com";
    private const string AdminPassword = "SuperUser123!";

    private static readonly HttpClient _client = new();
    private static string _token = "";

    static async Task Main(string[] args)
    {
        // Configuración inicial del cliente HTTP
        _client.BaseAddress = new Uri(BaseUrl);
        _client.Timeout = TimeSpan.FromMinutes(10); // Timeout largo para cargas masivas

        Console.Title = "NorthWind Performance Testing Client";
        ShowHeader();

        // 1. ESPERA ACTIVA A QUE LA API LEVANTE
        if (!await WaitForApiAsync())
        {
            return;
        }

        // 2. INICIAR SESIÓN (Obtener Token)
        if (!await LoginAsync())
        {
            Console.WriteLine("\n[FATAL] No se pudo obtener el token. Presione una tecla para salir.");
            Console.ReadKey();
            return;
        }

        // 3. BUCLE DEL MENÚ PRINCIPAL
        bool exit = false;
        while (!exit)
        {
            Console.Clear();
            ShowHeader();
            Console.WriteLine($" [Estado]: API Online ✅ | Usuario: {AdminEmail}");
            Console.WriteLine(" ================================================");
            Console.WriteLine(" PRUEBAS DE PRODUCTOS:");
            Console.WriteLine(" 1. Test INSERT Productos (Carga Masiva)");
            Console.WriteLine(" 2. Test SELECT Productos (Consulta Paginada)");
            Console.WriteLine();
            Console.WriteLine(" PRUEBAS DE CLIENTES (CUSTOMERS):");
            Console.WriteLine(" 3. Test INSERT Clientes (Carga Masiva)");
            Console.WriteLine(" 4. Test SELECT Clientes (Consulta Paginada)");
            Console.WriteLine();
            Console.WriteLine(" 5. Salir");
            Console.WriteLine(" ================================================");
            Console.Write("\n > Seleccione una opción: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1": await RunInsertTest(); break;
                case "2": await RunSelectTest(); break;
                // Nuevos métodos para Customers
                case "3": await RunInsertCustomersTest(); break;
                case "4": await RunSelectCustomersTest(); break;
                case "5": exit = true; break;
                default:
                    Console.WriteLine(" Opción inválida. Presione una tecla...");
                    Console.ReadKey();
                    break;
            }
        }

        Console.WriteLine("\n Gracias por usar NorthWind Performance Client. ¡Hasta pronto!");
    }

    #region TESTS DE RENDIMIENTO (PRODUCTOS)

    // ========== TEST INSERT (PRODUCTOS) ==========
    private static async Task RunInsertTest()
    {
        Console.Clear();
        ShowHeader();
        Console.WriteLine(" [TEST INSERT - CARGA MASIVA DE PRODUCTOS]");
        Console.WriteLine(" ================================================\n");

        Console.Write(" [?] ¿Cuántos productos desea insertar? (1-100000): ");
        if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
        {
            quantity = 100;
            Console.WriteLine($" >> Valor inválido. Se usará {quantity} por defecto.");
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n [⚡] Iniciando carga de {quantity:N0} productos...");
        Console.WriteLine(" [i] Por favor espere, esto puede tomar varios minutos...\n");
        Console.ResetColor();

        var requestData = new { Quantity = quantity };
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await _client.PostAsJsonAsync("/api/performance/products/insert", requestData);
            stopwatch.Stop();

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PerformanceResultDto>();
                PrintInsertResults(quantity, result?.Quantity ?? 0, result?.ElapsedMilliseconds ?? 0, result?.Message ?? "", "Productos");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                PrintFail(response.StatusCode.ToString(), errorContent);
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            PrintFail("Excepción Cliente", ex.Message);
        }
    }

    // ========== TEST SELECT (PRODUCTOS) ==========
    private static async Task RunSelectTest()
    {
        Console.Clear();
        ShowHeader();
        Console.WriteLine(" [TEST SELECT - CONSULTA PAGINADA DE PRODUCTOS]");
        Console.WriteLine(" ================================================\n");

        Console.Write(" [?] ¿Cuántos productos desea consultar?: ");
        if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
        {
            quantity = 1000;
            Console.WriteLine($" >> Valor inválido. Se usará {quantity} por defecto.");
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n [⚡] Iniciando consulta de {quantity:N0} productos...");
        Console.WriteLine(" [i] El servidor paginará automáticamente los resultados...\n");
        Console.ResetColor();

        var requestData = new { Quantity = quantity };
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await _client.PostAsJsonAsync("/api/performance/products/select", requestData);
            stopwatch.Stop();

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PerformanceResultDto>();
                PrintSelectResults(quantity, result?.Quantity ?? 0, result?.ElapsedMilliseconds ?? 0, result?.Message ?? "", "Productos");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                PrintFail(response.StatusCode.ToString(), errorContent);
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            PrintFail("Excepción Cliente", ex.Message);
        }
    }

    #endregion

    #region TESTS DE CLIENTES (CUSTOMERS)

    // ========== TEST INSERT CLIENTES ==========
    private static async Task RunInsertCustomersTest()
    {
        Console.Clear();
        ShowHeader();
        Console.WriteLine(" [TEST INSERT - CARGA MASIVA DE CLIENTES]");
        Console.WriteLine(" ================================================\n");

        Console.Write(" [?] ¿Cuántos clientes desea insertar? (1-100000): ");
        if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
        {
            quantity = 100;
            Console.WriteLine($" >> Valor inválido. Se usará {quantity} por defecto.");
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n [⚡] Iniciando carga de {quantity:N0} clientes...");
        Console.WriteLine(" [i] Generando IDs aleatorios, Correos y Cédulas ficticias...");
        Console.WriteLine(" [i] Por favor espere, esto puede tomar varios minutos...\n");
        Console.ResetColor();

        var requestData = new { Quantity = quantity };
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // CAMBIO: Apunta al endpoint de clientes
            var response = await _client.PostAsJsonAsync("/api/performance/customers/insert", requestData);
            stopwatch.Stop();

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PerformanceResultDto>();
                PrintInsertResults(quantity, result?.Quantity ?? 0, result?.ElapsedMilliseconds ?? 0, result?.Message ?? "", "Clientes");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                PrintFail(response.StatusCode.ToString(), errorContent);
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            PrintFail("Excepción Cliente", ex.Message);
        }
    }

    // ========== TEST SELECT CLIENTES ==========
    private static async Task RunSelectCustomersTest()
    {
        Console.Clear();
        ShowHeader();
        Console.WriteLine(" [TEST SELECT - CONSULTA PAGINADA DE CLIENTES]");
        Console.WriteLine(" ================================================\n");

        Console.Write(" [?] ¿Cuántos clientes desea consultar?: ");
        if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
        {
            quantity = 1000;
            Console.WriteLine($" >> Valor inválido. Se usará {quantity} por defecto.");
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n [⚡] Iniciando consulta de {quantity:N0} clientes...");
        Console.WriteLine(" [i] El servidor paginará automáticamente los resultados...\n");
        Console.ResetColor();

        var requestData = new { Quantity = quantity };
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // CAMBIO: Apunta al endpoint de clientes
            var response = await _client.PostAsJsonAsync("/api/performance/customers/select", requestData);
            stopwatch.Stop();

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PerformanceResultDto>();
                PrintSelectResults(quantity, result?.Quantity ?? 0, result?.ElapsedMilliseconds ?? 0, result?.Message ?? "", "Clientes");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                PrintFail(response.StatusCode.ToString(), errorContent);
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            PrintFail("Excepción Cliente", ex.Message);
        }
    }

    #endregion

    #region HELPERS VISUALES

    private static void PrintInsertResults(int requested, int inserted, long serverTime, string message, string entityName)
    {
        Console.WriteLine("\n╔════════════════════════════════════════════════════╗");
        Console.WriteLine($"║          RESULTADOS - INSERT MASIVO ({entityName.ToUpper()})       ║");
        Console.WriteLine("╚════════════════════════════════════════════════════╝");
        Console.WriteLine($" {entityName} solicitados:         {requested:N0}");
        Console.WriteLine($" {entityName} insertados:          {inserted:N0}");
        Console.WriteLine($" Tiempo servidor:               {serverTime:N0} ms ({serverTime / 1000.0:F2} seg)");

        if (serverTime > 0 && inserted > 0)
        {
            double tps = (double)inserted / (serverTime / 1000.0);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($" Rendimiento:                   {tps:F2} {entityName.ToLower()}/seg");
            Console.ResetColor();
        }

        // Estado final
        if (inserted == requested)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n ✓ {message}");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n ✗ {message}");
            Console.ResetColor();
        }

        Console.WriteLine("════════════════════════════════════════════════════");
        Console.WriteLine("\n Presione cualquier tecla para continuar...");
        Console.ReadKey();
    }

    private static void PrintSelectResults(int requested, int obtained, long serverTime, string message, string entityName)
    {
        Console.WriteLine("\n╔════════════════════════════════════════════════════╗");
        Console.WriteLine($"║          RESULTADOS - SELECT PAGINADO ({entityName.ToUpper()})     ║");
        Console.WriteLine("╚════════════════════════════════════════════════════╝");
        Console.WriteLine($" {entityName} solicitados:         {requested:N0}");
        Console.WriteLine($" {entityName} obtenidos:           {obtained:N0}");
        Console.WriteLine($" Tiempo servidor:               {serverTime:N0} ms ({serverTime / 1000.0:F2} seg)");

        if (serverTime > 0 && obtained > 0)
        {
            double tps = (double)obtained / (serverTime / 1000.0);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($" Rendimiento:                   {tps:F2} {entityName.ToLower()}/seg");
            Console.ResetColor();
        }

        // Estado final
        if (obtained >= requested)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n ✓ {message}");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n ⚠ {message}");
            Console.ResetColor();
        }

        Console.WriteLine("════════════════════════════════════════════════════");
        Console.WriteLine("\n Presione cualquier tecla para continuar...");
        Console.ReadKey();
    }

    private static void PrintFail(string code, string detail)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\n╔════════════════════════════════════════════════════╗");
        Console.WriteLine("║                     ERROR                          ║");
        Console.WriteLine("╚════════════════════════════════════════════════════╝");
        Console.WriteLine($" Código:    {code}");
        Console.WriteLine($" Detalle:   {detail}");
        Console.WriteLine("════════════════════════════════════════════════════");
        Console.ResetColor();
        Console.WriteLine("\n Presione cualquier tecla para continuar...");
        Console.ReadKey();
    }

    #endregion

    #region CONEXIÓN Y AUTENTICACIÓN

    private static async Task<bool> WaitForApiAsync()
    {
        Console.Write(" [...] Esperando a que la API esté disponible");
        int retries = 0;
        int maxRetries = 30; // 60 segundos

        while (retries < maxRetries)
        {
            try
            {
                await _client.GetAsync("/");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(" ✓ [CONECTADO]");
                Console.ResetColor();
                return true;
            }
            catch (HttpRequestException)
            {
                retries++;
                Console.Write(".");
                await Task.Delay(2000);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n\n [ERROR] Inesperado: {ex.Message}");
                Console.ResetColor();
                Console.ReadKey();
                return false;
            }
        }

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\n\n [TIMEOUT] La API no respondió después de 60 segundos.");
        Console.WriteLine(" Verifique que el proyecto API se esté ejecutando correctamente.");
        Console.ResetColor();
        Console.ReadKey();
        return false;
    }

    private static async Task<bool> LoginAsync()
    {
        Console.Write(" [...] Autenticando... ");

        var loginDto = new { email = AdminEmail, password = AdminPassword };

        try
        {
            var response = await _client.PostAsJsonAsync("/user/login", loginDto);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();

                if (!jsonString.Contains("{"))
                {
                    _token = jsonString.Replace("\"", "").Trim();
                }
                else
                {
                    using var doc = JsonDocument.Parse(jsonString);
                    if (doc.RootElement.TryGetProperty("accessToken", out var tokenProp))
                    {
                        _token = tokenProp.GetString();
                    }
                    else if (doc.RootElement.TryGetProperty("token", out var altTokenProp))
                    {
                        _token = altTokenProp.GetString();
                    }
                }

                if (!string.IsNullOrEmpty(_token))
                {
                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("✓ OK");
                    Console.ResetColor();
                    return true;
                }
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"✗ ERROR ({response.StatusCode})");
            Console.ResetColor();
            return false;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n [X] Excepción Login: {ex.Message}");
            Console.ResetColor();
            return false;
        }
    }

    private static void ShowHeader()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("╔════════════════════════════════════════════════════╗");
        Console.WriteLine("║    NORTHWIND PERFORMANCE TESTING CLIENT v4.0       ║");
        Console.WriteLine("╚════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();
    }

    #endregion
}

#region DTOs

public class PerformanceResultDto
{
    public string Operation { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public long ElapsedMilliseconds { get; set; }
    public string Message { get; set; } = string.Empty;
}

#endregion