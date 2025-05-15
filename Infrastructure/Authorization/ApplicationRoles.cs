namespace Infrastructure.Authorization;

public static class ApplicationRoles
{
    public const string Admin = "Admin";
    public const string Manager = "Manager";
    public const string Specialist = "Specialist";
    public const string Client = "Client";
    public const string Unauthenticated = "Unauthenticated";

    public static readonly string[] AllRoles = [Admin, Manager, Specialist, Client, Unauthenticated];
}