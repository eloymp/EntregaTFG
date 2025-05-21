package com.example.proyectoandroid

import android.content.Intent
import android.net.Uri
import android.os.Bundle
import android.view.View
import android.widget.ImageButton
import android.widget.TextView
import androidx.activity.enableEdgeToEdge
import androidx.appcompat.app.AppCompatActivity
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat

class AboutUsActivity : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContentView(R.layout.activity_about_us)
        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main)) { v, insets ->
            val systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars())
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom)
            insets
        }

        //region Menu inferior
        val btnHome = findViewById<ImageButton>(R.id.btnHome)
        val btnAbout = findViewById<ImageButton>(R.id.btnAbout)
        val btnFaq = findViewById<ImageButton>(R.id.btnFaq)

        btnHome.setOnClickListener {
            val intent = Intent(this, LoginActivity::class.java)
            startActivity(intent)
        }

        btnAbout.setOnClickListener {}

        btnFaq.setOnClickListener {
            val intent = Intent(this, FaqActivity::class.java)
            startActivity(intent)
        }
        //endregion

        //region Texto de los desplegables
        findViewById<TextView>(R.id.tituloAbout).text = getString(R.string.TitleAboutUs)
        findViewById<TextView>(R.id.titleIntro).text = "▾ " + getString(R.string.IntroAboutUs)
        findViewById<TextView>(R.id.contentIntro).text = getString(R.string.TextBlockAboutUs1)
        findViewById<TextView>(R.id.titleObj).text = "▾ " + getString(R.string.ObjetivesAboutUs)

        val contentObj = findViewById<TextView>(R.id.contentObj)
        contentObj.text = listOf(
            getString(R.string.TextBlockAboutUs2),
            getString(R.string.TextBlockAboutUs3),
            getString(R.string.TextBlockAboutUs4),
            getString(R.string.TextBlockAboutUs5),
            getString(R.string.TextBlockAboutUs6)
        ).joinToString("\n")
        //endregion

        //region Desplegables
        val introTitle = findViewById<TextView>(R.id.titleIntro)
        val introContent = findViewById<TextView>(R.id.contentIntro)

        introTitle.setOnClickListener {
            val isVisible = introContent.visibility == View.VISIBLE
            introContent.visibility = if (isVisible) View.GONE else View.VISIBLE
            introTitle.text = if (isVisible) "▾ Introducción" else "▴ Introducción"
        }

        val objTitle = findViewById<TextView>(R.id.titleObj)
        val objContent = findViewById<TextView>(R.id.contentObj)

        objTitle.setOnClickListener {
            val isVisible = objContent.visibility == View.VISIBLE
            objContent.visibility = if (isVisible) View.GONE else View.VISIBLE
            objTitle.text = if (isVisible) "▾ Objetivos" else "▴ Objetivos"
        }
        //endregion

        //region Contacto
        // Correo electrónico
        val correo = findViewById<TextView>(R.id.linkCorreo)
        correo.setOnClickListener {
            val intent = Intent(Intent.ACTION_SENDTO).apply {
                data = Uri.parse("mailto:administracion@gesem.com")
            }
            startActivity(intent)
        }

        // Página web
        val web = findViewById<TextView>(R.id.linkWeb)
        web.setOnClickListener {
            val intent = Intent(Intent.ACTION_VIEW).apply {
                data = Uri.parse("http://www.gesem.es")
            }
            startActivity(intent)
        }

        // Teléfono
        val telefono = findViewById<TextView>(R.id.linkTelefono)
        telefono.setOnClickListener {
            val intent = Intent(Intent.ACTION_DIAL).apply {
                data = Uri.parse("tel:+34666666666")
            }
            startActivity(intent)
        }

        // Dirección (abre Google Maps)
        val direccion = findViewById<TextView>(R.id.linkDireccion)
        direccion.setOnClickListener {
            val intent = Intent(Intent.ACTION_VIEW).apply {
                data = Uri.parse("geo:0,0?q=Calle+Hermanos+Becerril+3,+16004+Cuenca")
            }
            startActivity(intent)
        }
        //endregion

    }
}