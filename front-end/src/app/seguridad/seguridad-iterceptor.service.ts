import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { SeguridadService } from './seguridad.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SeguridadIterceptorService implements HttpInterceptor {

  constructor(private seguridadService: SeguridadService) { }
  
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = this.seguridadService.obtenerToken();

    if(token){
      req = req.clone({
        setHeaders: {Authorization: `Bearer ${token}`}
      })
    }

    return next.handle(req);
  }

}
