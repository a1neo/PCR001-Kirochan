# Plan de Tareas - Sistema POS Ferretería CS

## Estructura del Documento

Este documento desglosa el desarrollo en **6 fases** con tareas específicas, estimaciones de tiempo, prioridades y dependencias. Cada tarea es ejecutable y verificable.

**Convenciones:**
- ⏱️ Estimación en horas
- 🔴 Alta prioridad | 🟡 Media | 🟢 Baja
- ✅ Completada | 🚧 En progreso | ⏸️ Bloqueada | ⬜ Pendiente

---

## Fase 1: Setup y Fundamentos (2 semanas - 80 horas)

### 1.1 Setup de Repositorio y Estructura Base

#### T1.1.1: Inicializar Repositorio Git
**Prioridad:** 🔴 Alta  
**Estimación:** ⏱️ 1h  
**Estado:** ⬜ Pendiente

**Descripción:**
- Crear repositorio Git local e inicializar
- Crear `.gitignore` para .NET y React
- Configurar `.editorconfig` para consistencia de código
- Primer commit con estructura base

**Criterios de Aceptación:**
- [ ] Repositorio Git inicializado
- [ ] `.gitignore` ignora `bin/`, `obj/`, `node_modules/`, `.env`
- [ ] Estructura de carpetas raíz creada

**Comandos:**
```bash
git init
git add .
git commit -m "Initial commit: Project structure"
```

---

#### T1.1.2: Crear Estructura de Proyecto .NET (Backend)
**Prioridad:** 🔴 Alta  
**Estimación:** ⏱️ 3h  
**Estado:** ⬜ Pendiente  
**Depende de:** T1.1.1

**Descripción:**
Crear solución .NET con arquitectura de capas DDD:

```
FerreteriaCS.sln
├── src/
│   ├── FerreteriaCS.Domain/          (Class Library - .NET 8)
│   ├── FerreteriaCS.Application/     (Class Library - .NET 8)
│   ├── FerreteriaCS.Infrastructure/  (Class Library - .NET 8)
│   └── FerreteriaCS.API/             (Web API - .NET 8)
└── tests/
    ├── FerreteriaCS.Domain.Tests/
    ├── FerreteriaCS.Application.Tests/
    └── FerreteriaCS.API.Tests/
```

**Comandos:**
```bash
dotnet new sln -n FerreteriaCS
dotnet new classlib -n FerreteriaCS.Domain -o src/FerreteriaCS.Domain
dotnet new classlib -n FerreteriaCS.Application -o src/FerreteriaCS.Application
dotnet new classlib -n FerreteriaCS.Infrastructure -o src/FerreteriaCS.Infrastructure
dotnet new webapi -n FerreteriaCS.API -o src/FerreteriaCS.API
dotnet new xunit -n FerreteriaCS.Domain.Tests -o tests/FerreteriaCS.Domain.Tests

dotnet sln add src/**/*.csproj tests/**/*.csproj
```

**Criterios de Aceptación:**
- [ ] Solución compila sin errores
- [ ] Referencias entre proyectos configuradas correctamente
- [ ] Tests proyectos creados

---

#### T1.1.3: Configurar NuGet Packages
**Prioridad:** 🔴 Alta  
**Estimación:** ⏱️ 2h  
**Estado:** ⬜ Pendiente  
**Depende de:** T1.1.2

**Descripción:**
Instalar paquetes necesarios en cada proyecto:

**Domain:**
- Ninguno (sin dependencias externas)

**Application:**
```bash
dotnet add package AutoMapper
dotnet add package FluentValidation
dotnet add package MediatR
```

**Infrastructure:**
```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package BCrypt.Net-Next
```

**API:**
```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Swashbuckle.AspNetCore
dotnet add package Serilog.AspNetCore
```

**Tests:**
```bash
dotnet add package FluentAssertions
dotnet add package Moq
dotnet add package Microsoft.EntityFrameworkCore.InMemory
```

**Criterios de Aceptación:**
- [ ] Todos los paquetes instalados sin conflictos
- [ ] Proyecto compila sin errores

---

#### T1.1.4: Crear Proyecto React + TypeScript
**Prioridad:** 🔴 Alta  
**Estimación:** ⏱️ 2h  
**Estado:** ⬜ Pendiente

**Descripción:**
Inicializar frontend con Vite:

```bash
npm create vite@latest src/FerreteriaCS.WebUI -- --template react-ts
cd src/FerreteriaCS.WebUI
npm install
npm install react-router-dom axios zustand
npm install @mui/material @emotion/react @emotion/styled
npm install react-hook-form zod @hookform/resolvers
npm install -D @types/node
```

**Estructura inicial:**
```
/src/FerreteriaCS.WebUI
├── /src
│   ├── /components
│   ├── /pages
│   ├── /services
│   ├── /stores
│   ├── /types
│   ├── /utils
│   ├── App.tsx
│   └── main.tsx
├── package.json
├── tsconfig.json
└── vite.config.ts
```

**Criterios de Aceptación:**
- [ ] `npm run dev` inicia servidor de desarrollo
- [ ] TypeScript configurado sin errores
- [ ] Estructura de carpetas creada

---

### 1.2 Base de Datos y Entity Framework

#### T1.2.1: Configurar ApplicationDbContext
**Prioridad:** 🔴 Alta  
**Estimación:** ⏱️ 4h  
**Estado:** ⬜ Pendiente  
**Depende de:** T1.1.3

**Descripción:**
Crear `ApplicationDbContext.cs` en Infrastructure:

```csharp
public class ApplicationDbContext : DbContext
{
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Inventario> Inventarios { get; set; }
    public DbSet<TipoCambio> TiposCambio { get; set; }
    public DbSet<Venta> Ventas { get; set; }
    public DbSet<DetalleVenta> DetallesVenta { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
```

**Criterios de Aceptación:**
- [ ] DbContext configurado con DbSets
- [ ] Connection string en `appsettings.json`
- [ ] Compila sin errores

---

#### T1.2.2: Crear Entidades de Dominio
**Prioridad:** 🔴 Alta  
**Estimación:** ⏱️ 8h  
**Estado:** ⬜ Pendiente  
**Depende de:** T1.1.2

**Descripción:**
Implementar entidades según `design.md`:

**Archivos a crear:**
- `Domain/Entities/Usuario.cs`
- `Domain/Entities/Categoria.cs`
- `Domain/Entities/Producto.cs`
- `Domain/Entities/Inventario.cs`
- `Domain/Entities/TipoCambio.cs`
- `Domain/Entities/Venta.cs` (Aggregate Root)
- `Domain/Entities/DetalleVenta.cs`
- `Domain/Entities/MovimientoInventario.cs`
- `Domain/Entities/PrecioProducto.cs`

**Criterios de Aceptación:**
- [ ] Todas las entidades implementadas con propiedades privadas
- [ ] Factory methods para creación
- [ ] Value Objects implementados (SKU, Precio)
- [ ] Enums creados (TipoMoneda, Rol, TipoMovimiento)

---

#### T1.2.3: Configurar Fluent API (Entity Configurations)
**Prioridad:** 🔴 Alta  
**Estimación:** ⏱️ 6h  
**Estado:** ⬜ Pendiente  
**Depende de:** T1.2.2

**Descripción:**
Crear configuraciones EF Core en `Infrastructure/Data/Configurations/`:

**Ejemplo (`ProductoConfiguration.cs`):**
```csharp
public class ProductoConfiguration : IEntityTypeConfiguration<Producto>
{
    public void Configure(EntityTypeBuilder<Producto> builder)
    {
        builder.ToTable("productos");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Nombre)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.OwnsOne(p => p.Sku, sku =>
        {
            sku.Property(s => s.Valor)
                .HasColumnName("sku")
                .HasMaxLength(50)
                .IsRequired();
        });
        
        builder.Property(p => p.FormulaPreciosOverride)
            .HasColumnName("formula_precios_json")
            .HasColumnType("jsonb");
        
        builder.HasIndex(p => p.Sku.Valor).IsUnique();
    }
}
```

**Criterios de Aceptación:**
- [ ] Configuraciones para todas las entidades
- [ ] Constraints de DB aplicados (UNIQUE, CHECK, FK)
- [ ] Nombres de columnas en snake_case

---

#### T1.2.4: Crear Migración Inicial
**Prioridad:** 🔴 Alta  
**Estimación:** ⏱️ 2h  
**Estado:** ⬜ Pendiente  
**Depende de:** T1.2.1, T1.2.3

**Descripción:**
Generar y aplicar migración inicial:

```bash
cd src/FerreteriaCS.API
dotnet ef migrations add InitialCreate --project ../FerreteriaCS.Infrastructure
dotnet ef database update
```

**Criterios de Aceptación:**
- [ ] Migración generada sin errores
- [ ] Base de datos creada en PostgreSQL
- [ ] Todas las tablas e índices creados
- [ ] Script SQL de migración revisado

---

### 1.3 Autenticación y Autorización

#### T1.3.1: Implementar PasswordHasher Service
**Prioridad:** 🔴 Alta  
**Estimación:** ⏱️ 2h  
**Estado:** ⬜ Pendiente

**Descripción:**
Crear `Infrastructure/Services/PasswordHasher.cs`:

```csharp
public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password) 
        => BCrypt.Net.BCrypt.HashPassword(password);
    
    public bool Verify(string password, string hash) 
        => BCrypt.Net.BCrypt.Verify(password, hash);
}
```

**Criterios de Aceptación:**
- [ ] Interfaz `IPasswordHasher` en Application
- [ ] Implementación con BCrypt
- [ ] Tests unitarios: hash y verify

---

#### T1.3.2: Implementar JWT Token Service
**Prioridad:** 🔴 Alta  
**Estimación:** ⏱️ 3h  
**Estado:** ⬜ Pendiente

**Descripción:**
Crear `Infrastructure/Services/JwtTokenService.cs`:

```csharp
public class JwtTokenService : IJwtTokenService
{
    public string GenerarToken(Usuario usuario)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Name, usuario.Nombre),
            new Claim(ClaimTypes.Role, usuario.Rol.ToString())
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(60),
            signingCredentials: creds
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

**Criterios de Aceptación:**
- [ ] Token generado incluye claims correctos
- [ ] Expiración configurable
- [ ] Tests unitarios

---

#### T1.3.3: Implementar Login Use Case
**Prioridad:** 🔴 Alta  
**Estimación:** ⏱️ 4h  
**Estado:** ⬜ Pendiente  
**Depende de:** T1.3.1, T1.3.2

**Descripción:**
Crear `Application/UseCases/Auth/LoginUseCase.cs`:

```csharp
public class LoginUseCase
{
    public async Task<LoginResponse> EjecutarAsync(LoginCommand command)
    {
        var usuario = await _usuarioRepo.ObtenerPorEmailAsync(command.Email);
        
        if (usuario == null || !_passwordHasher.Verify(command.Password, usuario.PasswordHash))
            throw new UnauthorizedException("Credenciales inválidas");
        
        if (!usuario.Activo)
            throw new UnauthorizedException("Usuario inactivo");
        
        var token = _jwtService.GenerarToken(usuario);
        
        return new LoginResponse
        {
            Token = token,
            Usuario = _mapper.Map<UsuarioDto>(usuario),
            ExpiresIn = 3600
        };
    }
}
```

**Criterios de Aceptación:**
- [ ] Validación de credenciales
- [ ] Generación de token JWT
- [ ] Manejo de errores (usuario no existe, inactivo)

---

#### T1.3.4: Crear AuthController
**Prioridad:** 🔴 Alta  
**Estimación:** ⏱️ 2h  
**Estado:** ⬜ Pendiente  
**Depende de:** T1.3.3

**Descripción:**
Crear `API/Controllers/AuthController.cs`:

```csharp
[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginCommand command)
    {
        var response = await _loginUseCase.EjecutarAsync(command);
        return Ok(ApiResponse<LoginResponse>.Success(response));
    }
}
```

**Criterios de Aceptación:**
- [ ] Endpoint `/api/v1/auth/login` funcional
- [ ] Retorna token JWT válido
- [ ] Swagger documenta el endpoint

---

#### T1.3.5: Configurar JWT en Program.cs
**Prioridad:** 🔴 Alta  
**Estimación:** ⏱️ 2h  
**Estado:** ⬜ Pendiente  
**Depende de:** T1.3.2

**Descripción:**
Configurar autenticación JWT en `API/Program.cs`:

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();
```

**Criterios de Aceptación:**
- [ ] Middleware de autenticación agregado
- [ ] Endpoints protegidos requieren token
- [ ] Token inválido retorna 401

---

### 1.4 Frontend Base

#### T1.4.1: Configurar Router y Layout
**Prioridad:** 🔴 Alta  
**Estimación:** ⏱️ 3h  
**Estado:** ⬜ Pendiente

**Descripción:**
Crear rutas principales:

```tsx
// App.tsx
function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route element={<PrivateRoute />}>
          <Route path="/" element={<Layout />}>
            <Route index element={<DashboardPage />} />
            <Route path="productos" element={<ProductosPage />} />
            <Route path="pos" element={<POSPage />} />
            <Route path="inventario" element={<InventarioPage />} />
            <Route path="reportes" element={<ReportesPage />} />
          </Route>
        </Route>
      </Routes>
    </BrowserRouter>
  );
}
```

**Criterios de Aceptación:**
- [ ] Rutas configuradas
- [ ] Layout con menú lateral
- [ ] Rutas protegidas requieren autenticación

---

#### T1.4.2: Crear AuthService y Axios Config
**Prioridad:** 🔴 Alta  
**Estimación:** ⏱️ 3h  
**Estado:** ⬜ Pendiente

**Descripción:**
Crear `services/api.ts` y `services/authService.ts`:

```typescript
// api.ts
const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5000/api/v1',
  timeout: 10000,
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// authService.ts
export const authService = {
  async login(email: string, password: string): Promise<LoginResponse> {
    const response = await api.post('/auth/login', { email, password });
    const { token, usuario } = response.data.data;
    
    localStorage.setItem('token', token);
    localStorage.setItem('usuario', JSON.stringify(usuario));
    
    return response.data.data;
  },
  
  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('usuario');
  },
  
  isAuthenticated(): boolean {
    return !!localStorage.getItem('token');
  }
};
```

**Criterios de Aceptación:**
- [ ] Axios configurado con interceptores
- [ ] Token agregado automáticamente a requests
- [ ] Login guarda token en localStorage

---

#### T1.4.3: Crear LoginPage
**Prioridad:** 🔴 Alta  
**Estimación:** ⏱️ 4h  
**Estado:** ⬜ Pendiente  
**Depende de:** T1.4.2

**Descripción:**
Crear página de login con validación:

```tsx
export const LoginPage = () => {
  const navigate = useNavigate();
  const { register, handleSubmit, formState: { errors } } = useForm<LoginForm>();
  
  const onSubmit = async (data: LoginForm) => {
    try {
      await authService.login(data.email, data.password);
      navigate('/');
    } catch (error) {
      toast.error('Credenciales inválidas');
    }
  };
  
  return (
    <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
      <Card sx={{ p: 4, maxWidth: 400 }}>
        <Typography variant="h5" mb={3}>Iniciar Sesión</Typography>
        <form onSubmit={handleSubmit(onSubmit)}>
          <TextField
            label="Email"
            fullWidth
            {...register('email', { required: 'Email requerido' })}
            error={!!errors.email}
            helperText={errors.email?.message}
          />
          <TextField
            label="Contraseña"
            type="password"
            fullWidth
            {...register('password', { required: 'Contraseña requerida' })}
            error={!!errors.password}
            helperText={errors.password?.message}
          />
          <Button type="submit" variant="contained" fullWidth>
            Ingresar
          </Button>
        </form>
      </Card>
    </Box>
  );
};
```

**Criterios de Aceptación:**
- [ ] Formulario funcional con validación
- [ ] Redirección al dashboard tras login exitoso
- [ ] Mensajes de error amigables

---

#### T1.4.4: Crear Dashboard Básico
**Prioridad:** 🟡 Media  
**Estimación:** ⏱️ 3h  
**Estado:** ⬜ Pendiente

**Descripción:**
Dashboard con cards resumen estáticas (datos mock):

```tsx
export const DashboardPage = () => {
  return (
    <Box>
      <Typography variant="h4" mb={3}>Dashboard</Typography>
      <Grid container spacing={3}>
        <Grid item xs={12} md={4}>
          <Card>
            <CardContent>
              <Typography variant="h6">Ventas del Día</Typography>
              <Typography variant="h3">Bs 5,240.50</Typography>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} md={4}>
          <Card>
            <CardContent>
              <Typography variant="h6">Stock Bajo Mínimo</Typography>
              <Typography variant="h3">12</Typography>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} md={4}>
          <Card>
            <CardContent>
              <Typography variant="h6">Tipo de Cambio</Typography>
              <Typography variant="h3">6.96</Typography>
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Box>
  );
};
```

**Criterios de Aceptación:**
- [ ] Dashboard muestra 3 cards
- [ ] Responsive en móvil
- [ ] Datos estáticos por ahora

---

### 1.5 DevOps Inicial

#### T1.5.1: Configurar Docker Compose para Desarrollo
**Prioridad:** 🟡 Media  
**Estimación:** ⏱️ 2h  
**Estado:** ⬜ Pendiente

**Descripción:**
Crear `docker-compose.yml`:

```yaml
version: '3.8'
services:
  postgres:
    image: postgres:15
    environment:
      POSTGRES_USER: ferreteria
      POSTGRES_PASSWORD: dev123
      POSTGRES_DB: ferreteria_db
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

volumes:
  postgres_data:
```

**Criterios de Aceptación:**
- [ ] PostgreSQL corre en Docker
- [ ] Connection string apunta a contenedor
- [ ] `docker-compose up` levanta DB

---

#### T1.5.2: Seed de Datos Iniciales
**Prioridad:** 🟡 Media  
**Estimación:** ⏱️ 3h  
**Estado:** ⬜ Pendiente

**Descripción:**
Crear seed en `Infrastructure/Data/DbInitializer.cs`:

```csharp
public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext context, IPasswordHasher hasher)
    {
        // Crear usuario SuperAdmin inicial
        if (!await context.Usuarios.AnyAsync())
        {
            var admin = new Usuario
            {
                Nombre = "Super Admin",
                Email = "admin@ferreteria.com",
                PasswordHash = hasher.Hash("Admin123!"),
                Rol = Rol.SuperAdmin,
                Activo = true
            };
            context.Usuarios.Add(admin);
        }
        
        // Crear categorías básicas
        if (!await context.Categorias.AnyAsync())
        {
            var categorias = new List<Categoria>
            {
                new() { Nombre = "Herramientas", Descripcion = "Herramientas manuales y eléctricas" },
                new() { Nombre = "Materiales Eléctricos", Descripcion = "Cables, interruptores, etc." },
                new() { Nombre = "Plomería", Descripcion = "Tuberías, llaves, accesorios" }
            };
            context.Categorias.AddRange(categorias);
        }
        
        // Tipo de cambio inicial
        if (!await context.TiposCambio.AnyAsync())
        {
            context.TiposCambio.Add(new TipoCambio
            {
                Fecha = DateTime.Today,
                UsdABob = 6.96m,
                BobAUsd = 0.1437m
            });
        }
        
        await context.SaveChangesAsync();
    }
}
```

**Criterios de Aceptación:**
- [ ] Usuario admin creado (email: admin@ferreteria.com, pass: Admin123!)
- [ ] 3 categorías base creadas
- [ ] Tipo de cambio inicial registrado
- [ ] Seed ejecutado en startup

---

## Resumen Fase 1

**Total de Tareas:** 20  
**Tiempo Estimado:** 80 horas (2 semanas)  
**Estado General:** ⬜ 0/20 completadas

**Entregables:**
- ✅ Repositorio Git configurado
- ✅ Solución .NET con 4 proyectos + tests
- ✅ Frontend React con TypeScript
- ✅ Base de datos PostgreSQL con schema completo
- ✅ Autenticación JWT funcional
- ✅ Login y dashboard básico
- ✅ Docker Compose para desarrollo local

---

## Fase 2: Gestión de Productos e Inventario (2 semanas - 80 horas)

### 2.1 Backend - Productos

#### T2.1.1: Implementar GeneradorSKU Domain Service
**Prioridad:** 🔴 Alta  
**Estimación:** ⏱️ 3h  
**Estado:** ⬜ Pendiente

**Descripción:**
Implementar `Domain/Services/GeneradorSKU.cs` según `design.md`.

**Criterios de Aceptación:**
- [ ] Genera SKU formato: `CATEGORIA-NOMBRE-VARIANTE`
- [ ] Normaliza texto (sin acentos, espacios, caracteres especiales)
- [ ] Verifica unicidad con sufijo numérico si existe
- [ ] Tests unitarios: casos normales y colisiones

---

#### T2.1.2: Implementar ProductoRepository
**Prioridad:** 🔴 Alta  
**Estimación:** ⏱️ 4h  
**Estado:** ⬜ Pendiente

**Descripción:**
Crear `Infrastructure/Repositories/ProductoRepository.cs`:

```csharp
public class ProductoRepository : IProductoRepository
{
    public async Task<Producto?> ObtenerPorIdAsync(Guid id);
    public async Task<Producto?> ObtenerPorSkuAsync(string sku);
    public async Task<PaginatedList<Producto>> ListarAsync(ProductoFiltros filtros, int page, int pageSize);
    public async Task<bool> ExisteSkuAsync(string sku);
    public Task AgregarAsync(Producto producto);
    public Task ActualizarAsync(Producto producto);
    public Task EliminarAsync(Guid id);  // Soft delete
}
```

**Criterios de Aceptación:**
- [ ] Todos los métodos implementados
- [ ] Búsqueda full-text por nombre
- [ ] Filtros: categoría, stock bajo, activo
- [ ] Soft delete (activo = false)

---

#### T2.1.3: Crear CrearProductoUseCase
**Prioridad:** 🔴 Alta  
**Estimación:** ⏱️ 4h  
**Estado:** ⬜ Pendiente  
**Depende de:** T2.1.1, T2.1.2

**Descripción:**
Implementar caso de uso para crear producto:

```csharp
public class CrearProductoUseCase
{
    public async Task<ProductoDto> EjecutarAsync(CrearProductoCommand command)
    {
        // 1. Validar categoría existe
        var categoria = await _categoriaRepo.ObtenerPorIdAsync(command.CategoriaId);
        
        // 2. Crear producto
        var producto = Producto.Crear(command.Nombre, command.CategoriaId, command.UnidadMedida, command.Variante);
        
        // 3. Generar SKU único
        var sku = await _generadorSKU.GenerarUnicoAsync(categoria.Nombre, command.Nombre, command.Variante);
        producto.AsignarSku(sku);
        
        // 4. Crear inventario inicial
        var inventario = new Inventario
        {
            ProductoId = producto.Id,
            StockActual = 0,
            StockMinimo = command.StockMinimo,
            StockMaximo = command.StockMaximo
        };
        
        // 5. Guardar
        await _productoRepo.AgregarAsync(producto);
        await _inventarioRepo.AgregarAsync(inventario);
        await _unitOfWork.CommitAsync();
        
        return _mapper.Map<ProductoDto>(producto);
    }
}
```

**Criterios de Aceptación:**
- [ ] Producto creado con SKU auto-generado
- [ ] Inventario inicial creado
- [ ] Validaciones: categoría existe, nombre no vacío
- [ ] Tests unitarios

---

#### T2.1.4: Crear ProductosController
**Prioridad:** 🔴 Alta  
**Estimación:** ⏱️ 5h  
**Estado:** ⬜ Pendiente  
**Depende de:** T2.1.3

**Descripción:**
Implementar endpoints CRUD de productos:

```csharp
[ApiController]
[Route("api/v1/productos")]
[Authorize]
public class ProductosController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedList<ProductoDto>>>> Listar([FromQuery] ProductoFiltros filtros);
    
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ProductoDto>>> ObtenerPorId(Guid id);
    
    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<ProductoDto>>> Crear([FromBody] CrearProductoCommand command);
    
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<ProductoDto>>> Actualizar(Guid id, [FromBody] ActualizarProductoCommand command);
    
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<bool>>> Eliminar(Guid id);
    
    [HttpPatch("{id}/proteger-recalculo")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<ApiResponse<bool>>> ProtegerRecalculo(Guid id, [FromBody] bool proteger);
}
```

**Criterios de Aceptación:**
- [ ] CRUD completo funcional
- [ ] Autorización por roles correcta
- [ ] Paginación implementada
- [ ] Swagger documenta endpoints

---

### 2.2 Frontend - Productos

#### T2.2.1: Crear ProductoService
**Prioridad:** 🔴 Alta  
**Estimación:** ⏱️ 2h  
**Estado:** ⬜ Pendiente

**Descripción:**
Crear `services/productoService.ts`:

```typescript
export const productoService = {
  async listar(filtros: ProductoFiltros): Promise<PaginatedResponse<Producto>> {
    const response = await api.get('/productos', { params: filtros });
    return response.data.data;
  },
  
  async obtenerPorId(id: string): Promise<Producto> {
    const response = await api.get(`/productos/${id}`);
    return response.data.data;
  },
  
  async crear(data: CrearProductoDto): Promise<Producto> {
    const response = await api.post('/productos', data);
    return response.data.data;
  },
  
  async actualizar(id: string, data: ActualizarProductoDto): Promise<Producto> {
    const response = await api.put(`/productos/${id}`, data);
    return response.data.data;
  },
  
  async eliminar(id: string): Promise<void> {
    await api.delete(`/productos/${id}`);
  }
};
```

**Criterios de Aceptación:**
- [ ] Todos los métodos CRUD implementados
- [ ] Tipos TypeScript definidos
- [ ] Manejo de errores

---

#### T2.2.2: Crear ProductosPage (Tabla y Filtros)
**Prioridad:** 🔴 Alta  
**Estimación:** ⏱️ 8h  
**Estado:** ⬜ Pendiente  
**Depende de:** T2.2.1

**Descripción:**
Página con tabla de productos, filtros y paginación:

**Componentes:**
- Tabla con columnas: SKU, Nombre, Categoría, Stock, Precio Venta, Estado
- Filtros: Búsqueda, Categoría, Stock Bajo Mínimo
- Botones: Nuevo, Editar, Eliminar
- Paginación

**Criterios de Aceptación:**
- [ ] Tabla muestra productos paginados
- [ ] Filtros funcionan correctamente
- [ ] Botones abren modals
- [ ] Responsive en móvil

---

#### T2.2.3: Crear Modal Crear/Editar Producto
**Prioridad:** 🔴 Alta  
**Estimación:** ⏱️ 6h  
**Estado:** ⬜ Pendiente

**Descripción:**
Modal con formulario completo:

**Campos:**
- Nombre (requerido)
- Descripción
- Categoría (dropdown)
- Código de barras
- Unidad de medida (dropdown)
- Variante
- Stock mínimo
- Stock máximo
- Checkbox "No recalcular precios"

**Criterios de Aceptación:**
- [ ] Validación de campos requeridos
- [ ] Dropdown de categorías carga desde API
- [ ] SKU se muestra después de crear (auto-generado)
- [ ] Mensajes de éxito/error

---

### 2.3 Backend - Inventario

#### T2.3.1: Implementar RegistrarEntradaUseCase
**Prioridad:** 🔴 Alta  
**Estimación:** ⏱️ 6h  
**Estado:** ⬜ Pendiente

**Descripción:**
Caso de uso para entrada de mercancía con cálculo de precios:

```csharp
public class RegistrarEntradaUseCase
{
    public async Task<EntradaResultado> EjecutarAsync(RegistrarEntradaCommand command)
    {
        // 1. Obtener producto
        var producto = await _productoRepo.ObtenerPorIdAsync(command.ProductoId);
        
        // 2. Obtener tipo de cambio actual
        var tipoCambio = await _tipoCambioRepo.ObtenerActualAsync();
        
        // 3. Obtener fórmula de precios
        var formula = producto.FormulaPreciosOverride 
            ?? await _productoRepo.ObtenerFormulaCategoria(producto.CategoriaId);
        
        // 4. Calcular precios
        var precios = _calculadoraPrecios.CalcularPrecios(
            producto,
            command.PrecioEntrada,
            command.MonedaCompra,
            tipoCambio,
            formula
        );
        
        // 5. Incrementar stock
        await _inventarioRepo.IncrementarStockAsync(producto.Id, command.Cantidad);
        
        // 6. Registrar movimiento
        var movimiento = new MovimientoInventario
        {
            ProductoId = producto.Id,
            TipoMovimiento = TipoMovimiento.Entrada,
            Cantidad = command.Cantidad,
            MonedaCompra = command.MonedaCompra,
            PrecioEntrada = command.PrecioEntrada,
            TipoCambioId = tipoCambio.Id,
            UsuarioId = command.UsuarioId,
            Motivo = $"Entrada de mercancía - {command.Proveedor}"
        };
        
        await _inventarioRepo.RegistrarMovimientoAsync(movimiento);
        
        // 7. Actualizar precios del producto
        await _productoRepo.ActualizarPrecioAsync(precios);
        await _unitOfWork.CommitAsync();
        
        return new EntradaResultado
        {
            MovimientoId = movimiento.Id,
            StockActual = await _inventarioRepo.ObtenerStockAsync(producto.Id),
            PreciosCalculados = _mapper.Map<PreciosDto>(precios)
        };
    }
}
```

**Criterios de Aceptación:**
- [ ] Stock incrementado correctamente
- [ ] Precios calculados según fórmula
- [ ] Movimiento registrado con tipo de cambio del día
- [ ] Tests unitarios

---

#### T2.3.2: Crear InventarioController
**Prioridad:** 🔴 Alta  
**Estimación:** ⏱️ 4h  
**Estado:** ⬜ Pendiente  
**Depende de:** T2.3.1

**Descripción:**
Endpoints de inventario:

```csharp
[ApiController]
[Route("api/v1/inventario")]
[Authorize(Roles = "Inventario,Admin,SuperAdmin")]
public class InventarioController : ControllerBase
{
    [HttpPost("entrada")]
    public async Task<ActionResult<ApiResponse<EntradaResultado>>> RegistrarEntrada([FromBody] RegistrarEntradaCommand command);
    
    [HttpPost("ajuste")]
    public async Task<ActionResult<ApiResponse<AjusteResultado>>> RegistrarAjuste([FromBody] RegistrarAjusteCommand command);
    
    [HttpGet("alertas-stock")]
    public async Task<ActionResult<ApiResponse<List<ProductoStockBajoDto>>>> ObtenerAlertasStock();
    
    [HttpGet("movimientos")]
    public async Task<ActionResult<ApiResponse<PaginatedList<MovimientoDto>>>> ListarMovimientos([FromQuery] MovimientoFiltros filtros);
}
```

**Criterios de Aceptación:**
- [ ] Entrada de mercancía funcional
- [ ] Ajustes de inventario (positivos/negativos)
- [ ] Alertas de stock bajo mínimo
- [ ] Historial de movimientos

---

### 2.4 Frontend - Inventario

#### T2.4.1: Crear Página Entrada de Mercancía
**Prioridad:** 🔴 Alta  
**Estimación:** ⏱️ 8h  
**Estado:** ⬜ Pendiente

**Descripción:**
Formulario para registrar entrada:

**Campos:**
- Búsqueda y selección de producto (autocomplete)
- Radio: Moneda de compra (USD / BOB)
- Tipo de cambio actual (readonly, indicador si es de hoy)
- Precio de entrada (input decimal)
- Cantidad
- Proveedor (opcional)
- Panel de vista previa: precios calculados (Salida, Venta, Rebaja, Mínimo)

**Criterios de Aceptación:**
- [ ] Autocomplete de productos funcional
- [ ] Vista previa calcula precios en tiempo real
- [ ] Alerta si tipo de cambio no es de hoy
- [ ] Formulario responsive

---

#### T2.4.2: Crear Página Alertas de Stock
**Prioridad:** 🟡 Media  
**Estimación:** ⏱️ 4h  
**Estado:** ⬜ Pendiente

**Descripción:**
Lista de productos bajo stock mínimo:

**Columnas:**
- SKU, Nombre, Stock Actual, Stock Mínimo, Diferencia

**Criterios de Aceptación:**
- [ ] Lista ordenada por urgencia (stock más bajo primero)
- [ ] Link rápido a entrada de mercancía
- [ ] Badge con cantidad de alertas en menú

---

## Resumen Fase 2

**Total de Tareas:** 15  
**Tiempo Estimado:** 80 horas (2 semanas)  
**Estado General:** ⬜ 0/15 completadas

**Entregables:**
- ✅ CRUD de productos completo
- ✅ Generación automática de SKU
- ✅ Sistema de entrada de mercancía
- ✅ Cálculo automático de precios
- ✅ Alertas de stock bajo mínimo
- ✅ Historial de movimientos

---

## Fase 3: Sistema de Precios y Tipo de Cambio (2 semanas - 80 horas)

### 3.1 Backend - Tipo de Cambio

#### T3.1.1: Implementar CalculadoraPrecios Domain Service
**Prioridad:** 🔴 Alta  
**Estimación:** ⏱️ 8h  
**Estado:** ⬜ Pendiente

**Descripción:**
Implementar `Domain/Services/CalculadoraPrecios.cs` según `design.md`.

**Métodos:**
- `CalcularPrecios(producto, precioEntrada, moneda, tipoCambio, formula)`
- `AplicarFormula(precioBase, configuracion, tipoCambio)`

**Criterios de Aceptación:**
- [ ] Aplica fórmulas configurables (operaciones + margen)
- [ ] Convierte monedas correctamente
- [ ] Valida precio venta >= precio mínimo
- [ ] Tests unitarios exhaustivos (10+ casos)

---

#### T3.1.2: Crear Value Object FormulaPrecio
**Prioridad:** 🔴 Alta  
**Estimación:** ⏱️ 4h  
**Estado:** ⬜ Pendiente

**Descripción:**
Implementar `Domain/ValueObjects/FormulaPrecio.cs`:

```csharp
public record FormulaPrecio
{
    public ConfiguracionPrecio PrecioSalida { get; init; }
    public ConfiguracionPrecio PrecioVenta { get; init; }
    public ConfiguracionPrecio PrecioRebaja { get; init; }
    public ConfiguracionPrecio PrecioMinimo { get; init; }
}

public record ConfiguracionPrecio
{
    public string Base { get; init; }  // "PrecioEntrada", "PrecioSalida"
    public bool AplicarTipoCambio { get; init; }
    public TipoCambioTipo? TipoCambioTipo { get; init; }  // Compra, Actual
    public List<Operacion> Operaciones { get; init; }
    public decimal? Margen { get; init; }
}

public record Operacion
{
    public TipoOperacion Tipo { get; init; }  // Multiplicar, Dividir, Sumar, Restar
    public decimal Valor { get; init; }
}
```

**Criterios de Aceptación:**
- [ ] Serializable a JSONB
- [ ] Validación de valores (margen > 0, etc.)
- [ ] Tests de serialización/deserialización

---

#### T3.1.3: Implementar ActualizarTipoCambioUseCase
**Prioridad:** 🔴 Alta  
**Estimación:** ⏱️ 4h  
**Estado:** ⬜ Pendiente

**Descripción:**
Caso de uso para actualizar tipo de cambio:

```csharp
public class ActualizarTipoCambioUseCase
{
    public async Task<TipoCambioDto> EjecutarAsync(ActualizarTipoCambioCommand command)
    {
        // 1. Validar fecha no futura
        if (command.Fecha > DateTime.Today)
            throw new BusinessException("No se puede ingresar tipo de cambio futuro");
        
        // 2. Verificar si ya existe para esa fecha
        var existe = await _tipoCambioRepo.ExisteParaFechaAsync(command.Fecha);
        if (existe)
        {
            // Actualizar existente
            var tipoCambioExistente = await _tipoCambioRepo.ObtenerPorFechaAsync(command.Fecha);
            tipoCambioExistente.Actualizar(command.UsdABob);
            await _tipoCambioRepo.ActualizarAsync(tipoCambioExistente);
        }
        else
        {
            // Crear nuevo
            var tipoCambio = TipoCambio.Crear(command.Fecha, command.UsdABob, command.UsuarioId);
            await _tipoCambioRepo.AgregarAsync(tipoCambio);
        }
        
        await _unitOfWork.CommitAsync();
        
        // 3. Si se solicita recalculo, encolar job asíncrono
        if (command.RecalcularPrecios)
        {
            var jobId = await _backgroundJobService.EnqueueRecalculoPreciosAsync(tipoCambio.Id, command.UsuarioId);
            return new TipoCambioDto
            {
                TipoCambioId = tipoCambio.Id,
                JobId = jobId,
                RecalculoEnProgreso = true
            };
        }
        
        return _mapper.Map<TipoCambioDto>(tipoCambio);
    }
}
```

**Criterios de Aceptación:**
- [ ] Tipo de cambio actualizado correctamente
- [ ] BOB a USD calculado automáticamente (1 / UsdABob)
- [ ] Recalculo opcional lanzado como job async
- [ ] Tests unitarios

---

#### T3.1.4: Implementar RecalcularPreciosUseCase (Asíncrono)
**Prioridad:** 🔴 Alta  
**Estimación:** ⏱️ 8h  
**Estado:** ⬜ Pendiente  
**Depende de:** T3.1.1

**Descripción:**
Caso de uso asíncrono para recalcular precios en lote (ver `design.md` sección 5.2).

**Características:**
- Procesa productos en lotes (100 por vez)
- Omite productos con `NoRecalcularPrecios = true`
- Registra log de recalculo
- Manejo robusto de errores (continúa si uno falla)

**Criterios de Aceptación:**
- [ ] Recalcula todos los productos elegibles
- [ ] Log de auditoría generado
- [ ] Errores individuales no detienen el proceso
- [ ] Tests con mocks

---

#### T3.1.5: Crear TipoCambioController
**Prioridad:** 🔴 Alta  
**Estimación:** ⏱️ 3h  
**Estado:** ⬜ Pendiente

**Descripción:**
Endpoints de tipo de cambio:

```csharp
[ApiController]
[Route("api/v1/tipo-cambio")]
[Authorize]
public class TipoCambioController : ControllerBase
{
    [HttpGet("actual")]
    public async Task<ActionResult<ApiResponse<TipoCambioDto>>> ObtenerActual();
    
    [HttpGet("historial")]
    public async Task<ActionResult<ApiResponse<PaginatedList<TipoCambioDto>>>> Historial([FromQuery] int page = 1);
    
    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponse<TipoCambioDto>>> Actualizar([FromBody] ActualizarTipoCambioCommand command);
    
    [HttpGet("recalculo/{jobId}/status")]
    public async Task<ActionResult<ApiResponse<RecalculoStatusDto>>> ConsultarEstadoRecalculo(string jobId);
}
```

**Criterios de Aceptación:**
- [ ] Endpoint actual retorna tipo de cambio de hoy
- [ ] Historial paginado
- [ ] Actualización genera job async
- [ ] Status de recalculo consultable

---

### 3.2 Backend - Configurador de Fórmulas

#### T3.2.1: Crear ConfigurarFormulasUseCase
**Prioridad:** 🔴 Alta  
**Estimación:** ⏱️ 6h  
**Estado:** ⬜ Pendiente

**Descripción:**
Caso de uso para guardar fórmulas (por categoría o producto):

```csharp
public class ConfigurarFormulasUseCase
{
    public async Task<FormulaDto> EjecutarAsync(ConfigurarFormulasCommand command)
    {
        // 1. Validar fórmulas
        ValidarFormulas(command.Formula);
        
        // 2. Si es para categoría, actualizar categoría
        if (command.CategoriaId.HasValue)
        {
            var categoria = await _categoriaRepo.ObtenerPorIdAsync(command.CategoriaId.Value);
            categoria.AsignarFormulaPrecio(command.Formula);
            await _categoriaRepo.ActualizarAsync(categoria);
        }
        else if (command.ProductoId.HasValue)
        {
            // Si es para producto, crear override
            var producto = await _productoRepo.ObtenerPorIdAsync(command.ProductoId.Value);
            producto.AsignarFormulaOverride(command.Formula);
            await _productoRepo.ActualizarAsync(producto);
        }
        
        await _unitOfWork.CommitAsync();
        
        // 3. Si se solicita aplicar inmediatamente
        if (command.AplicarAhora)
        {
            var productos = command.CategoriaId.HasValue
                ? await _productoRepo.ListarPorCategoriaAsync(command.CategoriaId.Value)
                : new List<Producto> { await _productoRepo.ObtenerPorIdAsync(command.ProductoId.Value) };
            
            // Recalcular cada producto
            foreach (var producto in productos)
            {
                var precioActual = await _productoRepo.ObtenerPrecioAsync(producto.Id);
                var tipoCambio = await _tipoCambioRepo.ObtenerActualAsync();
                
                var precioNuevo = _calculadoraPrecios.CalcularPrecios(
                    producto, precioActual.PrecioEntrada, TipoMoneda.BOB, tipoCambio, command.Formula);
                
                await _productoRepo.ActualizarPrecioAsync(precioNuevo);
            }
        }
        
        return _mapper.Map<FormulaDto>(command.Formula);
    }
    
    private void ValidarFormulas(FormulaPrecio formula)
    {
        // Validar que todas las configuraciones produzcan precios > 0
        // Validar que precio venta >= precio mínimo con datos de ejemplo
    }
}
```

**Criterios de Aceptación:**
- [ ] Fórmulas guardadas en JSONB
- [ ] Validación de fórmulas coherentes
- [ ] Aplicación opcional inmediata
- [ ] Tests

---

#### T3.2.2: Crear FormulasController
**Prioridad:** 🔴 Alta  
**Estimación:** ⏱️ 3h  
**Estado:** ⬜ Pendiente

**Descripción:**
Endpoints para configurar fórmulas:

```csharp
[ApiController]
[Route("api/v1/formulas")]
[Authorize(Roles = "SuperAdmin")]
public class FormulasController : ControllerBase
{
    [HttpGet("categoria/{categoriaId}")]
    public async Task<ActionResult<ApiResponse<FormulaDto>>> ObtenerFormulaCategoria(Guid categoriaId);
    
    [HttpGet("producto/{productoId}")]
    public async Task<ActionResult<ApiResponse<FormulaDto>>> ObtenerFormulaProducto(Guid productoId);
    
    [HttpPost]
    public async Task<ActionResult<ApiResponse<FormulaDto>>> Configurar([FromBody] ConfigurarFormulasCommand command);
    
    [HttpPost("vista-previa")]
    public async Task<ActionResult<ApiResponse<VistaPreviaDto>>> CalcularVistaPrevia([FromBody] VistaPreviaCommand command);
}
```

**Criterios de Aceptación:**
- [ ] Solo SuperAdmin puede configurar
- [ ] Vista previa calcula precios sin guardar
- [ ] Retorna fórmula actual

---

### 3.3 Frontend - Tipo de Cambio

#### T3.3.1: Crear Página Configuración Tipo de Cambio
**Prioridad:** 🔴 Alta  
**Estimación:** ⏱️ 6h  
**Estado:** ⬜ Pendiente

**Descripción:**
Página con:

**Card actual:**
- Último tipo de cambio (fecha, USD→BOB, usuario)
- Indicador visual si es de hoy

**Formulario:**
- Fecha (readonly, hoy)
- USD a BOB (input decimal)
- BOB a USD (calculado automáticamente, readonly)
- Checkbox "Recalcular precios de productos"
- Botón "Actualizar"

**Historial:**
- Tabla con últimos 30 registros

**Criterios de Aceptación:**
- [ ] BOB a USD se calcula en tiempo real al escribir
- [ ] Checkbox de recalculo muestra advertencia
- [ ] Al confirmar, muestra progreso si recalcula
- [ ] Historial paginado

---

#### T3.3.2: Crear Modal de Progreso de Recalculo
**Prioridad:** 🟡 Media  
**Estimación:** ⏱️ 4h  
**Estado:** ⬜ Pendiente

**Descripción:**
Modal que consulta estado del job cada 2 segundos:

**Contenido:**
- Título: "Recalculando precios..."
- Barra de progreso (%)
- Texto: "X de Y productos procesados"
- Lista de errores (si hay)
- Botón "Cerrar" (solo cuando termina)

**Criterios de Aceptación:**
- [ ] Polling cada 2 segundos hasta completar
- [ ] Muestra progreso real
- [ ] Lista errores si los hay
- [ ] No se puede cerrar mientras corre

---

### 3.4 Frontend - Configurador de Fórmulas

#### T3.4.1: Crear Componente Formula Builder
**Prioridad:** 🔴 Alta  
**Estimación:** ⏱️ 12h  
**Estado:** ⬜ Pendiente

**Descripción:**
Componente visual para construir fórmulas sin escribir código:

**Tabs:** Precio Salida | Precio Venta | Precio Rebaja | Precio Mínimo

**Por cada tab:**
- Dropdown: Precio base (Precio Entrada / Precio Salida)
- Checkbox: Aplicar tipo de cambio
  - Si checked: Radio (Tipo cambio de compra / Actual)
- Sección de operaciones dinámicas:
  - Botón "+ Agregar operación"
  - Cada operación: Dropdown (× ÷ + -) + Input (valor)
  - Botón "X" para eliminar
- Input: Margen (%)
- Panel de vista previa:
  - Ejemplo con valores ficticios
  - Resultado calculado en tiempo real

**Criterios de Aceptación:**
- [ ] Builder intuitivo y visual
- [ ] Vista previa en tiempo real
- [ ] Validación: operaciones coherentes
- [ ] Exporta FormulaPrecio JSON correcto

---

#### T3.4.2: Integrar Formula Builder en Categorías y Productos
**Prioridad:** 🔴 Alta  
**Estimación:** ⏱️ 6h  
**Estado:** ⬜ Pendiente  
**Depende de:** T3.4.1

**Descripción:**
Agregar botón "Configurar Fórmulas" en modales de categoría y producto:

**Al abrir:**
- Carga fórmula actual (si existe)
- Permite editar con Formula Builder
- Checkbox "Aplicar a productos existentes ahora"

**Criterios de Aceptación:**
- [ ] Modal integrado en edición de categoría
- [ ] Modal integrado en edición de producto
- [ ] Guarda fórmula correctamente
- [ ] Aplica inmediatamente si se solicita

---

## Resumen Fase 3

**Total de Tareas:** 13  
**Tiempo Estimado:** 80 horas (2 semanas)  
**Estado General:** ⬜ 0/13 completadas

**Entregables:**
- ✅ Sistema de tipo de cambio con historial
- ✅ Recalculo masivo de precios asíncrono
- ✅ Configurador visual de fórmulas
- ✅ Calculadora de precios con fórmulas personalizables
- ✅ Vista previa de cálculos en tiempo real

---

## Fase 4: Punto de Venta (POS) (2 semanas - 80 horas)

_Las tareas de esta fase se detallarán tras completar Fase 3. Incluyen:_

- Backend: ProcesarVentaUseCase, VentasController
- Frontend: Interfaz POS con carrito, selección de moneda, cálculo de cambio
- Generación de tickets
- Validación de stock en tiempo real

---

## Fase 5: Reportes y Dashboard (2 semanas - 80 horas)

_Tareas pendientes de detallar._

---

## Fase 6: Refinamiento y Deploy (2 semanas - 80 horas)

_Tareas pendientes de detallar._

---

## Métricas y Seguimiento

### Progreso General

| Fase | Tareas Completadas | Total Tareas | Progreso | Tiempo Usado | Tiempo Estimado |
|------|-------------------|--------------|----------|--------------|-----------------|
| 1    | 0                 | 20           | 0%       | 0h           | 80h             |
| 2    | 0                 | 15           | 0%       | 0h           | 80h             |
| 3    | 0                 | 13           | 0%       | 0h           | 80h             |
| 4    | 0                 | TBD          | 0%       | 0h           | 80h             |
| 5    | 0                 | TBD          | 0%       | 0h           | 80h             |
| 6    | 0                 | TBD          | 0%       | 0h           | 80h             |
| **Total** | **0**         | **48+**      | **0%**   | **0h**       | **480h**        |

---

## Dependencias Críticas

```
T1.1.1 (Git Init) → T1.1.2 (Estructura .NET) → T1.1.3 (NuGet) → T1.2.1 (DbContext)
                                                               ↓
T1.2.2 (Entidades) → T1.2.3 (Fluent API) → T1.2.4 (Migración)
                                                               ↓
T1.3.1 (PasswordHasher) + T1.3.2 (JWT Service) → T1.3.3 (LoginUseCase) → T1.3.4 (AuthController)

T2.1.1 (GeneradorSKU) + T2.1.2 (ProductoRepo) → T2.1.3 (CrearProductoUseCase) → T2.1.4 (ProductosController)

T3.1.1 (CalculadoraPrecios) → T3.1.4 (RecalcularPreciosUseCase)
T3.1.2 (FormulaPrecio VO) → T3.2.1 (ConfigurarFormulasUseCase)
```

---

## Convenciones de Commit

```bash
feat: nueva funcionalidad
fix: corrección de bug
refactor: refactorización sin cambio de comportamiento
test: agregar o modificar tests
docs: documentación
chore: tareas de mantenimiento (dependencias, config)

Ejemplos:
git commit -m "feat(productos): implementar generador de SKU único"
git commit -m "fix(inventario): corregir validación de stock negativo"
git commit -m "test(ventas): agregar tests para ProcesarVentaUseCase"
```

---

**FIN DEL PLAN DE TAREAS**
