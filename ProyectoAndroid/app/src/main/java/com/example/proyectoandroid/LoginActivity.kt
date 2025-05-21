package com.example.proyectoandroid

import android.content.Intent
import android.net.Uri
import android.os.Bundle
import android.widget.Button
import android.widget.CheckBox
import android.widget.EditText
import android.widget.ImageButton
import android.widget.TextView
import android.widget.Toast
import androidx.activity.enableEdgeToEdge
import androidx.appcompat.app.AppCompatActivity
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat
import retrofit2.Call
import retrofit2.Response
import retrofit2.Callback
import java.util.Locale


class LoginActivity : AppCompatActivity() {

    private lateinit var entryUsuario: EditText
    private lateinit var entryContraseña: EditText
    private lateinit var btnLogin: Button
    private lateinit var cbRecordarme: CheckBox
    private lateinit var db: DbHelper



    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContentView(R.layout.activity_login)

        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main)) { v, insets ->
            val sb = insets.getInsets(WindowInsetsCompat.Type.systemBars())
            v.setPadding(sb.left, sb.top, sb.right, sb.bottom)
            insets
        }

        // Referencias
        entryUsuario    = findViewById(R.id.entryUsuario)
        entryContraseña = findViewById(R.id.entryContraseña)
        btnLogin        = findViewById(R.id.btnLogin)
        cbRecordarme    = findViewById(R.id.checkBoxRecordarme)
        db              = DbHelper(this)

        // Cargar credenciales si las hay
        db.load()?.let { (email, pass) ->
            entryUsuario.setText(email)
            entryContraseña.setText(pass)
            cbRecordarme.isChecked = true
        }

        //region boton olvide la contraseña
        findViewById<TextView>(R.id.textViewPasswordLost).setOnClickListener {
            val adminEmail = "administracion@gesem.com"
            val asunto = "Recuperación de contraseña - Gesem"
            val cuerpo = "Hola,\n\nSolicito restablecer mi contraseña.\n\n" +
                    "Fecha y hora: ${java.text.SimpleDateFormat("dd/MM/yyyy HH:mm").format(java.util.Date())}"

            val intent = Intent(Intent.ACTION_SENDTO, Uri.parse("mailto:$adminEmail")).apply {
                putExtra(Intent.EXTRA_SUBJECT, asunto)
                putExtra(Intent.EXTRA_TEXT, cuerpo)
            }

            if (intent.resolveActivity(packageManager) != null) {
                startActivity(intent)
            } else {
                Toast.makeText(this, "No hay ninguna app de correo instalada", Toast.LENGTH_LONG).show()
            }
        }
        //endregion

        //region  Menu inferior
        findViewById<ImageButton>(R.id.btnHome).setOnClickListener {
        }
        findViewById<ImageButton>(R.id.btnAbout).setOnClickListener {
            startActivity(Intent(this, AboutUsActivity::class.java))
        }
        findViewById<ImageButton>(R.id.btnFaq).setOnClickListener {
            startActivity(Intent(this, FaqActivity::class.java))
        }
        //endregion

        // region Boton login y recordarme
        btnLogin.setOnClickListener {
            val email = entryUsuario.text.toString()
                .trim()
                .lowercase(Locale.getDefault())
            val pass  = entryContraseña.text.toString()
                .trim()
                .lowercase(Locale.getDefault())

            // Llamada a la API
            RetrofitClient.instance
                .login(LoginRequest(email, pass))
                .enqueue(object : Callback<LoginResponse> {
                    override fun onResponse(
                        call: Call<LoginResponse>,
                        resp: Response<LoginResponse>
                    ) {
                        val body = resp.body()
                        if (body == null) {
                            Toast.makeText(this@LoginActivity, "Usuario o contraseña incorrectos", Toast.LENGTH_SHORT).show()
                            return
                        }
                        if (!body.success) {
                            Toast.makeText(this@LoginActivity, body.mensaje, Toast.LENGTH_SHORT).show()
                            return
                        }

                        val user = body.usuario
                        if (user == null) {
                            Toast.makeText(this@LoginActivity, "No se recibió usuario", Toast.LENGTH_SHORT).show()
                            return
                        }

                        // Solo cuentas activas
                        if (user.estadoCuenta != "activa") {
                            Toast.makeText(
                                this@LoginActivity,
                                "Cuenta ${user.estadoCuenta}",
                                Toast.LENGTH_LONG
                            ).show()
                            return
                        }

                        // Solo rol usuario
                        if (user.rol != "usuario") {
                            Toast.makeText(
                                this@LoginActivity,
                                "Acceso solo para usuarios con rol “usuario”",
                                Toast.LENGTH_LONG
                            ).show()
                            return
                        }

                        // Guardar o borrar credenciales
                        if (cbRecordarme.isChecked) db.save(email, pass)
                        else db.clear()

                        // Preparamos la lista completa (aunque venga vacía)
                        val listaSolicitudes = ArrayList(body.solicitudes ?: emptyList())

                        // Creamos el Intent a MainActivity y le pasamos LA LISTA
                        val intent = Intent(this@LoginActivity, MainActivity::class.java).apply {
                            putParcelableArrayListExtra("solicitudes", listaSolicitudes)
                        }
                        startActivity(intent)
                        finish()
                        }



                    override fun onFailure(call: Call<LoginResponse>, t: Throwable) {
                        Toast.makeText(
                            this@LoginActivity,
                            "Error de red: ${t.localizedMessage}",
                            Toast.LENGTH_LONG
                        ).show()
                    }
                })
        }
        //endregion
    }
}