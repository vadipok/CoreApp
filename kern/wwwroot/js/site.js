$('#validatedCustomFile').on('change', function () {
    //get the file name
    var input = $(this),
        numFiles = input.get(0).files ? input.get(0).files.length : 1,
        label = input.val().replace(/\\/g, '/').replace(/.*\//, '');
    //get the file name
    var fileName = $(this).val();
    //replace the "Choose a file" label
    $(this).next('.custom-file-label').html(label);
})


$(document).on('change', ':file', function () {
    var input = $(this),
        numFiles = input.get(0).files ? input.get(0).files.length : 1,
        label = input.val().replace(/\\/g, '/').replace(/.*\//, '');
    input.trigger('fileselect', [numFiles, label]);
});

$(document).ready(
    $(function () {
        $.ajaxSetup({ cache: false });

        $(document).on('click', '.modItem', function (e) {
            e.preventDefault();
            $.get(this.href, function (data) {
                $('#dialogContent').html(data);
                $("#modDialog").modal('show');
                $('#modDialog').on('shown.bs.modal', function (e) {
                    $.validator.unobtrusive.parse(document);
                    var form = $('.container').find("#List_Form");
                    form.valid();
                })
                
            });
        });
    })
)


var checkedValue = null;
var inputElements = document.getElementsByClassName('user-roles-checkbox');
for (var i = 0; inputElements[i]; ++i) {
    if (inputElements[i].checked) {
        checkedValue = inputElements[i].value;
        print(checkedValue);
        break;
    }
}

$('.user-roles-checkbox input[type="checkbox"]').on('change', function () {
    alert('Yaay, I was changed');
});

$(document).ready(function () {
    $('.my-checkbox').change(function () {
        alert($(this).prop('checked'))
    });

    $(".custom-control-input").change(function () {
        if (this.checked) {
            alert($(this).val());
        }
    });
})

//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// Изменение роли пользвателя в БД. Срабатывает при переключении списка прав.
// Открывается из модального окна EditUser контроллера AccountController.
// Вызывается методом AddDellRoleUser контроллера AccountController.
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
$('.container').on('change', '.user-role-check', function () {
    $.post("/Account/AddDellRoleUser",
        {
            domainLogin: $('#DomainLogin').val(),
            roleName: $(this).prop('id')
        },
        function (response) {
            if (response == 1) {
                alert("Роль успешно изменена");
            } else {
                alert("Ошибка! Обратитесь к тому дурочку, который написал эту каку!");
            }
        });
});

// Показать результаты ajax-запроса
function showLoader() {
    $('#results').show();
};

// Скрыть результаты ajax-запроса
function hideLoader() {
    $('#results').hide();
    down();
};

function reload_fields() {
    $.ajax({
        url: '/DataBank/AjaxListFields',
        contentType: 'application/html; charset=utf-8',
        type: 'GET',
        dataType: 'html',
        data: {
            nameField: $('#NameField').val(),
            deposit: $('#Deposit').val()
        },
        success: function (result) {
            $('#resultSearch').html(result);
        },
        error: function (xhr, status) {
            // alert(status);
        }
    });
}

reload_fields();

// public ActionResult AjaxListFields(string nameField, string deposit)
$(".editor-modify").keyup(function (e) {
    e.preventDefault();
    reload_fields();
});


function showDialog() {
    // Форма для редактирования данных
    var form = $('.container').find("#List_Form");

    // Свойства формы
    var urlForm = form.attr('action'); // Извлекаем адрес
    var dataForm = form.serialize(); // Извлекаем заполненный данные из формы
    var methodForm = form.attr('method'); // Извлекаем метод Get или Post

    // Проверка правильности заполненных форм
    if (form.valid()) {
        // Отправляем запрос в контроллер
        $.ajax({
            url: urlForm, // Устанавливаем адрес
            data: dataForm, // Данные
            type: methodForm, // Метод
            success: function (result) {
                // Вывод на экран сообщения об успешном выполнении
                var msg = alertify.success();
                msg.delay(3).setContent(result);
            },
            error: function (xhr, status) {
                // Вывод на экран сообщения об ошибке
                var msg = alertify.error();
                msg.delay(3).setContent("An error occurred. Contact your administrator.");
            }
        });
    }
    else {
        // Сообщение об о неуспешном прохождении проверки в форме
        var msg = alertify.error();
        msg.delay(3).setContent("Valid: " + form.valid());
    }
};





