namespace GLMS.Api.Dtos;

public class AgreementDto
{
    public int ContractId { get; set; }
    public string? SignedAgreementPath { get; set; }
    public bool HasAgreement => !string.IsNullOrWhiteSpace(SignedAgreementPath);
}

public class AgreementUploadResultDto
{
    public int ContractId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DateTime UploadedAtUtc { get; set; } = DateTime.UtcNow;
}
