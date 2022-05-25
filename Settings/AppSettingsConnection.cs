namespace FakeUsersAPI
{
    public class AppSettingsConnection
    {
        public string DBConnection { get; set; } = null!;
        public string RabbitHost { get; set; } = null!;
        public string RabbitLogin { get; set; } = null!;
        public string RabbitPassword { get; set; } = null!;
    }
}
