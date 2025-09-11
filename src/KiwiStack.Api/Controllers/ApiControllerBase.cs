using System;
using System.Security.Claims;
using KiwiStack.Shared.Entities;
using KiwiStack.Shared.Enums;
using Microsoft.AspNetCore.Mvc;

namespace KiwiStack.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ApiControllerBase : ControllerBase
{
    public Guid CurrentUserId
    {
        get
        {
            // 尝试多种可能的 claim 类型以提高兼容性
            var userIdClaim = User.FindFirst("userId")
                              ?? User.FindFirst(ClaimTypes.NameIdentifier)
                              ?? User.FindFirst("sub");

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("User ID claim is missing or invalid.");
            }
            return userId;
        }
    }

    public RoleEnum CurrentUserRole
    {
        get
        {
            var roleClaim = User.FindFirst("role")
                            ?? User.FindFirst(ClaimTypes.Role);

            if (roleClaim == null || !Enum.TryParse<RoleEnum>(roleClaim.Value, out var role))
            {
                throw new UnauthorizedAccessException("Role claim is missing or invalid.");
            }
            return role;
        }
    }
}
