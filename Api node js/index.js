//Imports
const express = require('express');
const mongoose = require('mongoose');
const cors = require('cors');
const crypto = require('crypto');

//Permite entrar a cualquiera
const app = express();
app.use(cors({
    origin: '*',
    methods: ['GET', 'POST', 'PUT', 'DELETE'],
    allowedHeaders: ['Content-Type', 'Authorization']
}));

const PORT = 3000;

app.listen(PORT, ()=>{
    console.log(`Servidor corriendo en http://localhost:${PORT}`);
});

app.use(express.json());

mongoose.connect('mongodb+srv://eloymp:YlA7Xgq7wr2GmQMB@tfgemp.ksxh21a.mongodb.net/gesem?retryWrites=true&w=majority&appName=TFGEMP', {
  useNewUrlParser: true,
  useUnifiedTopology: true
}).then(() => console.log("Conectado a MongoDB Atlas"))
  .catch(err => console.error("Error al conectar con MongoDB:", err));

// Esquema usuarios
const Usuario = mongoose.model('Usuario', new mongoose.Schema({
  email: {
    type: String,
    required: true,
    unique: true
  },
  contraseña_hasheada: {
    type: String,
    required: true
  },
  nombre: String,
  apellidos: String,
  rol: String,
  estado_cuenta: String,
  idioma: String,
  tema: String,
  ultimo_inicio_sesion: Date,
  descripcion_empresa: String,
  dirección: String,
  nif: String,
  nombre_empresa: String,
  teléfono: String,
  foto_perfil: Buffer
}, { strict: false }), 'usuarios');

// Esquema solicitudes
const Solicitud = mongoose.model('Solicitud', new mongoose.Schema({
  usuario_id: {
    type: mongoose.Schema.Types.ObjectId,
    ref: 'Usuario',
    required: true
  },
  tipo_servicio: String,
  subtipo: String,
  estado: String,
  fecha_creacion: Date,

  archivos_adjuntos: [
    {
      nombre: String,
      comentario: String,
      contenido: Buffer     // guarda el binario directamente
    }
  ],

  respuesta_admin: {
    comentario: String,
    archivos: [
      {
        nombre: String,
        comentario: String,
        contenido: Buffer
      }
    ]
  },

  detalles: {
    tipo_asistencia: String,
    descripcion_cliente: String,
    requiere_visita: Boolean,
    fecha_limite_entrega: Date,
    ubicacion_obra: String
  },

  fecha_actualizacion: Date
}, { strict: false }), 'solicitudes');


// Ruta de login
app.post('/login', async (req, res) => {
  const { email, contraseña } = req.body;

  //Buscar usuario
  const usuario = await Usuario.findOne({ email }).lean();
  if (!usuario) {
    return res
      .status(401)
      .json({ success: false, mensaje: "Usuario o contraseña incorrectos" });
  }

  //Comprobar contraseña
  const hashEntrada = sha256(contraseña.trim());
  if (hashEntrada !== usuario.contraseña_hasheada) {
    return res
      .status(401)
      .json({ success: false, mensaje: "Usuario o contraseña incorrectos" });
  }

  delete usuario.contraseña_hasheada;

  const solicitudes = await Solicitud.find({ usuario_id: usuario._id }).lean();

  //Devolver todo en una sola respuesta
  return res.json({
    success: true,
    mensaje: "Login correcto",
    usuario,        // devuelve el usuario
    solicitudes     // devuelve las solicitudes de ese usuario
  });
});

function sha256(texto) {
    return crypto.createHash('sha256').update(texto).digest('hex');
}