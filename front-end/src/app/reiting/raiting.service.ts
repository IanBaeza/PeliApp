import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class RaitingService {

  constructor(private http: HttpClient) { }
  apiURL = environment.apiURL + 'rating';

  puntuar(peliculaId: number, puntuacion: number){
    return this.http.post(`${this.apiURL}`, {peliculaId, puntuacion});
  }

}
