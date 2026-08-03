namespace RFactory.Shared.Constants;

/// <summary>
/// Cross-cutting constant values shared across the whole system.
/// </summary>
public static class AppConstants
{
    public const string CorsPolicy = "CorsPolicy";

    public static class Paging
    {
        public const int DefaultPage = 1;
        public const int DefaultPageSize = 20;
        public const int MaxPageSize = 200;
    }
}
