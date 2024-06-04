import whisper

def transcrivi_audio(percorso_file_audio):
    modello = whisper.load_model("base")
    risultato = modello.transcribe(percorso_file_audio)
    return risultato["text"]

if __name__ == "__main__":
    import argparse

    # Parser degli argomenti da riga di comando
    parser = argparse.ArgumentParser(description="Trascrivi audio con Whisper")
    parser.add_argument("file_audio", help="Percorso del file audio da trascrivere")
    argomenti = parser.parse_args()

    # Trascrizione del file audio
    percorso_file_audio = argomenti.file_audio
    testo_trascritto = transcrivi_audio(percorso_file_audio)

    # Stampa del testo trascritto
    print(testo_trascritto)