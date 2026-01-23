namespace EventAssist.Models.DTOs
{
    public class TwoFactorQrCodeResponse
    {
        public required string QrCodeUrl { get; set; }
        public required string InputKey { get; set; }
    }
}
