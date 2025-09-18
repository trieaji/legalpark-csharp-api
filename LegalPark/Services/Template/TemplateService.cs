using RazorLight;

namespace LegalPark.Services.Template
{
    public class TemplateService : ITemplateService
    {
        private readonly RazorLightEngine _razorLightEngine;

        // Konstruktor ini menerima RazorLightEngine melalui Dependency Injection.
        public TemplateService()
        {
            // Konfigurasi RazorLightEngine.
            // Basis direktori ditentukan sebagai "Templates" di root proyek.
            // Ini akan mencari file template di folder yang bernama "Templates".
            _razorLightEngine = new RazorLightEngineBuilder()
                .SetOperatingAssembly(typeof(TemplateService).Assembly)
                .UseFileSystemProject(Path.Combine(Directory.GetCurrentDirectory(), "Templates"))
                .Build();
        }

        public async Task<string> ProcessEmailTemplateAsync<T>(string templateName, T model)
        {
            // Method ini memproses template.
            // Parameter templateName merujuk pada nama file, misalnya "EmailConfirmation.cshtml".
            // Parameter model adalah objek yang berisi data yang akan digunakan di template.
            // RazorLight akan merender template dan mengembalikan string HTML yang telah diproses.

            // Catatan: Pastikan Anda telah menginstal paket NuGet RazorLight.
            // Perintahnya: dotnet add package RazorLight

            // Nama template harus menyertakan ekstensi file
            var fullTemplatePath = $"{templateName}.cshtml";

            return await _razorLightEngine.CompileRenderAsync(fullTemplatePath, model);
        }
    }
}
