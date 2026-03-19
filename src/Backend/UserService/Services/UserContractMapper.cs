using Service.Contracts.Responses;
using Service.Contracts.Shared;
using UserService.Features.Users.Entities;

namespace UserService.Services;

public static class UserContractMapper
{
    public static UserModel ToResponse(this UserEntity user)
    {
        return new UserModel
        {
            UserId = user.Id.ToString("D"),
            Name = user.Name,
            Status = (UserStatusEnum)user.Status
        };
    }

    public static ListUsersResponse ToListResponse(this UserListEntity users)
    {
        return new ListUsersResponse
        {
            Users = users.Users.Select(ToResponse).ToList()
        };
    }
}
