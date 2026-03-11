Proyecto Angular minimal para frontend del sistema de barbería.

Requisitos:
- Node.js (16+ recomendado)
- Angular CLI (opcional)

Instrucciones rápidas:
1) Desde la carpeta `frontend` ejecutar `npm install`.
2) Ejecutar `npm start` para levantar la aplicación en modo desarrollo.

Notas:
- El servicio Angular espera una API REST en `http://localhost:5000/api` con endpoints:
  - GET/POST/PUT/DELETE `/api/tiposdecorte`
  - GET/POST/PUT/DELETE `/api/clientes`
  - GET `/api/localidades`
- Ajusta `ApiService.baseUrl` si tu backend corre en otro puerto o ruta.
