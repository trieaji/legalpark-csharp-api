namespace LegalPark.Services.Template
{
    public interface ITemplateService
    {
        /// <summary>
        /// Memproses template string dengan data yang diberikan.
        /// </summary>
        /// <param name="templateName">Nama template untuk diproses (tanpa ekstensi).</param>
        /// <param name="model">Objek yang berisi data untuk template.</param>
        /// <returns>String HTML yang telah diproses.</returns>
        Task<string> ProcessEmailTemplateAsync<T>(string templateName, T model);
    }
}
