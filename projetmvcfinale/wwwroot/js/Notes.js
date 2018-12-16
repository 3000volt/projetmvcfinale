﻿$(function () {
    alert("test");
    ChagerAjax();
    $("#IdCateg").on('change', function () {
        ChagerAjax();
    });

    $("#CategorieAssociee").val($("#IdCateg option:selected").text());
    $("#IdCateg").on('change', function () {
        $("#CategorieAssociee").val($("#IdCateg option:selected").text());
    });

    //$("#CategorieAssociee").val($("#IdCateg option:selected").text());

});

//Charger le selectlist du groupe
function ChagerAjax() {
    var url = "/NoteDeCours/RetirerSousCateg";
    $.ajax({
        data: { id: $("#IdCateg").val() },
        type: "POST",
        async: false,
        url: url,
        datatype: "json",
        success: function (data) {
            $("select[id='SousCategorie']").empty();
            var choix = '';
            for (var i = 0; i < data.length; i++) {
                choix += '<option value="' + data[i] + '">' + data[i] + '</option>';
            }
            $("select[id='SousCategorie']").append(choix);
            //https://stackoverflow.com/questions/3446069/populate-dropdown-select-with-array-using-jquery
        },
        error: function (xhr, status) { alert("erreur:" + status); }
    });
}


function CreerSousCategorieAjax() {
    var url = "/NoteDeCours/CreerSousCategorie";
    var data = {
        NomSousCategorie: $("#NomSousCategorie").val(),
        idCateg: $("#IdCateg").val(),
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