package com.example.proyectoandroid

import android.content.ContentValues
import android.content.Context
import android.database.sqlite.SQLiteDatabase
import android.database.sqlite.SQLiteOpenHelper

// Clase de la bd
class DbHelper(context: Context) :
    SQLiteOpenHelper(context, "users.db", null, 2) {

    override fun onCreate(db: SQLiteDatabase) {
        db.execSQL("""
            CREATE TABLE usuarios (
                id INTEGER PRIMARY KEY,
                email TEXT,
                contraseña TEXT
            )
        """.trimIndent())
    }

    override fun onUpgrade(db: SQLiteDatabase, oldVersion: Int, newVersion: Int) {
        // al detectar versión antigua, borramos y recreamos
        db.execSQL("DROP TABLE IF EXISTS usuarios")
        onCreate(db)
    }



    fun save(email: String, contraseña: String) {
        writableDatabase.use { db ->
            val cv = ContentValues().apply {
                put("id", 1)               // siempre el mismo registro
                put("email", email)
                put("contraseña", contraseña)
            }
            // intenta actualizar,si no existe, lo inserta
            val updated = db.update("usuarios", cv, "id=1", null)
            if (updated == 0) {
                db.insert("usuarios", null, cv)
            }
        }
    }

    fun clear() {
        writableDatabase.use { db ->
            db.delete("usuarios", "id=1", null)
        }
    }

    fun load(): Pair<String, String>? {
        readableDatabase.use { db ->
            db.rawQuery("SELECT email, contraseña FROM usuarios WHERE id=1", null)
                .use { cur ->
                    return if (cur.moveToFirst()) {
                        cur.getString(0) to cur.getString(1)
                    } else null
                }
        }
    }
}