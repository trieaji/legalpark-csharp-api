using RazorLight;

namespace LegalPark.Services.Template
{
    public class TemplateService : ITemplateService
    {
        private readonly RazorLightEngine _razorLightEngine;

        
        public TemplateService()
        {
            // RazorLightEngine configuration.
            // The base directory is specified as “Templates” in the project root.
            // It will search for template files in the folder named “Templates”.
            _razorLightEngine = new RazorLightEngineBuilder()
                .SetOperatingAssembly(typeof(TemplateService).Assembly)
                .UseFileSystemProject(Path.Combine(Directory.GetCurrentDirectory(), "Templates"))
                .Build();
        }

        public async Task<string> ProcessEmailTemplateAsync<T>(string templateName, T model)
        {
            // This method processes templates.
            // The templateName parameter refers to the file name, for example, “EmailConfirmation.cshtml”.
            // The model parameter is an object that contains the data to be used in the template.
            // RazorLight will render the template and return the processed HTML string.



            // The template name must include the file extension.
            var fullTemplatePath = $"{templateName}.cshtml";

            return await _razorLightEngine.CompileRenderAsync(fullTemplatePath, model);
        }
    }
}
