namespace SafeBiteV2.API.Contracts.Responses.UserManagement
{
    public class UserResponse
    {
        public string Id { get; set; } = default!;
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public bool IsActive { get; set; }
        public bool IsLocked { get; set; }
        public List<UserRoleResponse> Roles { get; set; }

    }
}
