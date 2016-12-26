
-- Evento Cobro
Insert into Permiso (Nombre, Notas, EsABM, Activo)
values ('Evento Cobro', 'Permite acceder y modificar los eventos de cobro', 1, 1)

insert into PermisoConcedido (PermisoID, RolID, PuedeEntrar, PuedeEditar, PuedeBorrar) values (13, 1, 1, 1, 1)