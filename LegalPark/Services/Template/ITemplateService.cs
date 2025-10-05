namespace LegalPark.Services.Template
{
    public interface ITemplateService
    {
        Task<string> ProcessEmailTemplateAsync<T>(string templateName, T model);
    }
}
