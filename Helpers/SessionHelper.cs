namespace EventManagement.Helpers;

public static class SessionHelper
{
    public const string UserIdKey = "UserId";
    public const string FullNameKey = "FullName";
    public const string RoleKey = "Role";

    public static int? GetUserId(this HttpContext context)
    {
        var v = context.Session.GetString(UserIdKey);
        return int.TryParse(v, out var id) ? id : null;
    }

    public static string? GetFullName(this HttpContext context)
        => context.Session.GetString(FullNameKey);

    public static string? GetRole(this HttpContext context)
        => context.Session.GetString(RoleKey);

    public static bool IsLoggedIn(this HttpContext context)
        => context.Session.GetString(UserIdKey) != null;

    public static bool IsInRole(this HttpContext context, string role)
        => string.Equals(context.Session.GetString(RoleKey), role, StringComparison.OrdinalIgnoreCase);

    public static void SetUser(this HttpContext context, int userId, string fullName, string role)
    {
        context.Session.SetString(UserIdKey, userId.ToString());
        context.Session.SetString(FullNameKey, fullName);
        context.Session.SetString(RoleKey, role);
    }

    public static void ClearUser(this HttpContext context)
    {
        context.Session.Clear();
    }
}
