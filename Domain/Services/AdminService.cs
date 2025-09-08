using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Domain.DTOs;
using MinimalApi.Infrastructure.Db;

namespace MinimalApi.Domain.Services;

public class AdminService : IAdminService
{
    private readonly DbContexto _context;
    public AdminService(DbContexto context)
    {
        _context = context;

    }

    public Admin? Login(LoginDTO loginDTO)
    {
        var adm = _context.Admins.Where(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password).FirstOrDefault();
        return adm;
    }
    
}