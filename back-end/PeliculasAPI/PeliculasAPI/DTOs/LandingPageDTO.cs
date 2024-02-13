using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeliculasAPI.DTOs
{
    public class LandingPageDTO
    {
        public List<PeliculaDTO> pelisEnCinesDTO { get; set; }
        public List<PeliculaDTO> pelisProximosEstrenosDTO { get; set; }
    }
}
