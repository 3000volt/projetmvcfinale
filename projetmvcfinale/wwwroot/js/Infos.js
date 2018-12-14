var formModal;
$(function () {
   // $("#divAjouterCategorie").attr("hidden", true);
    formModal = fndefinirModal();
    $("#createRoleModal").on('click', function () {
        $(formModal).dialog("open");
        //$("form").prop("title", "Add");
       // $("#divAjouterCategorie").removeAttr('hidden');
    });
});

function fndefinirModal() {
    var formModal = $("#divAjouterCategorie").dialog({
        autoOpen: false,
        height: 400,
        width: 350,
        modal: true,
        close: function () {
            formModal.dialog("close");
        }
    });
    return formModal;
}

function CreerCategorieAjax(i) {
    alert();
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
            // alert(status);
            return true;
        },
        error: function (xhr, status) { alert("erreur:" + status); }
    });
    return false;
}