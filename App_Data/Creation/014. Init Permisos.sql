--Permisos iniciales del sistema y la asignacion al rol administrador.

-- Permiso total.
Insert into Permiso (Nombre, Notas, EsABM, Activo)
values ('Usuarios, roles y acceso', 'Permite toda la administración de la seguridad y el acceso a la aplicación', 1, 1)

insert into PermisoConcedido (PermisoID, RolID, PuedeEntrar, PuedeEditar, PuedeBorrar) values (1, 1, 1, 1, 1)

-- Permiso productos.
Insert into Permiso (Nombre, Notas, EsABM, Activo)
values ('Productos', 'Permite la gestión de los productos', 1, 1)

insert into PermisoConcedido (PermisoID, RolID, PuedeEntrar, PuedeEditar, PuedeBorrar) values (2, 1, 1, 1, 1)

-- Evento Envío
Insert into Permiso (Nombre, Notas, EsABM, Activo)
values ('Evento Envío', 'Permite la gestión de los envíos de cargamentos', 1, 1)

insert into PermisoConcedido (PermisoID, RolID, PuedeEntrar, PuedeEditar, PuedeBorrar) values (3, 1, 1, 1, 0)

-- Evento Recepción
Insert into Permiso (Nombre, Notas, EsABM, Activo)
values ('Evento Recepción', 'Permite la gestión de las recepciones de cargamentos', 1, 1)

insert into PermisoConcedido (PermisoID, RolID, PuedeEntrar, PuedeEditar, PuedeBorrar) values (4, 1, 1, 1, 1)

-- Evento Venta
Insert into Permiso (Nombre, Notas, EsABM, Activo)
values ('Evento Venta', 'Permite la gestión de las ventas de mercaderias de los cargamentos', 1, 1)

insert into PermisoConcedido (PermisoID, RolID, PuedeEntrar, PuedeEditar, PuedeBorrar) values (5, 1, 1, 1, 1)

-- Evento Pago
Insert into Permiso (Nombre, Notas, EsABM, Activo)
values ('Evento Pago', 'Permite la gestión de los pagos sobre los cargamentos', 1, 1)

insert into PermisoConcedido (PermisoID, RolID, PuedeEntrar, PuedeEditar, PuedeBorrar) values (6, 1, 1, 1, 1)


-- Permiso Cargamentos.
Insert into Permiso (Nombre, Notas, EsABM, Activo)
values ('Cargamentos', 'Permite la gestión de los Cargamentos', 1, 1)

insert into PermisoConcedido (PermisoID, RolID, PuedeEntrar, PuedeEditar, PuedeBorrar) values (7, 1, 1, 1, 1)

-- Evento Decomisación
Insert into Permiso (Nombre, Notas, EsABM, Activo)
values ('Evento Decomisación', 'Permite la gestión de las decomisaciones de mercadería sobre los cargamentos', 1, 1)

insert into PermisoConcedido (PermisoID, RolID, PuedeEntrar, PuedeEditar, PuedeBorrar) values (8, 1, 1, 1, 1)

-- Modificar Ayuda
Insert into Permiso (Nombre, Notas, EsABM, Activo)
values ('Modificar Ayuda', 'Permite cargar la información de ayuda a los usuarios', 0, 1)

insert into PermisoConcedido (PermisoID, RolID, PuedeEntrar, PuedeEditar, PuedeBorrar) values (9, 1, 1, 0, 0)

-- Liquidación
Insert into Permiso (Nombre, Notas, EsABM, Activo)
values ('Liquidación', 'Permite acceder a la liquidación', 0, 1)

insert into PermisoConcedido (PermisoID, RolID, PuedeEntrar, PuedeEditar, PuedeBorrar) values (10, 1, 1, 0, 0)

-- Modificar Incidentes
Insert into Permiso (Nombre, Notas, EsABM, Activo)
values ('Modificar cualquier incidente', 'Permite abrir o cerrar cualquier incidente como asi también editar incidentes de otras personas', 0, 1)

insert into PermisoConcedido (PermisoID, RolID, PuedeEntrar, PuedeEditar, PuedeBorrar) values (11, 1, 1, 0, 0)

-- Remanente
Insert into Permiso (Nombre, Notas, EsABM, Activo)
values ('Remanente', 'Permite acceder a las mercaderías remanentes del cargamento', 0, 1)

insert into PermisoConcedido (PermisoID, RolID, PuedeEntrar, PuedeEditar, PuedeBorrar) values (12, 1, 1, 0, 0)