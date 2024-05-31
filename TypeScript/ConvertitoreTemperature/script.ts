function convert(): void {
    let tmpIniziale = document.getElementById("temp") as HTMLInputElement;
    let sclaInizialeElement = document.getElementById("inp_I") as HTMLSelectElement;
    let scalaFinaleElement = document.getElementById("inp_F") as HTMLSelectElement;
    let risultato = document.getElementById("risultato") as HTMLInputElement;

    let sclaIniziale = sclaInizialeElement.value;
    let scalaFinale = scalaFinaleElement.value;

    let temp = tmpIniziale.value;

    if (temp === "") {
        alert("Per favore, inserisci una temperatura.");
        return;
    }

    let temp_convertita = parseFloat(temp);

    if (isNaN(temp_convertita)) {
        risultato.value = "Input non valido";
        return;
    }

    let tmp: number;

    // Conversione a gradi Celsius
    let tempINCelsius: number;
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

    // Conversione dalla scala Celsius alla scala finale
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

    risultato.value = tmp.toFixed(2);

    // Reimposta le selezioni delle scale di temperatura a "K"
    sclaInizialeElement.value = "K";
    scalaFinaleElement.value = "K";

    // Resetta il campo di input della temperatura iniziale
    tmpIniziale.value = "";
}