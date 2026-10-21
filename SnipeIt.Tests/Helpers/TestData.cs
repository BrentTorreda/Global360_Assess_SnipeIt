namespace SnipeIt.Tests.Helpers;

public static class TestData
{
    public static string GenerateUniqueAssetTag() =>
        $"PW-MBP-{DateTime.UtcNow:yyyyMMddHHmmss}";
}