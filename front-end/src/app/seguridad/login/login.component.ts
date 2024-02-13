import { Component, OnInit } from '@angular/core';
import { CredencialesUsuarioDTO, RespuestaAutenticacionDTO } from '../seguridad';
import { SeguridadService } from '../seguridad.service';
import { Router } from '@angular/router';
import { parsearErroresAPI } from 'src/app/utilidades/utilidades';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  constructor(
    private seguridadService: SeguridadService,
    private router: Router
    ) { }

  errores: string[] = [];

  ngOnInit(): void {
  }

  login(credencialesDTO: CredencialesUsuarioDTO){
    this.seguridadService.login(credencialesDTO)
      .subscribe( respAutentiacionDTO => {
        this.seguridadService.guardarToken(respAutentiacionDTO);
        this.router.navigate(['/']);
      }, errores => this.errores = parsearErroresAPI(errores));
  }

}
