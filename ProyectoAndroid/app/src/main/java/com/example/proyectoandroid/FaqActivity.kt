package com.example.proyectoandroid

import android.content.Intent
import android.os.Bundle
import android.view.View
import android.widget.ImageButton
import android.widget.TextView
import androidx.activity.enableEdgeToEdge
import androidx.appcompat.app.AppCompatActivity
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat

class FaqActivity : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContentView(R.layout.activity_faq)
        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main)) { v, insets ->
            val systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars())
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom)
            insets
        }

        //region Desplegables
        findViewById<TextView>(R.id.a1).text = getString(R.string.Answer1Help)
        findViewById<TextView>(R.id.a2).text = getString(R.string.Answer2Help)
        findViewById<TextView>(R.id.a3).text = getString(R.string.Answer3Help)
        findViewById<TextView>(R.id.a4).text = getString(R.string.Answer4Help)
        findViewById<TextView>(R.id.a5).text = getString(R.string.Answer5Help)


        val q1 = findViewById<TextView>(R.id.q1)
        val a1 = findViewById<TextView>(R.id.a1)
        q1.setOnClickListener {
            val visible = a1.visibility == View.VISIBLE
            a1.visibility = if (visible) View.GONE else View.VISIBLE
            q1.text = if (visible) "▾ ${getString(R.string.Question1Help)}" else "▴ ${getString(R.string.Question1Help)}"
        }

        val q2 = findViewById<TextView>(R.id.q2)
        val a2 = findViewById<TextView>(R.id.a2)
        q2.setOnClickListener {
            val visible = a2.visibility == View.VISIBLE
            a2.visibility = if (visible) View.GONE else View.VISIBLE
            q2.text = if (visible) "▾ ${getString(R.string.Question2Help)}" else "▴ ${getString(R.string.Question2Help)}"
        }

        val q3 = findViewById<TextView>(R.id.q3)
        val a3 = findViewById<TextView>(R.id.a3)
        q3.setOnClickListener {
            val visible = a3.visibility == View.VISIBLE
            a3.visibility = if (visible) View.GONE else View.VISIBLE
            q3.text = if (visible) "▾ ${getString(R.string.Question3Help)}" else "▴ ${getString(R.string.Question3Help)}"
        }

        val q4 = findViewById<TextView>(R.id.q4)
        val a4 = findViewById<TextView>(R.id.a4)
        q4.setOnClickListener {
            val visible = a4.visibility == View.VISIBLE
            a4.visibility = if (visible) View.GONE else View.VISIBLE
            q4.text = if (visible) "▾ ${getString(R.string.Question4Help)}" else "▴ ${getString(R.string.Question4Help)}"
        }

        val q5 = findViewById<TextView>(R.id.q5)
        val a5 = findViewById<TextView>(R.id.a5)
        q5.setOnClickListener {
            val visible = a5.visibility == View.VISIBLE
            a5.visibility = if (visible) View.GONE else View.VISIBLE
            q5.text = if (visible) "▾ ${getString(R.string.Question5Help)}" else "▴ ${getString(R.string.Question5Help)}"
        }
        //endregion

        //region Menu inferior
        val btnHome = findViewById<ImageButton>(R.id.btnHome)
        val btnAbout = findViewById<ImageButton>(R.id.btnAbout)
        val btnFaq = findViewById<ImageButton>(R.id.btnFaq)

        btnHome.setOnClickListener {
            val intent = Intent(this, LoginActivity::class.java)
            startActivity(intent)
        }

        btnAbout.setOnClickListener {
            val intent = Intent(this, AboutUsActivity::class.java)
            startActivity(intent)
        }

        btnFaq.setOnClickListener {

        }
        //endregion
    }

}