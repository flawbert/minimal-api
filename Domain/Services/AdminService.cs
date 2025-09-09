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

    public Admin? GetAdminById(int id)
    {
        return _context.Admins.Where(a => a.Id == id).FirstOrDefault();
    }

    public List<Admin> GetAllAdmins(int? page)
    {
        var query = _context.Admins.AsQueryable();
        int itensPerPage = 10;

        if (page != null)
        {
            query = query.Skip((int)(page - 1) * itensPerPage).Take(itensPerPage);
        }

        return query.ToList();
    }

    public Admin IncludeAdmin(Admin admin)
    {
        _context.Admins.Add(admin);
        _context.SaveChanges();
        return admin;
    }

    public Admin? Login(LoginDTO loginDTO)
    {
        var adm = _context.Admins.Where(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password).FirstOrDefault();
        return adm;
    }
    
}