## Requisitos Previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)
- [pnpm](https://pnpm.io/installation)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (local o remoto)
- [Expo Go](https://expo.dev/go) instalado en tu dispositivo móvil (o un emulador Android/iOS)

---

## Estructura del Proyecto

```
Prueba-CT/
├── Simbiosys.Api/          # Backend - API REST .NET 8 + Dapper
│   ├── Controllers/        # ProductosController, PedidosController
│   ├── Services/           # Lógica de negocio
│   ├── Repositories/       # Acceso a datos con Dapper
│   ├── Models/             # DTOs
│   ├── Exceptions/         # Excepciones de dominio
│   ├── schema.sql          # Script para crear la base de datos
│   └── Program.cs          # Configuración de la app
├── app-productos/          # Frontend - React Native (Expo SDK 57)
│   ├── src/app/            # Pantallas (Expo Router file-based)
│   ├── src/services/       # Cliente HTTP (axios)
│   ├── src/types/          # Interfaces TypeScript
│   └── .env                # Variables de entorno (no versionado)
└── README.md
```

---

## 1. Base de Datos

### Crear la base de datos y tablas

Ejecuta el script SQL incluido en el proyecto contra tu instancia de SQL Server:

```sql
-- Desde SQL Server Management Studio o sqlcmd:
-- Abrir y ejecutar: database/schema.sql
```

Este script crea:
- Base de datos `SimbiosysDB`
- Tablas: `Productos`, `Pedidos`, `DetallePedidos`
Ejecutar el SP: database/store_procedure.sql

---

## 2. Backend (Simbiosys.Api)

### Configurar la conexión a la base de datos

Edita `Simbiosys.Api/appsettings.json` y reemplaza el placeholder de password:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SimbiosysOrdersDB;User Id=sa;Password=TU_PASSWORD_AQUI;TrustServerCertificate=True;"
  }
}
```

### Ejecutar la API

```powershell
cd Simbiosys.Api
dotnet run --launch-profile http
```

La API estará disponible en: **http://0.0.0.0:5000**

- Swagger UI: http://localhost:5000/swagger
- GET productos: http://localhost:5000/api/v1/productos
- GET pedidos: http://localhost:5000/api/v1/pedidos
- POST pedidos: http://localhost:5000/api/v1/pedidos
- GET detalle: http://localhost:5000/api/v1/pedidos/{id}/detalle

### Verificar conectividad desde el celular

Abre el navegador de tu celular y visita:
```
http://<TU_IP_LOCAL>:5000/api/v1/productos
```

Si no carga, revisa:
1. Que la API esté corriendo con `0.0.0.0:5000` (no `localhost`)
2. Que el firewall de Windows permita el puerto 5000:
   ```powershell
   netsh advfirewall firewall add rule name="API Dev 5000" dir=in action=allow protocol=TCP localport=5000
   ```
3. Que el celular y la PC estén en la misma red WiFi

---

## 3. Frontend (app-productos)

### Instalar dependencias

```powershell
cd app-productos
pnpm install
```

### Configurar la URL de la API

Crea un archivo `.env` en la raíz de `app-productos/` (junto a `package.json`):

```env
EXPO_PUBLIC_API_URL=http://TU_IP_LOCAL:5000/api/v1
```

Reemplaza `TU_IP_LOCAL` con la IP de tu máquina en la red local. Para obtenerla:

```powershell
ipconfig
```

Busca la dirección IPv4 del adaptador WiFi (ej: `192.168.1.15`).

> **Importante:** Existe un archivo `.env.example` como referencia. No edites ese, copia su contenido a `.env` y ajusta la IP.

### Ejecutar la app

```powershell
cd app-productos
pnpm start
```

Esto abre el servidor de desarrollo de Expo. Luego:

- **Dispositivo físico:** Escanea el QR con Expo Go
- **Emulador Android:** Presiona `a` en la terminal
- **Emulador iOS:** Presiona `i` en la terminal

### Notas importantes

- Si cambias el `.env`, **debes reiniciar Expo** (Ctrl+C y `pnpm start --clear`) porque Metro cachea las variables de entorno al iniciar.
- Para emulador Android Studio, puedes usar `10.0.2.2` como IP (alias a localhost del host).
- Para iOS Simulator, `localhost` funciona directamente.

---

## Endpoints de la API

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/v1/productos` | Lista todos los productos |
| GET | `/api/v1/pedidos` | Lista todos los pedidos |
| GET | `/api/v1/pedidos/{id}/detalle` | Detalle de un pedido específico |
| POST | `/api/v1/pedidos` | Crea un nuevo pedido |

### Ejemplo de body para POST /api/v1/pedidos

```json
{
  "cliente": "Juan Pérez",
  "items": [
    { "productoId": 1, "cantidad": 2 },
    { "productoId": 3, "cantidad": 1 }
  ]
}
```

### Respuestas

- `201 Created` — Pedido creado exitosamente
- `400 Bad Request` — Validación fallida o stock insuficiente (`{ "message": "..." }`)
- `500 Internal Server Error` — Error inesperado

---

## Funcionalidades de la App Móvil

### Tab Carrito (Registro de Pedido)
- Lista de productos con paginación
- Modal para seleccionar producto y cantidad
- Validación de stock disponible
- Carrito con total en tiempo real
- Confirmación de pedido con nombre de cliente

### Tab Historial (Historial de Pedidos)
- Lista de pedidos con pull-to-refresh
- Se recarga automáticamente al entrar al tab
- Modal con detalle del pedido (productos, cantidades, subtotales)
- Manejo de estados vacío y error con reintento

---

## Tecnologías

| Componente | Stack |
|-----------|-------|
| Backend | .NET 8, ASP.NET Core, Dapper, SQL Server |
| Frontend | React Native, Expo SDK 57, TypeScript, Axios |
| Navegación | Expo Router (file-based) |
| Gestor de paquetes | pnpm |
