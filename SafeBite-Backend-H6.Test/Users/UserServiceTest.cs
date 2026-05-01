using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using SafeBite_Backend_H6.API.Entities.Users;
using SafeBite_Backend_H6.API.Interfaces.Repositories;
using SafeBite_Backend_H6.API.Services.Users;
using SafeBite_Backend_H6.API.Shared;

namespace SafeBite_Backend_H6.Test.Users;

public class UserServiceTest
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
    private readonly Mock<IUserManagementRepository> _userManagementRepoMock;
    private readonly UserService _userService;

    public UserServiceTest()
    {
        //Setup UserManager mock
        var userStore = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStore.Object, null, null, null, null, null, null, null, null);

        //Setup RoleManager mock
        var roleStore = new Mock<IRoleStore<IdentityRole>>();
        _roleManagerMock = new Mock<RoleManager<IdentityRole>>(roleStore.Object, null, null, null, null);

        _userManagementRepoMock = new Mock<IUserManagementRepository>();
        _userService = new UserService(_userManagerMock.Object, _roleManagerMock.Object, _userManagementRepoMock.Object);
    }

    [Fact]
    public async Task ActivateUserAsync_ValidUser_SetDeactivatedToFalseAndUpdates()
    {
        //Arrange
        var userId = "This-user-id";
        var user = new ApplicationUser
        {
            Id = userId,
            IsDeactivated = true
        };

        _userManagerMock.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(user);
        _userManagerMock.Setup(u => u.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        //Act
        await _userService.ActivateUserAsync(userId);

        //Assert
        Assert.False(user.IsDeactivated);
        _userManagerMock.Verify(u => u.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task DeactivateUserAsync_ValidUser_SetDeactivatedToTrueAndUpdates()
    {
        //Arrange
        var userId = "This-user-id";
        var user = new ApplicationUser
        {
            Id = userId,
            IsDeactivated = false
        };

        _userManagerMock.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(user);
        _userManagerMock.Setup(u => u.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        //Act
        await _userService.DeactivateUserAsync(userId);

        //Assert
        Assert.True(user.IsDeactivated);
        _userManagerMock.Verify(u => u.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task ActivateUserAsync_UserNotFound_ThrowNoKeyFoundException()
    {
        //Arrange
        var userId = "This-user-id";

        _userManagerMock.Setup(u => u.FindByIdAsync(userId));

        //Act & Assert
        var response = await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.ActivateUserAsync(userId));
        Assert.Equal("User not found", response.Message);
    }

    [Fact]
    public async Task DeactivateUserAsync_UserNotFound_ThrowNoKeyFoundException()
    {
        //Arrange
        var userId = "This-user-id";

        _userManagerMock.Setup(u => u.FindByIdAsync(userId));

        //Act & Assert
        var response = await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.DeactivateUserAsync(userId));
        Assert.Equal("User not found", response.Message);
    }

    [Fact]
    public async Task AssignRoleAsync_ValidUserAndRole_AddsRole()
    {
        //Arrange
        var userId = "This-user-id";
        var roleName = "Admin";
        var user = new ApplicationUser { Id = userId };

        _userManagerMock.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(user);
        _roleManagerMock.Setup(r => r.RoleExistsAsync(roleName)).ReturnsAsync(true);
        _userManagerMock.Setup(u => u.AddToRoleAsync(user, roleName)).ReturnsAsync(IdentityResult.Success);

        //Act
        await _userService.AssignRoleAsync(userId, roleName);

        //Assert
        _userManagerMock.Verify(r => r.AddToRoleAsync(user, roleName), Times.Once);
    }

    [Fact]
    public async Task GetUserPagedAsync_ReturnsPagedUsers()
    {
        //Arrange
        var parameters = new PaginationParameters { Page = 1, PageSize = 10 };
        var users = new List<ApplicationUser>
        {
            new ApplicationUser
            {
                Id = "1",
                Email = "test1@test.com",
                IsDeactivated = false,
                UserRoles = new List<ApplicationUserRole>()
            },
            new ApplicationUser
            {
                Id = "2",
                Email = "test2@test.com",
                IsDeactivated = false,
                UserRoles = new List<ApplicationUserRole>()
            }
        };

        var query = users.AsQueryable();
        var pagedResult = new PagedResult<ApplicationUser>
        {
            Data = users,
            Page = 1,
            PageSize = 10,
            TotalCount = 2,
            TotalPages = 1
        };

        _userManagementRepoMock.Setup(repo => repo.QueryFilter(null)).Returns(query);
        _userManagementRepoMock.Setup(repo => repo.GetPagedAsync(parameters, query)).ReturnsAsync(pagedResult);

        //Act
        var result = await _userService.GetUsersPagedAsync(parameters);

        //Assert
        Assert.Equal(2, result.Data.Count());
        Assert.Equal(1, result.Page);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal("test1@test.com", result.Data.First().Email);
        _userManagementRepoMock.Verify(repo => repo.QueryFilter(null), Times.Once);
        _userManagementRepoMock.Verify(repo => repo.GetPagedAsync(parameters, query), Times.Once);
    }
}
