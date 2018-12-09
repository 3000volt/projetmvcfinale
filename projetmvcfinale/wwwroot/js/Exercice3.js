$(function () {
    alert("bao!!");
    var compteur;
    //$("#btnExerciceTermine").attr("disabled", true);
});

function CommencerExercice() {
    compteur = 0;
    //Valider que ce numéro est disponible
    var numero = $("#inNumeroQuestion").val();
    if (VerifierNumero(numero) == true) {
        //Mettre au texterea
        if ($("#txtExercice").val() == "") {
            $("#txtExercice").val(numero + " - ");
        }
        else {
            var textInitiale = $("#txtExercice").val();
            $("#txtExercice").val(textInitiale + "\n" + numero + " - ");
            //https://css-tricks.com/forums/topic/solved-jquery-append-text-to-textarea/
            //$("#txtExercice").append("\n" + numero + " - ");
        }
        //Desactiver le boton de commencemen de question
        $("#btnCreerQuestion").attr('disabled', true);
        $("#inNumeroQuestion").attr("readonly", true);
        $("#divActiverCreation").removeAttr('hidden');
        //Mettre la fin de l'exercice enable
        $("#btnExerciceTermine").prop("disabled", true);

    }
    else {
        alert("Vous devez entrer un numero valide qui n'a pas été utilisé");
    }
}

function RetirerDerneirChoix() {
    //S'il y a au moins 3 colonnes dans le tableau
    if ($("#tbChoixReponses tr").length != 3) {
        $("#tbChoixReponses tr:last").remove();
    }
    else {
        alert("Vous devez concerver au moins 2 choix de réposne!");
    }
}

function AfficherTableau() {
    //Retier la possibilité de modifier la phrase
    $("#divActiverCreation *").not("#btnFinPhrase").attr("disabled", true);
    //Affihcer le tableau a choix de reponse
    $("#divTableau").removeAttr('hidden');
    //Mettre disable le div précedent
    $("#divActiverCreation *").not("#btnFinPhrase").attr("disabled", true);
    //Mettre le bouton de fin de phrase actif
    $("#btnFinPhrase").attr("disabled", true);
}

function AjouterColonne() {
    if ($("#tbChoixReponses tr:last-child td input").val() != "") {
        $("#tbChoixReponses").append('<tr><td><input asp-for="ChoixDeReponse" /></td><td></tr>');
    }
    else {
        alert("Remplissez la dernière colonne!");
    }
}

function OuvrirBonneReponse() {
    $("#tbChoixReponses tr").not(':first').each(function () {
        if ($(this).find("td input").val() == "") {
            alert("Les champs de choix de réponse doivent être tous remplis!");
            return false;
            //https://stackoverflow.com/questions/4996521/jquery-selecting-each-td-in-a-tr
        }
    });
    if (true) {
        //Mettre le selectlist vide
        $("#selectChoixReponse").empty();
        //Avoir la liste des choix de réponse
        var liste = new Array();
        $("#tbChoixReponses tr").not(':first').each(function () {
            liste.push($(this).find('td input').val());
        }); //https://stackoverflow.com/questions/4996521/jquery-selecting-each-td-in-a-tr/4996553
        //les envoyer vers le selectlistitem
        var choix = '';
        for (var i = 0; i < liste.length; i++) {
            choix += '<option value="' + liste[i] + '">' + liste[i] + '</option>';
        }
        $("#selectChoixReponse").append(choix);
        //https://stackoverflow.com/questions/3446069/populate-dropdown-select-with-array-using-jquery
        //Rendre la div visible
        $("#divBonneReponse").removeAttr('hidden');
        //Mettre la div precedente disabled
        $("#divTableau *").attr("disabled", true);

    }
}

function ChangerChoix() {
    //Recacher le div de fin de phrase
    $("#divFinPhrase").attr("hidden", true);
    //Remmettre le selectiond e bonne reposne disponible
    $("#divBonneReponse *").removeAttr("disabled");
    //Baisser le compteur de 1
    compteur--;
}


function RetourChoix() {
    //Cacher la selection de bonne reponse
    $("#divBonneReponse").attr("hidden", true);
    //Remmetre celui de creation des choix disponible
    $("#divTableau *").removeAttr("disabled");
    //Retirer la pharse du coté serveur si necessaire
    //TODO
}

function AnnulerChoixReponse() {
    //Vider et reinitialiser le tableau a choix de reponse
    //Vider le tableau et lui mettre 2 tr
    $("#tbChoixReponses tr").slice(3).each(function () {
        $(this).remove();
    });
    //Cacher le tableau
    $("#divTableau").attr('hidden', true);
    $("#divActiverCreation *").not("#btnFinPhrase").removeAttr("disabled");
    //Si une partie de phrase a été entré, remmettre le bouton de fin de pharse disponible
    //Mettre le boutin de fin de phrase actif
    var numero = $("#inNumeroQuestion").val();
    if ($("#txtExercice").val().split(numero + ' - ')[1] != "") {
        $("#btnFinPhrase").removeAttr("disabled");
    }

}

function ContinuerPhrase() {
    //Indiquer a la session que le prochain bout de phrase doit etre coller au vieux
    //InsererPartie(numero, boutPhrase);
    //Mettre le textbox valide
    var bonneReponse = $("#selectChoixReponse option:selected").val();
    $("#divActiverCreation *").not("#btnFinPhrase").removeAttr("disabled");
    //Cacher le reste non necessaire
    //Vider le tableau et lui mettre 2 tr
    $("#tbChoixReponses tr").slice(3).each(function () {
        $(this).remove();
    });
    $("#tbChoixReponses tr").each(function () {
        $(this).find('td input').val("");
    });
    //Vider selectlist
    $("#selectChoixReponse").html('');
    //Cacher la selection de bonne reposne
    $("#divBonneReponse").attr("hidden", true);
    //Cacher le tableau
    $("#divTableau").attr("hidden", true);
    //Cacher le div
    $("#divFinPhrase").attr("hidden", true);
    $("#lblPhrase").html("Suite phrase");
    ActiverDocs();
    //Mettre le boutin de fin de phrase actif
    $("#btnFinPhrase").removeAttr("disabled");
    //Insérer la partie de phrase dna sle textarea
    //$("#txtExercice").append($("#boutLigne").val());
    var phrase = $("#txtExercice").val();
    $("#txtExercice").val(phrase + $("#boutLigne").val() + "(" + bonneReponse + ")");

    //Le vider
    $("#boutLigne").val("");
}

function ActiverDocs() {
    //Remmetre le document accessible
    $("#divTableau *").removeAttr("disabled");
    $("#divBonneReponse *").removeAttr("disabled");
}


function ChoixReponse() {
    compteur++;
    //Inserer au textarea la phrase et la reponse
    //bout de phrase
    var boutPhrase = $("#boutLigne").val();
    //la réponse
    var choix = $("#selectChoixReponse").find(":selected").text();
    //$("#txtExercice").append(boutPhrase + "(" + choix + ")");
    //var textInitiale = $("#txtExercice").val();
    //$("#txtExercice").val(boutPhrase);
    //Montrer la textarea
    $("#divPhrase").removeAttr('hidden');
    //Envoyer les donnees vers controlleur
    //Envoyer l ligne
    var numero = $("#inNumeroQuestion").val();
    //Envoyer les choix de reponse
    var tableau = new Array();
    $("#selectChoixReponse option").each(function () {
        //https://stackoverflow.com/questions/590163/how-to-get-all-options-of-a-select-using-jquery
        var choixReponse = $(this).val();
        var Reponse;
        if (choixReponse == choix) {
            Reponse = true;
        }
        else {
            Reponse = false;
        }
        var ordre = compteur;
        //Insérer au tableau de choix de reponse
        tableau.push({ ChoixDeReponse1: choixReponse, Response: Reponse, NoOrdre: ordre });
    });
    CreationLigne(numero, boutPhrase, tableau);
    $("#divFinPhrase").removeAttr('hidden');
    $("#divBonneReponse *").attr("disabled", true);
}

function FinPhrase() {
    //Mettre la fin de l'exercice enable
    $("#btnExerciceTermine").prop("disabled", false);
    ActiverDocs();
    var bonneReponse = $("#selectChoixReponse option:selected").val();
    //Reactiver  le bouton de commencement de question
    $("#btnCreerQuestion").attr("disabled", false);
    //Remmettre le compteur d'ordre de choix de rpeosne a 0
    compteur = 0;
    //Vider le tableau et lui mettre 2 tr
    $("#tbChoixReponses tr").slice(3).each(function () {
        $(this).remove();
    });
    $("#tbChoixReponses tr").each(function () {
        $(this).find('td input').val("");
    });
    //Vider selectlist
    $("#selectChoixReponse").html('');
    //Cacher la selection de bonne reposne
    $("#divBonneReponse").attr("hidden", true);
    //Cacher le tableau
    $("#divTableau").attr("hidden", true);
    //Cacher le div
    $("#divFinPhrase").attr("hidden", true);
    //Remmettre le texbox enable
    $("#divActiverCreation *").not("#btnFinPhrase").removeAttr("disabled");
    //Remmtre dispo le numeor de question
    $("#inNumeroQuestion").removeAttr("readonly");
    $("#inNumeroQuestion").val("");
    //Cahcer le reste du formulaire
    $("#divActiverCreation").attr("hidden", true);
    //Changer le titre du label
    $("#lblPhrase").html("Début Phrase");
    //Terminer dans le controlleur
    finPhraseAjax();
    //Mettre le text dans le textarea
    //$("#txtExercice").append($("#boutLigne").val());
    var textInitiale = $("#txtExercice").val();
    $("#txtExercice").val(textInitiale + $("#boutLigne").val() +"(" + bonneReponse +")");
    //Vider son contenu
    $("#boutLigne").val("");
    //Ne plus mettre disponible le bouton de fin de phrase
    $("#btnFinPhrase").attr("disabled", true);
}

function finPhraseAjax() {
    var url = "/Exercice/TerminerPhrase";
    $.ajax({
        type: "POST",
        url: url,
        datatype: "text/plain",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (request) {
            request.setRequestHeader("RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());
        },
        success: function (result) {
            // alert(status);
        },
        error: function (xhr, status) { alert("erreur:" + status); }
    });
}

function RetirerPhraseAjax() {
    var url = "/Exercice/RetirerPhrase";
    $.ajax({
        type: "POST",
        url: url,
        datatype: "text/plain",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (request) {
            request.setRequestHeader("RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());
        },
        success: function (result) {
            // alert(status);
        },
        error: function (xhr, status) { alert("erreur:" + status); }
    });
}


function TerminerPhrase() {
    //Envoyer ce qu'il y a dans la phrase (si aucun choix n'a été envoyé)
    var numero = $("#inNumeroQuestion").val();
    var ligne = $("#boutLigne").val();
    TerminerLigne(numero, ligne);
    //Reactiver  le bouton de commencement de question
    $("#btnCreerQuestion").attr("disabled", false);
    //Vider le tableau et lui mettre 2 tr
    $("#tbChoixReponses tr").slice(3).each(function () {
        $(this).remove();
    });
    $("#tbChoixReponses tr").each(function () {
        $(this).find('td input').val("");
    });
    //Vider selectlist
    $("#selectChoixReponse").html('');
    //Cacher la selection de bonne reposne
    $("#divBonneReponse").attr("hidden", true);
    //Cacher le tableau
    $("#divTableau").attr("hidden", true);
    //Cacher le div
    $("#divFinPhrase").attr("hidden", true);
    //Remmettre le texbox enable
    $("#divActiverCreation *").not("#btnFinPhrase").removeAttr("disabled");
    //Mettre le text dans le textarea
    //$("#txtExercice").append($("#boutLigne").val());
    var textInitiale = $("#txtExercice").val();
    $("#txtExercice").val(textInitiale + $("#boutLigne").val());
    //Vider son contenu
    $("#boutLigne").val("");
    //Remmtre dispo le numeor de question
    $("#inNumeroQuestion").removeAttr("readonly");
    $("#inNumeroQuestion").val("");
    //Cahcer le reste du formulaire
    $("#divActiverCreation").attr("hidden", true);
    //Changer le titre du label
    $("#lblPhrase").html("Début phrase");
    //Mettre la fin de l'exercice enable
    $("#btnExerciceTermine").prop("disabled", false);
    //Ne plus mettre le boutin de fin de phrase dispo
    $("#btnFinPhrase").attr("disabled", true);
}

function EnvoyerExercice() {
    compteur = 0;
    var url = "/Exercice/EnvoyerExercice";
    $.ajax({
        type: "POST",
        url: url,
        datatype: "text/plain",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (request) {
            request.setRequestHeader("RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());
        },
        success: function (data) {
            //alert(status);
            window.location.href = data;
            //Merci https://stackoverflow.com/questions/20011282/redirecttoaction-not-working-after-successful-jquery-ajax-post
        },
        error: function (xhr, status) { alert("erreur:" + status); }
    });
    return false;
}

function SauvagardeEnvoyerChoix(tableau_donner) {

    var url = "/Exercice/AjoutChoixReponse";
    var data = tableau_donner;
    $.ajax({
        data: JSON.stringify(data),
        type: "POST",
        url: url,
        datatype: "text/plain",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (request) {
            request.setRequestHeader("RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());
        },
        success: function (result) {
            //alert(status);
        },
        error: function (xhr, status) { alert("erreur:" + status); }
    });
    return false;
}


function CreationLigne(i, y, z) {

    var url = "/Exercice/CreationLigne";
    var data1 = {
        NumeroQuestion: i,
        Ligne: y,
        listeChoixReponses2: z
    };
    $.ajax({
        data: JSON.stringify(data1),
        type: "POST",
        url: url,
        datatype: "text/plain",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (request) {
            request.setRequestHeader("RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());
        },
        success: function (result) {
            // alert(status);
        },
        error: function (xhr, status) { alert("erreur:" + status); }
    });
    return false;
}

function TerminerLigne(i, y) {

    var url = "/Exercice/TerminerLigne";
    var data = {
        NumeroQuestion: i,
        Ligne: y
    };
    $.ajax({
        data: JSON.stringify(data),
        type: "POST",
        url: url,
        datatype: "text/plain",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (request) {
            request.setRequestHeader("RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());
        },
        success: function (result) {
            //alert(status);
        },
        error: function (xhr, status) { alert("erreur:" + status); }
    });
    return false;
}

function VerifierNumero(i) {
    var bool = false;
    var url = "/Exercice/VerifierNumero";
    $.ajax({
        data: { numero: i },
        type: "POST",
        async: false,
        url: url,
        dataType: "json",
        //contentType: "application/json; charset=utf-8",        
        success: function (data) {
            bool = data;
        },
        error: function (xhr, status) { alert("erreur:" + status); }
    });
    return bool;
}//https://stackoverflow.com/questions/23078650/ajax-return-true-false-i-have-implemented-a-callback

function InsererPartie(i, y) {

    var url = "/Exercice/InsererPartie";
    var data = {
        NumeroQuestion: i,
        Ligne: y
    };
    $.ajax({
        data: JSON.stringify(data),
        type: "POST",
        url: url,
        datatype: "text/plain",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (request) {
            request.setRequestHeader("RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());
        },
        success: function (result) {
            // alert(status);
        },
        error: function (xhr, status) { alert("erreur:" + status); }
    });
    return false;
}

function AnnulerASuivre() {

    var url = "/Exercice/AnnulerUneSuite";
    $.ajax({
        type: "POST",
        url: url,
        datatype: "text/plain",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (request) {
            request.setRequestHeader("RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());
        },
        success: function (result) {
            // alert(status);
        },
        error: function (xhr, status) { alert("erreur:" + status); }
    });
    return false;
}