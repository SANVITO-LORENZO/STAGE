function convert() {
    //NON PRENDO I VALUE DIRETTAMENTE QUA PER MODIFICARLI IN SEGUITO
    var tmpIniziale = document.getElementById("temp");
    var Selection1 = document.getElementById("inp_I");
    var Selection2 = document.getElementById("inp_F");
    var risultato = document.getElementById("risultato");
    //PRESA VALORI DALLE VARIABILI(non posso usare le altre perchè sono di tipo HTMLInputElement)
    var sclaIniziale = Selection1.value;
    var scalaFinale = Selection2.value;
    var temp = tmpIniziale.value;
    //CONTROLLO SE LA STRINGA E' VUOTA
    if (temp === "") {
        alert("Per favore, inserisci una temperatura.");
        return;
    }
    var temp_convertita = parseFloat(temp);
    //CONTROLLO SE IL NUMERO E' VALIDO
    if (isNaN(temp_convertita)) {
        risultato.value = "Input non valido";
        return;
    }
    //CONVERTO LA TEMPERATURA INSERITA INIZIALMENTE IN CELSIUS
    var tempINCelsius;
    switch (sclaIniziale) {
        case "C":
            tempINCelsius = temp_convertita;
            break;
        case "F":
            tempINCelsius = (temp_convertita - 32) * 5 / 9;
            break;
        case "K":
            tempINCelsius = temp_convertita - 273.15;
            break;
        default:
            risultato.value = "Scala iniziale non valida";
            return;
    }
    //CALCOLA LA TEMPERATURA FINALE
    var tmp;
    switch (scalaFinale) {
        case "C":
            tmp = tempINCelsius;
            break;
        case "F":
            tmp = tempINCelsius * 9 / 5 + 32;
            break;
        case "K":
            tmp = tempINCelsius + 273.15;
            break;
        default:
            risultato.value = "Scala finale non valida";
            return;
    }
    //IMPOSTO IL VALORE FINALE CONSIDERANDO SOLO DUE CIFRE DOPO LA VIRGOLA
    risultato.value = tmp.toFixed(2);
    //REIMPOSTO I VALORI A ZERO
    Selection1.value = "K";
    Selection2.value = "K";
    tmpIniziale.value = "";
}
