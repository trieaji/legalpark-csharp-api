using LegalPark.Exception;
using LegalPark.Helpers;
using LegalPark.Models.DTOs.Request.Merchant;
using LegalPark.Models.DTOs.Response.Merchant;
using LegalPark.Repositories.Merchant;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LegalPark.Services.Merchant
{
    public class MerchantService : IMerchantService
    {
        private readonly IMerchantRepository _merchantRepository;
        private readonly CodeGeneratorUtil _codeGeneratorUtil;
        private readonly ILogger<MerchantService> _logger;

        public MerchantService(IMerchantRepository merchantRepository, CodeGeneratorUtil codeGeneratorUtil, ILogger<MerchantService> logger)
        {
            _merchantRepository = merchantRepository;
            _codeGeneratorUtil = codeGeneratorUtil;
            _logger = logger;
        }

        public async Task<IActionResult> CreateNewMerchant(MerchantRequest request)
        {
            _logger.LogInformation("Attempting to create a new merchant with name: {MerchantName}", request.MerchantName);

            try
            {
                
                var newMerchant = new Models.Entities.Merchant
                {
                    MerchantName = request.MerchantName,
                    MerchantAddress = request.MerchantAddress,
                    ContactPerson = request.ContactPerson,
                    ContactPhone = request.ContactPhone,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                
                string uniqueShortCode = await _codeGeneratorUtil.GenerateUniqueMerchantShortCodeAsync();
                newMerchant.MerchantCode = uniqueShortCode;

                await _merchantRepository.AddAsync(newMerchant);
                await _merchantRepository.SaveChangesAsync();

                _logger.LogInformation("New merchant created successfully with ID: {MerchantId}", newMerchant.Id);

                var response = new MerchantResponse
                {
                    Id = newMerchant.Id.ToString(),
                    MerchantCode = newMerchant.MerchantCode,
                    MerchantName = newMerchant.MerchantName,
                    MerchantAddress = newMerchant.MerchantAddress,
                    ContactPerson = newMerchant.ContactPerson,
                    ContactPhone = newMerchant.ContactPhone
                };

                return ResponseHandler.GenerateResponseSuccess(response);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error creating new merchant: {Message}", ex.Message);
                return ResponseHandler.GenerateResponseError(HttpStatusCode.InternalServerError, "FAILED", $"Failed to create new merchant: {ex.Message}");
            }
        }

        public async Task<IActionResult> GetAllMerchants()
        {
            _logger.LogInformation("Attempting to retrieve all merchants.");
            try
            {
                var merchants = await _merchantRepository.GetAllAsync();

                
                var responseData = merchants.Select(m => new MerchantResponse
                {
                    Id = m.Id.ToString(),
                    MerchantCode = m.MerchantCode,
                    MerchantName = m.MerchantName,
                    MerchantAddress = m.MerchantAddress,
                    ContactPerson = m.ContactPerson,
                    ContactPhone = m.ContactPhone
                }).ToList();

                return ResponseHandler.GenerateResponseSuccess(responseData);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all merchants: {Message}", ex.Message);
                return ResponseHandler.GenerateResponseError(HttpStatusCode.InternalServerError, "FAILED", $"Failed to retrieve merchants: {ex.Message}");
            }
        }

        public async Task<IActionResult> DeleteMerchant(string id)
        {
            _logger.LogInformation("Attempting to delete merchant with ID: {Id}", id);
            try
            {
                if (!Guid.TryParse(id, out Guid merchantIdGuid))
                {
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Invalid merchant ID format.");
                }

                var merchant = await _merchantRepository.GetByIdAsync(merchantIdGuid);
                if (merchant == null)
                {
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", $"Merchant with ID {id} not found.");
                }

                _merchantRepository.Delete(merchant);
                await _merchantRepository.SaveChangesAsync();

                _logger.LogInformation("Merchant with ID {Id} deleted successfully.", id);

                return ResponseHandler.GenerateResponseSuccess(new { Message = "Merchant deleted successfully." });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error deleting merchant with ID {Id}: {Message}", id, ex.Message);
                return ResponseHandler.GenerateResponseError(HttpStatusCode.InternalServerError, "FAILED", $"Failed to delete merchant: {ex.Message}");
            }
        }

        public async Task<IActionResult> UpdateExistingMerchant(string id, MerchantRequest request)
        {
            _logger.LogInformation("Attempting to update merchant with ID: {Id}", id);
            try
            {
                if (!Guid.TryParse(id, out Guid merchantIdGuid))
                {
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Invalid merchant ID format.");
                }

                var existingMerchant = await _merchantRepository.GetByIdAsync(merchantIdGuid);

                if (existingMerchant == null)
                {
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", $"Merchant with ID {id} not found.");
                }

                if (request.MerchantName != null)
                {
                    existingMerchant.MerchantName = request.MerchantName;
                }
                if (request.MerchantAddress != null)
                {
                    existingMerchant.MerchantAddress = request.MerchantAddress;
                }
                if (request.ContactPerson != null)
                {
                    existingMerchant.ContactPerson = request.ContactPerson;
                }
                if (request.ContactPhone != null)
                {
                    existingMerchant.ContactPhone = request.ContactPhone;
                }

                existingMerchant.UpdatedAt = DateTime.UtcNow;

                _merchantRepository.Update(existingMerchant);
                await _merchantRepository.SaveChangesAsync();

                var updatedResponse = new MerchantResponse
                {
                    Id = existingMerchant.Id.ToString(),
                    MerchantCode = existingMerchant.MerchantCode,
                    MerchantName = existingMerchant.MerchantName,
                    MerchantAddress = existingMerchant.MerchantAddress,
                    ContactPerson = existingMerchant.ContactPerson,
                    ContactPhone = existingMerchant.ContactPhone
                };

                _logger.LogInformation("Merchant with ID {Id} updated successfully.", id);
                return ResponseHandler.GenerateResponseSuccess(updatedResponse);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error updating merchant with ID {Id}: {Message}", id, ex.Message);
                return ResponseHandler.GenerateResponseError(HttpStatusCode.InternalServerError, "FAILED", $"Failed to update merchant: {ex.Message}");
            }
        }

        public async Task<IActionResult> GetMerchantByCode(MerchantRequest request)
        {
            _logger.LogInformation("Attempting to get merchant by code: {Code}", request.MerchantCode);
            try
            {
                var merchant = await _merchantRepository.FindByMerchantCodeAsync(request.MerchantCode);

                if (merchant != null)
                {
                    var response = new MerchantResponse
                    {
                        Id = merchant.Id.ToString(),
                        MerchantCode = merchant.MerchantCode,
                        MerchantName = merchant.MerchantName,
                        MerchantAddress = merchant.MerchantAddress,
                        ContactPerson = merchant.ContactPerson,
                        ContactPhone = merchant.ContactPhone
                    };
                    return ResponseHandler.GenerateResponseSuccess(response);
                }
                else
                {
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", $"Merchant not found with code: {request.MerchantCode}");
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error retrieving merchant by code {Code}: {Message}", request.MerchantCode, ex.Message);
                return ResponseHandler.GenerateResponseError(HttpStatusCode.InternalServerError, "FAILED", $"Failed to retrieve merchant: {ex.Message}");
            }
        }
    }
}
