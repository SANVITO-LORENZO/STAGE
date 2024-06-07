import whisper
import pyodbc
import subprocess
import os

# Funzione per estrarre l'audio
def extract_audio(video_path):
    audio_path = video_path.replace('.mp4', '.mp3')
    try:
        subprocess.run(['ffmpeg', '-i', video_path, '-vn', '-acodec', 'libmp3lame', '-y', audio_path], check=True, timeout=300)  # Timeout di 5 minuti
    except subprocess.TimeoutExpired:
        print(f"Timeout extracting audio from {video_path}")
        raise
    return audio_path

# Funzione per trascrivere l'audio
def transcribe_audio(audio_path):
    model = whisper.load_model("base")
    result = model.transcribe(audio_path)
    return result["text"]

def main():
    conn = None
    try:
        # Connessione con il database
        conn = pyodbc.connect(
            'DRIVER={ODBC Driver 17 for SQL Server};'
            'SERVER=LORENZO\\SQLEXPRESS;'
            'DATABASE=Houses;'
            'Trusted_Connection=yes;'
            'Timeout=5;'
        )
        cursor = conn.cursor()

        # Selezione degli elementi con stato 0
        cursor.execute("SELECT id, Path FROM dbo.Videos WHERE status = 0")
        videos = cursor.fetchall()

        if not videos:
            print("No videos found with status = 0.")
            return

        for video_Id, video_Path in videos:
            #print(f"Processing video ID: {video_Id}, Path: {video_Path}")
            
            # Aggiornamento dello stato a 1 (in elaborazione)
            cursor.execute("UPDATE dbo.Videos SET status = 1 WHERE id = ?", video_Id)
            conn.commit()

            try:
                audio_path = extract_audio(video_Path)
                transcription = transcribe_audio(audio_path)
                
                # Output della trascrizione
                print({transcription})

                # Aggiornamento del database
                cursor.execute("UPDATE dbo.Videos SET Description = ?, Status = 2 WHERE id = ?", (transcription, video_Id))
                conn.commit()
                
                #print(f"Updated video ID: {video_Id} with transcription.")

                # Eliminazione del file audio utilizzato per la trascrizione
                if os.path.exists(audio_path):
                    os.remove(audio_path)
                    #print(f"Deleted audio file: {audio_path}")

            except Exception as e:
                print(f"Error processing video ID: {video_Id}, Error: {e}")
                # In caso di errore, si può scegliere di riportare lo stato a 0 o a un altro valore che indica il fallimento
                cursor.execute("UPDATE dbo.Videos SET status = 0 WHERE id = ?", video_Id)
                conn.commit()

        print("Processing completed.")

    except pyodbc.Error as e:
        print(f"SQL Server error: {e}")
    finally:
        if conn:
            conn.close()

if __name__ == "__main__":
    main()