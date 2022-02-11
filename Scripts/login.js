$(document).ready(function () {
    $.ajaxSetup({
        cache: false
     });

    $("input[name='btnLogar']").click(function () {
        var login = $('#login').val();
        var pwd = $('#pwd').val();

        var url = $('#urlLogin').val();

        $.ajax({
            url: url,
            data: {
                login: login,
                pwd: pwd,
                "_": $.now()
            },
            type: "POST",
            error: function () {
                alert("Error ao logar"); 
            },
            success: function (data) {
                $('#conteudo').html(data);
            }

        });

    });

   

});