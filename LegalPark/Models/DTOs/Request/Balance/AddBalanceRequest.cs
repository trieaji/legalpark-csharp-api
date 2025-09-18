using System.ComponentModel.DataAnnotations;

namespace LegalPark.Models.DTOs.Request.Balance
{
    public class AddBalanceRequest
    {
        // Atribut [Required] memastikan bahwa UserId tidak null atau string kosong.
        [Required(ErrorMessage = "User ID cannot be blank")]
        public string UserId { get; set; }

        // Atribut [Required] memastikan bahwa Amount tidak null.
        // Atribut [Range] memastikan nilai Amount adalah positif.
        [Required(ErrorMessage = "Amount cannot be null")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be positive")]
        public decimal Amount { get; set; }
    }
}
