var formModal;
var formModalDelete;
var idEnCours;
$(function () {
    $("#ModalSupprimerCategorie").modal('hide');
    formModal = fndefinirModal();
    $("#btnCreerCategorie").on('click', function () {
        $(formModal).dialog("open");
        $("form").prop("title", "Ajouter catégorie");
    });
    $("#btnEnvoyer").on("click", function () {
        $("form").prop("title") === "Ajouter catégorie" ? CreerCategorieAjax() : SoumettreModification(idEnCours);
    });
    $("#btnAnnuler").on("click", function () { $(formModal).dialog("close"); });
});

function fndefinirModal() {
    var formModal = $("#divAjouterCategorie").dialog({
        autoOpen: false,
        height: 400,
        width: 350,
        modal: true,//
        close: function () {
            formModal.dialog("close");
        }
    });
    return formModal;
}


function ValiderSupprimer(i) {
    var nbr = DocumentCategorie(i);
    idEnCours = i;
    //Ouvrir le modal de confirmation
    $("#ModalSupprimerCategorie").modal('show');
    $("#phraseAvertissement").text("Êtes-vous certain de vouloir supprimer cette catégorie? Vous avez " + nbr + " documents associé à celui-ci, ils seront tous supprimé si vous continué.")
}

function AnnulerSupprimer() {
    $("#ModalSupprimerCategorie").close();
}

function DocumentCategorie(i) {
    var nbr;
    var url = "/Infos/VoirDocumentsCategorie";
    var data = {
        idCateg: i
    };
    $.ajax({
        data: JSON.stringify(data),
        type: "POST",
        url: url,
        async: false,
        datatype: "text/plain",
        contentType: "application/json; charset=utf-8",
        // beforeSend: function (request) {
        //    request.setRequestHeader("RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());
        // },
        success: function (result) {
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
        datatype: "text",
        contentType: "application/json; charset=utf-8",
        // beforeSend: function (request) {
        //    request.setRequestHeader("RequestVerificationToken", $("input[name='__RequestVerificationToken']").val());
        // },
        success: function (result) {
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
    var url = "/Infos/ModifierCategorie";
    $.ajax({
        type: "GET",
        url: url,
        data: { idCategorie: i },
        //contentType: "application/html; charset=utf-8",
        datatype: "text/plain",
        success: function (result) {
            $("form").prop("title", "Modifier");
            //Affecter la variable globale en cours
            idEnCours = i;
            //alert("Soumettre " + idEnCours);
            formModal.dialog("open");
            //$("#Id").val(result.id);
            //$("#RoleName").val(result.roleName);
            $("#NomCategorie").val(result.NomCategorie);
        },
        error: function (xhr, status) { alert("erreur:" + status); }
    });
}

function SoumettreModification(i) {
    alert($("#NomCategorie").val());
    var url = "/Infos/ModifierCategorie";
    var data = {
        idCateg: i,
        NomCategorie: $("#NomCategorie").val()
    };
    $.ajax({
        data: JSON.stringify(data),
        type: "POST",
        url: url,
        contentType: "application/json; charset=utf-8",
        datatype: "text",
        success: function (result, textStatus) {
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