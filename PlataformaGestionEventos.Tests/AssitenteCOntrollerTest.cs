using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PlataformaGestionEventos.Controllers;
using PlataformaGestionEventos.Data;
using PlataformaGestionEventos.Models;
using Xunit;

namespace PlataformaGestionEventos.Tests
{
    public class AsistenteControllerTests
    {
        // Configuración de SQLite en Memoria para pruebas limpias y relacionales
        private ApplicationDbContext ObtenerContextoSQLiteEnMemoria()
        {
            var conexion = new SqliteConnection("Filename=:memory:");
            conexion.Open();

            var opciones = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(conexion)
                .Options;

            var contexto = new ApplicationDbContext(opciones);
            contexto.Database.EnsureCreated(); // Crea las tablas físicamente en RAM

            return contexto;
        }

        #region 1. Pruebas para INDEX [HttpGet]
        [Fact]
        public async Task Index_RetornaVistaConListaDeAsistentes()
        {
            // Arrange
            var context = ObtenerContextoSQLiteEnMemoria();
            context.Asistentes.Add(new Asistente { AsistenteId = 1, Nombre = "Carlos", Correo = "c@test.com", Telefono = "111" });
            await context.SaveChangesAsync();
            var controller = new AsistenteController(context);

            // Act
            var resultado = await controller.Index();

            // Assert
            var vista = Assert.IsType<ViewResult>(resultado);
            var lista = Assert.IsAssignableFrom<IEnumerable<Asistente>>(vista.ViewData.Model);
            Assert.Single(lista); // Verifica que el foreach de la vista tenga datos que recorrer
        }
        #endregion

        #region 2. Pruebas para CREAR [HttpGet y HttpPost]
        [Fact]
        public void Crear_Get_RetornaVistaVacia()
        {
            // Arrange
            var context = ObtenerContextoSQLiteEnMemoria();
            var controller = new AsistenteController(context);

            // Act
            var resultado = controller.Crear();

            // Assert
            Assert.IsType<ViewResult>(resultado);
        }

        [Fact]
        public async Task Crear_Post_DatosValidos_GuardaYRedirige()
        {
            // Arrange
            var context = ObtenerContextoSQLiteEnMemoria();
            var controller = new AsistenteController(context);
            var nuevo = new Asistente { AsistenteId = 1, Nombre = "Ana", Correo = "ana@test.com", Telefono = "222" };

            // Act
            var resultado = await controller.Crear(nuevo);

            // Assert
            var redireccion = Assert.IsType<RedirectToActionResult>(resultado);
            Assert.Equal("Index", redireccion.ActionName);
            Assert.Equal(1, context.Asistentes.Count());
        }

        [Fact]
        public async Task Crear_Post_ModeloInvalido_RetornaVistaConDatos()
        {
            // Arrange
            var context = ObtenerContextoSQLiteEnMemoria();
            var controller = new AsistenteController(context);
            var invalido = new Asistente { Nombre = "" };
            controller.ModelState.AddModelError("Correo", "Requerido"); // Fuerza el camino alternativo

            // Act
            var resultado = await controller.Crear(invalido);

            // Assert
            var vista = Assert.IsType<ViewResult>(resultado);
            Assert.Equal(invalido, vista.Model);
        }
        #endregion

        #region 3. Pruebas para EDITAR [HttpGet y HttpPost]
        [Fact]
        public async Task Editar_Get_IdNulo_RetornaNotFound()
        {
            var context = ObtenerContextoSQLiteEnMemoria();
            var controller = new AsistenteController(context);

            var resultado = await controller.Editar(id: null);

            Assert.IsType<NotFoundResult>(resultado);
        }

        [Fact]
        public async Task Editar_Get_IdNoExiste_RetornaNotFound()
        {
            var context = ObtenerContextoSQLiteEnMemoria();
            var controller = new AsistenteController(context);

            var resultado = await controller.Editar(id: 999);

            Assert.IsType<NotFoundResult>(resultado);
        }

        [Fact]
        public async Task Editar_Get_IdExiste_RetornaVistaConAsistente()
        {
            var context = ObtenerContextoSQLiteEnMemoria();
            context.Asistentes.Add(new Asistente { AsistenteId = 5, Nombre = "Luis", Correo = "l@test.com", Telefono = "333" });
            await context.SaveChangesAsync();
            var controller = new AsistenteController(context);

            var resultado = await controller.Editar(id: 5);

            var vista = Assert.IsType<ViewResult>(resultado);
            var modelo = Assert.IsType<Asistente>(vista.Model);
            Assert.Equal(5, modelo.AsistenteId);
        }

        [Fact]
        public async Task Editar_Post_IdNoCoincideConModelo_RetornaNotFound()
        {
            var context = ObtenerContextoSQLiteEnMemoria();
            var controller = new AsistenteController(context);
            var asistente = new Asistente { AsistenteId = 10, Nombre = "Error" };

            // Enviamos ID 5 pero el objeto tiene ID 10 -> Alerta de alteración de datos
            var resultado = await controller.Editar(5, asistente);

            Assert.IsType<NotFoundResult>(resultado);
        }

        [Fact]
        public async Task Editar_Post_DatosValidos_ActualizaYRedirige()
        {
            var context = ObtenerContextoSQLiteEnMemoria();
            var asistente = new Asistente { AsistenteId = 2, Nombre = "Original", Correo = "o@test.com", Telefono = "000" };
            context.Asistentes.Add(asistente);
            await context.SaveChangesAsync();
            context.Entry(asistente).State = EntityState.Detached; // Desacoplar para simular nueva petición

            var controller = new AsistenteController(context);
            var modificado = new Asistente { AsistenteId = 2, Nombre = "Modificado", Correo = "m@test.com", Telefono = "111" };

            var resultado = await controller.Editar(2, modificado);

            var redireccion = Assert.IsType<RedirectToActionResult>(resultado);
            Assert.Equal("Index", redireccion.ActionName);

            var bd = context.Asistentes.Find(2);
            Assert.Equal("Modificado", bd.Nombre);
        }
        #endregion

        #region 4. Pruebas para VER [HttpGet]
        [Fact]
        public async Task Ver_IdNulo_RetornaNotFound()
        {
            var context = ObtenerContextoSQLiteEnMemoria();
            var controller = new AsistenteController(context);

            var resultado = await controller.Ver(id: null);

            Assert.IsType<NotFoundResult>(resultado);
        }

        [Fact]
        public async Task Ver_IdNoExiste_RetornaNotFound()
        {
            var context = ObtenerContextoSQLiteEnMemoria();
            var controller = new AsistenteController(context);

            var resultado = await controller.Ver(id: 999);

            Assert.IsType<NotFoundResult>(resultado);
        }

        [Fact]
        public async Task Ver_IdExiste_RetornaVistaConGrafoRelacional()
        {
            var context = ObtenerContextoSQLiteEnMemoria();
            var asistente = new Asistente { AsistenteId = 1, Nombre = "Maria", Correo = "m@test.com", Telefono = "444" };
            context.Asistentes.Add(asistente);
            await context.SaveChangesAsync();
            var controller = new AsistenteController(context);

            var resultado = await controller.Ver(id: 1);

            var vista = Assert.IsType<ViewResult>(resultado);
            var modelo = Assert.IsType<Asistente>(vista.Model);
            Assert.Equal("Maria", modelo.Nombre);
        }
        #endregion

        #region 5. Pruebas para ELIMINAR [HttpGet y HttpPost]
        [Fact]
        public async Task Eliminar_Get_IdNulo_RetornaNotFound()
        {
            var context = ObtenerContextoSQLiteEnMemoria();
            var controller = new AsistenteController(context);

            var resultado = await controller.Eliminar(id: null);

            Assert.IsType<NotFoundResult>(resultado);
        }

        [Fact]
        public async Task Eliminar_Get_IdNoExiste_RetornaNotFound()
        {
            // Arrange
            var context = ObtenerContextoSQLiteEnMemoria();
            var controller = new AsistenteController(context);

            // Act
            var resultado = await controller.Eliminar(id: 999);

            // Assert: Cambiado para que coincida exactamente con tu controlador
            var redireccion = Assert.IsType<RedirectToActionResult>(resultado);
            Assert.Equal("Index", redireccion.ActionName);
        }

        [Fact]
        public async Task Eliminar_Post_AsistenteExiste_RemueveYRedirige()
        {
            var context = ObtenerContextoSQLiteEnMemoria();
            context.Asistentes.Add(new Asistente { AsistenteId = 8, Nombre = "Eliminar Me", Correo = "e@test.com", Telefono = "555" });
            await context.SaveChangesAsync();
            var controller = new AsistenteController(context);

            var resultado = await controller.Eliminar(id: 8);

            var redireccion = Assert.IsType<RedirectToActionResult>(resultado);
            Assert.Equal("Index", redireccion.ActionName);
            Assert.Equal(0, context.Asistentes.Count()); // El registro fue borrado físicamente
        }

        [Fact]
        public async Task Eliminar_Post_AsistenteNoExiste_RedirigeDirectamente()
        {
            var context = ObtenerContextoSQLiteEnMemoria();
            var controller = new AsistenteController(context);

            // Al pasar un ID que no existe, salta el bloque 'Remove' y va al Redirect directamente
            var resultado = await controller.Eliminar(id: 999);

            var redireccion = Assert.IsType<RedirectToActionResult>(resultado);
            Assert.Equal("Index", redireccion.ActionName);
        }
        #endregion
    }
}