# Documento de Diseño Técnico - Sistema POS Ferretería CS

## 1. Arquitectura del Sistema

### 1.1 Arquitectura de Alto Nivel

```
┌─────────────────────────────────────────────────────────────┐
│                    Cliente (Browser)                         │
│              React 18 + TypeScript + Vite                    │
└─────────────────────────────────────────────────────────────┘
                            ▼ HTTPS
┌─────────────────────────────────────────────────────────────┐
│                   API Gateway / Load Balancer                │
│                      (Vercel Edge Network)                   │
└─────────────────────────────────────────────────────────────┘
                            ▼
┌─────────────────────────────────────────────────────────────┐
│              ASP.NET Core 8.0 Web API                        │
│                  (Vercel Serverless / Railway)               │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐         │
│  │ Controllers │→ │  Use Cases  │→ │   Domain    │         │
│  │    (API)    │  │ (Application)│  │   (Logic)   │         │
│  └─────────────┘  └─────────────┘  └─────────────┘         │
│                            ▼                                 │
│  ┌─────────────────────────────────────────────────┐        │
│  │    Infrastructure (EF Core, Repositories)       │        │
│  └─────────────────────────────────────────────────┘        │
└─────────────────────────────────────────────────────────────┘
                            ▼
┌─────────────────────────────────────────────────────────────┐
│              PostgreSQL 15+ (Vercel Postgres)                │
│          Schemas: public, audit (para logs)                  │
└─────────────────────────────────────────────────────────────┘
```

### 1.2 Capas DDD y Responsabilidades

#### Domain Layer (FerreteriaCS.Domain)
**Responsabilidad:** Lógica de negocio pura, sin dependencias externas

```
/Domain
├── /Entities          → Entidades con identidad (Producto, Venta, Usuario)
├── /ValueObjects      → Objetos sin identidad (Precio, SKU, Moneda)
├── /Aggregates        → Raíces agregadas (Venta + DetalleVenta)
├── /DomainServices    → Lógica que no pertenece a una entidad
├── /Interfaces        → Contratos de repositorios
└── /Exceptions        → Excepciones de dominio
```

**Reglas:**
- No conoce Infrastructure ni Application
- No tiene referencias a EF Core, HTTP, JSON
- Solo tipos primitivos, colecciones y otros objetos de dominio

#### Application Layer (FerreteriaCS.Application)
**Responsabilidad:** Casos de uso y orquestación

```
/Application
├── /UseCases          → Un caso de uso = una acción del sistema
├── /DTOs              → Objetos de transferencia (entrada/salida)
├── /Mappers           → AutoMapper profiles
├── /Validators        → FluentValidation rules
└── /Interfaces        → Contratos de servicios externos
```

**Reglas:**
- Orquesta llamadas al dominio
- Transacciones y validaciones
- No tiene lógica de negocio (eso está en Domain)

#### Infrastructure Layer (FerreteriaCS.Infrastructure)
**Responsabilidad:** Implementaciones técnicas

```
/Infrastructure
├── /Data
│   ├── ApplicationDbContext.cs
│   └── /Configurations      → Fluent API para entidades
├── /Repositories            → Implementación de IRepository
├── /Services
│   ├── JwtTokenService.cs
│   ├── PasswordHasher.cs
│   └── ExchangeRateService.cs (futuro: API externa)
└── /Migrations              → EF Core migrations
```

#### API Layer (FerreteriaCS.API)
**Responsabilidad:** Punto de entrada HTTP

```
/API
├── /Controllers       → Endpoints REST
├── /Middleware        → Error handling, JWT validation
├── /Filters           → ActionFilters, ExceptionFilters
├── /Extensions        → ServiceCollection extensions
├── Program.cs
└── appsettings.json
```

---

## 2. Diseño de Base de Datos

### 2.1 Schema PostgreSQL

```sql
-- TABLA: usuarios
CREATE TABLE usuarios (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    nombre VARCHAR(100) NOT NULL,
    email VARCHAR(150) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    rol VARCHAR(20) NOT NULL CHECK (rol IN ('SuperAdmin', 'Admin', 'Inventario', 'Vendedor')),
    activo BOOLEAN DEFAULT true,
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fecha_actualizacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_usuarios_email ON usuarios(email);
CREATE INDEX idx_usuarios_rol ON usuarios(rol);

-- TABLA: categorias
CREATE TABLE categorias (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    nombre VARCHAR(100) NOT NULL,
    descripcion TEXT,
    formula_precios_json JSONB,  -- Almacena fórmulas por defecto
    activo BOOLEAN DEFAULT true,
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- TABLA: productos
CREATE TABLE productos (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    sku VARCHAR(50) UNIQUE NOT NULL,
    nombre VARCHAR(200) NOT NULL,
    descripcion TEXT,
    categoria_id UUID NOT NULL REFERENCES categorias(id),
    codigo_barras VARCHAR(50),
    unidad_medida VARCHAR(20) NOT NULL,  -- unidad, kg, litro, metro, caja
    variante VARCHAR(100),  -- "8mm", "rojo", etc.
    no_recalcular_precios BOOLEAN DEFAULT false,
    formula_precios_json JSONB,  -- Override de categoría (nullable)
    activo BOOLEAN DEFAULT true,
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_productos_sku ON productos(sku);
CREATE INDEX idx_productos_categoria ON productos(categoria_id);
CREATE INDEX idx_productos_codigo_barras ON productos(codigo_barras);
CREATE INDEX idx_productos_nombre ON productos USING gin(to_tsvector('spanish', nombre));

-- TABLA: inventario
CREATE TABLE inventario (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    producto_id UUID UNIQUE NOT NULL REFERENCES productos(id),
    stock_actual INTEGER NOT NULL DEFAULT 0 CHECK (stock_actual >= 0),
    stock_minimo INTEGER NOT NULL DEFAULT 0,
    stock_maximo INTEGER NOT NULL DEFAULT 1000,
    ubicacion VARCHAR(50),
    fecha_ultima_actualizacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_inventario_producto ON inventario(producto_id);
CREATE INDEX idx_inventario_stock_bajo ON inventario(stock_actual) WHERE stock_actual < stock_minimo;

-- TABLA: tipo_cambio
CREATE TABLE tipo_cambio (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    fecha DATE UNIQUE NOT NULL,
    usd_a_bob NUMERIC(10, 4) NOT NULL CHECK (usd_a_bob > 0),
    bob_a_usd NUMERIC(10, 4) NOT NULL CHECK (bob_a_usd > 0),
    usuario_id UUID REFERENCES usuarios(id),
    fecha_actualizacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_tipo_cambio_fecha ON tipo_cambio(fecha DESC);

-- TABLA: precio_producto (snapshot de precios calculados)
CREATE TABLE precio_producto (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    producto_id UUID UNIQUE NOT NULL REFERENCES productos(id),
    moneda_base VARCHAR(3) NOT NULL CHECK (moneda_base IN ('USD', 'BOB')),
    precio_entrada NUMERIC(12, 2) NOT NULL CHECK (precio_entrada >= 0),
    precio_salida NUMERIC(12, 2) NOT NULL CHECK (precio_salida >= 0),
    precio_venta NUMERIC(12, 2) NOT NULL CHECK (precio_venta >= 0),
    precio_rebaja NUMERIC(12, 2) NOT NULL CHECK (precio_rebaja >= 0),
    precio_minimo NUMERIC(12, 2) NOT NULL CHECK (precio_minimo >= 0),
    tipo_cambio_aplicado_id UUID REFERENCES tipo_cambio(id),
    fecha_calculo TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT chk_precio_venta_minimo CHECK (precio_venta >= precio_minimo),
    CONSTRAINT chk_precio_rebaja_minimo CHECK (precio_rebaja >= precio_minimo)
);

-- TABLA: movimiento_inventario
CREATE TABLE movimiento_inventario (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    producto_id UUID NOT NULL REFERENCES productos(id),
    tipo_movimiento VARCHAR(20) NOT NULL CHECK (tipo_movimiento IN ('entrada', 'salida', 'ajuste')),
    cantidad INTEGER NOT NULL,
    moneda_compra VARCHAR(3) CHECK (moneda_compra IN ('USD', 'BOB')),
    precio_entrada NUMERIC(12, 2) CHECK (precio_entrada >= 0),
    tipo_cambio_id UUID REFERENCES tipo_cambio(id),
    usuario_id UUID NOT NULL REFERENCES usuarios(id),
    motivo TEXT,
    fecha TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_movimiento_producto ON movimiento_inventario(producto_id);
CREATE INDEX idx_movimiento_fecha ON movimiento_inventario(fecha DESC);
CREATE INDEX idx_movimiento_tipo ON movimiento_inventario(tipo_movimiento);

-- TABLA: ventas
CREATE TABLE ventas (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    numero_venta SERIAL UNIQUE NOT NULL,
    fecha TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    usuario_id UUID NOT NULL REFERENCES usuarios(id),  -- cajero
    moneda_pago VARCHAR(3) NOT NULL CHECK (moneda_pago IN ('USD', 'BOB')),
    subtotal NUMERIC(12, 2) NOT NULL CHECK (subtotal >= 0),
    total NUMERIC(12, 2) NOT NULL CHECK (total >= 0),
    monto_recibido NUMERIC(12, 2) NOT NULL CHECK (monto_recibido >= total),
    cambio NUMERIC(12, 2) NOT NULL CHECK (cambio >= 0),
    estado VARCHAR(20) DEFAULT 'completada' CHECK (estado IN ('completada', 'cancelada'))
);

CREATE INDEX idx_ventas_fecha ON ventas(fecha DESC);
CREATE INDEX idx_ventas_usuario ON ventas(usuario_id);
CREATE INDEX idx_ventas_numero ON ventas(numero_venta);

-- TABLA: detalle_venta
CREATE TABLE detalle_venta (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    venta_id UUID NOT NULL REFERENCES ventas(id) ON DELETE CASCADE,
    producto_id UUID NOT NULL REFERENCES productos(id),
    nombre_producto VARCHAR(200) NOT NULL,  -- snapshot
    sku VARCHAR(50) NOT NULL,  -- snapshot
    cantidad INTEGER NOT NULL CHECK (cantidad > 0),
    precio_unitario NUMERIC(12, 2) NOT NULL CHECK (precio_unitario >= 0),
    moneda_precio VARCHAR(3) NOT NULL CHECK (moneda_precio IN ('USD', 'BOB')),
    subtotal NUMERIC(12, 2) NOT NULL CHECK (subtotal >= 0)
);

CREATE INDEX idx_detalle_venta ON detalle_venta(venta_id);
CREATE INDEX idx_detalle_producto ON detalle_venta(producto_id);

-- TABLA: log_recalculo_precios (auditoría)
CREATE TABLE log_recalculo_precios (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tipo_cambio_id UUID NOT NULL REFERENCES tipo_cambio(id),
    productos_afectados INTEGER NOT NULL,
    usuario_id UUID REFERENCES usuarios(id),
    fecha TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    detalles JSONB  -- Array de {productoId, preciosAnteriores, preciosNuevos}
);
```

### 2.2 Relaciones y Cardinalidades

```
Usuario (1) ──< (N) Venta
Usuario (1) ──< (N) MovimientoInventario
Usuario (1) ──< (N) TipoCambio

Categoria (1) ──< (N) Producto
Producto (1) ──── (1) Inventario
Producto (1) ──── (1) PrecioProducto
Producto (1) ──< (N) MovimientoInventario
Producto (1) ──< (N) DetalleVenta

Venta (1) ──< (N) DetalleVenta

TipoCambio (1) ──< (N) MovimientoInventario (opcional)
TipoCambio (1) ──< (N) PrecioProducto (opcional)
```

---

## 3. Modelo de Dominio (Entidades y Value Objects)

### 3.1 Entidades Core

#### Producto.cs (Aggregate Root)
```csharp
public class Producto
{
    public Guid Id { get; private set; }
    public SKU Sku { get; private set; }  // Value Object
    public string Nombre { get; private set; }
    public string Descripcion { get; private set; }
    public Guid CategoriaId { get; private set; }
    public string? CodigoBarras { get; private set; }
    public UnidadMedida UnidadMedida { get; private set; }
    public string? Variante { get; private set; }
    public bool NoRecalcularPrecios { get; private set; }
    public FormulaPrecio? FormulaPreciosOverride { get; private set; }  // Value Object
    public bool Activo { get; private set; }
    
    // Navegación
    public Categoria Categoria { get; private set; }
    public Inventario Inventario { get; private set; }
    public PrecioProducto Precio { get; private set; }
    
    // Factory method
    public static Producto Crear(string nombre, Guid categoriaId, UnidadMedida unidad, 
                                  string? variante = null)
    {
        var producto = new Producto
        {
            Id = Guid.NewGuid(),
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre)),
            CategoriaId = categoriaId,
            UnidadMedida = unidad,
            Variante = variante,
            Activo = true
        };
        
        // SKU se genera automáticamente en Domain Service
        return producto;
    }
    
    public void AsignarSku(SKU sku) => Sku = sku;
    
    public void ProtegerDeRecalculo() => NoRecalcularPrecios = true;
    
    public void PermitirRecalculo() => NoRecalcularPrecios = false;
}
```

#### Venta.cs (Aggregate Root)
```csharp
public class Venta
{
    public Guid Id { get; private set; }
    public int NumeroVenta { get; private set; }
    public DateTime Fecha { get; private set; }
    public Guid UsuarioId { get; private set; }
    public TipoMoneda MonedaPago { get; private set; }  // Enum
    public decimal Subtotal { get; private set; }
    public decimal Total { get; private set; }
    public decimal MontoRecibido { get; private set; }
    public decimal Cambio { get; private set; }
    public EstadoVenta Estado { get; private set; }
    
    // Colección de líneas (parte del agregado)
    private readonly List<DetalleVenta> _detalles = new();
    public IReadOnlyCollection<DetalleVenta> Detalles => _detalles.AsReadOnly();
    
    public static Venta Crear(Guid usuarioId, TipoMoneda monedaPago, 
                              List<LineaVenta> lineas, decimal montoRecibido)
    {
        var venta = new Venta
        {
            Id = Guid.NewGuid(),
            Fecha = DateTime.UtcNow,
            UsuarioId = usuarioId,
            MonedaPago = monedaPago,
            MontoRecibido = montoRecibido,
            Estado = EstadoVenta.Completada
        };
        
        foreach (var linea in lineas)
        {
            venta.AgregarDetalle(linea);
        }
        
        venta.CalcularTotales();
        venta.ValidarPago();
        
        return venta;
    }
    
    private void AgregarDetalle(LineaVenta linea)
    {
        var detalle = DetalleVenta.Crear(linea.ProductoId, linea.Nombre, linea.Sku, 
                                          linea.Cantidad, linea.PrecioUnitario, MonedaPago);
        _detalles.Add(detalle);
    }
    
    private void CalcularTotales()
    {
        Subtotal = _detalles.Sum(d => d.Subtotal);
        Total = Subtotal;  // v1: sin impuestos
    }
    
    private void ValidarPago()
    {
        if (MontoRecibido < Total)
            throw new DomainException("Monto recibido insuficiente");
            
        Cambio = MontoRecibido - Total;
    }
}
```

### 3.2 Value Objects

#### SKU.cs
```csharp
public record SKU
{
    public string Valor { get; init; }
    
    private SKU(string valor) => Valor = valor;
    
    public static SKU Generar(string categoria, string nombre, string? variante = null)
    {
        var partes = new List<string>
        {
            Normalizar(categoria),
            Normalizar(nombre)
        };
        
        if (!string.IsNullOrWhiteSpace(variante))
            partes.Add(Normalizar(variante));
        
        var sku = string.Join("-", partes).ToUpperInvariant();
        
        // Limitar longitud
        if (sku.Length > 50)
            sku = sku.Substring(0, 50);
        
        return new SKU(sku);
    }
    
    private static string Normalizar(string texto)
    {
        // Remover acentos, espacios, caracteres especiales
        return Regex.Replace(texto.Trim(), @"[^a-zA-Z0-9]", "");
    }
}
```

#### Precio.cs
```csharp
public record Precio
{
    public decimal Monto { get; init; }
    public TipoMoneda Moneda { get; init; }
    
    public Precio(decimal monto, TipoMoneda moneda)
    {
        if (monto < 0)
            throw new ArgumentException("El precio no puede ser negativo", nameof(monto));
            
        Monto = monto;
        Moneda = moneda;
    }
    
    public Precio ConvertirA(TipoMoneda monedaDestino, decimal tipoCambio)
    {
        if (Moneda == monedaDestino)
            return this;
            
        var montoConvertido = Moneda == TipoMoneda.USD && monedaDestino == TipoMoneda.BOB
            ? Monto * tipoCambio
            : Monto / tipoCambio;
            
        return new Precio(Math.Round(montoConvertido, 2), monedaDestino);
    }
}
```

### 3.3 Domain Services

#### CalculadoraPrecios.cs
```csharp
public class CalculadoraPrecios
{
    public PrecioProducto CalcularPrecios(
        Producto producto, 
        decimal precioEntrada, 
        TipoMoneda monedaCompra,
        TipoCambio tipoCambio,
        FormulaPrecio formula)
    {
        // 1. Convertir precio entrada a moneda base (BOB) si está en USD
        var precioEntradaBob = monedaCompra == TipoMoneda.USD
            ? precioEntrada * tipoCambio.UsdABob
            : precioEntrada;
        
        // 2. Calcular cada tipo de precio según fórmulas
        var precioSalida = AplicarFormula(precioEntradaBob, formula.PrecioSalida, tipoCambio);
        var precioVenta = AplicarFormula(precioEntradaBob, formula.PrecioVenta, tipoCambio);
        var precioRebaja = AplicarFormula(precioEntradaBob, formula.PrecioRebaja, tipoCambio);
        var precioMinimo = AplicarFormula(precioEntradaBob, formula.PrecioMinimo, tipoCambio);
        
        // 3. Validaciones
        if (precioVenta < precioMinimo)
            throw new DomainException("Precio de venta no puede ser menor que precio mínimo");
        
        return new PrecioProducto
        {
            ProductoId = producto.Id,
            MonedaBase = TipoMoneda.BOB,
            PrecioEntrada = precioEntradaBob,
            PrecioSalida = precioSalida,
            PrecioVenta = precioVenta,
            PrecioRebaja = precioRebaja,
            PrecioMinimo = precioMinimo,
            TipoCambioAplicadoId = tipoCambio.Id,
            FechaCalculo = DateTime.UtcNow
        };
    }
    
    private decimal AplicarFormula(decimal precioBase, ConfiguracionPrecio config, TipoCambio tc)
    {
        var resultado = precioBase;
        
        // Aplicar tipo de cambio si es necesario
        if (config.AplicarTipoCambio)
        {
            resultado = config.TipoCambioTipo == TipoCambioTipo.Actual
                ? resultado  // Ya está en BOB con TC actual
                : resultado;  // Mantener TC de compra
        }
        
        // Aplicar operaciones
        foreach (var operacion in config.Operaciones)
        {
            resultado = operacion.Tipo switch
            {
                TipoOperacion.Multiplicar => resultado * operacion.Valor,
                TipoOperacion.Dividir => resultado / operacion.Valor,
                TipoOperacion.Sumar => resultado + operacion.Valor,
                TipoOperacion.Restar => resultado - operacion.Valor,
                _ => resultado
            };
        }
        
        // Aplicar margen si existe
        if (config.Margen.HasValue)
        {
            resultado *= (1 + config.Margen.Value);
        }
        
        return Math.Round(resultado, 2);
    }
}
```

#### GeneradorSKU.cs
```csharp
public class GeneradorSKU
{
    private readonly IProductoRepository _productoRepository;
    
    public GeneradorSKU(IProductoRepository productoRepository)
    {
        _productoRepository = productoRepository;
    }
    
    public async Task<SKU> GenerarUnicoAsync(string categoria, string nombre, string? variante)
    {
        var skuBase = SKU.Generar(categoria, nombre, variante);
        
        // Verificar unicidad
        var existe = await _productoRepository.ExisteSkuAsync(skuBase.Valor);
        
        if (!existe)
            return skuBase;
        
        // Si existe, agregar sufijo numérico
        int sufijo = 1;
        SKU skuUnico;
        do
        {
            skuUnico = new SKU($"{skuBase.Valor}-{sufijo}");
            existe = await _productoRepository.ExisteSkuAsync(skuUnico.Valor);
            sufijo++;
        } while (existe && sufijo < 100);
        
        if (sufijo >= 100)
            throw new DomainException("No se pudo generar SKU único después de 100 intentos");
        
        return skuUnico;
    }
}
```

---

## 4. API Design (RESTful Endpoints)

### 4.1 Convenciones de API

**Base URL:** `https://api.ferreteriaCS.com/api/v1`

**Autenticación:** JWT Bearer token en header
```
Authorization: Bearer <token>
```

**Formato de respuestas:**
```json
{
  "success": true,
  "data": { ... },
  "message": "Operación exitosa",
  "errors": []
}
```

**Códigos HTTP:**
- `200 OK`: Éxito
- `201 Created`: Recurso creado
- `400 Bad Request`: Validación fallida
- `401 Unauthorized`: No autenticado
- `403 Forbidden`: Sin permisos
- `404 Not Found`: Recurso no encontrado
- `500 Internal Server Error`: Error del servidor

### 4.2 Endpoints por Módulo

#### Autenticación

```http
POST /api/v1/auth/login
Content-Type: application/json

{
  "email": "admin@ferreteria.com",
  "password": "Password123!"
}

Response 200:
{
  "success": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "usuario": {
      "id": "uuid",
      "nombre": "Admin",
      "email": "admin@ferreteria.com",
      "rol": "Admin"
    },
    "expiresIn": 3600
  }
}
```

#### Productos

```http
GET /api/v1/productos?page=1&pageSize=20&categoria=uuid&busqueda=tornillo&stockBajo=true
Response 200:
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "uuid",
        "sku": "HERR-TORN-8MM",
        "nombre": "Tornillo Phillips 8mm",
        "categoria": "Herramientas",
        "stock": 50,
        "precioVenta": 15.50,
        "moneda": "BOB",
        "activo": true
      }
    ],
    "totalItems": 150,
    "currentPage": 1,
    "pageSize": 20
  }
}

POST /api/v1/productos
Authorization: Bearer <token>
{
  "nombre": "Tornillo Phillips 10mm",
  "descripcion": "Tornillo para madera",
  "categoriaId": "uuid",
  "codigoBarras": "7501234567890",
  "unidadMedida": "caja",
  "variante": "10mm",
  "stockMinimo": 20,
  "stockMaximo": 500
}

Response 201:
{
  "success": true,
  "data": {
    "id": "uuid",
    "sku": "HERR-TORN-10MM",  // auto-generado
    "nombre": "Tornillo Phillips 10mm",
    ...
  }
}

PUT /api/v1/productos/{id}
PATCH /api/v1/productos/{id}/proteger-recalculo
DELETE /api/v1/productos/{id}  // Soft delete (activo = false)
```

#### Inventario

```http
POST /api/v1/inventario/entrada
Authorization: Bearer <token>
{
  "productoId": "uuid",
  "cantidad": 100,
  "monedaCompra": "USD",
  "precioEntrada": 50.00,
  "proveedor": "Proveedor XYZ"
}

Response 201:
{
  "success": true,
  "data": {
    "movimientoId": "uuid",
    "stockActual": 150,
    "preciosCalculados": {
      "precioEntrada": 348.00,  // BOB (50 * 6.96)
      "precioSalida": 400.20,
      "precioVenta": 452.40,
      "precioRebaja": 407.16,
      "precioMinimo": 382.80
    }
  },
  "message": "Entrada registrada exitosamente"
}

POST /api/v1/inventario/ajuste
{
  "productoId": "uuid",
  "cantidad": -5,
  "motivo": "Producto dañado"
}

GET /api/v1/inventario/alertas-stock
Response: Lista de productos bajo stock mínimo
```

#### Tipo de Cambio

```http
GET /api/v1/tipo-cambio/actual
Response 200:
{
  "success": true,
  "data": {
    "fecha": "2026-10-15",
    "usdABob": 6.96,
    "bobAUsd": 0.1437,
    "esDeHoy": true
  }
}

POST /api/v1/tipo-cambio
Authorization: Bearer <token>
Roles: Admin, SuperAdmin
{
  "fecha": "2026-10-15",
  "usdABob": 6.98,
  "recalcularPrecios": true
}

Response 202:  // Accepted (proceso asíncrono)
{
  "success": true,
  "data": {
    "tipoCambioId": "uuid",
    "jobId": "recalculo-job-uuid",
    "estimatedTime": 30  // segundos
  },
  "message": "Tipo de cambio actualizado. Recálculo de precios en progreso."
}

GET /api/v1/tipo-cambio/recalculo/{jobId}/status
Response 200:
{
  "success": true,
  "data": {
    "status": "completed",  // pending, running, completed, failed
    "progress": 100,
    "productosAfectados": 234,
    "errores": []
  }
}
```

#### Punto de Venta (POS)

```http
POST /api/v1/ventas
Authorization: Bearer <token>
{
  "monedaPago": "BOB",
  "montoRecibido": 500.00,
  "lineas": [
    {
      "productoId": "uuid",
      "cantidad": 2
    },
    {
      "productoId": "uuid",
      "cantidad": 1
    }
  ]
}

Response 201:
{
  "success": true,
  "data": {
    "ventaId": "uuid",
    "numeroVenta": 1542,
    "total": 485.20,
    "cambio": 14.80,
    "ticketUrl": "/api/v1/ventas/1542/ticket"
  }
}

GET /api/v1/ventas/{numeroVenta}/ticket
Response: HTML del ticket o PDF
```

#### Reportes

```http
GET /api/v1/reportes/ventas?fechaInicio=2026-10-01&fechaFin=2026-10-15&cajero=uuid&moneda=BOB
Response 200:
{
  "success": true,
  "data": {
    "totalVentas": 234,
    "montoTotal": 125680.50,
    "moneda": "BOB",
    "ventasPorDia": [
      { "fecha": "2026-10-01", "cantidad": 15, "monto": 8540.30 },
      ...
    ],
    "productosMasVendidos": [
      { "sku": "HERR-TORN-8MM", "nombre": "...", "cantidadVendida": 450 }
    ]
  }
}

GET /api/v1/reportes/inventario/stock-bajo
GET /api/v1/reportes/inventario/valor-total
```

---

## 5. Implementación de Casos de Uso

### 5.1 Use Case: Procesar Venta

**Archivo:** `FerreteriaCS.Application/UseCases/Ventas/ProcesarVentaUseCase.cs`

```csharp
public class ProcesarVentaUseCase
{
    private readonly IProductoRepository _productoRepo;
    private readonly IVentaRepository _ventaRepo;
    private readonly IInventarioRepository _inventarioRepo;
    private readonly ITipoCambioRepository _tipoCambioRepo;
    private readonly IUnitOfWork _unitOfWork;
    
    public async Task<VentaDto> EjecutarAsync(ProcesarVentaCommand command)
    {
        // 1. Validar tipo de cambio actual
        var tipoCambio = await _tipoCambioRepo.ObtenerActualAsync()
            ?? throw new BusinessException("Tipo de cambio no actualizado");
        
        // 2. Obtener productos y validar stock
        var lineasVenta = new List<LineaVenta>();
        foreach (var linea in command.Lineas)
        {
            var producto = await _productoRepo.ObtenerPorIdAsync(linea.ProductoId)
                ?? throw new NotFoundException($"Producto {linea.ProductoId} no encontrado");
            
            var inventario = await _inventarioRepo.ObtenerPorProductoAsync(producto.Id)
                ?? throw new BusinessException($"Inventario no encontrado para {producto.Nombre}");
            
            // Validar stock disponible
            if (inventario.StockActual < linea.Cantidad)
                throw new BusinessException($"Stock insuficiente para {producto.Nombre}");
            
            // Obtener precio actual
            var precio = await _productoRepo.ObtenerPrecioAsync(producto.Id);
            var precioEnMonedaPago = ConvertirPrecio(precio.PrecioVenta, TipoMoneda.BOB, 
                                                      command.MonedaPago, tipoCambio);
            
            lineasVenta.Add(new LineaVenta
            {
                ProductoId = producto.Id,
                Nombre = producto.Nombre,
                Sku = producto.Sku.Valor,
                Cantidad = linea.Cantidad,
                PrecioUnitario = precioEnMonedaPago
            });
        }
        
        // 3. Crear venta (domain logic)
        var venta = Venta.Crear(
            command.UsuarioId,
            command.MonedaPago,
            lineasVenta,
            command.MontoRecibido
        );
        
        // 4. Reducir stock y registrar movimientos
        foreach (var linea in command.Lineas)
        {
            await _inventarioRepo.ReducirStockAsync(linea.ProductoId, linea.Cantidad);
            
            await _inventarioRepo.RegistrarMovimientoAsync(new MovimientoInventario
            {
                ProductoId = linea.ProductoId,
                TipoMovimiento = TipoMovimiento.Salida,
                Cantidad = -linea.Cantidad,
                UsuarioId = command.UsuarioId,
                Motivo = $"Venta #{venta.NumeroVenta}"
            });
        }
        
        // 5. Guardar venta
        await _ventaRepo.AgregarAsync(venta);
        await _unitOfWork.CommitAsync();
        
        return MapToDto(venta);
    }
}
```

### 5.2 Use Case: Recalcular Precios (Asíncrono)

```csharp
public class RecalcularPreciosUseCase
{
    private readonly IProductoRepository _productoRepo;
    private readonly ITipoCambioRepository _tipoCambioRepo;
    private readonly CalculadoraPrecios _calculadora;
    private readonly ILogger<RecalcularPreciosUseCase> _logger;
    
    public async Task<RecalculoResultado> EjecutarAsync(Guid tipoCambioId, Guid usuarioId)
    {
        var tipoCambio = await _tipoCambioRepo.ObtenerPorIdAsync(tipoCambioId)
            ?? throw new NotFoundException("Tipo de cambio no encontrado");
        
        // Obtener productos que permiten recalculo
        var productos = await _productoRepo.ObtenerRecalculablesAsync();
        
        var resultado = new RecalculoResultado
        {
            TipoCambioId = tipoCambioId,
            UsuarioId = usuarioId,
            TotalProductos = productos.Count,
            ProductosAfectados = 0,
            Errores = new List<string>()
        };
        
        foreach (var producto in productos)
        {
            try
            {
                // Obtener fórmula (producto o categoría)
                var formula = producto.FormulaPreciosOverride 
                    ?? await _productoRepo.ObtenerFormulaCategoria(producto.CategoriaId);
                
                if (formula == null)
                {
                    _logger.LogWarning($"Producto {producto.Sku} sin fórmula configurada");
                    continue;
                }
                
                // Obtener precio entrada actual
                var precioActual = await _productoRepo.ObtenerPrecioAsync(producto.Id);
                
                // Recalcular
                var precioNuevo = _calculadora.CalcularPrecios(
                    producto,
                    precioActual.PrecioEntrada,
                    TipoMoneda.BOB,
                    tipoCambio,
                    formula
                );
                
                await _productoRepo.ActualizarPrecioAsync(precioNuevo);
                resultado.ProductosAfectados++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error recalculando producto {producto.Sku}");
                resultado.Errores.Add($"{producto.Sku}: {ex.Message}");
            }
        }
        
        // Registrar log de recalculo
        await _tipoCambioRepo.RegistrarLogRecalculoAsync(resultado);
        
        return resultado;
    }
}
```

---

## 6. Seguridad y Autenticación

### 6.1 JWT Configuration

```csharp
// appsettings.json
{
  "Jwt": {
    "Key": "YourSecretKeyHere_MinimumLengthIs32Characters",
    "Issuer": "FerreteriaCS.API",
    "Audience": "FerreteriaCS.WebUI",
    "ExpireMinutes": 60
  }
}

// Program.cs - Configuración de JWT
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

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => 
        policy.RequireRole("Admin", "SuperAdmin"));
    
    options.AddPolicy("RequireInventarioRole", policy => 
        policy.RequireRole("Inventario", "Admin", "SuperAdmin"));
});
```

### 6.2 Autorización por Endpoints

```csharp
[Authorize(Roles = "Admin,SuperAdmin")]
[HttpPost("tipo-cambio")]
public async Task<ActionResult<ApiResponse<TipoCambioDto>>> ActualizarTipoCambio(...)

[Authorize(Roles = "Inventario,Admin,SuperAdmin")]
[HttpPost("inventario/entrada")]
public async Task<ActionResult<ApiResponse<MovimientoDto>>> RegistrarEntrada(...)

[Authorize(Roles = "Vendedor,Admin,SuperAdmin")]
[HttpPost("ventas")]
public async Task<ActionResult<ApiResponse<VentaDto>>> ProcesarVenta(...)
```

---

## 7. Performance y Optimizaciones

### 7.1 Índices de Base de Datos

```sql
-- Búsqueda full-text de productos
CREATE INDEX idx_productos_busqueda ON productos 
USING gin(to_tsvector('spanish', nombre || ' ' || COALESCE(descripcion, '')));

-- Productos con stock bajo (queries frecuentes)
CREATE INDEX idx_inventario_bajo_stock ON inventario(producto_id, stock_actual)
WHERE stock_actual < stock_minimo;

-- Ventas por fecha (reportes)
CREATE INDEX idx_ventas_fecha_desc ON ventas(fecha DESC);

-- Composite index para filtros comunes
CREATE INDEX idx_productos_categoria_activo ON productos(categoria_id, activo);
```

### 7.2 Caching Strategy

```csharp
// Tipo de cambio actual (cache 1 hora)
services.AddMemoryCache();

public class TipoCambioService
{
    private readonly IMemoryCache _cache;
    private const string CacheKey = "TipoCambioActual";
    
    public async Task<TipoCambio> ObtenerActualAsync()
    {
        return await _cache.GetOrCreateAsync(CacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
            return await _repository.ObtenerActualAsync();
        });
    }
    
    public void InvalidarCache()
    {
        _cache.Remove(CacheKey);
    }
}
```

### 7.3 Paginación

```csharp
public class PaginatedList<T>
{
    public List<T> Items { get; set; }
    public int TotalItems { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
    
    public static async Task<PaginatedList<T>> CrearAsync(
        IQueryable<T> source, int page, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return new PaginatedList<T>
        {
            Items = items,
            TotalItems = count,
            CurrentPage = page,
            PageSize = pageSize
        };
    }
}
```

---

## 8. Testing Strategy

### 8.1 Unit Tests (Domain Layer)

```csharp
public class ProductoTests
{
    [Fact]
    public void Crear_DebeGenerarProductoValido()
    {
        // Arrange
        var nombre = "Tornillo Phillips";
        var categoriaId = Guid.NewGuid();
        var unidad = UnidadMedida.Caja;
        
        // Act
        var producto = Producto.Crear(nombre, categoriaId, unidad);
        
        // Assert
        Assert.NotNull(producto);
        Assert.Equal(nombre, producto.Nombre);
        Assert.True(producto.Activo);
    }
    
    [Fact]
    public void Crear_ConNombreNulo_DebeLanzarExcepcion()
    {
        Assert.Throws<ArgumentNullException>(() => 
            Producto.Crear(null, Guid.NewGuid(), UnidadMedida.Unidad));
    }
}

public class VentaTests
{
    [Fact]
    public void Crear_ConMontoInsuficiente_DebeLanzarExcepcion()
    {
        // Arrange
        var lineas = new List<LineaVenta>
        {
            new() { ProductoId = Guid.NewGuid(), Cantidad = 1, PrecioUnitario = 100 }
        };
        
        // Act & Assert
        Assert.Throws<DomainException>(() =>
            Venta.Crear(Guid.NewGuid(), TipoMoneda.BOB, lineas, montoRecibido: 50));
    }
}
```

### 8.2 Integration Tests (API)

```csharp
public class VentasControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    
    [Fact]
    public async Task ProcesarVenta_ConDatosValidos_DebeRetornar201()
    {
        // Arrange
        var command = new ProcesarVentaCommand
        {
            MonedaPago = TipoMoneda.BOB,
            MontoRecibido = 500,
            Lineas = new List<LineaVentaDto>
            {
                new() { ProductoId = TestData.ProductoId, Cantidad = 2 }
            }
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/ventas", command);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<VentaDto>>();
        result.Success.Should().BeTrue();
        result.Data.NumeroVenta.Should().BeGreaterThan(0);
    }
}
```

---

## 9. Deployment y DevOps

### 9.1 Variables de Entorno

```bash
# .env.production
DATABASE_URL=postgresql://user:password@host:5432/ferreteria_db
JWT_SECRET_KEY=your-secret-key-here
JWT_EXPIRE_MINUTES=60
CORS_ORIGINS=https://ferreteria.vercel.app
ASPNETCORE_ENVIRONMENT=Production
```

### 9.2 Docker (Opcional para desarrollo local)

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/FerreteriaCS.API/FerreteriaCS.API.csproj", "FerreteriaCS.API/"]
RUN dotnet restore "FerreteriaCS.API/FerreteriaCS.API.csproj"
COPY . .
RUN dotnet build "src/FerreteriaCS.API/FerreteriaCS.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/FerreteriaCS.API/FerreteriaCS.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FerreteriaCS.API.dll"]
```

### 9.3 GitHub Actions CI/CD

```yaml
name: Deploy to Vercel

on:
  push:
    branches: [main]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'
      - name: Run tests
        run: dotnet test --logger "console;verbosity=detailed"
  
  deploy:
    needs: test
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Deploy Backend to Railway
        run: railway up --service backend
      - name: Deploy Frontend to Vercel
        run: vercel --prod --token=${{ secrets.VERCEL_TOKEN }}
```

---

**FIN DEL DOCUMENTO DE DISEÑO**
