import { Component, OnInit, Input, Output,EventEmitter } from '@angular/core';
import { Icon, LeafletMouseEvent, Marker, icon, latLng, marker, tileLayer } from 'leaflet';
import { Coordenada, CoordenadaConMensaje } from './coordenada';

@Component({
  selector: 'app-mapa',
  templateUrl: './mapa.component.html',
  styleUrls: ['./mapa.component.css']
})
export class MapaComponent implements OnInit {

  constructor() { }

  @Input()
  coordenadasIniciales: CoordenadaConMensaje[] = [];

  @Input()
  soloLectura: boolean = false;

  @Output()
  coordenadaSeleccionada : EventEmitter<Coordenada> = new EventEmitter<Coordenada>();

  ngOnInit(): void {
    this.capas = this.coordenadasIniciales.map(coordenada =>{
        let marcador = marker([coordenada.latitud, coordenada.longitud],this.iconConfig);

        if(coordenada.mensaje){
          marcador.bindPopup(coordenada.mensaje, {autoClose: false, autoPan: false});
        }

        return marcador;
      } 
    );
  }

  iconConfig = {
    icon: icon({
      iconSize: [25,41],
      iconAnchor: [13,41],
      iconUrl: 'assets/marker-icon.png',
      iconRetinaUrl: 'assets/marker-icon-2x.png',
      shadowUrl: 'assets/marker-shadow.png'
    })
  };

  options = {
    layers: [
      tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', { maxZoom: 18, attribution: '...' })
    ],
    zoom: 11,
    center: latLng(-33.44630321069465, -70.64414978027345)
  }; //Me traigo el mapa de la librería, importo lo que haga falta

  capas : Marker<any>[] = [];

  manejarClick(event: LeafletMouseEvent){
    if(!this.soloLectura){
      const latitud = event.latlng.lat; //con el event me traigo la latitu y la longitud 
      const longitud = event.latlng.lng;
      console.log({latitud,longitud});
  
      this.capas = []; //con esto puedo configurar el marker para que me marque solo una coordenada
  
      this.capas.push(
        marker([latitud,longitud],this.iconConfig)
      );
  
      this.coordenadaSeleccionada.emit({latitud: latitud, longitud: longitud});
    }
  }

}
