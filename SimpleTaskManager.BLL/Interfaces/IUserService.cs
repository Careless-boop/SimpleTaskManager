﻿using SimpleTaskManager.BLL.DTOs;
using SimpleTaskManager.DAL.Models;

namespace SimpleTaskManager.BLL.Interfaces
{
    public interface IUserService
    {
        Task<User?> AuthenticateUserAsync(LoginUserDTO loginUserDto);
        Task<(bool Success, string Message)> RegisterUserAsync(RegisterUserDTO registerUserDto);
    }
}
