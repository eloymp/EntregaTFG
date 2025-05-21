package com.example.proyectoandroid

import android.os.Parcelable
import kotlinx.parcelize.Parcelize

// Solicitud.kt
@Parcelize
data class Solicitud(
    val _id: String,
    val usuario_id: String,
    val tipo_servicio: String?,
    val subtipo: String?,
    val estado: String?,
    val fecha_creacion: String?,
    val archivos_adjuntos: List<ArchivoAdjunto>?,
    val respuesta_admin: RespuestaAdmin?,
    val detalles: Detalles?,
    val fecha_actualizacion: String?
) : Parcelable

@Parcelize
data class ArchivoAdjunto(
    val nombre: String?,
    val comentario: String?,
    val contenido: String?
) : Parcelable

@Parcelize
data class RespuestaAdmin(
    val comentario: String?,
    val archivos: List<ArchivoAdjunto>?
) : Parcelable

@Parcelize
data class Detalles(
    val tipo_asistencia: String?,
    val descripcion_cliente: String?,
    val requiere_visita: Boolean?,
    val fecha_limite_entrega: String?,
    val ubicacion_obra: String?
) : Parcelable
