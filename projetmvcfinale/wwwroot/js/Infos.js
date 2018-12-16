//var formModal;
var formModalDelete;
var idEnCours;
$(function () {
    $("#ModalSupprimerCategorie").modal('hide');
    $("#ModalAjouterCategorie").modal('hide');
    $("#ModalModifierCategorie").modal('hide');
    //formModal = fndefinirModal();
    $("#btnCreerCategorie").on('click', function () {
        //$(formModal).dialog("open");
        //$("form").prop("title", "Ajouter catégorie");
        $("#ModalAjouterCategorie").modal('show');

    });
    $("#btnEnvoyer").on("click", function () {
        $("form").prop("title") === "Ajouter catégorie" ? CreerCategorieAjax() : SoumettreModification(idEnCours);
    });
    //$("#btnAnnuler").on("click", function () { $(formModal).dialog("close"); });

    $("#btnCreerSousCategorie").on('click', function () {
        //$(formModal).dialog("open");
        //$("form").prop("title", "Ajouter catégorie");
        $("#ModalAjouterSousCategorie").modal('show');

    });
    $("#btnCreerNiveau").on('click', function () {
        //$(formModal).dialog("open");
        //$("form").prop("title", "Ajouter catégorie");
        $("#ModalAjouterNiveau").modal('show');

    });
});

//Section catégorie

function ValiderSupprimer(i) {
    var nbr = DocumentCategorie(i);
    idEnCours = i;
    //Ouvrir le modal de confirmation
    $("#ModalSupprimerCategorie").modal('show');
    $("#phraseAvertissement").text("Êtes-vous certain de vouloir supprimer cette catégorie? Vous avez " + nbr + " documents associé à celui-ci, ils seront tous supprimé si vous continué.")
}

function AnnulerSupprimer() {
    $("#ModalSupprimerCategorie").modal('hide');
}

function AnnulerAjouter() {
    $("#ModalAjouterCategorie").modal('hide');
}

function DocumentCategorie(i) {
    var nbr;
    var url = "/Infos/VoirDocumentsCategorie";
    $.ajax({
        data: { idCateg: i },
        type: "POST",
        url: url,
        async: false,
        datatype: "text",
        // beforeSend: function (request) {
        //    request.setRequestHeader("RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());
        // },
        success: function (result) {
            alert(result);
            nbr = result;
        },
        error: function (xhr, status) { alert("erreur:" + status); }
    });
    return nbr;
}

function CreerCategorieAjax() {
    var url = "/Infos/CreerCategorie";
    var data = {
        NomCategorie: $("#NomCategorie").val()
    };
    $.ajax({
        data: JSON.stringify(data),
        type: "POST",
        url: url,
        datatype: "text/plain",
        contentType: "application/json; charset=utf-8",
        // beforeSend: function (request) {
        //    request.setRequestHeader("RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());
        // },
        success: function (result) {
            $("#ModalAjouterCategorie").modal('hide');
            alert("Catégorie ajouté avec succès!");
            //Rafriachir la page
            location.reload();
            return true;
        },
        error: function (xhr, status) { alert("erreur:" + status); }
    });
    //return false;
}

function supprimerCategorieAjax() {
    //Fermer le modal 
    $("#ModalSupprimerCategorie").modal('hide');
    var url = "/Infos/SupprimerCategorie";
    $.ajax({
        data: { idCategorie: idEnCours },
        type: "POST",
        url: url,
        datatype: "text",
        success: function () {
            alert("Catégorie supprimé avec succès!");
            //Retirer de la liste
            $("#listeCategorie tr[id=\"" + idEnCours + "\"]").remove();
            return true;
        },
        error: function (xhr, status) { alert("erreur:" + status); }
    });
    return false;
}

function afficherModification(i) {
    idEnCours = i;
    var url = "/Infos/ModifierCategorie";
    $.ajax({
        type: "GET",
        url: url,
        data: { idCategorie: i },
        contentType: "application/html; charset=utf-8",
        datatype: "text/plain",
        success: function (result) {
            $("form").prop("title", "Modifier");
            $("#ModalModifierCategorie").modal('show');
            alert(result);
            //Affecter la variable globale en cours
            idEnCours = i;
            //formModal.dialog("open");
            //$("#Id").val(result.id);
            //$("#RoleName").val(result.roleName);
            $("#NomCategorieModif").val(result);
        },
        error: function (xhr, status) { alert("erreur:" + xhr + status); }
    });
}

function SoumettreModification() {
    var url = "/Infos/ModifierCategorie";
    var data = {
        idCateg: idEnCours,
        NomCategorie: $("#NomCategorieModif").val()
    };
    $.ajax({
        data: JSON.stringify(data),
        type: "POST",
        url: url,
        contentType: "application/json; charset=utf-8",
        datatype: "text",
        success: function (result, textStatus) {
            $("#ModalAjouterCategorie").modal('hide');
            alert("Modification réussite!");
            var tr = "<td>" + $("#Id").val() + "</td><td>" + $("#RoleName").val() + "</td><td>" + $("#Description").val() + "</td>" +
                "<td>" +
                "<a class=\"btn btn-info\" onclick=\"fneditGet(" + $("#Id").val() + ")\"> <i class=\"glyphicon glyphicon-pencil\"></i> Edit </a>" +
                " <a class=\"btn btn-danger\" onclick=\"fndelete(" + $("#Id").val() + ")\"><i class=\"glyphicon glyphicon-trash\"></i>Delete</a>" +
                "</td>";
            $("#listeTable tr[id=\"" + $("#Id").val() + "\"]").html(tr);
            //Rafriachir la page
            location.reload();
            return true;
        },
        error: function (xhr, status) { alert("erreur:" + status); }
    });
}

//Section Sous catégorie


function CreerSousCategorieAjax() {
    var url = "/Infos/CreerSousCategorie";
    var data = {
        NomSousCategorie: $("#NomSousCategorie").val(),
        idCateg: $("#CategorieAssociee").val(),
    };
    $.ajax({
        data: JSON.stringify(data),
        type: "POST",
        url: url,
        datatype: "text/plain",
        contentType: "application/json; charset=utf-8",
        // beforeSend: function (request) {
        //    request.setRequestHeader("RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());
        // },
        success: function (result) {
            $("#ModalAjouterSousCategorie").modal('hide');
            alert("Sous-catégorie ajouté avec succès!");
            //Rafriachir la page
            location.reload();
            return true;
        },
        error: function (xhr, status) { alert("erreur:" + status); }
    });
    //return false;
}

function AnnulerAjouterSousCategorie() {
    $("#ModalAjouterSousCategorie").modal('hide');
}

function AnnulerModifierSousCategorie() {
    $("#ModalModifierSousCategorie").modal('hide');
}

function ValiderSupprimerSousCategorie(i) {
    var nbr = DocumentSousCategorie(i);
    idEnCours = i;
    //Ouvrir le modal de confirmation
    $("#ModalSupprimerSousCategorie").modal('show');
    $("#phraseAvertissement2").text("Êtes-vous certain de vouloir supprimer cette sous-catégorie? Vous avez " + nbr + " documents associé à celui-ci, ils seront tous supprimés si vous continué.")
}

function DocumentSousCategorie(i) {
    var nbr;
    var url = "/Infos/VoirSousDocumentsCategorie";
    $.ajax({
        data: { idSousCateg: i },
        type: "POST",
        url: url,
        async: false,
        datatype: "text",
        // beforeSend: function (request) {
        //    request.setRequestHeader("RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());
        // },
        success: function (result) {
            alert(result);
            nbr = result;
        },
        error: function (xhr, status) { alert("erreur:" + status); }
    });
    return nbr;
}

function supprimerSousCategorieAjax() {
    //Fermer le modal 
    $("#ModalSupprimerSousCategorie").modal('hide');
    var url = "/Infos/SupprimerSousCategorie";
    $.ajax({
        data: { idSousCategorie: idEnCours },
        type: "POST",
        url: url,
        datatype: "text",
        success: function () {
            alert("Sous catégorie supprimé avec succès!");
            //Retirer de la liste
            $("#listeSousCategorie tr[id=\"" + idEnCours + "\"]").remove();
            return true;
        },
        error: function (xhr, status) { alert("erreur:" + status); }
    });
    return false;
}

function afficherModificationSousCategorie(i) {
    idEnCours = i;
    var url = "/Infos/ModifierSousCategorie";
    $.ajax({
        type: "GET",
        url: url,
        data: { idSousCategorie: i },
        contentType: "application/html; charset=utf-8",
        datatype: "text/plain",
        success: function (result) {
            $("form").prop("title", "Modifier");
            $("#ModalModifierSousCategorie").modal('show');
            alert(result);
            //Affecter la variable globale en cours
            idEnCours = i;
            //formModal.dialog("open");
            //$("#Id").val(result.id);
            //$("#RoleName").val(result.roleName);
            $("#NomSousCategorieModif").val(result);
        },
        error: function (xhr, status) { alert("erreur:" + xhr + status); }
    });
}

function SoumettreModificationSousCategorie() {
    var url = "/Infos/ModifierSousCategorie";
    var data = {
        IdSousCategorie: idEnCours,
        NomSousCategorie: $("#NomSousCategorieModif").val()
    };
    $.ajax({
        data: JSON.stringify(data),
        type: "POST",
        url: url,
        contentType: "application/json; charset=utf-8",
        datatype: "text",
        success: function (result, textStatus) {
            $("#ModalModifierCategorie").modal('hide');
            alert("Modification réussite!");
            var tr = "<td>" + $("#Id").val() + "</td><td>" + $("#RoleName").val() + "</td><td>" + $("#Description").val() + "</td>" +
                "<td>" +
                "<a class=\"btn btn-info\" onclick=\"fneditGet(" + $("#Id").val() + ")\"> <i class=\"glyphicon glyphicon-pencil\"></i> Edit </a>" +
                " <a class=\"btn btn-danger\" onclick=\"fndelete(" + $("#Id").val() + ")\"><i class=\"glyphicon glyphicon-trash\"></i>Delete</a>" +
                "</td>";
            $("#listeSousCategories tr[id=\"" + $("#Id").val() + "\"]").html(tr);
            //Rafriachir la page
            location.reload();
            return true;
        },
        error: function (xhr, status) { alert("erreur:" + status); }
    });
}

//partie niveau de difficulté
function ValiderSupprimerNiveau(i) {
    var nbr = DocumentNiveau(i);
    idEnCours = i;
    //Ouvrir le modal de confirmation
    $("#ModalSupprimerNiveau").modal('show');
    $("#phraseAvertissement3").text("Êtes-vous certain de vouloir supprimer cette catégorie? Vous avez " + nbr + " documents associé à celui-ci, ils seront tous supprimé si vous continué.")
}

function DocumentNiveau(i) {
    var nbr;
    var url = "/Infos/VoirDocumentsNiveau";
    $.ajax({
        data: { idNiveau: i },
        type: "POST",
        url: url,
        async: false,
        datatype: "text",
        // beforeSend: function (request) {
        //    request.setRequestHeader("RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());
        // },
        success: function (result) {
            alert(result);
            nbr = result;
        },
        error: function (xhr, status) { alert("erreur:" + status); }
    });
    return nbr;
}

function AnnulerSupprimerNiveau() {
    $("#ModalSupprimerNiveau").modal('hide');
}

function supprimerNiveauAjax() {
    //Fermer le modal 
    $("#ModalSupprimerNiveau").modal('hide');
    var url = "/Infos/SupprimerNiveau";
    $.ajax({
        data: { idNiveau: idEnCours },
        type: "POST",
        url: url,
        datatype: "text",
        success: function () {
            alert("Niveau supprimé avec succès!");
            //Retirer de la liste
            $("#listeNiveau tr[id=\"" + idEnCours + "\"]").remove();
            return true;
        },
        error: function (xhr, status) { alert("erreur:" + status); }
    });
    return false;
}

function AnnulerSupprimerNiveau() {
    $("#ModalSupprimerNiveau").modal('hide');
}

function afficherModificationNiveau(i) {
    idEnCours = i;
    var url = "/Infos/ModifierNiveau";
    $.ajax({
        type: "GET",
        url: url,
        data: { idNiveau: i },
        contentType: "application/html; charset=utf-8",
        datatype: "text/plain",
        success: function (result) {
            $("form").prop("title", "Modifier");
            $("#ModalModifierNiveau").modal('show');
            //Affecter la variable globale en cours
            idEnCours = i;
            //formModal.dialog("open");
            //$("#Id").val(result.id);
            //$("#RoleName").val(result.roleName);
            $("#NomNiveauModif").val(result);
        },
        error: function (xhr, status) { alert("erreur:" + xhr + status); }
    });
}

function SoumettreModificationNiveau() {
    var url = "/Infos/ModifierNiveau";
    var data = {
        IdDifficulte: idEnCours,
        NiveauDifficulte: $("#NomNiveauModif").val()
    };
    $.ajax({
        data: JSON.stringify(data),
        type: "POST",
        url: url,
        contentType: "application/json; charset=utf-8",
        datatype: "text",
        success: function (result, textStatus) {
            $("#ModalAjouterNiveau").modal('hide');
            alert("Modification réussite!");
            var tr = "<td>" + $("#Id").val() + "</td><td>" + $("#RoleName").val() + "</td><td>" + $("#Description").val() + "</td>" +
                "<td>" +
                "<a class=\"btn btn-info\" onclick=\"fneditGet(" + $("#Id").val() + ")\"> <i class=\"glyphicon glyphicon-pencil\"></i> Edit </a>" +
                " <a class=\"btn btn-danger\" onclick=\"fndelete(" + $("#Id").val() + ")\"><i class=\"glyphicon glyphicon-trash\"></i>Delete</a>" +
                "</td>";
            $("#listeNiveau tr[id=\"" + $("#Id").val() + "\"]").html(tr);
            //Rafriachir la page
            location.reload();
            return true;
        },
        error: function (xhr, status) { alert("erreur:" + status); }
    });
}

function CreerNiveauAjax(){
    var url = "/Infos/CreerNiveau";
    var data = {
        NiveauDifficulte: $("#NomNiveau").val()
    };
    $.ajax({
        data: JSON.stringify(data),
        type: "POST",
        url: url,
        datatype: "text/plain",
        contentType: "application/json; charset=utf-8",
        // beforeSend: function (request) {
        //    request.setRequestHeader("RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());
        // },
        success: function (result) {
            $("#ModalAjouterNiveau").modal('hide');
            alert("Niveau ajouté avec succès!");
            //Rafriachir la page
            location.reload();
            return true;
        },
        error: function (xhr, status) { alert("erreur:" + status); }
    });
    //return false;
}