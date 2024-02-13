import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { cineCreacionDTO } from '../cine';
import { Coordenada } from 'src/app/utilidades/mapa/coordenada';

@Component({
  selector: 'app-formulario-cine',
  templateUrl: './formulario-cine.component.html',
  styleUrls: ['./formulario-cine.component.css']
})
export class FormularioCineComponent implements OnInit {
  
  constructor(private formBuider : FormBuilder){}

  form : FormGroup;

  @Input()
  modelo: cineCreacionDTO;

  @Input()
  errores: string[] = [];

  @Output()
  guardarCambios : EventEmitter<cineCreacionDTO> = new EventEmitter<cineCreacionDTO>();

  coordenadaInicial: Coordenada[] = [];

  ngOnInit(): void {
    this.form = this.formBuider.group({
      nombre: ['',{validators: [Validators.required]}],
      latitud: ['',{validatos: [Validators.required]}],
      longitud: ['',{validators: [Validators.required]}],
    });

    if(this.modelo !== undefined){
      this.form.patchValue(this.modelo);
      this.coordenadaInicial.push({latitud: this.modelo.latitud, longitud: this.modelo.longitud});
    }
  }

  coordenadaSeleccionada(coordenada: Coordenada){
    this.form.patchValue(coordenada);
  }

  OnSubmit(){
    this.guardarCambios.emit(this.form.value);
  }
}
