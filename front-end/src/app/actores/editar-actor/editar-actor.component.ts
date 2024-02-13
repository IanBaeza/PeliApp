import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { actorCreacionDTO, actorDTO } from '../actor';
import { ActoresService } from '../actores.service';
import { parsearErroresAPI } from 'src/app/utilidades/utilidades';

@Component({
  selector: 'app-editar-actor',
  templateUrl: './editar-actor.component.html',
  styleUrls: ['./editar-actor.component.css']
})
export class EditarActorComponent {

  constructor(
    private router : Router,
    private actorService: ActoresService,
    private activatedRoute: ActivatedRoute
  ){}

  modelo: actorDTO;
  errores : string [] = [];

  ngOnInit(): void{
    this.activatedRoute.params.subscribe((params) =>{
      this.actorService.obtenerPorId(params.id)
        .subscribe( actor => {
          this.modelo = actor;
          }, () => this.router.navigate(['/actores'])
        )
      }
    );
  }

  guardarCambios(actor: actorCreacionDTO){
    this.actorService.editar(this.modelo.id, actor)
      .subscribe(() => {
          this.router.navigate(['/actores']);
      },error => this.errores = parsearErroresAPI(error))
  }
}
