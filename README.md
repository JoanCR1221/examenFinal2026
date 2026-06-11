# Backend — Módulo de Reporte de Fraudes (LABCIBE-UNA)

API pública en **.NET 8** con **Entity Framework Core** y **PostgreSQL (Supabase)** para registrar y consultar reportes de fraude desde el sitio de LABCIBE-UNA. No requiere autenticación: el módulo permite escritura y lectura pública de reportes.

> Proyecto basado en [LibrariesApiTest (rama `feature/supabase`)](https://github.com/AlexVillegasC/LibrariesApiTest/tree/feature/supabase). Los módulos de Libraries/Books y la autenticación JWT del proyecto base fueron eliminados por no ser necesarios para la solución final.

## Estructura

```
HackerRank1/
├── Controllers/FraudController.cs   # Endpoints públicos GET y POST
├── Services/FraudService.cs         # IFraudService/FraudService: lógica de inserción y consulta
├── Data/AppDbContext.cs             # DbContext con DbSet<Fraud>
├── Entities/Fraud.cs                # Entidad: Id, ImpostorDetails, ContactInfo, Comments, CreatedAt
├── DTO/FraudForm.cs                 # Modelo del formulario con validaciones
├── Migrations/                      # Migración CreateFraudsTable
└── Startup.cs                       # DI, DbContext, CORS, Swagger
scripts/create_frauds_table.sql      # Script SQL equivalente a la migración
```

## Endpoints

| Método | Ruta         | Descripción                              | Respuestas |
|--------|--------------|------------------------------------------|------------|
| GET    | `/api/fraud` | Lista todos los reportes (más recientes primero) | `200 OK` |
| POST   | `/api/fraud` | Registra un nuevo reporte                | `201 Created`, `400 Bad Request` |

Ejemplo de cuerpo para `POST /api/fraud`:

```json
{
  "impostorDetails": "Persona haciéndose pasar por funcionario de LABCIBE",
  "contactInfo": "+506 8888-8888 / falso@correo.com",
  "comments": "Solicitó datos bancarios por WhatsApp"
}
```

`impostorDetails` y `contactInfo` son obligatorios (también se rechazan envíos con solo espacios). `comments` es opcional. `id` y `createdAt` (UTC) los asigna el servidor.

Swagger UI disponible en `/swagger` (también en producción, para facilitar la verificación).

## Configuración de la base de datos (Supabase)

La cadena de conexión **no se versiona en el repositorio**. Configúrela por variable de entorno:

```
ConnectionStrings__DefaultConnection = Host=aws-0-us-east-1.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.<ref-del-proyecto>;Password=<su-password>;Ssl Mode=Require
```

(La cadena exacta se obtiene en Supabase → Project Settings → Database → Connection string → .NET.)

Para desarrollo local también puede usar user-secrets:

```bash
cd HackerRank1
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<cadena-de-supabase>"
```

La tabla `Frauds` se crea automáticamente al iniciar el API (`Database.Migrate()`). Alternativas manuales:

```bash
dotnet tool restore
dotnet ef database update --project HackerRank1
```

o ejecutar [scripts/create_frauds_table.sql](scripts/create_frauds_table.sql) en el SQL Editor de Supabase.

## Ejecución local

```bash
dotnet restore
dotnet build
dotnet run --project HackerRank1
```

El API queda en `http://localhost:5219` (y `https://localhost:7098`). Prueba rápida:

```bash
curl -X POST http://localhost:5219/api/fraud \
  -H "Content-Type: application/json" \
  -d '{"impostorDetails":"Prueba","contactInfo":"test@test.com","comments":"Caso de prueba"}'

curl http://localhost:5219/api/fraud
```

## CORS

Los orígenes permitidos se configuran en `Cors:AllowedOrigins` (por defecto `http://localhost:5173` para el Frontend con Vite). En el ambiente desplegado, agregue la URL del Frontend publicado con variables de entorno:

```
Cors__AllowedOrigins__0 = http://localhost:5173
Cors__AllowedOrigins__1 = https://<su-frontend>.netlify.app
```

## Despliegue (MonsterASP u equivalente)

1. Publicar el proyecto: `dotnet publish HackerRank1 -c Release`.
2. Subir el contenido de `publish/` al hosting.
3. Definir las variables de entorno `ConnectionStrings__DefaultConnection` (Supabase) y `Cors__AllowedOrigins__1` (URL del Frontend en Netlify).
4. Verificar `https://<su-api>/swagger` y el flujo completo: insertar un reporte (POST) y consultarlo (GET / página `/reportes` del Frontend).
