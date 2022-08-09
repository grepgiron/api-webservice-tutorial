using AuthServiceDavivienda.Context;
using AuthServiceDavivienda.Helper;
using AuthServiceDavivienda.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuthServiceDavivienda.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagementController : ControllerBase
    {
        private readonly userDbContext _context;

        public UserManagementController(userDbContext context)
        {
            _context = context;
        }
        [HttpPost("create-user")]
        public object RegistrarUsuario(Employed employed)
        {
            try
            {
                //Validar Contexto
                if (_context.Employeds == null)
                {
                    return NotFound();
                }
                //Validar Api-Key

                employed.CreatedAt = DateTime.Now;
                employed.UpdateAt = DateTime.Now;
                employed.State = 1; //Activo
                employed.EncryptedPass = Seguridad.EncodeHash(employed.EncryptedPass);
                employed.CurrentIp = HttpContext.Connection.RemoteIpAddress.ToString();

                _context.Employeds.Add(employed);
                _context.SaveChanges();


                return Ok(new
                {
                    message = "Se registro correctamente",
                    employed = employed
                });
            }
            catch (Exception)
            {
                return BadRequest(new
                {
                    message = "Error"
                });
            }
        }

        [HttpPost("assign-role")]
        public object AssignRole(UserRole userRole) 
        {
            try
            {
                //Validar
                if(_context.UserRoles == null)
                {
                    return NotFound();
                }

                if (!_context.Employeds.Any(c => c.Identification.Equals(userRole.UserIdentity.ToString()))) return NotFound();
                if (!_context.Roles.Any(c => c.Id.Equals(userRole.RoleId))) return NotFound();

                userRole.CreatedAt = DateTime.Now;
                userRole.UpdatedAt = DateTime.Now;
                
                _context.UserRoles.Add(userRole);
                _context.SaveChanges();

                return Ok( new
                {
                    message = "Role asignado",
                    userRole = userRole
                });


            }catch (Exception)
            {
                return BadRequest();
            }
            return Ok();
        }

        [HttpPost("login")]
        public object Login([Required][FromHeader] string Authorization)
        {

            string base64 = Authorization.Replace("Basic ", "");
            var encodeTextBytes = Convert.FromBase64String(base64);
            string textoPlano = Encoding.UTF8.GetString(encodeTextBytes);
            string[] authUser = textoPlano.Split(":");
            string user = authUser[0];
            string password = authUser[1];

            if(!_context.Employeds.Any(c => c.Email.Equals(user))) return BadRequest(new
            {
                user = user,
                password = password
            });
                
            Employed employed = _context.Employeds.Where(c => c.Email.Equals(user)).FirstOrDefault();
            string passDencoded = Seguridad.DecodeHash(employed.EncryptedPass);

            if(!password.Equals(passDencoded)) return Unauthorized();

            UserToken userToken = Seguridad.TokenGenerate(employed);

            UserToken uT = _context.UserTokens.Where(c => c.UserIdentity.Equals(userToken.UserIdentity)).FirstOrDefault();
                
            if(uT != null)
            {
                if (TokenValidation.tokenValidation(uT))
                {
                    return Ok(new
                    {
                        usuario = employed,
                        textoPlano = textoPlano,
                        token = uT.HashToken
                    });
                }
                else
                {
                    uT.LastToken = uT.CurrentToken;
                    uT.CurrentToken = DateTime.Now;
                    uT.HashToken = userToken.HashToken;

                    _context.Entry(uT).State = EntityState.Detached;
                    _context.Entry(uT).State = EntityState.Modified;
                    _context.SaveChanges();
                    return Ok(new
                    {
                        usuario = employed,
                        textoPlano = textoPlano,
                        token = uT.HashToken
                    });
                }
            }else
            {
                UserToken uToken = userToken;

                _context.UserTokens.Add(uToken);
                _context.SaveChanges();

                return Ok(new
                {
                    usuario = employed,
                    textoPlano = textoPlano,
                    token = uToken.HashToken
                });
            }
       
        }
    }
}
