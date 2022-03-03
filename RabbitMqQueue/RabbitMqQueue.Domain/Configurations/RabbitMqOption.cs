namespace RabbitMqQueue.Domain.Configurations;

public class RabbitMqOption
{
    public string HostName { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;
    
    public string Password { get; set; } = string.Empty;
    
    public int Port { get; set; }

    public string VHost { get; set; } = string.Empty;
}