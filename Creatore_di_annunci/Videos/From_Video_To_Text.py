import whisper
import pyodbc
import subprocess
import os

def extract_audio(video_path):
    audio_path = video_path.replace('.mp4', '.mp3')
    subprocess.run(['ffmpeg', '-i', video_path, '-vn', '-acodec', 'libmp3lame', '-y', audio_path], check=True)
    return audio_path

def transcribe_audio(audio_path):
    model = whisper.load_model("base")
    result = model.transcribe(audio_path)
    return result["text"]

def main():
    conn = None
    try:
        # Connect to the SQL Server database
        conn = pyodbc.connect(
            'DRIVER={ODBC Driver 17 for SQL Server};'
            'SERVER=LORENZO\\SQLEXPRESS;'
            'DATABASE=Houses;'
            'Trusted_Connection=yes;'
            'Timeout=5;'
        )
        cursor = conn.cursor()

        # Retrieve videos with stato = 0
        cursor.execute("SELECT id, Path FROM dbo.Videos WHERE status = 0")
        videos = cursor.fetchall()

        if not videos:
            print("No videos found with stato = 0.")
            return

        for video_Id, video_Path in videos:
            print(f"Processing video ID: {video_Id}, Path: {video_Path}")
            audio_path = extract_audio(video_Path)
            transcription = transcribe_audio(audio_path)

            # Update the description in the database
            cursor.execute("UPDATE dbo.Videos SET Description = ?, Status = 1 WHERE id = ?", (transcription, video_Id))
            conn.commit()

            # Delete the audio file
            if os.path.exists(audio_path):
                os.remove(audio_path)
                print(f"Deleted audio file: {audio_path}")

        print("Processing completed.")

    except pyodbc.Error as e:
        print(f"SQL Server error: {e}")
    finally:
        if conn:
            conn.close()

if __name__ == "__main__":
    main()