alter table Usuario add ProveedorID int null constraint FKUsuario_ProveedorID_UsuarioID FOREIGN KEY (ProveedorID) REFERENCES Usuario(UsuarioID)