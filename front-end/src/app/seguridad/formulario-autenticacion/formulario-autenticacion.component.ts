import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CredencialesUsuarioDTO } from '../seguridad';

@Component({
  selector: 'app-formulario-autenticacion',
  templateUrl: './formulario-autenticacion.component.html',
  styleUrls: ['./formulario-autenticacion.component.css']
})
export class FormularioAutenticacionComponent implements OnInit {

  constructor(private formBuilder: FormBuilder) { }

  form: FormGroup;

  @Input()
  errores : string[] = [];
  @Input()
  accion: string;

  @Output()
  onSubmit: EventEmitter<CredencialesUsuarioDTO> = new EventEmitter<CredencialesUsuarioDTO>();

  ngOnInit(): void {
    this.form = this.formBuilder.group({
      email: ['',{validators: [Validators.required, Validators.email]}],
      password: ['',{validators: [Validators.required]}] 
    });
  }

  obtenerMensajeError(){
    var campo = this.form.get('email');

    if(campo.hasError('required')){
      return 'El campo Email es requerido';
    }

    if(campo.hasError('email')){
      return 'El email no es válido';
    }
    
    return '';
  }

}
