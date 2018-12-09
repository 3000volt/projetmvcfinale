$(function () {
    alert("bb");
})

function SoummettreCorrection() {

    var listeReponses = new Array();
    //Pour chauqe choix de reponse du document
    $("#Exercice select").each(function () {
        //Ajouter a  la liste la reponse
        listeReponses.push($(this).val());
    });
    //Corriger
    //CorrigerExercice(listeReponses);
    var correction = CorrigerExercice(listeReponses);
    var compteur = 0;
    //Indiquer les corrections
    $("#Exercice tbody tr").each(function () {
        //alert($(this + " td p select").val());
        $(this).find("td p select").each(function () {
            if (correction[compteur] == true) {
                alert("bon!");
                $(this).css('backgroundColor', 'green');
            }
            else {
                alert("pas bon");
                $(this).css('backgroundColor', 'red');
            }
            //https://stackoverflow.com/questions/5068087/set-background-colour-of-select-to-selected-option-in-jquery
            //Ajouter au compteur
            compteur++;
        });
    });
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
            //for (var i in data) {
            //alert(data);
            //liste.push(data[i]);
            //alert(data);
            //}
            liste = data;
            //alert(data);
        },
        error: function (xhr, status) { alert("erreur:" + status); }
    });
    return liste;
}