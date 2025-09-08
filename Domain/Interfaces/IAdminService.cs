using MinimalApi.Domain.Entities;
using MinimalApi.Domain.DTOs;

namespace MinimalApi.Domain.Interfaces;

public interface IAdminService
{
    Admin? Login(LoginDTO loginDTO);
}