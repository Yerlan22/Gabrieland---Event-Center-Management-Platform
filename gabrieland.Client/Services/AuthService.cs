using System.Text.Json;
using System.Net.Http.Json;


namespace gabrieland.Client.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private readonly CustomAuthStateProvider _authStateProvider;

        public AuthService(HttpClient http, CustomAuthStateProvider authStateProvider)
        {
            _http = http;
            _authStateProvider = authStateProvider;
        }

        public async Task<string> LoginAsync(string idUsuario, string contrasena)
        {
            var response = await _http.PostAsJsonAsync("auth/login", new
            {
                Username = idUsuario,
                Contraseña = contrasena
            });

            var resultJson = await response.Content.ReadAsStringAsync();
            Console.WriteLine(resultJson);

            try
            {
                var result = JsonSerializer.Deserialize<LoginResponse>(resultJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (result != null && result.Success)
            {
                await _authStateProvider.MarkUserAsAuthenticated(result.Token);
                return string.Empty;
            }

                var error = JsonSerializer.Deserialize<ErrorResponse>(resultJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return error?.Message ?? "Error desconocido al iniciar sesión";
            }
            catch (Exception ex)
            {
                return $"Error al procesar la respuesta: {ex.Message}";
            }
        }
    }

    // Definir LoginResponse
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
        public UserDto? User { get; set; }

        public class UserDto
        {
            public int ID_Usuario { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public string Apellido { get; set; } = string.Empty;
            public string Correo { get; set; } = string.Empty;
            public int TUO_ID_Tipo_Usuario { get; set; }
        }
    }
    public class ErrorResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }

}
