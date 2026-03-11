-- Script de datos iniciales para la base de datos BdBarberia
-- Ejecutar en MySQL: source seed_bdbarberia.sql; o ejecutar el contenido en el cliente MySQL

USE BdBarberia;

-- Localidades (al menos 3)
INSERT INTO Localidades (Localidad) VALUES
('Palermo'),
('Recoleta'),
('San Telmo');

-- Tipos de corte (al menos 3)
INSERT INTO TiposDeCorte (TipoDeCorte, PrecioServicio) VALUES
('Corte Clásico', 800.00),
('Degradado', 1200.00),
('Barba', 500.00);

-- Clientes (al menos 3)
-- Validaciones respetadas: TipoDoc en {DNI, CUIT, CUIL, PASAPORTE, CI, LE, LC}
-- Documento digits only: CUIT/CUIL = 11 dígitos; otros = 7-8 dígitos

INSERT INTO Clientes (TipoDoc, Documento, Apellido, Nombres, Domicilio, IdLocalidad, TelefonoFijo, TelefonoCelular, Facebook, Twitter) VALUES
('DNI', '12345678', 'Gomez', 'Juan', 'Calle Falsa 123', 1, '01112345678', '1551234567', 'juan.gomez', 'juang'),
('CUIT', '20301234567', 'Martinez', 'Ana', 'Av Siempreviva 742', 2, '', '1159876543', '', ''),
('DNI', '87654321', 'Perez', 'Luis', 'Calle Real 456', 3, '01198765432', '1565554444', 'luis.perez', 'luisp');

-- VentaDeServicios (crear registros en distintas fechas para que la consulta por día devuelva resultados)
-- Usamos NOW() y fechas relativas para tener datos en varios días
INSERT INTO VentaDeServicios (FechaVenta, IdCliente) VALUES
(NOW(), 1),
(NOW(), 2),
(NOW() - INTERVAL 1 DAY, 1),
(NOW() - INTERVAL 1 DAY, 3),
(NOW() - INTERVAL 2 DAY, 2);

-- Fin del script
