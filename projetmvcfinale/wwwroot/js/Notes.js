$(function () {
    alert("test2");
    ChagerAjax();
    $("#IdCateg").on('change', function () {
        ChagerAjax();
    });

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
