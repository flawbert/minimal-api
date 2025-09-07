using MinimalApi.Domain.Entities;
using MinimalApi.DTOS;

namespace MinimalApi.Domain.Interfaces;

public interface IAdminService
{
    Admin? Login(LoginDTO loginDTO);
}