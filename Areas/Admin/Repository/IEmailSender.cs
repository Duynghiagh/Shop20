namespace ShoppingDemo.Areas.Admin.Repository
{
    public interface IEmailSender
    {
        Task SenEmailAsync(string email, string subject, string message);

    }
}
