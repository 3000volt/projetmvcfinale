$(function () {
    //Variable globale permettant de compter l'ordre d'un chois de réponse dans une phrase
    var compteur;
});


function CommencerExercice() {
    //Commencer le compteur
    compteur = 0;
    //Valider que ce numéro est disponible
    var numero = $("#inNumeroQuestion").val();
    //Voir si le numéro entré est valide
    if (VerifierNumero(numero) == true) {
        //Mettre au textarea
        //Si celui-ci est vide, ajouter simplement le numéro
        if ($("#txtExercice").val() == "") {
            $("#txtExercice").val(numero + " - ");
        }
        //sinon, prendre la valeur actuelle dans une variable, ajouter le numéro et mettre cette variable au textarea
        else {
            var textInitiale = $("#txtExercice").val();
            $("#txtExercice").val(textInitiale + "\n" + numero + " - ");
            //https://css-tricks.com/forums/topic/solved-jquery-append-text-to-textarea/
        }
        //Desactiver / Activer les trucs appropriés
        $("#btnCreerQuestion").attr('disabled', true);
        $("#inNumeroQuestion").attr("readonly", true);
        $("#divActiverCreation").removeAttr('hidden');
        $("#btnExerciceTermine").prop("disabled", true);

    }
    //Si le numéro n'est pas valide
    else {
        alert("Vous devez entrer un numero valide qui n'a pas été utilisé");
    }
}

//fonction pour retirer un choix de réponse
function RetirerDerneirChoix() {
    //S'il y a au moins 3 colonnes dans le tableau (nous devons inclure le titre)
    if ($("#tbChoixReponses tr").length != 3) {
        $("#tbChoixReponses tr:last").remove();
    }
    else {
        //Sinon, empêcher de retirer une colonne
        alert("Vous devez concerver au moins 2 choix de réponse!");
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
    //if ($("#tbChoixReponses tr:last-child td input").val() != "") {
    var remplis = true
    $("#tbChoixReponses tr").each(function () {
        if ($(this).val() == "") {
            remplis == false;
        }
    });
    if (remplis == true) {
         $("#tbChoixReponses").append('<tr><td><input asp-for="ChoixDeReponse" /></td><td></tr>');
    }
    //Si une colonne est vide
    else {
        alert("Remplissez toutes les colonnes!");
    }
}

function OuvrirBonneReponse() {
    var tab = new Array();
    $("#tbChoixReponses tr").not(':first').each(function () {
        //Si une tavle est vide
        if ($(this).find("td input").val() == "") {
            alert("Les champs de choix de réponse doivent être tous remplis!");
            return false;
            //https://stackoverflow.com/questions/4996521/jquery-selecting-each-td-in-a-tr            

        }
        tab.push($(this).find("td input").val());
    });
    //Voir si certaine case ont la même valeur
    for (var i = 0; i < tab.length; i++) {
        for (var j = 0; j < tab.length; j++) {
            if (i != j) {
                if (tab[i] == tab[j]) {
                    alert("Vous ne pouvez pas envoyer la même réponse plus d'une fois!");
                    return false;
                }
            }
        }
    }
    //https://stackoverflow.com/questions/19655975/check-if-an-array-contains-duplicate-values/45803283
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

function SupprimerLigne() {
    //Afficher le select pour sélectionner le ligne a effacer
    $("#selectLigneSupprimer").removeAttr("hidden");
    //Peupler le selectList
    $("#selectLigneSupprimer").empty();
    var tableau = RetirerListeNumero();
    alert(tableau[0]);
    var choix = '';
    for (var i = 0; i < tableau.length; i++) {
        choix += '<option value="' + tableau[i] + '">' + tableau[i] + '</option>';
    }
    $("#selectLigneSupprimer").append(choix);
    //https://stackoverflow.com/questions/3446069/populate-dropdown-select-with-array-using-jquery
    //Afficher le bouton pour supprimer
    $("#btnConfirmerSupprimer").removeAttr("hidden");
}

function ConfrimerSupprimerLigne() {
    //Trouver la ligne à supprimer
    var ligne = $("#selectLigneSupprimer option:selected").val();
    alert(ligne);
    //Supprimer la ligne
    SupprimerLigneServeur(ligne);
    //Retirer du textarea
    var text = $("#txtExercice").val();
    var debut = text.indexOf(ligne);
    var fin = text.indexOf('\n', debut);
    //var phrase = text.split($("#txtExercice").val().substring(debut, fin));
    var phrase = text.replace($("#txtExercice").val().substring(debut, fin), "(Ligne supprimée)");
    $("#txtExercice").val(phrase);
    //https://stackoverflow.com/questions/9313071/retrieve-substring-between-two-characters
    //Voir s'il reste d'autre ligne

}

function SupprimerLigneServeur(i) {
    alert(i);
    var url = "/Exercice/SupprimerLigne";
    $.ajax({
        data: { Ligne: i },
        type: "POST",
        url: url,
        datatype: "text/plain",       
        success: function (result) {
        },
        error: function (xhr, status) { alert("erreur:"); }
    });
}


function RetirerListeNumero() {
    var liste = new Array();
    var url = "/Exercice/ListeNumero";
    $.ajax({
        type: "POST",
        async: false,
        url: url,
        dataType: "json",
        success: function (data) {
            liste = data;
        },
        error: function (xhr, status) { alert("erreur:" + status); }
    });
    return liste;
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
    $("#txtExercice").val(textInitiale + $("#boutLigne").val() + "(" + bonneReponse + ")");
    //Vider son contenu
    $("#boutLigne").val("");
    //Ne plus mettre disponible le bouton de fin de phrase
    $("#btnFinPhrase").attr("disabled", true);
    //mettre le bouton d'effacement de la phrase visible
    $("#btnSupprimerLigne").removeAttr("disabled");
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

//fonction pour valider si le numéro est valide (et s'il s'agit d'un numéro)
function VerifierNumero(i) {
    var bool = false;
    var url = "/Exercice/VerifierNumero";
    $.ajax({
        data: { numero: i },
        type: "POST",
        async: false,
        url: url,
        dataType: "json",
        success: function (data) {
            bool = data;
        },
        error: function (xhr, status) { alert("erreur:" + status); }
    });
    //Retouner la validité
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