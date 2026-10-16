# Sistema POS Ferretería CS

Sistema web de gestión de inventarios y punto de venta para ferretería, con soporte multimoneda (USD/BOB) y cálculo dinámico de precios.

## 📚 Documentación

- **[Requirements](./requirements.md)** - Requerimientos funcionales y casos de uso
- **[Design](./design.md)** - Arquitectura técnica y diseño de base de datos  
- **[Tasks](./tasks.md)** - Plan de tareas y seguimiento de desarrollo

## 🛠️ Stack Tecnológico

### Backend
- ASP.NET Core 8.0
- Entity Framework Core
- PostgreSQL
- JWT Authentication

### Frontend
- React 18 + TypeScript
- Vite
- Material-UI / Ant Design
- Zustand / Redux Toolkit

### Deploy
- Vercel (Frontend + Database)
- Docker

## 🚀 Características Principales

- ✅ Multimoneda (USD/BOB) con tipo de cambio dinámico
- ✅ 5 tipos de precios calculados automáticamente
- ✅ Fórmulas de precios configurables por categoría/producto
- ✅ SKU auto-generado
- ✅ Sistema de roles y permisos (RBAC)
- ✅ POS con cálculo de cambio
- ✅ Alertas de stock mínimo
- ✅ Reportes de ventas e inventario

## 📦 Versión 1.0 (Alcance Inicial)

- Una sucursal
- Pagos en efectivo
- Usuarios locales (sin SSO)
- Reportes básicos

## 🔮 Roadmap Futuro

- OCR para captura de productos desde imágenes
- Múltiples sucursales
- Ventas a crédito
- Integración con proveedores

## 📄 Licencia

Privado - Ferretería CS

## 👤 Autor

**a1neo**
