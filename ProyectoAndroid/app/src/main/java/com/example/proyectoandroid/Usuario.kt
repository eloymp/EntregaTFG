package com.example.proyectoandroid

import com.google.gson.annotations.SerializedName

data class Usuario(
    @SerializedName("_id")
    val id: String,

    val email: String,

    val nombre: String?,

    val apellidos: String?,

    val rol: String?,

    @SerializedName("estado_cuenta")
    val estadoCuenta: String?,

    val idioma: String?,

    val tema: String?,

    @SerializedName("ultimo_inicio_sesion")
    val ultimoInicioSesion: String?,

    @SerializedName("descripcion_empresa")
    val descripcionEmpresa: String?,

    @SerializedName("dirección")
    val direccion: String?,

    val nif: String?,

    @SerializedName("nombre_empresa")
    val nombreEmpresa: String?,

    @SerializedName("teléfono")
    val telefono: String?,

    @SerializedName("foto_perfil")
    val fotoPerfil: String?
)