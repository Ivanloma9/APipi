﻿using APIpi.Controllers.UsuarioController;
using APIpi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIpi.Controllers
{
    [ApiController] // Indica que esta clase es un controlador de API
    [Route("[controller]")] // Define la ruta del controlador
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger; // Logger para registrar información y errores
        private readonly AppDbContext contextDB; // Contexto de base de datos

        // Constructor que inicializa el logger y el contexto de base de datos
        public UserController(ILogger<UserController> logger, AppDbContext context)
        {
            _logger = logger;
            contextDB = context;
        }

        [HttpPost(Name = "PostUser")] // Maneja las solicitudes HTTP POST
        public async Task<ActionResult<PostUserResponse>> Post(PostUserRequest request)
        {
            // Crea un nuevo usuario a partir de la solicitud
            var usuario = new Usuario
            {
                Nombre = request.Nombre,
                Apellido = request.Apellido,
                Correo_Electrónico = request.Correo_Electrónico,
                Contraseña = request.Contraseña,
                Teléfono = request.Teléfono,
                Dirección = request.Dirección,
                Tipo = request.Tipo
            };
            // Agrega el nuevo usuario al contexto
            contextDB.Usuario.Add(usuario);
            // Guarda los cambios en la base de datos
            await contextDB.SaveChangesAsync();

            // Prepara la respuesta
            var response = new PostUserResponse
            {
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Correo_Electrónico = usuario.Correo_Electrónico,
                Teléfono = usuario.Teléfono,
                Dirección = usuario.Dirección,
                Tipo = usuario.Tipo
            };
            Console.WriteLine("Aqui");
            // Devuelve una respuesta 201 Created con la ubicación del nuevo recurso
            return CreatedAtAction(nameof(GetByCorreo), new { correoElectronico = usuario.Correo_Electrónico }, response);
        }

        [HttpGet("{correoElectronico}")] // Maneja las solicitudes HTTP GET con un parámetro id
        public async Task<ActionResult<GetUserResponse>> GetByCorreo(string correoElectronico)
        {
            // Registra información sobre la búsqueda del usuario
            _logger.LogInformation(
                $"Buscando usuario con correo electronico [{correoElectronico}] en la base de datos...");
            // Busca el usuario en la base de datos por ID
            var usuario = await contextDB.Usuario.FindAsync(correoElectronico);
            if (usuario == null)
            {
                // Si no se encuentra el usuario, registra una advertencia y devuelve 404 NotFound
                _logger.LogWarning($"No se econtro un usuario con el correo electronico [{correoElectronico}]");
                return NotFound();
            }

            // Registra información sobre el usuario encontrado
            _logger.LogInformation($"Se econtro un usuario con el correo electronico [{correoElectronico}]");
            // Prepara la respuesta
            var response = new GetUserResponse
            {
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Correo_Electrónico = usuario.Correo_Electrónico,
                Teléfono = usuario.Teléfono,
                Dirección = usuario.Dirección,
                Tipo = usuario.Tipo
            };

            // Devuelve la respuesta 200 OK con el usuario encontrado
            return Ok(response);
        }

        [HttpGet(Name = "GetAllUsers")] // Maneja las solicitudes HTTP GET para obtener todos los usuarios
        public async Task<ActionResult<IEnumerable<GetUserResponse>>> GetAll()
        {
            // Obtiene todos los usuarios de la base de datos
            var usuarios = await contextDB.Usuario.ToListAsync();
            // Prepara la respuesta
            var response = usuarios.Select(usuario => new GetUserResponse
            {
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Correo_Electrónico = usuario.Correo_Electrónico,
                Teléfono = usuario.Teléfono,
                Dirección = usuario.Dirección,
                Tipo = usuario.Tipo
            }).ToList();

            // Devuelve la respuesta 200 OK con la lista de usuarios
            return Ok(response);
        }

        [HttpPut("{correoElectronico}")] // Maneja las solicitudes HTTP PUT con un parámetro id
        public async Task<ActionResult<PutUserResponse>> Put(string correoElectronico, PutUserRequest request)
        {
            // Crea un nuevo objeto Usuario con los datos proporcionados
            var usuarioActualizado = new Usuario
            {
                Nombre = request.Nombre,
                Apellido = request.Apellido,
                Correo_Electrónico = correoElectronico,
                Teléfono = request.Teléfono,
                Dirección = request.Dirección,
                Tipo = request.Tipo
            };

            // Actualiza el usuario en el contexto
            contextDB.Usuario.Update(usuarioActualizado);
            // Guarda los cambios en la base de datos
            await contextDB.SaveChangesAsync();

            var response = new PutUserResponse
            {
                Nombre = usuarioActualizado.Nombre,
                Apellido = usuarioActualizado.Apellido,
                Correo_Electrónico = usuarioActualizado.Correo_Electrónico,
                Teléfono = usuarioActualizado.Teléfono,
                Dirección = usuarioActualizado.Dirección,
                Tipo = usuarioActualizado.Tipo
            };

            // Devuelve la respuesta 200 OK con el usuario actualizado
            return Ok(response);
        }

        [HttpDelete("{correoElectronico}")] // Maneja las solicitudes HTTP DELETE con un parámetro id
        public async Task<IActionResult> Delete(string correoElectronico)
        {
            // Busca el usuario en la base de datos por ID
            var usuario = await contextDB.Usuario.FindAsync(correoElectronico);
            if (usuario == null)
            {
                // Si no se encuentra el usuario, devuelve 404 NotFound
                return NotFound();
            }

            // Elimina el usuario del contexto
            contextDB.Usuario.Remove(usuario);
            // Guarda los cambios en la base de datos
            await contextDB.SaveChangesAsync();
            // Devuelve una respuesta 204 No Content
            return NoContent();
        }
    }
}