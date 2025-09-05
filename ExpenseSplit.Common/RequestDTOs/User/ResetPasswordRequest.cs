namespace ExpenseSplit.Common.RequestDTOs.User;

public class ResetPasswordRequest
{
    public string Email { get; set; }
    public string ResetToken { get; set; }
    public string NewPassword { get; set; }
}
