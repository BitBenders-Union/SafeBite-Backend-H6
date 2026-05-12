using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Moq;
using SafeBite_Backend_H6.API.Data.IdentityDb;
using SafeBite_Backend_H6.API.Entities.Users;
using SafeBite_Backend_H6.API.Interfaces.Repositories;
using SafeBite_Backend_H6.API.Repositories;
using SafeBite_Backend_H6.API.Services.Users;
using SafeBite_Backend_H6.API.Shared;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace SafeBite_Backend_H6.Test.Users;

public class UserServiceIntegrationTests
{
    private readonly UserService _userService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserServiceIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<IdentityDbContext<ApplicationUser>>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var context = new IdentityDbContext<ApplicationUser>(options);

        // stores
        var userStore = new UserStore<ApplicationUser>(context);
        var roleStore = new RoleStore<IdentityRole>(context);

        //managers
        _userManager = new UserManager<ApplicationUser>(userStore,null,new PasswordHasher<ApplicationUser>(),new List<IUserValidator<ApplicationUser>>(),
            new List<IPasswordValidator<ApplicationUser>>(),null,null,null,null);

        _roleManager = new RoleManager<IdentityRole>(roleStore, new List<IRoleValidator<IdentityRole>>(), null, null, null);

        // Repository not relevant for role logic → mock is fine
        var repoMock = new Mock<IUserManagementRepository>();


        _userService = new UserService(_userManager, _roleManager, repoMock.Object);
    }

    [Fact]
    public async Task AssignRoleAsync_AddsRoleToUser_InDatabase()
    {
        // Arrange
        var roleName = "Admin";
        await _roleManager.CreateAsync(new IdentityRole(roleName));

        var user = new ApplicationUser
        {
            UserName = "test@test.com",
            Email = "test@test.com"
        };

        await _userManager.CreateAsync(user, "Password123!");

        // Act
        await _userService.AssignRoleAsync(user.Id, roleName);

        // Assert
        var roles = await _userManager.GetRolesAsync(user);
        Assert.Contains(roleName, roles);
    }

    [Fact]
    public async Task RemoveRoleAsync_RemovesRoleFromUser_InDatabase()
    {
        // Arrange
        var roleName = "Admin";
        await _roleManager.CreateAsync(new IdentityRole(roleName));

        var user = new ApplicationUser
        {
            UserName = "test@test.com",
            Email = "test@test.com"
        };

        await _userManager.CreateAsync(user, "Password123!");
        await _userManager.AddToRoleAsync(user, roleName);

        // Act
        await _userService.RemoveRoleAsync(user.Id, roleName);

        // Assert
        var roles = await _userManager.GetRolesAsync(user);
        Assert.DoesNotContain(roleName, roles);
    }
    [Fact]
    public async Task GetUsersPagedAsync_ReturnsOnlySearchedUsers()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AuthDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        using var context = new AuthDbContext(options);
        context.Database.EnsureCreated();

        context.Users.AddRange(
            new ApplicationUser
            {
                Id = "1",
                Email = "test1@test.com",
                UserName = "test1@test.com",
                IsDeactivated = false
            },
            new ApplicationUser
            {
                Id = "2",
                Email = "test2@test.com",
                UserName = "test2@test.com",
                IsDeactivated = false
            },
            new ApplicationUser
            {
                Id = "3",
                Email = "jhon@jhonson.com",
                UserName = "jhon@jhonson.com",
                IsDeactivated = false
            }
        );

        await context.SaveChangesAsync();

        var repository = new UserManagementRepository(context);

        var service = new UserService(_userManager, _roleManager, repository);

        var parameters = new PaginationParameters
        {
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await service.GetUsersPagedAsync(parameters, "jhon");

        // Assert
        Assert.Single(result.Data);
        Assert.Equal("jhon@jhonson.com", result.Data.First().Email);
    }
}
