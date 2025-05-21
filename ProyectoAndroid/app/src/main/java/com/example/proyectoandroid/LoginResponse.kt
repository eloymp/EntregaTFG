package com.example.proyectoandroid

data class LoginResponse(
    val success: Boolean,
    val mensaje: String,
    val usuario: Usuario?,
    val solicitudes: List<Solicitud>? = null
)
