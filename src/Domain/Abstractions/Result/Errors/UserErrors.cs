namespace Domain.Abstractions.Result.Errors;

public static class UserErrors
{
    public static Error NotFoundById(string id) => 
        new ("UserNotFound", $"User with id {id} does not exist");
    
    public static Error NotFoundByUsername(string username) =>
        new ("UserNotFound", $"User with username {username} does not exist");
    
    public static Error UsernameAlreadyExists(string username) =>
        new ("UsernameAlreadyExists", $"User with username {username} already exists");
    
    public static Error WrongPassword() =>
        new ("WrongPassword", $"Wrong password");
    
    public static Error RolesNumberShouldBeAtLeastOne() =>
        new ("RolesNumberShouldBeAtLeastOne", $"Roles number should be at least one");
    
    public static Error RoleAlreadyAssignedToUser(string role) =>
        new ("RoleAlreadyAssignedToUser", $"Role {role} already assigned to user");
}