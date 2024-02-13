import { AbstractControl, ValidatorFn } from "@angular/forms";


export function primeraLetraMayuscula():ValidatorFn{
    return(control: AbstractControl) => {
        const valor = <string> control.value;
        if (!valor) return;
        if(valor.length === 0) return;

        const primerLetra = valor[0];
        if(primerLetra !==primerLetra.toUpperCase()){
            return{
                primeraLetraMayuscula:{
                    mensaje : 'La primera letra debe ser mayuscula'
                }
            };
        }

        return;
    }
}