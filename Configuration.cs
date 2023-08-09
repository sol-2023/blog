namespace Blog; // a partir do C#10, não precisa das chaves

public static class Configuration
{
    // token : formato mais comum , Jwt = Jason web key - inicializado com chave robusta, sen chars especiais
    public static string  JwtKey  = "";
    // API-Key 
    public static string ApiKeyName = "";
    public static string ApiKey = "";
    public static SmtpConfiguration Smtp = new();

     // config para envio de email
    public class SmtpConfiguration // classe dentro da classe 
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 25;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

}
/*
    public static string  JwtKey  = "ZmVkYWY3ZDg4NjNiNDhlMTk3YjkyODdkNDkyYjcwOGU=";
    public static string ApiKeyName = "api_key";
    public static string ApiKey = "curso_api_IlTevUM/z0ey3NwCV/unWg==";

    */
