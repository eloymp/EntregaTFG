package com.example.proyectoandroid

import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory

//Singleton para hacer llamadas a la api
object RetrofitClient {
    private const val BASE_URL = "https://entregatfg.onrender.com/"

    val instance: ApiService by lazy { //Solo se ejecuta la priemera vez y luego se reutiliza
        val retrofit = Retrofit.Builder()
            .baseUrl(BASE_URL)
            .addConverterFactory(GsonConverterFactory.create()) //Convertidor json
            .build()

        retrofit.create(ApiService::class.java)
    }
}
