namespace Domain.Abstractions.Result.Errors;

public static class UserErrors
{
    public static Error NotFoundById(string id) => 
        new Error("UserNotFound", $"User with id {id} does not exist");
    
    public static Error NotFoundByUsername(string username) =>
        new Error("UserNotFound", $"User with username {username} does not exist");
    
    public static Error UsernameAlreadyExists(string username) =>
        new Error("UsernameAlreadyExists", $"User with username {username} already exists");
    
    public static Error WrongPassword() =>
        new Error("WrongPassword", $"Wrong password");
    
    public static Error RolesNumberShouldBeAtLeastOne() =>
        new Error("RolesNumberShouldBeAtLeastOne", $"Roles number should be at least one");
    
    public static Error RoleAlreadyAssignedToUser(string role) =>
        new Error("RoleAlreadyAssignedToUser", $"Role {role} already assigned to user");
}