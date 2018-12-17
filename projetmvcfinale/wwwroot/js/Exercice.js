$(function () {
});

function CommencerExercice() {
    //Valider que ce numéro est disponible
    var numero = $("#inNumeroQuestion").val();
    if (VerifierNumero(numero) == true) {
        $("#inNumeroQuestion").attr("readonly", true);
        $("#ActiverCreation").removeAttr('hidden');
    }
    else {
        alert("Vous devez entrer un numero valide qui n'a pas été utilisé");
    }
}


function AfficherTableau() {
    //Retier la possibilité de modifier la phrase
    $("#boutLigne").attr('readonly', true);
    //Affihcer le tableau a choix de reponse
    $("#divTableau").removeAttr('hidden');
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
    alert("PreEntre");
    $("#tbChoixReponses tr").not(':first').each(function () {
        if ($(this).find("td input").val() == "") {
            alert("Les champs de choix de réponse doivent être tous remplis!");
            return false;
            //https://stackoverflow.com/questions/4996521/jquery-selecting-each-td-in-a-tr
        }
    });
    if (true) {
        alert("sortie");
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
    }

}

function AnnulerChoixReponse() {
    //Vider et reinitialiser le tableau a choix de reponse
    //Vider le tableau et lui mettre 2 tr
    $("#tbChoixReponses tr").slice(3).each(function () {
        $(this).remove();
    });
    //Cacher le tableau
    $("#divTableau").attr('hidden', true);
    //Remmtre la phrase modifiable
    $("#boutLigne").removeAttr('readonly');
}

function ContinuerPhrase() {
    //Mettre le textbox valide
    $("#boutLigne").removeAttr("readonly");
    //Le vider
    $("#boutLigne").val("");
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
    $("#divDecision").attr("hidden", true);
    //Indiquer a la session que le prochain bout de phrase doit etre coller au vieux
}


function ChoixReponse() {
    //Inserer au textarea la phrase et la reponse
    //bout de phrase
    var boutPhrase = $("#boutLigne").val();
    //la réponse
    var choix = $("#selectChoixReponse").find(":selected").text();
    $("#TextPhrase").append(boutPhrase + "(" + choix + ")");
    //Montrer la textarea
    $("#divPhrase").removeAttr('hidden');
    //Envoyer les donnees vers controlleur
    //Envoyer l ligne
    var numero = $("#inNumeroQuestion").val();
    CreationLigne(numero, boutPhrase);
    //Envoyer les choix de reponse
    var compteur = 0;
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
        var ordre = boutPhrase.length;
        alert(choixReponse);
        alert(Reponse);
        alert(ordre);
        SauvagardeEnvoyerChoix(choixReponse, Reponse, ordre);
    });
    $("#divDecision").removeAttr('hidden');
}

function TerminerPhrase() {
    //Retour au bout de phrase
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
    $("#divDecision").attr("hidden", true);
    //Remmettre le texbox enable
    $("#boutLigne").removeAttr("readonly");
    //Vider son contenu
    $("#boutLigne").val("");
    //Remmtre dispo le numeor de question
    $("#inNumeroQuestion").removeAttr("readonly");
    $("#inNumeroQuestion").val("");
    //Cahcer le reste du formulaire
    $("#ActiverCreation").attr("hidden", true);
    //Changer le titre du label
    $("#lblPhrase").val("Suite Phrase");
    //Terminer dans le controlleur
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
            alert(status);
        },
        error: function (xhr, status) { alert("erreur:" + status); }
    });
    return false;
}

function EnvoyerExercice() {

    var url = "/Exercice/EnvoyerExercice";
    $.ajax({
        type: "POST",
        url: url,
        datatype: "text/plain",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (request) {
            request.setRequestHeader("RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());
        },
        success: function (result) {
            alert(status);
        },
        error: function (xhr, status) { alert("erreur:" + status); }
    });
    return false;
}

function SauvagardeEnvoyerChoix(i, y, z) {

    var url = "/Exercice/AjoutChoixReponse";
    var data = {
        ChoixDeReponse1: i,
        Response: y,
        NoOrdre: z,
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
            alert(status);
        },
        error: function (xhr, status) { alert("erreur:" + status); }
    });
    return false;
}


function CreationLigne(i, y) {

    var url = "/Exercice/CreationLigne";
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
            alert(status);
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