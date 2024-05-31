function convert(): void {
    const initialTempInput = document.getElementById("initialTemp") as HTMLInputElement;
    const scaleInitial = (document.getElementById("inp_I") as HTMLSelectElement).value;
    const scaleFinal = (document.getElementById("inp_F") as HTMLSelectElement).value;
    const resultInput = document.getElementById("result") as HTMLInputElement;

    const initialTemp = initialTempInput.value;

    if (initialTemp.trim() === "") {
        alert("Per favore, inserisci una temperatura.");
        return;
    }

    const initialTempValue = parseFloat(initialTemp);

    if (isNaN(initialTempValue)) {
        resultInput.value = "Input non valido";
        return;
    }

    let result: number;

    // Conversione a gradi Celsius
    let tempInCelsius: number;
    switch (scaleInitial) {
        case "C":
            tempInCelsius = initialTempValue;
            break;
        case "F":
            tempInCelsius = (initialTempValue - 32) * 5 / 9;
            break;
        case "K":
            tempInCelsius = initialTempValue - 273.15;
            break;
        default:
            resultInput.value = "Scala iniziale non valida";
            return;
    }

    // Conversione dalla scala Celsius alla scala finale
    switch (scaleFinal) {
        case "C":
            result = tempInCelsius;
            break;
        case "F":
            result = tempInCelsius * 9 / 5 + 32;
            break;
        case "K":
            result = tempInCelsius + 273.15;
            break;
        default:
            resultInput.value = "Scala finale non valida";
            return;
    }

    resultInput.value = result.toFixed(2);
}