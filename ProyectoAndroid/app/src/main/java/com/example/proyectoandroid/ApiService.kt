package com.example.proyectoandroid

import retrofit2.http.POST
import retrofit2.Call
import retrofit2.http.Body

//endpoint para el login
interface ApiService {
    @POST("login")
    fun login(@Body request: LoginRequest): Call<LoginResponse>
}