import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { CredencialesUsuarioDTO, RespuestaAutenticacionDTO, usuarioDTO } from './seguridad';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SeguridadService {
  obtenerPaginado(pagina: number, cantidadElementosAMostrar: any) {
    throw new Error('Method not implemented.');
  }

  constructor(private http : HttpClient) { }

  apiURL = environment.apiURL + 'cuentas';
  private readonly llaveToken = 'token';
  private readonly llaveExpiracion = 'token-expiracion';
  private readonly campoRol = 'role';

  obtenerUsuarios(pagina: number, recordsPorPagina: number):Observable<any>{
    let params = new HttpParams();

    params = params.append('pagina', pagina.toString());
    params = params.append('recordsPorPagina', recordsPorPagina.toString());

    return this.http.get<usuarioDTO[]>(`${this.apiURL}/listadousuarios`, 
      {observe: 'response', params});
  }

  hacerAdmin(usuarioId: string){
    const headers = new HttpHeaders('Content-Type: application/json');
    return this.http.post(`${this.apiURL}/HacerAdmin`, JSON.stringify(usuarioId), {headers});
  }

  removerAdmin(usuarioId: string){
    const headers = new HttpHeaders('Content-Type: application/json');
    return this.http.post(`${this.apiURL}/removerAdmin`, JSON.stringify(usuarioId), {headers});
  }

  registrar(credencialesDTO: CredencialesUsuarioDTO): Observable<RespuestaAutenticacionDTO>{
    return this.http.post<RespuestaAutenticacionDTO>(`${this.apiURL}/crear`, credencialesDTO);
  }

  login(credencialesDTO: CredencialesUsuarioDTO): Observable<RespuestaAutenticacionDTO>{
    return this.http.post<RespuestaAutenticacionDTO>(`${this.apiURL}/login`, credencialesDTO);
  }

  guardarToken(respAutenticacionDTO: RespuestaAutenticacionDTO){
    localStorage.setItem(this.llaveToken, respAutenticacionDTO.token);
    localStorage.setItem(this.llaveExpiracion, respAutenticacionDTO.expiracion.toString());
  }

  logout(){
    localStorage.removeItem(this.llaveToken);
    localStorage.removeItem(this.llaveExpiracion);
  }

  obtenerRol():string{
    return this.obtenerCampoJWT(this.campoRol);
  }

  estaLogeado():boolean{
    const token = localStorage.getItem(this.llaveToken);

    if(!token){ return false; }

    const expiracion = localStorage.getItem(this.llaveExpiracion);
    const expiracionFecha = new Date(expiracion);

    if(expiracionFecha <= new Date()){
      this.logout();
      return false;
    }

    return true;
  }

  obtenerCampoJWT(campo: string): string{
    const token = localStorage.getItem(this.llaveToken);
    
    if(!token){ 
      return ''; 
    }

    var dataToken = JSON.parse(atob(token.split('.')[1]));
    return dataToken[campo];
  }

  obtenerToken(){
    return localStorage.getItem(this.llaveToken);
  }
  




}
