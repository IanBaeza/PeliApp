import { Component, OnInit } from '@angular/core';
import { CredencialesUsuarioDTO } from '../seguridad';
import { SeguridadService } from '../seguridad.service';
import { parsearErroresAPI } from 'src/app/utilidades/utilidades';
import { Router } from '@angular/router';

@Component({
  selector: 'app-registro',
  templateUrl: './registro.component.html',
  styleUrls: ['./registro.component.css']
})
export class RegistroComponent implements OnInit {

  constructor(
    private seguridadService: SeguridadService,
    private router : Router
    ) { }

  errores: string[] = [];

  ngOnInit(): void {
  }

  registrar(credencialesDTO: CredencialesUsuarioDTO){
    this.seguridadService.registrar(credencialesDTO)
      .subscribe(respAutenticacionDTO =>{
        this.seguridadService.guardarToken(respAutenticacionDTO);
        this.router.navigate(['/']);
      }, errores => this.errores = parsearErroresAPI(errores));
  }

}
