import psycopg2  # Importa libreria per la connessione al database
import whisper  # Importa libreria Whisper per la trascrizione audio
import subprocess

# Parametri di connessione al database
db_name = "LORENZO\SQLEXPRESS" 
db_user = "nome_utente"  # Sostituisci con il nome utente del tuo database
db_password = "password"  # Sostituisci con la password del tuo database
db_host = "localhost"  # Sostituisci se il database è su un altro host
db_port = "5432"  # Sostituisci se il database utilizza una porta diversa

# Connessione al database
connessione = psycopg2.connect(dbname=db_name, user=db_user, password=db_password, host=db_host, port=db_port)
cursore = connessione.cursor()

# Query per selezionare i record con stato 0
query = """
SELECT id, path
FROM public
WHERE status = 0;
"""

cursore.execute(query)
risultati = cursore.fetchall()

# Ciclo per ogni record con stato 0
for record in risultati:
    id_record = record[0]
    percorso_file_audio = record[1]

    # Trascrizione del file audio con Whisper
    modello = whisper.load_model("base")
    risultato_trascrizione = modello.transcribe(percorso_file_audio)
    testo_trascritto = risultato_trascrizione["text"]

    # Aggiornamento dello stato del record a 1
    query_aggiornamento = """
    UPDATE public
    SET status = 1, description = description || $1
    WHERE id = $2;
    """
    dati_aggiornamento = (testo_trascritto, id_record)
    cursore.execute(query_aggiornamento, dati_aggiornamento)

    # Esecuzione dello script di creazione annunci con il testo trascritto
    comando_script = f"python C:\\Users\\loris\\Documents\\GitHub\\STAGE\\Creatore_di_annunci\\Videos\\whisper_script.py {percorso_file_audio} {testo_trascritto}"
    subprocess.call(comando_script)

# Commit delle modifiche al database
connessione.commit()

# Chiusura della connessione al database
connessione.close()