# Despliegue en Docker con deploy.bat

Requisitos
- Windows con Docker Desktop (Docker Compose v2)
- Puertos libres: 8080 (API), 8081 (Web), 1433 (SQL)

Pasos rápidos
- Abrir una terminal en la raíz del repo
- Ejecutar: `deploy.bat up`
- Esperar a que los contenedores arranquen
- Comprobar endpoints:
  - Web: http://localhost:8081
  - API: http://localhost:8080/health y http://localhost:8080/swagger

Comandos útiles
- `deploy.bat logs`  Ver logs en vivo
- `deploy.bat ps`    Ver estado de contenedores
- `deploy.bat restart` Reiniciar contenedores
- `deploy.bat down`  Detener y eliminar contenedores
- `deploy.bat reset` Detener, eliminar volúmenes y recrear todo (pierde datos de SQL)

Notas
- La primera construcción puede tardar varios minutos.
- Si SQL aparece "unhealthy": usar `deploy.bat reset`, verificar que 1433 no esté ocupado y disponer de ? 2 GB de RAM libre.
