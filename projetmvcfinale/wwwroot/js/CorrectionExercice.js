$(function () {
    alert("marche");
})

function SoummettreCorrection() {

    var listeReponses = new Array();
    //Pour chauqe choix de reponse du document
    $("#Exercice select").each(function () {
        //Ajouter a  la liste la reponse
        listeReponses.push($(this).val());
    });
    //Corriger
    CorrigerExercice(listeReponses);
}

function CorrigerExercice(i) {
    var liste = new Array();
    var url = "/Exercice/Correction";
    $.ajax({
        data: { ListReponse: i },
        type: "POST",
        async: false,
        url: url,
        dataType: "json",
        //contentType: "application/json; charset=utf-8",        
        success: function (data) {
            liste = data;
        },
        error: function (xhr, status) { alert("erreur:" + status); }
    });
    return liste;

}