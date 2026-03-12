Barbería - Proyecto Final

Proyecto práctico de Algoritmos y Estructuras de Datos II
Instituto del Profesorado “Fco. de Paula Robles”

------------------------------------------------------------

Estructura del repo:

barberia/          # Backend C# WinForms
frontend/          # Frontend Angular
seed_bdbarberia.sql  # Script de la base de datos

------------------------------------------------------------

Funcionalidades:

- Tipos de Corte: CRUD, validaciones, no eliminar si está en ventas
- Clientes: CRUD, validaciones de documento, localidad y teléfono
- Ventas: Registrar ventas con cliente, tipo de corte, cantidad y precio
- Clientes por día: Grilla con cantidad de clientes por fecha y total

------------------------------------------------------------

Cómo ejecutar:

Backend: abrir barberia.slnx en Visual Studio y configurar Config.cs con MySQL
Base de datos: crear BdBarberia y ejecutar seed_bdbarberia.sql
Frontend: cd frontend → npm install → ng serve

------------------------------------------------------------
- Todas las validaciones exigidas por el práctico están implementadas
- El formulario de ventas permite registrar nuevas ventas correctamente
- Proyecto organizado para separar backend, frontend y base de datos
