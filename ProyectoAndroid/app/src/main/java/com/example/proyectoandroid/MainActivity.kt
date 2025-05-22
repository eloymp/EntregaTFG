package com.example.proyectoandroid

import android.content.Intent
import android.os.Bundle
import android.widget.ImageButton
import android.widget.TableLayout
import android.widget.TableRow
import android.widget.TextView
import androidx.activity.enableEdgeToEdge
import androidx.appcompat.app.AppCompatActivity
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat

class MainActivity : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContentView(R.layout.activity_main)

        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main)) { v, insets ->
            val sb = insets.getInsets(WindowInsetsCompat.Type.systemBars())
            v.setPadding(sb.left, sb.top, sb.right, sb.bottom)
            insets
        }

        //region Mostrar todas las solicitudes
        // Recuperamos la lista de solicitudes (puede venir vacía)
        val solicitudes = intent
            .getParcelableArrayListExtra<Solicitud>("solicitudes")
            ?: emptyList()

        val table = findViewById<TableLayout>(R.id.tableSolicitud)

        // Añade un título de sección
        fun addSection(title: String) {
            val row = TableRow(this)
            val tv = TextView(this).apply {
                text = title
                setTypeface(null, android.graphics.Typeface.BOLD)
                setPadding(8, 24, 8, 8)
                setTextColor(android.graphics.Color.BLACK)
            }
            row.addView(tv)
            table.addView(row)
        }



        // Añade fila clave / valor
        fun addRow(key: String, value: String?) {
            val row = TableRow(this)
            val tvKey = TextView(this).apply {
                text = key
                setTypeface(null, android.graphics.Typeface.BOLD)
                setPadding(8, 8, 8, 8)
                setTextColor(android.graphics.Color.parseColor("#333333"))

            }
            val tvValue = TextView(this).apply {
                text = value ?: "-"
                setPadding(8, 8, 8, 8)
                setTextColor(android.graphics.Color.parseColor("#333333"))

            }
            row.addView(tvKey)
            row.addView(tvValue)
            table.addView(row)
        }

        // Si no hay solicitudes, mostramos un mensaje
        if (solicitudes.isEmpty()) {
            addSection("No hay solicitudes")
        } else {
            solicitudes.forEachIndexed { index, sol ->
                addSection("Solicitud ${index + 1}")
                addRow("Tipo de servicio", sol.tipo_servicio)
                addRow("Subtipo", sol.subtipo)
                addRow("Estado", sol.estado)
                addRow("Descripción", sol.detalles?.descripcion_cliente)
                addRow("Fecha de creación", sol.fecha_creacion)
                if (!sol.fecha_actualizacion.isNullOrEmpty()) {
                    addRow("Fecha de respuesta", sol.fecha_actualizacion)
                }
            }
        }
        //endregion

        //region Cerrar sesión
        val btnLogout = findViewById<ImageButton>(R.id.btnLogout)
        btnLogout.setOnClickListener {
            // Para limpiar credenciales al cerrar:
            // DbHelper(this).clear()

            // Volvemos a LoginActivity y limpiamos la pila
            val intent = Intent(this, LoginActivity::class.java).apply {
                flags = Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TASK
            }
            startActivity(intent)
            finish()
        }
        //endregion
    }
}