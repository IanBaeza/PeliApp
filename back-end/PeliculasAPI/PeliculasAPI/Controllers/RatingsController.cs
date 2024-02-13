using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeliculasAPI.Controllers
{
    [Route("api/rating")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RatingsController: ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ApplicationDbContext context;

        public RatingsController(
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.context = context;
        }

        [HttpPost]
        
        public async Task<ActionResult> Post([FromBody] RatingDTO ratingDTO)
        {
            var email = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "email")
                                               .Value;

            var usuario = await userManager.FindByEmailAsync(email);
            var usuarioID = usuario.Id;

            var ratingActual = await context.Ratings.FirstOrDefaultAsync(
                                ratingEntity => ratingEntity.PeliculId == ratingDTO.PeliculaId &&
                                                ratingEntity.UsuarioId == usuarioID);

            if(ratingActual == null)
            {
                var rating = new Rating
                {
                    PeliculId = ratingDTO.PeliculaId,
                    Puntuacion = ratingDTO.Puntuacion,
                    UsuarioId = usuarioID
                };
                context.Add(rating);
            }
            else
            {
                ratingActual.Puntuacion = ratingDTO.Puntuacion;
            }

            await context.SaveChangesAsync();
            return NoContent();
        }

    }
}
