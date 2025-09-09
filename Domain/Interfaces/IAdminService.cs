using MinimalApi.Domain.Entities;
using MinimalApi.Domain.DTOs;

namespace MinimalApi.Domain.Interfaces;

public interface IAdminService
{
    Admin? Login(LoginDTO loginDTO);
    Admin IncludeAdmin(Admin admin);
    List<Admin> GetAllAdmins(int? page);
    Admin? GetAdminById(int id);
}