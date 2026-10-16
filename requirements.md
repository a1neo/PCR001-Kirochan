# Sistema de Gestión de Inventarios y POS - Ferretería CS

## 1. Visión General del Proyecto

### 1.1 Descripción
Sistema web de gestión de inventarios y punto de venta (POS) para ferretería, con soporte multimoneda (USD/BOB) y cálculo dinámico de precios basado en tipo de cambio y fórmulas personalizables.

### 1.2 Objetivos
- Gestionar inventario de productos con categorías y variantes
- Procesar ventas en punto de venta con múltiples monedas
- Calcular precios automáticamente según tipo de cambio y fórmulas definidas
- Controlar accesos por roles y permisos
- Generar alertas de stock mínimo

### 1.3 Alcance - Versión 1.0
- Una sucursal
- Ventas en efectivo únicamente
- Pagos en USD o BOB (cliente elige)
- Tickets/facturas propias del sistema (no fiscales)
- Reportes básicos de ventas y stock

### 1.4 Futuras Versiones (Roadmap)
- Múltiples sucursales
- Múltiples formas de pago (tarjeta, transferencia)
- Sistema de crédito a clientes
- OCR para captura de productos desde imágenes (nombre, código de barras, cantidad/medida)
- Reportes avanzados y analytics

---

## 2. Usuarios y Roles

### 2.1 Roles del Sistema

| Rol | Cantidad Estimada | Permisos Principales |
|-----|-------------------|---------------------|
| **Super Administrador** | 1 | Acceso total, configuración de fórmulas de precios, gestión de usuarios, modificación de estructura de campos |
| **Administrador** | 1-2 | Gestión de inventario, reportes, configuración de productos y categorías, tipo de cambio |
| **Encargado de Inventario** | 1 | Entrada de mercancía, ajustes de inventario, control de stock |
| **Vendedor/Cajero** | 2-3 | Procesar ventas en POS, consultar inventario (solo lectura) |

### 2.2 Casos de Uso por Rol

#### Super Administrador
- Crear/editar/eliminar usuarios y asignar roles
- Configurar fórmulas de precios (por categoría y producto)
- Modificar estructura de campos del sistema
- Acceso a todas las funcionalidades

#### Administrador
- Actualizar tipo de cambio diario (USD/BOB)
- Crear/editar categorías y productos
- Configurar márgenes de ganancia
- Consultar reportes de ventas y stock
- Gestionar alertas de stock mínimo

#### Encargado de Inventario
- Registrar entrada de mercancía (en USD o BOB)
- Realizar ajustes de inventario (mermas, devoluciones)
- Consultar niveles de stock
- Generar alertas de reabastecimiento

#### Vendedor/Cajero
- Procesar ventas (búsqueda de productos, agregar al carrito)
- Seleccionar moneda de pago (USD/BOB)
- Registrar pago en efectivo y calcular cambio
- Imprimir ticket de venta

---

## 3. Módulos Funcionales

### 3.1 Módulo de Autenticación y Autorización
**Prioridad:** Alta

#### Funcionalidades
- Login/Logout con usuario y contraseña
- Control de acceso basado en roles (RBAC)
- Sesiones con JWT o similar
- Recuperación de contraseña (versión futura)

#### Reglas de Negocio
- Usuarios únicos por correo/username
- Contraseñas seguras (mínimo 8 caracteres, mayúsculas, números)
- Super Administrador no puede ser eliminado

---

### 3.2 Módulo de Gestión de Productos e Inventario
**Prioridad:** Alta

#### 3.2.1 Entidades Principales

**Categoría**
- Nombre
- Descripción
- Fórmula de precios por defecto (heredable a productos)
- Estado (activo/inactivo)

**Producto**
- SKU (generado automáticamente: `CATEGORIA-NOMBRE-VARIANTE`)
- Nombre
- Descripción
- Categoría (relación)
- Código de barras (opcional)
- Unidad de medida (unidad, kg, litro, metro, caja, etc.)
- Variantes (tallas, medidas, colores)
- Estado (activo/inactivo)

**Inventario (por producto)**
- Stock actual
- Stock mínimo (alerta)
- Stock máximo
- Ubicación en bodega (opcional)
- Fecha de última entrada
- Lote/Fecha de vencimiento (si aplica)

#### 3.2.2 Funcionalidades

**Gestión de Categorías**
- CRUD de categorías
- Asignar fórmula de precios por categoría
- Listar productos por categoría

**Gestión de Productos**
- CRUD de productos
- Generación automática de SKU al crear producto
- Búsqueda por nombre, SKU, código de barras
- Filtros: categoría, estado, stock bajo mínimo
- Soporte para variantes (ej: Tornillo Phillips 8mm, 10mm, 12mm)
- Checkbox "No recalcular precios automáticamente"

**Entrada de Mercancía**
- Registrar compra de productos
- Moneda de compra (USD o BOB)
- Tipo de cambio del día de compra (capturado automáticamente)
- Precio de entrada (costo unitario en moneda original)
- Cantidad ingresada
- Fecha y hora de entrada
- Proveedor (opcional para v1)

**Ajustes de Inventario**
- Incrementar/decrementar stock manualmente
- Motivo del ajuste (merma, devolución, corrección, etc.)
- Registro de auditoría (quién, cuándo, por qué)

**Alertas de Stock**
- Notificación cuando stock < stock mínimo
- Dashboard con productos bajo stock mínimo

---

### 3.3 Módulo de Precios y Tipo de Cambio
**Prioridad:** Alta

#### 3.3.1 Gestión de Tipo de Cambio

**Entidad TipoCambio**
- Fecha
- USD a BOB (ejemplo: 1 USD = 6.96 BOB)
- BOB a USD (calculado automáticamente)
- Usuario que lo actualizó
- Hora de actualización

**Funcionalidades**
- Actualizar tipo de cambio diario (manual)
- Historial de tipos de cambio
- Validación: no permitir fechas futuras
- Uso del tipo de cambio actual para ventas

#### 3.3.2 Sistema de Fórmulas de Precios

**Tipos de Precios Calculados**
1. **Precio de Entrada**: Costo de compra (no se recalcula)
2. **Precio de Salida**: Calculado con fórmula
3. **Precio de Venta**: Calculado con fórmula
4. **Precio de Rebaja**: Calculado con fórmula (para promociones)
5. **Precio Mínimo**: Calculado con fórmula (límite de descuento)

**Variables Disponibles en Fórmulas**
- `PrecioEntrada`: Costo de compra en moneda original
- `TipoCambio`: Tipo de cambio del día de compra (si aplica)
- `TipoCambioActual`: Tipo de cambio actual (para recalcular)
- `Margen`: Porcentaje de ganancia (ej: 0.30 para 30%)

**Interfaz de Configuración de Fórmulas**
- Modo visual/builder (no texto libre)
- Componentes:
  - Precio base (seleccionar: Precio Entrada)
  - Aplicar tipo de cambio (sí/no, cuál)
  - Operaciones: multiplicar, dividir, sumar, restar
  - Agregar margen (porcentaje)
- Vista previa del resultado con ejemplo

**Niveles de Configuración**
1. **Por Categoría**: Fórmula por defecto heredada a todos los productos
2. **Por Producto**: Fórmula específica que sobrescribe la de categoría

**Recalculo de Precios**
- Trigger: Cambio de tipo de cambio
- Alcance: Todos los productos EXCEPTO:
  - Productos con checkbox "No recalcular" activado
  - Precio de Entrada (nunca se recalcula)
- Proceso:
  1. Leer tipo de cambio actual
  2. Para cada producto sin protección:
     - Aplicar fórmula de Precio de Salida
     - Aplicar fórmula de Precio de Venta
     - Aplicar fórmula de Precio de Rebaja
     - Aplicar fórmula de Precio Mínimo
  3. Registrar log de recalculo (fecha, productos afectados)

**Ejemplo de Fórmula**
```
Precio de Venta:
  - Base: Precio de Entrada ($100 USD)
  - Aplicar: Tipo de Cambio Actual (6.96)
  - Resultado: $696 BOB
  - Agregar Margen: 30%
  - PRECIO FINAL: $904.80 BOB
```

#### 3.3.3 Reglas de Negocio
- Un producto debe tener al menos Precio de Entrada y Precio de Venta configurados
- Precio de Venta >= Precio Mínimo
- Precio de Rebaja >= Precio Mínimo
- Tipo de cambio debe actualizarse diariamente (alerta si no se actualiza)
- Fórmulas inválidas deben bloquearse antes de guardar

---

### 3.4 Módulo de Punto de Venta (POS)
**Prioridad:** Alta

#### 3.4.1 Funcionalidades del POS

**Interfaz de Venta**
- Búsqueda rápida de productos (por nombre, SKU, código de barras)
- Agregar productos al carrito
- Modificar cantidad en carrito
- Eliminar productos del carrito
- Visualización del subtotal en tiempo real

**Selección de Moneda de Pago**
- Radio buttons o dropdown: USD / BOB
- Conversión automática del total según moneda seleccionada
- Uso del tipo de cambio actual

**Procesamiento de Pago (Efectivo)**
- Mostrar total a pagar en moneda seleccionada
- Input: Monto recibido
- Cálculo automático de cambio
- Validación: Monto recibido >= Total
- Botón "Completar Venta"

**Ticket de Venta**
- Número de venta (autoincremental)
- Fecha y hora
- Cajero (usuario logueado)
- Detalle de productos (nombre, cantidad, precio unitario, subtotal)
- Total
- Moneda de pago
- Monto recibido
- Cambio devuelto
- Opción de imprimir (o guardar PDF)

#### 3.4.2 Entidad Venta

**Venta (Cabecera)**
- ID / Número de venta
- Fecha y hora
- Usuario (cajero)
- Moneda de pago (USD/BOB)
- Subtotal
- Total
- Monto recibido
- Cambio devuelto
- Estado (completada, cancelada - para v2)

**DetalleLínea**
- Producto (relación)
- Cantidad vendida
- Precio unitario (snapshot del momento de venta)
- Subtotal de línea
- Moneda del precio (USD/BOB)

#### 3.4.3 Reglas de Negocio
- Una venta reduce automáticamente el stock
- No se puede vender si stock < cantidad solicitada
- Precio capturado en venta es snapshot (no afectado por futuros recalculos)
- Ventas completadas no son editables (para v1)
- Solo efectivo en v1 (tarjeta/crédito en v2)

---

### 3.5 Módulo de Reportes
**Prioridad:** Media

#### 3.5.1 Reportes Básicos (v1)

**Reporte de Ventas**
- Filtros: Fecha inicio, fecha fin, cajero, moneda
- Información:
  - Total de ventas (cantidad de transacciones)
  - Monto total vendido (por moneda)
  - Productos más vendidos (top 10)
  - Ventas por día/hora (gráfico básico)

**Reporte de Inventario**
- Productos con stock bajo mínimo
- Valor total del inventario (en BOB y USD)
- Productos sin movimiento (sin ventas en X días)

**Reporte de Movimientos de Inventario**
- Filtros: Fecha, tipo de movimiento (entrada, venta, ajuste)
- Historial de entradas y salidas por producto

#### 3.5.2 Dashboard Administrativo
- Cards resumen:
  - Ventas del día (BOB y USD)
  - Productos bajo stock mínimo (número)
  - Última actualización de tipo de cambio
- Gráficos simples (Chart.js o similar):
  - Ventas de la última semana
  - Categorías más vendidas

---

## 4. Arquitectura y Stack Tecnológico

### 4.1 Arquitectura: Domain-Driven Design (DDD) con Capas

```
┌─────────────────────────────────────────┐
│         Presentation Layer              │
│  (React SPA - UI Components)            │
└─────────────────────────────────────────┘
                  ▼
┌─────────────────────────────────────────┐
│         API Layer (ASP.NET Core)        │
│  (Controllers, Middleware, DTOs)        │
└─────────────────────────────────────────┘
                  ▼
┌─────────────────────────────────────────┐
│      Application Layer                  │
│  (Use Cases, Services, Interfaces)      │
└─────────────────────────────────────────┘
                  ▼
┌─────────────────────────────────────────┐
│         Domain Layer                    │
│  (Entities, Value Objects, Business     │
│   Logic, Domain Services)               │
└─────────────────────────────────────────┘
                  ▼
┌─────────────────────────────────────────┐
│      Infrastructure Layer               │
│  (EF Core, Repositories, External       │
│   Services, Database Context)           │
└─────────────────────────────────────────┘
                  ▼
┌─────────────────────────────────────────┐
│         PostgreSQL Database             │
└─────────────────────────────────────────┘
```

### 4.2 Stack Tecnológico

#### Frontend
- **Framework**: React 18+ con TypeScript
- **Build Tool**: Vite
- **UI Library**: Material-UI (MUI) o Ant Design
- **State Management**: Zustand o React Context + useReducer
- **API Client**: Axios con interceptores
- **Routing**: React Router v6
- **Forms**: React Hook Form + Zod para validación
- **Responsive**: Mobile-first, TailwindCSS para utilidades

#### Backend
- **Framework**: ASP.NET Core 8.0 (Web API)
- **ORM**: Entity Framework Core 8.0
- **Authentication**: JWT Bearer tokens
- **Validation**: FluentValidation
- **Mapping**: AutoMapper
- **API Documentation**: Swagger/OpenAPI

#### Base de Datos
- **DBMS**: PostgreSQL 15+ (Vercel Postgres o Supabase)
- **Migraciones**: EF Core Migrations
- **Justificación**:
  - Transacciones ACID críticas para POS
  - Tipos numéricos precisos (DECIMAL para dinero)
  - Soporte JSON para datos semiestructurados (futuro)
  - Escalabilidad a múltiples sucursales

#### Deployment
- **Frontend**: Vercel (static hosting)
- **Backend**: Vercel Serverless Functions o Railway/Render
- **Base de Datos**: Vercel Postgres / Supabase / Railway Postgres

### 4.3 Estructura de Proyecto (Arquitectura de Carpetas)

```
/FerreteriaCS
├── /src
│   ├── /FerreteriaCS.Domain           # Domain Layer
│   │   ├── /Entities
│   │   │   ├── Producto.cs
│   │   │   ├── Categoria.cs
│   │   │   ├── Venta.cs
│   │   │   ├── DetalleVenta.cs
│   │   │   ├── Usuario.cs
│   │   │   ├── TipoCambio.cs
│   │   │   ├── Inventario.cs
│   │   │   └── MovimientoInventario.cs
│   │   ├── /ValueObjects
│   │   │   ├── Moneda.cs
│   │   │   ├── Precio.cs
│   │   │   ├── SKU.cs
│   │   │   └── FormulaPrecio.cs
│   │   ├── /Enums
│   │   │   ├── Rol.cs
│   │   │   ├── TipoMoneda.cs
│   │   │   └── TipoMovimiento.cs
│   │   ├── /Interfaces
│   │   │   ├── IProductoRepository.cs
│   │   │   ├── IVentaRepository.cs
│   │   │   └── ITipoCambioRepository.cs
│   │   └── /Services (Domain Services)
│   │       ├── CalculadoraPrecios.cs
│   │       └── GeneradorSKU.cs
│   │
│   ├── /FerreteriaCS.Application      # Application Layer
│   │   ├── /UseCases
│   │   │   ├── /Productos
│   │   │   │   ├── CrearProductoUseCase.cs
│   │   │   │   ├── ActualizarProductoUseCase.cs
│   │   │   │   └── ListarProductosUseCase.cs
│   │   │   ├── /Ventas
│   │   │   │   ├── ProcesarVentaUseCase.cs
│   │   │   │   └── ConsultarVentasUseCase.cs
│   │   │   ├── /Inventario
│   │   │   │   ├── RegistrarEntradaUseCase.cs
│   │   │   │   └── AjustarInventarioUseCase.cs
│   │   │   └── /TipoCambio
│   │   │       ├── ActualizarTipoCambioUseCase.cs
│   │   │       └── RecalcularPreciosUseCase.cs
│   │   ├── /DTOs
│   │   │   ├── ProductoDto.cs
│   │   │   ├── VentaDto.cs
│   │   │   └── TipoCambioDto.cs
│   │   ├── /Mappers
│   │   │   └── AutoMapperProfile.cs
│   │   └── /Interfaces
│   │       └── IEmailService.cs (futuro)
│   │
│   ├── /FerreteriaCS.Infrastructure   # Infrastructure Layer
│   │   ├── /Data
│   │   │   ├── ApplicationDbContext.cs
│   │   │   └── /Configurations (Fluent API)
│   │   │       ├── ProductoConfiguration.cs
│   │   │       └── VentaConfiguration.cs
│   │   ├── /Repositories
│   │   │   ├── ProductoRepository.cs
│   │   │   ├── VentaRepository.cs
│   │   │   └── RepositoryBase.cs
│   │   ├── /Services
│   │   │   ├── JwtTokenService.cs
│   │   │   └── PasswordHasher.cs
│   │   └── /Migrations
│   │       └── (EF Core migrations)
│   │
│   ├── /FerreteriaCS.API              # API Layer
│   │   ├── /Controllers
│   │   │   ├── ProductosController.cs
│   │   │   ├── VentasController.cs
│   │   │   ├── InventarioController.cs
│   │   │   ├── TipoCambioController.cs
│   │   │   └── AuthController.cs
│   │   ├── /Middleware
│   │   │   ├── ErrorHandlingMiddleware.cs
│   │   │   └── JwtAuthMiddleware.cs
│   │   ├── /Filters
│   │   │   └── ValidationFilter.cs
│   │   ├── Program.cs
│   │   └── appsettings.json
│   │
│   └── /FerreteriaCS.WebUI            # Frontend (React)
│       ├── /src
│       │   ├── /components
│       │   │   ├── /common
│       │   │   ├── /productos
│       │   │   ├── /ventas
│       │   │   └── /inventario
│       │   ├── /pages
│       │   │   ├── LoginPage.tsx
│       │   │   ├── DashboardPage.tsx
│       │   │   ├── ProductosPage.tsx
│       │   │   ├── POSPage.tsx
│       │   │   └── ReportesPage.tsx
│       │   ├── /services
│       │   │   ├── api.ts
│       │   │   ├── authService.ts
│       │   │   └── productosService.ts
│       │   ├── /stores (Zustand)
│       │   │   ├── authStore.ts
│       │   │   ├── cartStore.ts
│       │   │   └── productsStore.ts
│       │   ├── /types
│       │   │   └── index.ts
│       │   ├── /utils
│       │   │   ├── formatters.ts
│       │   │   └── validators.ts
│       │   ├── App.tsx
│       │   └── main.tsx
│       ├── package.json
│       └── vite.config.ts
│
└── /tests
    ├── /FerreteriaCS.Domain.Tests
    ├── /FerreteriaCS.Application.Tests
    └── /FerreteriaCS.API.Tests
```

### 4.4 Patrones de Diseño a Aplicar

- **Repository Pattern**: Abstracción de acceso a datos
- **Unit of Work**: Transacciones atómicas (EF DbContext)
- **CQRS (Command Query Responsibility Segregation)**: Separación de comandos y consultas (simplificado en v1)
- **Specification Pattern**: Consultas complejas reutilizables (futuro)
- **Factory Pattern**: Generación de SKUs, creación de entidades complejas
- **Strategy Pattern**: Cálculo de precios con diferentes fórmulas

---

## 5. Modelo de Datos (Conceptual)

### 5.1 Entidades Principales

```
Usuario
├── Id (PK)
├── Nombre
├── Email (unique)
├── PasswordHash
├── Rol (enum: SuperAdmin, Admin, Inventario, Vendedor)
├── Activo (bool)
└── FechaCreacion

Categoria
├── Id (PK)
├── Nombre
├── Descripcion
├── FormulaPreciosJson (JSON)
└── Activo

Producto
├── Id (PK)
├── SKU (unique, auto-generado)
├── Nombre
├── Descripcion
├── CategoriaId (FK)
├── CodigoBarras
├── UnidadMedida
├── Variante (ej: "8mm", "rojo")
├── NoRecalcularPrecios (bool)
├── FormulaPreciosJson (JSON, nullable - usa de categoría si null)
└── Activo

Inventario
├── Id (PK)
├── ProductoId (FK, unique)
├── StockActual
├── StockMinimo
├── StockMaximo
├── Ubicacion
└── FechaUltimaActualizacion

MovimientoInventario
├── Id (PK)
├── ProductoId (FK)
├── TipoMovimiento (entrada, salida, ajuste)
├── Cantidad
├── MonedaCompra (USD/BOB, nullable)
├── PrecioEntrada (decimal, nullable - solo en entradas)
├── TipoCambioId (FK, nullable - del día de compra)
├── UsuarioId (FK)
├── Motivo
└── Fecha

PrecioProducto (tabla de precios calculados)
├── Id (PK)
├── ProductoId (FK)
├── MonedaBase (USD/BOB)
├── PrecioEntrada (decimal)
├── PrecioSalida (decimal)
├── PrecioVenta (decimal)
├── PrecioRebaja (decimal)
├── PrecioMinimo (decimal)
├── TipoCambioAplicadoId (FK, nullable)
└── FechaCalculo

TipoCambio
├── Id (PK)
├── Fecha (unique)
├── UsdABob (decimal)
├── BobAUsd (decimal, calculado)
├── UsuarioId (FK)
└── FechaActualizacion

Venta
├── Id (PK)
├── NumeroVenta (unique, auto-incremental)
├── Fecha
├── UsuarioId (FK - cajero)
├── MonedaPago (USD/BOB)
├── Subtotal (decimal)
├── Total (decimal)
├── MontoRecibido (decimal)
├── Cambio (decimal)
└── Estado (completada, cancelada)

DetalleVenta
├── Id (PK)
├── VentaId (FK)
├── ProductoId (FK)
├── NombreProducto (snapshot)
├── SKU (snapshot)
├── Cantidad
├── PrecioUnitario (snapshot del momento)
├── MonedaPrecio (USD/BOB)
└── Subtotal
```

### 5.2 Relaciones Clave

- `Categoria 1 → N Producto`
- `Producto 1 → 1 Inventario`
- `Producto 1 → N MovimientoInventario`
- `Producto 1 → 1 PrecioProducto` (relación 1:1 con último precio calculado)
- `Venta 1 → N DetalleVenta`
- `Venta N → 1 Usuario` (cajero)
- `TipoCambio 1 → N MovimientoInventario` (tipo de cambio del día de compra)

---

## 6. Casos de Uso Detallados

### 6.1 UC-001: Registrar Entrada de Mercancía

**Actor:** Encargado de Inventario

**Precondiciones:**
- Usuario autenticado con rol Inventario o superior
- Productos ya creados en el sistema

**Flujo Principal:**
1. Usuario accede a módulo "Entrada de Mercancía"
2. Busca y selecciona producto(s) a ingresar
3. Sistema muestra el tipo de cambio actual (USD/BOB)
4. Usuario ingresa:
   - Moneda de compra (USD o BOB)
   - Precio de entrada (costo unitario)
   - Cantidad
   - Proveedor (opcional)
5. Sistema captura tipo de cambio del día
6. Sistema calcula precios (Salida, Venta, Rebaja, Mínimo) usando fórmulas configuradas
7. Usuario confirma entrada
8. Sistema:
   - Incrementa stock en Inventario
   - Registra MovimientoInventario (tipo: entrada)
   - Guarda PrecioProducto calculado
9. Sistema muestra confirmación

**Flujos Alternativos:**
- 3a. Tipo de cambio no actualizado hoy → Sistema muestra alerta
- 5a. Producto sin fórmula configurada → Sistema usa fórmula de categoría
- 5b. Producto con "No recalcular" activo → Sistema mantiene precios existentes (excepto entrada)

---

### 6.2 UC-002: Procesar Venta en POS

**Actor:** Vendedor/Cajero

**Precondiciones:**
- Usuario autenticado con rol Vendedor o superior
- Productos con stock disponible

**Flujo Principal:**
1. Usuario accede al POS
2. Busca productos (por nombre, SKU o código de barras)
3. Agrega productos al carrito con cantidad
4. Sistema valida stock disponible para cada producto
5. Sistema muestra subtotal en BOB y USD (conversión automática)
6. Usuario selecciona moneda de pago (USD o BOB)
7. Sistema muestra total en moneda seleccionada
8. Usuario ingresa "Monto recibido"
9. Sistema calcula y muestra cambio
10. Usuario confirma venta
11. Sistema:
    - Crea registro de Venta y DetalleVenta
    - Reduce stock de productos vendidos
    - Registra MovimientoInventario (tipo: salida) para cada producto
12. Sistema genera ticket de venta
13. Usuario imprime/muestra ticket al cliente

**Flujos Alternativos:**
- 4a. Stock insuficiente → Sistema muestra error, no permite agregar
- 8a. Monto recibido < Total → Sistema muestra error, no permite confirmar
- 10a. Usuario cancela venta → Sistema limpia carrito, no registra nada

---

### 6.3 UC-003: Actualizar Tipo de Cambio y Recalcular Precios

**Actor:** Administrador

**Precondiciones:**
- Usuario autenticado con rol Admin o SuperAdmin
- Tipo de cambio del día aún no registrado (recomendado)

**Flujo Principal:**
1. Usuario accede a "Configuración > Tipo de Cambio"
2. Sistema muestra tipo de cambio actual y último registrado
3. Usuario ingresa nuevo tipo de cambio (USD a BOB, ej: 6.98)
4. Sistema calcula automáticamente BOB a USD (1/6.98)
5. Usuario confirma actualización
6. Sistema pregunta: "¿Recalcular precios de todos los productos?"
7. Usuario confirma recalculo
8. Sistema inicia proceso en background:
   - Para cada producto SIN checkbox "No recalcular":
     - Obtiene fórmulas (producto o categoría)
     - Recalcula Precio Salida, Venta, Rebaja, Mínimo
     - Actualiza PrecioProducto
     - Registra log de recalculo
9. Sistema muestra progreso y notifica al completar
10. Sistema genera reporte de productos recalculados

**Flujos Alternativos:**
- 6a. Usuario cancela recalculo → Solo actualiza tipo de cambio, no toca precios
- 8a. Producto tiene fórmula inválida → Sistema lo omite y lo reporta en log de errores

---

### 6.4 UC-004: Configurar Fórmula de Precios

**Actor:** Super Administrador

**Precondiciones:**
- Usuario autenticado con rol SuperAdmin
- Categoría o Producto ya creado

**Flujo Principal:**
1. Usuario accede a edición de Categoría o Producto
2. Usuario selecciona "Configurar Fórmulas de Precios"
3. Sistema muestra interfaz builder para cada tipo de precio:
   - Precio de Salida
   - Precio de Venta
   - Precio de Rebaja
   - Precio Mínimo
4. Para cada precio, usuario configura:
   - Precio base: [PrecioEntrada]
   - Aplicar tipo de cambio: [Sí/No] [TipoCambioCompra / TipoCambioActual]
   - Operaciones: [Multiplicar por X] [Dividir por Y] [Sumar Z]
   - Margen: [30%] (ejemplo)
5. Sistema muestra vista previa del cálculo con producto ejemplo
6. Usuario guarda configuración
7. Sistema serializa fórmulas a JSON y las almacena
8. Sistema pregunta: "¿Aplicar esta fórmula a productos existentes ahora?"
9. Usuario confirma → Sistema recalcula precios de productos afectados

**Flujos Alternativos:**
- 4a. Fórmula resulta en precio negativo o cero → Sistema muestra error, no permite guardar
- 5a. Precio Venta < Precio Mínimo → Sistema muestra advertencia

---

## 7. Reglas de Negocio Globales

### 7.1 Inventario
- RN-001: Stock no puede ser negativo
- RN-002: No se puede vender un producto con stock 0
- RN-003: Toda entrada/salida/ajuste debe tener motivo y usuario registrado
- RN-004: Stock mínimo debe ser >= 0 y <= Stock máximo

### 7.2 Precios
- RN-005: Precio de Venta debe ser >= Precio Mínimo
- RN-006: Precio de Rebaja debe ser >= Precio Mínimo y <= Precio de Venta
- RN-007: Precio de Entrada nunca se recalcula automáticamente
- RN-008: Productos con "No recalcular" activo solo actualizan Precio Entrada en nuevas compras
- RN-009: Fórmulas deben producir precios > 0

### 7.3 Tipo de Cambio
- RN-010: Solo puede haber un tipo de cambio por fecha
- RN-011: Tipo de cambio no puede ser 0 o negativo
- RN-012: Sistema debe alertar si tipo de cambio no se actualizó hoy
- RN-013: BOB a USD se calcula automáticamente (1 / USD_a_BOB)

### 7.4 Ventas
- RN-014: Monto recibido debe ser >= Total de venta
- RN-015: Una venta reduce stock automáticamente al completarse
- RN-016: Ventas completadas no son editables (v1)
- RN-017: Ticket debe capturar snapshot de precios (no referencias)

### 7.5 Seguridad
- RN-018: Solo SuperAdmin puede modificar estructura de campos y fórmulas
- RN-019: Solo Admin/SuperAdmin pueden actualizar tipo de cambio
- RN-020: Vendedores solo pueden leer inventario, no modificarlo
- RN-021: Todas las acciones críticas deben loguearse (auditoría)

---

## 8. Requisitos No Funcionales

### 8.1 Rendimiento
- **RNF-001**: Búsqueda de productos en POS debe responder en < 1 segundo
- **RNF-002**: Procesamiento de venta debe completarse en < 2 segundos
- **RNF-003**: Recalculo masivo de precios debe ser asíncrono y no bloquear UI
- **RNF-004**: Sistema debe soportar al menos 5 usuarios concurrentes

### 8.2 Usabilidad
- **RNF-005**: Interfaz responsive (mobile-first)
- **RNF-006**: Accesible desde computadoras y tablets/celulares
- **RNF-007**: UI en español (Bolivia)
- **RNF-008**: Flujo de POS optimizado para velocidad (mínimos clics)

### 8.3 Seguridad
- **RNF-009**: Autenticación con JWT, tokens con expiración
- **RNF-010**: Contraseñas hasheadas (bcrypt o similar)
- **RNF-011**: HTTPS obligatorio en producción
- **RNF-012**: Validación de inputs en backend (prevenir SQL injection, XSS)
- **RNF-013**: Control de acceso basado en roles (RBAC)

### 8.4 Disponibilidad
- **RNF-014**: Sistema debe estar disponible 99% del tiempo en horario laboral (8am-8pm)
- **RNF-015**: Backup automático de base de datos diario

### 8.5 Escalabilidad
- **RNF-016**: Arquitectura debe permitir agregar múltiples sucursales en v2
- **RNF-017**: Base de datos debe soportar crecimiento de inventario (10,000+ productos)

### 8.6 Mantenibilidad
- **RNF-018**: Código con arquitectura DDD en capas (separación de responsabilidades)
- **RNF-019**: Tests unitarios para lógica de negocio crítica (>= 70% cobertura)
- **RNF-020**: Documentación de API con Swagger/OpenAPI

---

## 9. Interfaz de Usuario (Wireframes / Mockups)

### 9.1 Páginas Principales

#### Login
- Campo usuario/email
- Campo contraseña
- Botón "Iniciar Sesión"
- Link "¿Olvidaste tu contraseña?" (v2)

#### Dashboard (Home)
- **Menú lateral:**
  - Inicio / Dashboard
  - Productos
  - Inventario
  - Punto de Venta (POS)
  - Reportes
  - Configuración (Admin+)
  - Cerrar Sesión
- **Cards resumen:**
  - Ventas del día (BOB / USD)
  - Productos bajo stock mínimo
  - Tipo de cambio actual
- **Gráfico:** Ventas de la última semana

#### Gestión de Productos
- **Tabla de productos:**
  - Columnas: SKU, Nombre, Categoría, Stock, Precio Venta (BOB), Estado
  - Filtros: Categoría, Stock bajo mínimo, Búsqueda
  - Botones: "Nuevo Producto", Editar, Eliminar (desactivar)
- **Modal Crear/Editar Producto:**
  - Nombre, Descripción, Categoría, Código de Barras
  - Unidad de Medida, Variante
  - Stock Mínimo, Stock Máximo
  - Checkbox "No recalcular precios automáticamente"
  - Botón "Configurar Fórmulas" (SuperAdmin)

#### Punto de Venta (POS)
- **Layout de 2 columnas:**
  - **Izquierda (60%):** Búsqueda y catálogo de productos
    - Input búsqueda (nombre/SKU/código barras)
    - Grid de productos (cards con imagen, nombre, precio, stock)
  - **Derecha (40%):** Carrito de venta
    - Lista de productos agregados (nombre, cantidad, subtotal)
    - Subtotal general (BOB y USD)
    - Radio buttons: Moneda de pago (USD / BOB)
    - Total en moneda seleccionada
    - Input: Monto recibido
    - Texto: Cambio calculado
    - Botón "Completar Venta" (grande, destacado)
    - Botón "Cancelar" (secundario)

#### Entrada de Mercancía
- **Formulario:**
  - Buscar y seleccionar producto (autocomplete)
  - Radio: Moneda de compra (USD / BOB)
  - Tipo de cambio actual (readonly, con indicador si es de hoy)
  - Precio de entrada (input decimal)
  - Cantidad (input número)
  - Proveedor (input texto, opcional)
  - Vista previa de precios calculados (Salida, Venta, Rebaja, Mínimo)
  - Botones: "Registrar Entrada", "Cancelar"

#### Configuración de Tipo de Cambio
- **Card actual:**
  - Último tipo de cambio registrado (fecha, USD→BOB, usuario)
- **Formulario actualización:**
  - Fecha (readonly, hoy)
  - USD a BOB (input decimal, requerido)
  - BOB a USD (calculado automáticamente, readonly)
  - Checkbox "Recalcular precios de productos"
  - Botón "Actualizar Tipo de Cambio"
- **Historial:** Tabla con fechas, valores, usuario

#### Configurador de Fórmulas (SuperAdmin)
- **Tabs:** Precio Salida | Precio Venta | Precio Rebaja | Precio Mínimo
- **Builder por tab:**
  - Dropdown: Precio base (Precio Entrada)
  - Checkbox: Aplicar tipo de cambio (con dropdown: Compra/Actual)
  - Campos dinámicos: Operaciones (× ÷ + -)
  - Input: Margen (%)
  - Panel de vista previa con cálculo en vivo
- **Botones:** "Guardar", "Cancelar", "Restablecer a valores de Categoría"

---

## 10. Plan de Desarrollo (Fases)

### Fase 1: Setup y Fundamentos (Semanas 1-2)
**Backend:**
- Configurar proyecto .NET con arquitectura en capas
- Configurar EF Core + PostgreSQL
- Implementar modelo de datos (Entities)
- Configurar JWT Authentication
- CRUD básico de Usuarios y Roles

**Frontend:**
- Setup React + TypeScript + Vite
- Configurar routing y layout base
- Implementar página de Login
- Configurar Axios con interceptores JWT
- Implementar Dashboard básico

**DevOps:**
- Configurar Git repository
- Setup CI/CD básico (GitHub Actions o similar)
- Deploy inicial a Vercel (frontend + backend serverless)

---

### Fase 2: Gestión de Productos e Inventario (Semanas 3-4)
**Backend:**
- CRUD de Categorías
- CRUD de Productos con generación de SKU
- Entidad Inventario y MovimientoInventario
- Repository e Use Cases de inventario
- API endpoints de productos e inventario

**Frontend:**
- Página de Gestión de Productos (tabla, filtros, modales)
- Página de Entrada de Mercancía
- Componentes de búsqueda y autocompletado
- Alertas de stock bajo mínimo

**Testing:**
- Unit tests de generación de SKU
- Unit tests de validaciones de producto

---

### Fase 3: Sistema de Precios y Tipo de Cambio (Semanas 5-6)
**Backend:**
- Entidad TipoCambio y PrecioProducto
- Domain Service: CalculadoraPrecios
- Configuración y parsing de fórmulas (JSON)
- Use Case: ActualizarTipoCambio + RecalcularPrecios (async)
- API endpoints de tipo de cambio

**Frontend:**
- Página de Configuración de Tipo de Cambio
- Interfaz Builder de Fórmulas de Precios (drag-drop o modal estructurado)
- Vista previa de cálculos en tiempo real
- Notificaciones de recalculo en progreso

**Testing:**
- Unit tests de CalculadoraPrecios con múltiples escenarios
- Integration tests de recalculo masivo

---

### Fase 4: Punto de Venta (POS) (Semanas 7-8)
**Backend:**
- Entidad Venta y DetalleVenta
- Use Case: ProcesarVenta (transacción atómica)
- Validación de stock en tiempo real
- Generación de número de venta autoincremental
- API endpoints de ventas

**Frontend:**
- Página de POS con layout de 2 columnas
- Carrito de compras (Zustand store)
- Búsqueda rápida de productos
- Cálculo de cambio en tiempo real
- Modal de confirmación de venta
- Generación de ticket (HTML para imprimir o PDF básico)

**Testing:**
- Unit tests de lógica de carrito
- Integration tests de procesamiento de venta
- Tests de validación de stock concurrente

---

### Fase 5: Reportes y Finalización (Semanas 9-10)
**Backend:**
- Queries optimizadas para reportes
- API endpoints de reportes (ventas, inventario, movimientos)
- Implementar paginación y filtros avanzados

**Frontend:**
- Página de Reportes con tabs
- Gráficos con Chart.js (ventas por día, categorías)
- Filtros de fecha, usuario, moneda
- Exportación a CSV/Excel básica (futuro)

**Testing:**
- Tests de performance de queries de reportes
- Tests de integración end-to-end (Playwright o Cypress)

**Deploy y Optimización:**
- Deploy a producción en Vercel
- Configurar variables de entorno
- Configurar backup de base de datos
- Documentación de API (Swagger)
- Manual de usuario básico

---

### Fase 6: Refinamiento y Producción (Semanas 11-12)
- Corrección de bugs identificados en testing
- Optimización de performance (indexación DB, lazy loading)
- Mejoras de UX según feedback
- Capacitación de usuarios
- Monitoreo de errores (Sentry o similar)
- Go-live y soporte post-lanzamiento

---

## 11. Riesgos y Mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
|--------|--------------|---------|------------|
| Complejidad del sistema de fórmulas | Media | Alto | Empezar con fórmulas simples, iterar según feedback |
| Inconsistencias en recalculo de precios | Media | Alto | Tests exhaustivos, logs detallados, rollback manual |
| Performance en recalculo masivo | Baja | Medio | Procesar en background con queue, mostrar progreso |
| Concurrencia en ventas (double-sell) | Media | Alto | Transacciones ACID, locks optimistas en EF Core |
| Deployment en Vercel con .NET | Baja | Medio | Evaluar Railway/Render como alternativa para backend |
| Adopción por usuarios no técnicos | Media | Alto | UI intuitiva, capacitación, tooltips y ayudas contextuales |

---

## 12. Criterios de Aceptación (Checklist de Lanzamiento)

### Funcionalidades Core
- [ ] Usuario puede autenticarse con rol asignado
- [ ] SuperAdmin puede crear/editar usuarios y roles
- [ ] Se pueden crear categorías con fórmulas de precios
- [ ] Se pueden crear productos con SKU auto-generado
- [ ] Encargado puede registrar entrada de mercancía en USD o BOB
- [ ] Sistema calcula automáticamente 4 tipos de precios según fórmulas
- [ ] Admin puede actualizar tipo de cambio diario
- [ ] Sistema recalcula precios masivamente (respetando "No recalcular")
- [ ] Vendedor puede buscar productos rápidamente en POS
- [ ] Vendedor puede procesar venta en efectivo (USD o BOB)
- [ ] Sistema calcula cambio correctamente
- [ ] Venta reduce stock automáticamente
- [ ] Se genera ticket de venta imprimible
- [ ] Dashboard muestra ventas del día y alertas de stock
- [ ] Reportes muestran ventas por período y productos bajo stock

### Calidad y Seguridad
- [ ] Tests unitarios con >= 70% cobertura en lógica de negocio
- [ ] Tests de integración para flujo de venta completo
- [ ] Validaciones de input en backend (protección SQL injection, XSS)
- [ ] Contraseñas hasheadas, JWT con expiración
- [ ] Logs de auditoría para acciones críticas
- [ ] Manejo de errores con mensajes amigables

### Deployment
- [ ] Frontend desplegado en Vercel, accesible desde web y móvil
- [ ] Backend desplegado y accesible (Vercel/Railway/Render)
- [ ] Base de datos PostgreSQL en producción con backups
- [ ] Variables de entorno configuradas correctamente
- [ ] HTTPS habilitado
- [ ] Documentación de API con Swagger disponible

### Usabilidad
- [ ] Interfaz responsive en móvil, tablet y desktop
- [ ] Flujo de POS optimizado (< 30 segundos por venta)
- [ ] Tooltips y ayudas contextuales en formularios complejos
- [ ] Manual de usuario básico entregado

---

## 13. Glosario

- **SKU (Stock Keeping Unit):** Código único que identifica cada producto/variante
- **POS (Point of Sale):** Punto de venta, sistema de caja
- **BOB:** Boliviano (moneda de Bolivia)
- **USD:** Dólar estadounidense
- **RBAC:** Role-Based Access Control (control de acceso basado en roles)
- **JWT:** JSON Web Token (token de autenticación)
- **ACID:** Atomicity, Consistency, Isolation, Durability (propiedades de transacciones)
- **ORM:** Object-Relational Mapping (mapeo objeto-relacional)
- **DDD:** Domain-Driven Design (diseño dirigido por dominio)
- **EF Core:** Entity Framework Core (ORM de .NET)
- **CQRS:** Command Query Responsibility Segregation

---

## 14. Anexos

### Anexo A: Ejemplo de Fórmula de Precios (JSON)

```json
{
  "precioSalida": {
    "base": "PrecioEntrada",
    "aplicarTipoCambio": true,
    "tipoCambioTipo": "Actual",
    "operaciones": [
      { "tipo": "multiplicar", "valor": 1.15 }
    ]
  },
  "precioVenta": {
    "base": "PrecioSalida",
    "aplicarTipoCambio": false,
    "operaciones": [
      { "tipo": "multiplicar", "valor": 1.30 }
    ],
    "margen": 0.30
  },
  "precioRebaja": {
    "base": "PrecioVenta",
    "aplicarTipoCambio": false,
    "operaciones": [
      { "tipo": "multiplicar", "valor": 0.90 }
    ]
  },
  "precioMinimo": {
    "base": "PrecioEntrada",
    "aplicarTipoCambio": true,
    "tipoCambioTipo": "Actual",
    "operaciones": [
      { "tipo": "multiplicar", "valor": 1.10 }
    ]
  }
}
```

### Anexo B: Flujo de Recalculo de Precios (Diagrama)

```
Administrador actualiza Tipo de Cambio
           ↓
Sistema guarda nuevo TipoCambio (con fecha de hoy)
           ↓
Sistema pregunta: "¿Recalcular precios?"
           ↓ (Sí)
Sistema obtiene todos los productos donde NoRecalcular = false
           ↓
Para cada producto:
  ├─ Leer fórmulas (producto o categoría)
  ├─ Obtener PrecioEntrada actual
  ├─ Calcular PrecioSalida con nuevo TipoCambio
  ├─ Calcular PrecioVenta
  ├─ Calcular PrecioRebaja
  ├─ Calcular PrecioMinimo
  ├─ Validar: PrecioVenta >= PrecioMinimo
  └─ Actualizar PrecioProducto + log
           ↓
Sistema genera reporte de productos recalculados
           ↓
Administrador recibe notificación de finalización
```

---

## 15. Contacto y Aprobaciones

**Propietario del Proyecto:** [Tu Nombre]  
**Fecha de Creación:** 2026-10-15  
**Versión del Documento:** 1.0  
**Estado:** ✅ En Revisión / Aprobado para Desarrollo

---

**Firmas de Aprobación:**

| Rol | Nombre | Firma | Fecha |
|-----|--------|-------|-------|
| Propietario | [Tu Nombre] | __________ | ______ |
| Desarrollador Lead | [Si aplica] | __________ | ______ |

---

**FIN DEL DOCUMENTO**
