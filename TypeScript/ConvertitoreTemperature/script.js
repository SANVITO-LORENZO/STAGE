function convert() {
    var initialTempInput = document.getElementById("initialTemp");
    var scaleInitial = document.getElementById("inp_I").value;
    var scaleFinal = document.getElementById("inp_F").value;
    var resultInput = document.getElementById("result");
    var initialTemp = initialTempInput.value;
    if (initialTemp.trim() === "") {
        alert("Per favore, inserisci una temperatura.");
        return;
    }
    var initialTempValue = parseFloat(initialTemp);
    if (isNaN(initialTempValue)) {
        resultInput.value = "Input non valido";
        return;
    }
    var result;
    // Conversione a gradi Celsius
    var tempInCelsius;
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
