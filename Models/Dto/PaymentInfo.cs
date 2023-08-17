namespace SeeSay.Models.Dto;

public class PaymentInfo
{
    public string CardNumber { get; set; } = null!;
    public string CardHolderName { get; set; } = null!;
    public int ExpirationMonth { get; set; }
    public int ExpirationYear { get; set; }
    public int Cvv { get; set; }
    public decimal Amount { get; set; }

    public bool Validate(out string? errorMessage)
    {
        if (string.IsNullOrWhiteSpace(CardNumber))
        {
            errorMessage = "Card number is required";
            return false;
        }

        if (string.IsNullOrWhiteSpace(CardHolderName))
        {
            errorMessage = "Card holder name is required";
            return false;
        }

        if (ExpirationMonth < 1 || ExpirationMonth > 12 || (ExpirationYear == DateTime.Now.Year && ExpirationMonth < DateTime.Now.Month))
        {
            errorMessage = "Expiration month is invalid";
            return false;
        }

        if (ExpirationYear < DateTime.Now.Year || ExpirationYear > 2050)
        {
            errorMessage = "Expiration year is invalid";
            return false;
        }

        if (Cvv.ToString().Length != 3)
        {
            errorMessage = "CVV is invalid";
            return false;
        }

        if (Amount is < 1 or > 5000)
        {
            errorMessage = "Amount is invalid";
            return false;
        }

        errorMessage = null;
        return true;
    }
}