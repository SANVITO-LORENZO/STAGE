import whisper
import pyodbc
import subprocess
import os

# FUNZIONE PER ESTRARRE L'AUDIO
def extract_audio(video_path):
    audio_path = video_path.replace('.mp4', '.mp3')
    subprocess.run(['ffmpeg', '-i', video_path, '-vn', '-acodec', 'libmp3lame', '-y', audio_path], check=True)
    return audio_path

# FUNZIONE PER TRASCRIVERE L'AUDIO
def transcribe_audio(audio_path):
    model = whisper.load_model("base")
    result = model.transcribe(audio_path)
    return result["text"]

def main():
    conn = None
    try:
        # CONNESSIONE CON IL DATABASE
        conn = pyodbc.connect(
            'DRIVER={ODBC Driver 17 for SQL Server};'
            'SERVER=LORENZO\\SQLEXPRESS;'
            'DATABASE=Houses;'
            'Trusted_Connection=yes;'
            'Timeout=5;'
        )
        cursor = conn.cursor()

        # SELEZIONE DEGLI ELEMENTI CON STATO 0
        cursor.execute("SELECT id, Path FROM dbo.Videos WHERE status = 0")
        
        videos = cursor.fetchall()

        if not videos:
            print("No videos found with status = 0.")
            return

        for video_Id, video_Path in videos:
            print(f"Processing video ID: {video_Id}, Path: {video_Path}")
            
            # AGGIORNAMENTO DELLO STATO A 1 (IN ELABORAZIONE)
            cursor.execute("UPDATE dbo.Videos SET status = 1 WHERE id = ?", video_Id)
            conn.commit()

            try:
                audio_path = extract_audio(video_Path)
                transcription = transcribe_audio(audio_path)

                # AGGIORNAMENTO DEL DATABASE
                cursor.execute("UPDATE dbo.Videos SET Description = ?, Status = 2 WHERE id = ?", (transcription, video_Id))
                conn.commit()
                
                print(f"Updated video ID: {video_Id} with transcription.")

                # ELIMINAZIONE DEL FILE AUDIO UTILIZZATO PER LA TRASCRIZIONE
                if os.path.exists(audio_path):
                    os.remove(audio_path)
                    print(f"Deleted audio file: {audio_path}")

                # ELIMINAZIONE DEL FILE VIDEO
                if os.path.exists(video_Path):
                    os.remove(video_Path)
                    print(f"Deleted video file: {video_Path}")

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