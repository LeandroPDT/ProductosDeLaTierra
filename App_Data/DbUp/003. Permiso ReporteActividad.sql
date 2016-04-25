
-- Reporte Actividad
Insert into Permiso (Nombre, Notas, EsABM, Activo)
values ('Reporte Actividad', 'Permite acceder a los reportes de actividad', 0, 1)

insert into PermisoConcedido (PermisoID, RolID, PuedeEntrar, PuedeEditar, PuedeBorrar) values (13, 1, 1, 0, 0)