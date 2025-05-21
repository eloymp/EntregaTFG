using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using ProyectoFinalEMP.Models;
using ProyectoFinalEMP.Singleton;

namespace ProyectoFinalEMP.Controller
{
    public class DatabaseConnection
    {
        private IMongoDatabase _database;
        private string _connectionString = "mongodb+srv://eloymp:YlA7Xgq7wr2GmQMB@tfgemp.ksxh21a.mongodb.net/?retryWrites=true&w=majority&appName=TFGEMP";

        private MongoClient _client;

        #region Conectar
        public bool Conectar()
        {
            string databaseName = "gesem";
            try
            {
                _client = new MongoClient(_connectionString);
                _database = _client.GetDatabase(databaseName);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region Actualizar tema del usuario
        public async Task ActualizarTemaUsuario(ObjectId idUsuario, string nuevoTema)
        {
            var coleccion = _database.GetCollection<BsonDocument>("usuarios");

            var filtro = Builders<BsonDocument>.Filter.Eq("_id", idUsuario);
            var actualizacion = Builders<BsonDocument>.Update
                .Set("tema", nuevoTema);

            await coleccion.UpdateOneAsync(filtro, actualizacion);
        }
        #endregion

        #region Actualizar idioma del usuario
        public async Task ActualizarIdiomaUsuario(ObjectId idUsuario, string nuevoIdioma)
        {
            var coleccion = _database.GetCollection<BsonDocument>("usuarios");

            var filtro = Builders<BsonDocument>.Filter.Eq("_id", idUsuario);
            var actualizacion = Builders<BsonDocument>.Update
                .Set("idioma", nuevoIdioma);

            await coleccion.UpdateOneAsync(filtro, actualizacion);
        }
        #endregion

        #region Obtener tema e idioma del usuario
        public async Task<(string idioma, string tema)> ObtenerPreferenciasUsuario(ObjectId usuarioId)
        {
            var coleccion = _database.GetCollection<BsonDocument>("usuarios");
            var filtro = Builders<BsonDocument>.Filter.Eq("_id", usuarioId);
            var proyeccion = Builders<BsonDocument>.Projection.Include("idioma").Include("tema");

            var resultado = await coleccion.Find(filtro).Project(proyeccion).FirstOrDefaultAsync();

            if (resultado == null)
                return ("español", "oscuro"); // valores por defecto

            string idioma = resultado.Contains("idioma") ? resultado["idioma"].AsString : "español";
            string tema = resultado.Contains("tema") ? resultado["tema"].AsString : "oscuro";

            return (idioma, tema);
        }
        #endregion


        #region Validar usuario y devolver sus datos si existe
        public async Task<BsonDocument> ObtenerUsuario(string email, string contraseñaHasheada)
        {
            var coleccion = _database.GetCollection<BsonDocument>("usuarios");

            var filtro = Builders<BsonDocument>.Filter.Eq("email", email) &
                         Builders<BsonDocument>.Filter.Eq("contraseña_hasheada", contraseñaHasheada);

            return await coleccion.Find(filtro).FirstOrDefaultAsync();
        }


        #endregion

        #region Actualizar foto de perfil
        public async Task ActualizarFotoPerfil(string email, byte[] nuevaFoto)
        {
            var coleccion = _database.GetCollection<BsonDocument>("usuarios");

            var filtro = Builders<BsonDocument>.Filter.Eq("email", email);
            var actualizacion = Builders<BsonDocument>.Update.Set("foto_perfil", new BsonBinaryData(nuevaFoto));

            await coleccion.UpdateOneAsync(filtro, actualizacion);
        }
        #endregion

        #region Actualizar informacion del usuario
        public async Task ActualizarUsuario(string email, UpdateDefinition<BsonDocument> actualizacion)
        {
            var coleccion = _database.GetCollection<BsonDocument>("usuarios");
            var filtro = Builders<BsonDocument>.Filter.Eq("email", email);

            await coleccion.UpdateOneAsync(filtro, actualizacion);
        }
        #endregion

        #region Listar usuarios
        public async Task<List<BsonDocument>> ObtenerUsuarios()
        {
            var coleccion = _database.GetCollection<BsonDocument>("usuarios");

            var filtro = Builders<BsonDocument>.Filter.Eq("rol", "usuario");

            return await coleccion.Find(filtro).ToListAsync();
        }
        #endregion

        #region Insertar usuario
        public async Task InsertarUsuario(BsonDocument usuario)
        {
            var coleccion = _database.GetCollection<BsonDocument>("usuarios");
            await coleccion.InsertOneAsync(usuario);
        }
        #endregion

        #region Comprobar si ya existe un usuario por su email
        public async Task<bool> ExisteUsuario(string email)
        {
            var coleccion = _database.GetCollection<BsonDocument>("usuarios");
            var filtro = Builders<BsonDocument>.Filter.Eq("email", email);
            return await coleccion.Find(filtro).AnyAsync();
        }
        #endregion

        #region Obtener un usuario por su email
        public async Task<BsonDocument> ObtenerUsuarioPorEmail(string email)
        {
            var coleccion = _database.GetCollection<BsonDocument>("usuarios");
            var filtro = Builders<BsonDocument>.Filter.Eq("email", email);
            return await coleccion.Find(filtro).FirstOrDefaultAsync();
        }
        #endregion

        #region Eliminar usuario por email
        public async Task EliminarUsuario(string email)
        {
            var coleccion = _database.GetCollection<BsonDocument>("usuarios");
            var filtro = Builders<BsonDocument>.Filter.Eq("email", email);
            await coleccion.DeleteOneAsync(filtro);
        }
        #endregion

        #region Obtener todos los usuarios menos el superadmin
        public async Task<List<BsonDocument>> ObtenerUsuariosSinSuperadmin()
        {
            var filtro = Builders<BsonDocument>.Filter.Ne("rol", "superadministrador");
            var coleccion = _database.GetCollection<BsonDocument>("usuarios");
            return await coleccion.Find(filtro).ToListAsync();
        }
        #endregion

        #region Listar administradores
        public async Task<List<BsonDocument>> ObtenerAdministradores()
        {
            var coleccion = _database.GetCollection<BsonDocument>("usuarios");
            var filtro = Builders<BsonDocument>.Filter.Eq("rol", "administrador");

            return await coleccion.Find(filtro).ToListAsync();
        }
        #endregion

        #region Obtener email de un usuario por su usuarioId
        public async Task<string> ObtenerEmailUsuario(ObjectId usuarioId)
        {
            var coleccion = _database.GetCollection<BsonDocument>("usuarios");
            var filtro = Builders<BsonDocument>.Filter.Eq("_id", usuarioId);
            var usuario = await coleccion.Find(filtro).FirstOrDefaultAsync();

            return usuario != null && usuario.Contains("email") ? usuario["email"].AsString : "";
        }

        #endregion

        #region Eliminar solicitud por id
        public async Task EliminarSolicitud(ObjectId id)
        {
            var coleccion = _database.GetCollection<BsonDocument>("solicitudes");
            var filtro = Builders<BsonDocument>.Filter.Eq("_id", id);
            await coleccion.DeleteOneAsync(filtro);
        }
        #endregion

        #region Obtener las solicitudes de un usuario
        public async Task<List<BsonDocument>> ObtenerSolicitudesUsuario(ObjectId usuarioId, string estado)
        {
            var coleccion = _database.GetCollection<BsonDocument>("solicitudes");

            var filtro = Builders<BsonDocument>.Filter.Eq("usuario_id", usuarioId);

            if (estado != "todos")
                filtro &= Builders<BsonDocument>.Filter.Eq("estado", estado);

            var solicitudes = await coleccion.Find(filtro).SortByDescending(s => s["fecha_creacion"]).ToListAsync();
            return solicitudes;
        }
        #endregion

        #region Obtener todas las solicitudes
        public async Task<List<BsonDocument>> ObtenerTodasLasSolicitudes(string estado)
        {
            var coleccion = _database.GetCollection<BsonDocument>("solicitudes");

            FilterDefinition<BsonDocument> filtro = Builders<BsonDocument>.Filter.Empty;

            if (estado != "todos")
            {
                filtro = Builders<BsonDocument>.Filter.Eq("estado", estado);
            }

            return await coleccion.Find(filtro).ToListAsync();
        }
        #endregion

        #region Actualizar el estado de las solicitudes
        public async Task ActualizarEstadoSolicitud(ObjectId id, string nuevoEstado)
        {
            var coleccion = _database.GetCollection<BsonDocument>("solicitudes");
            var filtro = Builders<BsonDocument>.Filter.Eq("_id", id);
            var update = Builders<BsonDocument>.Update
                .Set("estado", nuevoEstado)
                .Set("fecha_actualizacion", DateTime.UtcNow);

            await coleccion.UpdateOneAsync(filtro, update);
        }
        #endregion

        #region Insertar Asistencia Tecnica
        public async Task InsertarSolicitudAsistenciaTecnica(
            ObjectId usuarioId,
            string tipoAsistencia,
            string descripcion,
            bool requiereVisita,
            DateTime fechaLimite,
            string ubicacion,
            List<ArchivoAdjunto> archivosAdjuntos)
        {
            var coleccion = _database.GetCollection<BsonDocument>("solicitudes");

            var nuevaSolicitud = new BsonDocument
        {
        { "usuario_id", usuarioId },
        { "tipo_servicio", "servicios_tecnicos" },
        { "subtipo", "asistencia_tecnica" },
        { "estado", "pendiente" },
        { "fecha_creacion", DateTime.UtcNow },

        // Archivos adjuntos enviados por el cliente
        { "archivos_adjuntos", new BsonArray(
            archivosAdjuntos.Select(a => new BsonDocument
            {
                { "nombre", a.Nombre },
                { "comentario", a.Comentario },
                { "contenido", new BsonBinaryData(a.Contenido) }
            })
        )},
        // Estructura vacía para que el admin la complete después
        { "respuesta_admin", new BsonDocument
            {
                { "nombre", "" },
                { "comentario", "" },
                { "archivos_adjuntos", new BsonArray() }
            }
        },

        // Detalles específicos de esta solicitud
        { "detalles", new BsonDocument
            {
                { "tipo_asistencia", tipoAsistencia },
                { "descripcion_cliente", descripcion },
                { "requiere_visita", requiereVisita },
                { "fecha_limite_entrega", fechaLimite },
                { "ubicacion_obra", ubicacion }
            }
        }
        };

            await coleccion.InsertOneAsync(nuevaSolicitud);
        }
        #endregion

        #region Insertar respuesta del administrador y finalizar solicitud
        public async Task InsertarRespuestaAdmin(ObjectId solicitudId, string comentario, List<ArchivoAdjunto> archivosAdjuntosAdmin)
        {
            var coleccion = _database.GetCollection<BsonDocument>("solicitudes");

            var respuestaAdmin = new BsonDocument
        {
        { "comentario", comentario },
        { "archivos", new BsonArray(
            archivosAdjuntosAdmin.Select(a => new BsonDocument
            {
                { "nombre", a.Nombre },
                { "comentario", a.Comentario },
                { "contenido", new BsonBinaryData(a.Contenido) }
            })
        )}
        };

            var update = Builders<BsonDocument>.Update
                .Set("estado", "finalizada")
                .Set("fecha_actualizacion", DateTime.UtcNow)
                .Set("respuesta_admin", respuestaAdmin);

            var filtro = Builders<BsonDocument>.Filter.Eq("_id", solicitudId);

            await coleccion.UpdateOneAsync(filtro, update);
        }
        #endregion

        #region Insertar Intermediación Asesor Fiscal
        public async Task InsertarSolicitudIntermediacionAsesorFiscal(
            ObjectId usuarioId,
            string descripcionCliente,
            string nombreAsesor,
            string correoAsesor,
            string telefonoAsesor,
            string tipoPeriodo,
            string trimestre,
            List<ArchivoAdjunto> archivosAdjuntos)
        {
            var coleccion = _database.GetCollection<BsonDocument>("solicitudes");

            var nuevaSolicitud = new BsonDocument
    {
        { "usuario_id", usuarioId },
        { "tipo_servicio", "asesoria_administrativa" },
        { "subtipo", "intermediacion_asesor_fiscal" },
        { "estado", "pendiente" },
        { "fecha_creacion", DateTime.UtcNow },
        { "fecha_actualizacion", DateTime.UtcNow },
        { "archivos_adjuntos", new BsonArray(
            archivosAdjuntos.Select(a => new BsonDocument
            {
                { "nombre", a.Nombre },
                { "comentario", a.Comentario },
                { "contenido", new BsonBinaryData(a.Contenido) }
            })
        )},
        { "comentarios", new BsonArray() },
        { "respuesta_admin", new BsonDocument() },
        { "detalles", new BsonDocument
            {
                { "descripcion_cliente", descripcionCliente },
                { "nombre_asesor", nombreAsesor },
                { "correo_asesor", correoAsesor },
                { "telefono_asesor", telefonoAsesor },
                { "tipo_periodo", tipoPeriodo },
                { "trimestre", trimestre ?? "" }
            }
        }
    };

            await coleccion.InsertOneAsync(nuevaSolicitud);
        }
        #endregion


    }
}