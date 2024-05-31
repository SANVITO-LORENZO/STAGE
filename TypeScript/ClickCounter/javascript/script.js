function AGGIUNGI() {
    if (localStorage.clickcount) {
        localStorage.clickcount = (parseInt(localStorage.clickcount) + 1).toString();
    }
    else {
        localStorage.clickcount = "1";
    }
    AGGIORNA();
}
function SOTTRAI() {
    if (localStorage.clickcount && parseInt(localStorage.clickcount) > 0) {
        localStorage.clickcount = (parseInt(localStorage.clickcount) - 1).toString();
    }
    AGGIORNA();
}
function RESET() {
    localStorage.clear();
    localStorage.clickcount = "0";
    AGGIORNA();
}
function AGGIORNA() {
    var cont = document.getElementById("contatore");
    if (cont) {
        cont.innerHTML = localStorage.clickcount;
        if (parseInt(localStorage.clickcount) === 0) {
            cont.style.fontWeight = "bold";
        }
        else {
            cont.style.fontWeight = "normal";
        }
        if (parseInt(localStorage.clickcount) % 2 === 0) {
            cont.classList.remove("dispari");
            cont.classList.add("pari");
        }
        else {
            cont.classList.remove("pari");
            cont.classList.add("dispari");
        }
    }
}
document.addEventListener("DOMContentLoaded", function () {
    if (!localStorage.clickcount) {
        localStorage.clickcount = "0";
    }
    AGGIORNA();
});
