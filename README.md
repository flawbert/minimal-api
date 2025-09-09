# Minimal API with ASP.NET and C#

This project is a **Minimal API** built with **ASP.NET 7**, developed as part of a **Bootcamp project**.  
It demonstrates the use of authentication with **JWT (JSON Web Token)**, role-based authorization, and CRUD operations for managing **Admins** and **Vehicles**.  

---

## 🚀 Features

- **Authentication & Authorization**
  - JWT-based authentication
  - Role-based access control (`Admin`, `Editor`)
  - Swagger integration with JWT support

- **Entities**
  - **Admin**: Manages application users with roles
  - **Vehicle**: Example entity with CRUD endpoints

- **CRUD Operations**
  - `Admins`: Create, List (paginated), Get by Id
  - `Vehicles`: Create, List (paginated), Get by Id, Update, Delete

- **Validation**
  - Input validation for required fields
  - Business rules (e.g., vehicle year must be greater than 1950)

---

## 🛠️ Technologies Used

- **C#**
- **ASP.NET Minimal API**
- **Entity Framework Core** (with MySQL)
- **JWT Authentication**
- **Swagger / OpenAPI**
- **Dependency Injection**

---