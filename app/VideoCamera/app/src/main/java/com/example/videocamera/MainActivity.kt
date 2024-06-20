package com.example.videocamera

import android.content.Intent
import android.net.Uri
import android.os.Bundle
import android.provider.MediaStore
import android.view.View
import android.widget.MediaController
import android.widget.VideoView
import androidx.appcompat.app.AppCompatActivity
import okhttp3.*
import java.io.File
import java.io.IOException

class MainActivity : AppCompatActivity() {
    private lateinit var videoView: VideoView
    private var ourRequestCode: Int = 123

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)

        videoView = findViewById(R.id.videoView)
        val mediaController = MediaController(this)
        mediaController.setAnchorView(videoView)
        videoView.setMediaController(mediaController)

        // Inizializzazione OkHttp client
        val client = OkHttpClient()

        // Creazione della richiesta HTTP
        val request = Request.Builder()
            .url("http://localhost:3000") // Cambia l'URL del server se necessario
            .build()

        // Esecuzione della chiamata asincrona
        client.newCall(request).enqueue(object : Callback {
            override fun onFailure(call: Call, e: IOException) {
                e.printStackTrace()
                // Gestisci l'errore se necessario
            }

            override fun onResponse(call: Call, response: Response) {
                val responseData = response.body()?.string()
                runOnUiThread {
                    // Gestisci la risposta qui, ad esempio aggiorna l'UI con i dati ricevuti
                    // textView.text = responseData
                }
            }
        })
    }

    fun startVideo(view: View) {
        val intent = Intent(MediaStore.ACTION_VIDEO_CAPTURE)
        if (intent.resolveActivity(packageManager) != null) {
            startActivityForResult(intent, ourRequestCode)
        }
    }

    override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
        super.onActivityResult(requestCode, resultCode, data)
        if (requestCode == ourRequestCode && resultCode == RESULT_OK) {
            val videoUri = data?.data
            videoView.setVideoURI(videoUri)
            videoView.start()

            // Invio del video al server
            videoUri?.let {
                uploadVideoToServer(it)
            }
        }
    }

    private fun uploadVideoToServer(videoUri: Uri) {
        val client = OkHttpClient()
        val file = File(videoUri.path!!)
        val fileBody = RequestBody.create(MediaType.parse("video/mp4"), file)

        // Creazione del multipart body
        val requestBody = MultipartBody.Builder()
            .setType(MultipartBody.FORM)
            .addFormDataPart("VideoFile", file.name, fileBody)
            .build()

        // Creazione della richiesta POST
        val request = Request.Builder()
            .url("http://YOUR_SERVER_IP/Video/Add") // Sostituisci con l'URL del tuo server
            .post(requestBody)
            .build()

        client.newCall(request).enqueue(object : Callback {
            override fun onFailure(call: Call, e: IOException) {
                e.printStackTrace()
                // Gestisci l'errore se necessario
            }

            override fun onResponse(call: Call, response: Response) {
                val responseData = response.body()?.string()
                runOnUiThread {
                    // Gestisci la risposta qui, ad esempio aggiorna l'UI con i dati ricevuti
                    // textView.text = responseData
                }
            }
        })
    }
}
