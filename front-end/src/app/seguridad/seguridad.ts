export interface CredencialesUsuarioDTO{
    email: string;
    password: string;
}

export interface RespuestaAutenticacionDTO{
    token: string;
    expiracion: Date;
}

export interface usuarioDTO{
    id: string;
    email: string;
}