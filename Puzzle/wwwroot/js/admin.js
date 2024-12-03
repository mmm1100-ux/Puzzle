

$('.search-designer').change(function () {
    let li = $('.li-project');
    for (i = 0; i < li.length; i++) {
        if ($('.search-designer').val()) {
            let input = $(li[i]).find('.designer-name');
            let text = input.data('designer-name');
            if (text.toUpperCase().indexOf($('.search-designer').val().toUpperCase()) > -1) {
                li[i].style.display = '';
            } else {
                li[i].style.display = 'none';
            }
        } else {
            li[i].style.display = '';
        }
    }
});

$('.search-payment-type').change(function () {
    if ($('.search-payment-type').val()) {
        $('tbody > tr').hide()
        $('*[data-payment-type="' + $('.search-payment-type').val() + '"]').parent().show();
    }
    else{
        $('tbody > tr').show()
    }
});

$('.search-payment-designer').change(function () {
    if ($('.search-payment-designer').val()) {
        $('tbody > tr').hide()
        $('*[data-payment-designer="' + $('.search-payment-designer').val() + '"]').parent().show();
    }
    else {
        $('tbody > tr').show()
    }
});

$('body').on('click', '.change-delivery, .change-status', function () {

    $('.OverHide').show();
    $('.Loading').show();

    let projectId = $(this).data("id");
    let deliveryType = $(this).data("type");
    let This = $(this);

    $.post({
        data: { ProjectId: projectId, DeliveryType: deliveryType },
        url: "/Home/Delivery",
        success: function (result, status, xhr) {
            if (This.hasClass('change-status')) {
                This.toggleClass('change-status-active');
            } else {
                This.toggleClass('change-delivery-active');
            }

            $('.OverHide').hide();
            $('.Loading').hide();
        }
    });
});

$('body').on('click', '.favorite', function () {

    $('.OverHide').show();
    $('.Loading').show();

    let projectId = $(this).data("id");
    let This = $(this);

    $.post({
        data: { Id: projectId },
        url: "/project/favorite",
        success: function (result, status, xhr) {
            This.toggleClass('favorite-active');

            $('.OverHide').hide();
            $('.Loading').hide();
        }
    });
});

$('body').on('click', '.btn-popup-save', function () {

    $('.pop-up').hide();
    $('.Loading').show();
    $.post({
        data: { description: $('.popup-textarea').val(), id: $('#description-id').val() },
        url: "/Home/Description",
        success: function (result, status, xhr) {
            $('.OverHide').hide();
            $('.Loading').hide();
            $('*[data-description-id="' + $('#description-id').val() + '"]').data('description', $('.popup-textarea').val());
            if ($('.popup-textarea').val()) {
                $('*[data-description-id="' + $('#description-id').val() + '"]').addClass('btn-print-active');
            } else {
                $('*[data-description-id="' + $('#description-id').val() + '"]').removeClass('btn-print-active');
                $('*[data-description-id="' + $('#description-id').val() + '"]').addClass('btn-print');
            }
        }
    });
});

$('.btn-date').click(function () {
    let li = $('.li-project');
    for (i = 0; i < li.length; i++) {
        li[i].style.display = '';
    }
    $('.btn-delivery, .btn-designer').css("color", "#a1a6bb");
    $(this).css("color", "#3a405b");
});

$('.btn-delivery').click(function () {
    let li = $('.li-project');
    for (i = 0; i < li.length; i++) {
        let any = li[i].getElementsByClassName('no-delivery change-delivery-active');
        if (any.length) {
            li[i].style.display = 'none';
        } else {
            li[i].style.display = '';
        }
    }
    $('.btn-designer, .btn-date').css("color", "#a1a6bb");
    $(this).css("color", "#3a405b");
});

$('.btn-designer').click(function () {
    let li = $('.li-project');
    for (i = 0; i < li.length; i++) {
        let any = li[i].getElementsByClassName('designer-empty');
        if (any.length) {
            li[i].style.display = '';
        } else {
            li[i].style.display = 'none';
        }
    }
    $('.btn-delivery, .btn-date').css("color", "#a1a6bb");
    $(this).css("color", "#3a405b");
});

let firstShowChat = false;

$('.btn-project-edit').click(function () {
    $('#project-chat-box').hide()

    $('.repair-box').hide()

    $('#media-box').hide()
    $('#media-box').empty()
    $('#info').show()
    $('.btn-project-media').css("color", "#a1a6bb")
    $('.btn-project-chat').css("color", "#a1a6bb")
    $('.repair-box-head').css("color", "#a1a6bb")
    $(this).css("color", "#3a405b")
})

$('.btn-project-chat').click(function () {

    $('#media-box').hide();
    $('#media-box').empty();
    $('.repair-box').hide();
    $('#info').hide();
    $('#project-chat-box').show();
    $('.btn-project-media').css("color", "#a1a6bb");
    $('.btn-project-edit').css("color", "#a1a6bb");
    $('.repair-box-head').css("color", "#a1a6bb");
    $(this).css("color", "#3a405b");

    if (firstShowChat == false) {

        firstShowChat = true;
        $('.user-chats').scrollTop($('.user-chats > .chats').height());
        $('.OverHide').show();
        $('.Loading').show();

        let projectId = document.getElementById("projectId").value
        let conversationLastId = document.getElementById("conversationLastId").value

        if (window.location.href.indexOf('?id=') != -1) {
            const urlParams = new URLSearchParams(window.location.search);
            projectId = urlParams.get('id');
        }

        $.post({
            data: {
                ProjectId: projectId, ConversationId: conversationLastId
            },
            url: '/Conversation/Read',
            success: function (result, status, xhr) {
                $('.OverHide').hide();
                $('.Loading').hide();
            }
        });
    }
})

$('.repair-box-head').click(function () {
    $('.repair-box').hide()
    $('#project-chat-box').hide()
    $('#media-box').hide()
    $('#media-box').empty()
    $('#info').hide()

    $('.repair-box-head').css("color", "#a1a6bb")
    $('.btn-project-chat').css("color", "#a1a6bb")
    $('.btn-project-media').css("color", "#a1a6bb")
    $('.btn-project-edit').css("color", "#a1a6bb")

    $(this).show()
    $(this).css("color", "#3a405b")

    let id = this.id.replace('repair-index-', 'repair-box-index-')
    $('#' + id).show()
})

function cancelProject(projectId) {
    $('.OverHide').show();
    $('.Loading').show();

    let deliveryType = 'Cancel';

    $.post({
        data: { ProjectId: projectId, DeliveryType: deliveryType },
        url: "/Home/Delivery",
        success: function (result, status, xhr) {
            //if (This.hasClass('change-status')) {
            //    This.toggleClass('change-status-active');
            //} else {
            //    This.toggleClass('change-delivery-active');
            //}

            if ($('#cancel-' + projectId).text() == 'فعال') {
                $('#cancel-' + projectId).text('غیرفعال')
                $('#cancel-' + projectId).removeClass("btn-deactive-outline")
                $('#cancel-' + projectId).addClass("btn-danger-outline")
            } else {
                $('#cancel-' + projectId).text('فعال')
                $('#cancel-' + projectId).removeClass("btn-danger-outline")
                $('#cancel-' + projectId).addClass("btn-deactive-outline")
            }

            $('.OverHide').hide();
            $('.Loading').hide();
        }
    });
}

function archiveProject(projectId) {

    $('.OverHide').show();
    $('.Loading').show();

    let deliveryType = 'Archive';

    $.post({
        data: { ProjectId: projectId, DeliveryType: deliveryType },
        url: "/Home/Delivery",
        success: function (result, status, xhr) {

            if ($('#archive-' + projectId).text() == 'آرشیو کردن') {
                $('#archive-' + projectId).text('آرشیو شده')
                $('#archive-' + projectId).removeClass("btn-deactive-outline")
                $('#archive-' + projectId).addClass("btn-danger-outline")
            } else {
                $('#archive-' + projectId).text('آرشیو کردن')
                $('#archive-' + projectId).removeClass("btn-danger-outline")
                $('#archive-' + projectId).addClass("btn-deactive-outline")
            }

            $('.OverHide').hide();
            $('.Loading').hide();
        }
    });
}


$('.btn-project-media').click(function () {
    $('.OverHide').show()
    $('.Loading').show()

    $.post({
        data: {
            ProjectId: document.getElementById("projectId").value
        },
        url: '/project/get-media',
        success: function (result, status, xhr) {
            $('#media-box').html(result)
            $('#info').hide()
            $('#project-chat-box').hide()
            $('.repair-box').hide()
            $('#media-box').show()
            $('.repair-box-head').css("color", "#a1a6bb")
            $('.btn-project-edit').css("color", "#a1a6bb");
            $('.btn-project-chat').css("color", "#a1a6bb");
            $('.btn-project-media').css("color", "#3a405b");

            mediaGallery();

            $('.OverHide').hide();
            $('.Loading').hide();
            $('.loading-other').hide()
        }
    })
});

function GoBack() {
    if (sessionStorage.getItem("back")) {
        window.history.back();
    } else {
        location.href = '/project';
    }
}

$('#update-project-form').submit(function (e) {
    e.preventDefault();

    var Mobile = $('#Mobile').val();
    $('#Mobile').css('border-color', '#e4e7ef');
    $('.Mobile_Label').css('color', '#a1a6bb');
    $('.Mobile_Label').text('شماره تماس');

    var totalPrice = $('#TotalPrice').val();
    var approximate = $('#Approximate').val();
    var payment = $('.PaymentPrice').val();

    var projectId = $('#projectId').val();

    if ((Mobile.length < 10) || (Mobile.substr(0, 1) == "0" && Mobile.length < 11)) {
        $('#Mobile').css('border-color', '#fe5339');
        $('.Mobile_Label').text('شماره تماس را به درستی وارد کنید!');
        $('.Mobile_Label').css('color', '#fe5339');
    } else if (($('#ProjectCategory').val() != 'Cancel') && (!totalPrice || parseInt(totalPrice) <= 0) && (!approximate || parseInt(approximate) <= 0)) {
        $('#AlertMessage').css('background-color', '#fe5339');
        $('#AlertMessage').html('<p>هزینه کل یا هزینه پیش بینی شده را مشخص کنید.</p>').fadeIn(500).delay(3000).fadeOut(500);
    } else {
        $('.OverHide').show();
        $('.Loading').show();

        $.post({
            data: $('#update-project-form').serialize(),
            url: '/edit/' + projectId,
            success: function (result, status, xhr) {
                if (result == 2) {
                    GoBack();
                }
                else if (result == 5) {
                    $('#AlertMessage').css('background-color', '#fe5339');
                    $('#AlertMessage').html('<p>مشتری یافت نشد</p>').fadeIn(500).delay(3000).fadeOut(500);
                    $('.OverHide').hide();
                    $('.Loading').hide();
                } else if (result == 4) {
                    $('#AlertMessage').css('background-color', '#fe5339');
                    $('#AlertMessage').html('<p>مبلغ درخواستی بیشتر از اعتبار مشتری می باشد</p>').fadeIn(500).delay(3000).fadeOut(500);
                    $('.OverHide').hide();
                    $('.Loading').hide();
                }
                else if (result == 6) {
                    $('#AlertMessage').css('background-color', '#fe5339');
                    $('#AlertMessage').html('<p>مجموع مبلغ دریافتی پروژه نمیتواند منفی باشد</p>').fadeIn(500).delay(3000).fadeOut(500);
                    $('.OverHide').hide();
                    $('.Loading').hide();
                }
                else {
                    $('#AlertMessage').css('background-color', '#fe5339');
                    $('#AlertMessage').html('<p>خطای پیش بینی نشده</p>').fadeIn(500).delay(3000).fadeOut(500);
                    $('.OverHide').hide();
                    $('.Loading').hide();
                }
            }
        });
    }
});

$('#add-payment').click(function () {
    var html = $('#payment-box').html();
    $('#add-payment').before(html.replace("display: block;", ""));
});

$('#TotalPrice').keyup(function () {
    $('#TotalPrice').val($('#TotalPrice').val().replace(/[^0-9\.]+/g, '').replace(/,/g, '').replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    if ($('#TotalPrice').val() && parseInt($('#TotalPrice').val()) > 0) {
        $('#Approximate').prop('readonly', true);
    } else {
        $('#Approximate').prop('readonly', false);
    }
});

$(".project-form").on("keyup", ".PaymentPrice", function () {
    $(this).val($(this).val().replace(/[^0-9\.]+/g, '').replace(/,/g, '').replace(/\B(?=(\d{3})+(?!\d))/g, ","));
});

$('#Approximate').keyup(function () {
    $(this).val($(this).val().replace(/[^0-9\.]+/g, '').replace(/,/g, '').replace(/\B(?=(\d{3})+(?!\d))/g, ","));
});

$('.history').click(function () {
    $('.notification-drop-down').show()
});

$(document).mouseup(function (e) {
    var container = $('.notification-drop-down');
    if (container.is(":visible")) {
        if (!container.is(e.target) && container.has(e.target).length === 0) {
            container.hide();
        }
    }
});

if (window.location.href.indexOf('?tab=media') != -1) {
    $('.btn-project-media').click()
}
else if (window.location.href.indexOf('tab=conversation') != -1) {
    $('.btn-project-chat').click()
}
else if (window.location.href.indexOf('?id=') != -1) {
    const urlParams = new URLSearchParams(window.location.search);
    const id = urlParams.get('id');
    $('#repair-index-' + id).click()
}

$('.rapair-form').submit(function (e) {
    e.preventDefault();

    $('.OverHide').show();
    $('.Loading').show();

    $.post({
        data: $(this).serialize(),
        url: '/repair/edit',
        success: function (result, status, xhr) {
            if (result == 2) {
                GoBack();
            }
            else if (result == 5) {
                $('#AlertMessage').css('background-color', '#fe5339');
                $('#AlertMessage').html('<p>مشتری یافت نشد</p>').fadeIn(500).delay(3000).fadeOut(500);
            } else if (result == 4) {
                $('#AlertMessage').css('background-color', '#fe5339');
                $('#AlertMessage').html('<p>مبلغ درخواستی بیشتر از اعتبار مشتری می باشد</p>').fadeIn(500).delay(3000).fadeOut(500);
            }
            else if (result == 6) {
                $('#AlertMessage').css('background-color', '#fe5339');
                $('#AlertMessage').html('<p>مجموع مبلغ دریافتی پروژه نمیتواند منفی باشد</p>').fadeIn(500).delay(3000).fadeOut(500);
            }
            else {
                $('#AlertMessage').css('background-color', '#fe5339');
                $('#AlertMessage').html('<p>خطای پیش بینی نشده</p>').fadeIn(500).delay(3000).fadeOut(500);
            }
        }
    });
});

$('.add-payment').click(function () {
    let id = this.id
    var html = $('#payment-box' + id).html();
    $(this).before(html);
});

for (var ix = 0; ix < $('.date-picker').length; ix++) {
    kamaDatepicker($('.date-picker')[ix].getAttribute('id'), { markToday: true, markHolidays: true, highlightSelectedDay: true });
}

$(".rapair-form").on("keyup", ".PaymentPrice", function () {
    $(this).val($(this).val().replace(/[^0-9\.]+/g, '').replace(/,/g, '').replace(/\B(?=(\d{3})+(?!\d))/g, ","));
});

$('.rapair-form').on("keyup", ".TotalPrice", function () {
    $(this).val($(this).val().replace(/[^0-9\.]+/g, '').replace(/,/g, '').replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    if ($(this).val() && parseInt($(this).val()) > 0) {
        $(this).parent().parent().find('.Approximate').prop('readonly', true);
    } else {
        $(this).parent().parent().find('.Approximate').prop('readonly', false);
    }
});

$('.rapair-form').on("keyup", ".Approximate", function () {
    $(this).val($(this).val().replace(/[^0-9\.]+/g, '').replace(/,/g, '').replace(/\B(?=(\d{3})+(?!\d))/g, ","));
});

function GoBack() {
    if (sessionStorage.getItem("back")) {
        window.history.back();
    } else {
        location.href = '/project';
    }
}

$('#add-project-form').submit(function (e) {
    e.preventDefault();
    var Mobile = $('#Mobile').val();
    $('#Mobile').css('border-color', '#e4e7ef');
    $('.Mobile_Label').css('color', '#a1a6bb');
    $('.Mobile_Label').text('شماره تماس');

    var totalPrice = $('#TotalPrice').val();
    var approximate = $('#Approximate').val();
    var payment = $('.PaymentPrice').val();

    if ((Mobile.length < 10) || (Mobile.substr(0, 1) == "0" && Mobile.length < 11)) {
        $('#Mobile').css('border-color', '#fe5339');
        $('.Mobile_Label').text('شماره تماس را به درستی وارد کنید!');
        $('.Mobile_Label').css('color', '#fe5339');
    } else if (($('#ProjectCategory').val() != 'Amendment') && (!totalPrice || parseInt(totalPrice) <= 0) && (!approximate || parseInt(approximate) <= 0)) {
        $('#AlertMessage').css('background-color', '#fe5339');
        $('#AlertMessage').html('<p>هزینه کل یا هزینه پیش بینی شده را مشخص کنید.</p>').fadeIn(500).delay(3000).fadeOut(500);
    } else if (($('#ProjectCategory').val() != 'Amendment') && (!payment || parseInt(payment) < 0)) {
        $('#AlertMessage').css('background-color', '#fe5339');
        $('#AlertMessage').html('<p>مبلغ دریافتی را مشخص کنید.</p>').fadeIn(500).delay(3000).fadeOut(500);
    } else {

        let send = true;

        if (!$('#DesignerId').val()) {
            send = confirm("پروژه بدون نام طراح ثبت شود؟");
        }

        if (send) {

            $('.OverHide').show();
            $('.Loading').show();

            $.post({
                data: $('#add-project-form').serialize(),
                url: '/home',
                success: function (result, status, xhr) {
                    if (result == 2) {
                        GoBack();
                    }
                    else if (result == 1) {
                        $('#AlertMessage').css('background-color', '#fe5339');
                        $('#AlertMessage').html('<p>شماره معرف یافت نشد</p>').fadeIn(500).delay(3000).fadeOut(500);
                        $('.OverHide').hide();
                        $('.Loading').hide();
                    } else if (result == 4) {
                        $('#AlertMessage').css('background-color', '#fe5339');
                        $('#AlertMessage').html('<p>مبلغ درخواستی بیشتر از اعتبار مشتری می باشد</p>').fadeIn(500).delay(3000).fadeOut(500);
                        $('.OverHide').hide();
                        $('.Loading').hide();
                    }
                    else if (result == 6) {
                        $('#AlertMessage').css('background-color', '#fe5339');
                        $('#AlertMessage').html('<p>مجموع مبلغ دریافتی پروژه نمیتواند منفی باشد</p>').fadeIn(500).delay(3000).fadeOut(500);
                        $('.OverHide').hide();
                        $('.Loading').hide();
                    }
                    else {
                        $('#AlertMessage').css('background-color', '#fe5339');
                        $('#AlertMessage').html('<p>خطای پیش بینی نشده</p>').fadeIn(500).delay(3000).fadeOut(500);
                        $('.OverHide').hide();
                        $('.Loading').hide();
                    }
                }
            });
        }
    }
});

function FindByMobile() {

    let Mobile = $('#Mobile').val();

    $('#Mobile').css('border-color', '#e4e7ef');

    $('.Mobile_Label').css('color', '#a1a6bb');

    $('.Mobile_Label').text('شماره تماس');

    if ((Mobile.length < 10) || (Mobile.substr(0, 1) == "0" && Mobile.length < 11)) {

        $('#Mobile').css('border-color', '#fe5339');
        $('.Mobile_Label').text('شماره تماس را به درستی وارد کنید!');
        $('.Mobile_Label').css('color', '#fe5339');

    } else {

        $.get({
            data: { Mobile: $('#Mobile').val() },
            url: "/Home/FindByMobile",
            success: function (result, status, xhr) {

                if (result != null) {

                    $('#Address').val(result.address);
                    $('#Datepicker1').val(result.birthday);
                    $('#FirstName').val(result.firstName);
                    $('#LastName').val(result.lastName);
                    $('#Phone').val(result.phone);
                    $('#Social').val(result.social);
                    if (result.accounting > 0) {
                        $('#Accounting').parent().children('label').text('بستانکار')
                    }
                    else if (result.accounting < 0) {
                        $('#Accounting').parent().children('label').text('بدهکار')
                    }
                    $('#Credit').val(Math.abs(result.credit).toString().replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,"));
                    $('#Accounting').val(Math.abs(result.accounting).toString().replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,"));
                    $('#Mobile').prop("readonly", true);
                    $('#Credit').parent().show();
                    $('#Presenter, #Address, #FirstName, #LastName, #Social, #Accounting').prop("disabled", true);
                    $("#AlertMessage").css('background-color', '#4be1ab');
                    $("#AlertMessage").html('<p>اطلاعات کاربر قبلا ثبت شده است!</p>').fadeIn(500).delay(3000).fadeOut(500);

                    if (result.duplicate) {
                        $('<div class="alert-message col-lg-3 col-xs-12"><p>برای کاربر امروز پروژه ای ثبت شده است!</p></div>').insertAfter('#AlertMessage').fadeIn(500).delay(3000).fadeOut(500);
                    }
                }
                //else {

                //    $("#AlertMessage").css('background-color', '#fe5339');

                //    $("#AlertMessage").html('<p>یافت نشد!</p>').fadeIn(500).delay(3000).fadeOut(500);
                //}
            }
        });
    }
}

$('#TotalPrice').keyup(function () {
    $('#TotalPrice').val($('#TotalPrice').val().replace(/[^0-9\.]+/g, '').replace(/,/g, '').replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    if ($('#TotalPrice').val() && parseInt($('#TotalPrice').val()) > 0) {
        $('#Approximate').prop('readonly', true);
    } else {
        $('#Approximate').prop('readonly', false);
    }
});

$("#add-project-form").on("keyup", ".PaymentPrice", function () {
    $(this).val($(this).val().replace(/[^0-9\.]+/g, '').replace(/,/g, '').replace(/\B(?=(\d{3})+(?!\d))/g, ","));
});

$('#Approximate').keyup(function () {
    $(this).val($(this).val().replace(/[^0-9\.]+/g, '').replace(/,/g, '').replace(/\B(?=(\d{3})+(?!\d))/g, ","));
});

function toggleDescription() {
    $('#CustomerDescription').parent().toggle()
}

$('#update-customer-from').submit(function (e) {

    e.preventDefault();

    var Mobile = $('#Mobile').val();
    $('#Mobile').css('border-color', '#e4e7ef');
    $('.Mobile_Label').css('color', '#a1a6bb');
    $('.Mobile_Label').text('شماره تماس');

    if ((Mobile.length < 10) || (Mobile.substr(0, 1) == "0" && Mobile.length < 11)) {
        $('#Mobile').css('border-color', '#fe5339');
        $('.Mobile_Label').text('شماره تماس را به درستی وارد کنید!');
        $('.Mobile_Label').css('color', '#fe5339');
    } else {
        $.post({

            data: $("#update-customer-from").serialize(),
            url: "/Customer/Edit",

            success: function (result, status, xhr) {

                if (result == 1) {

                    var duplicate = confirm('شماره وارد شده متعلق به پروفایل دیگیری هست، آیا میخواید ادغام کنید؟')

                    if (duplicate) {
                        var oldMobile = $('#old-mobile').val();
                        mergeCustomer(oldMobile, Mobile);
                    }

                } else if (result == 2) {
                    $("#AlertMessage").css('background-color', '#4be1ab');
                    $("#AlertMessage").html('<p>اطلاعات مشتری با موفقیت ویرایش شد!</p>').fadeIn(500).delay(3000).fadeOut(500);

                    setTimeout(() => { location.reload(); }, 500);

                } else if (result == 3) {
                    $("#AlertMessage").css('background-color', '#fe5339');
                    $("#AlertMessage").html('<p>خطا در انجام عملیات!</p>').fadeIn(500).delay(3000).fadeOut(500);
                }
            }
        });
    }
});

$('.btn-merge').click(function () {
    var oldMobile = $('#old-profile').val();
    var newMobile = $('#new-profile').val();
    mergeCustomer(oldMobile, newMobile);
})

$('.show-merge-form').click(function () {

    $('.merge-from').show()
    $('.OverHide').show()
})

$('.hide-merge-form').click(function () {

    $('.merge-from').hide()
    $('.OverHide').hide()
})

function mergeCustomer(oldMobile, newMobile) {

    $('.merge-from').hide()

    $('.Loading').show()
    $('.OverHide').show()

    $.post({

        data: { oldMobile, newMobile },
        url: "/Customer/merge",

        success: function (result, status, xhr) {

            if (result == 1) {
                $('.merge-from').show()
                $("#AlertMessage").css('background-color', '#fe5339');
                $("#AlertMessage").html('<p>موبایل وارد شده یافت نشد</p>').fadeIn(500).delay(3000).fadeOut(500);
            } else if (result == 2) {
                $("#AlertMessage").css('background-color', '#4be1ab');
                $("#AlertMessage").html('<p>عملیات با موفقیت انجام شد</p>').fadeIn(500).delay(3000).fadeOut(500);
                $('.OverHide').hide()
                setTimeout(() => { location.reload(); }, 500)

            } else if (result == 3) {
                $('.merge-from').show()
                $("#AlertMessage").css('background-color', '#fe5339');
                $("#AlertMessage").html('<p>خطا در انجام عملیات</p>').fadeIn(500).delay(3000).fadeOut(500);
            }

            $('.Loading').hide()
        }
    });
}

function getProject(page) {
    $.post({
        beforeSend: function (xhr) {
            $('.Loading').show()
            $('.OverHide').show()
        },
        data: { id: $('#customerId').val(), page: page, status: projectStatus },
        url: "/customer/project",
        success: function (result, status, xhr) {
            $('#project-partial').html(result);
            $('.Loading').hide()
            $('.OverHide').hide()
        }
    });
}

function hideAddRepair() {
    $('.repair-add-box').hide()
    $('.OverHide').hide()
}

function showAddRepair(projectId) {
    $('.repair-add-box .projectId').val(projectId)
    $('.repair-add-box').show()
    $('.OverHide').show()
}

$('.repair-add-box').on("keyup", ".TotalPrice", function () {
    $(this).val($(this).val().replace(/[^0-9\.]+/g, '').replace(/,/g, '').replace(/\B(?=(\d{3})+(?!\d))/g, ","));
    if ($(this).val() && parseInt($(this).val()) > 0) {
        $(this).parent().parent().find('.Approximate').prop('readonly', true);
    } else {
        $(this).parent().parent().find('.Approximate').prop('readonly', false);
    }
});

$('.repair-add-box').on("keyup", ".Approximate", function () {
    $(this).val($(this).val().replace(/[^0-9\.]+/g, '').replace(/,/g, '').replace(/\B(?=(\d{3})+(?!\d))/g, ","));
});


function RepairAddSave(byContinue) {

    var totalPrice = $('#TotalPrice').val();
    var approximate = $('#Approximate').val();
    var payment = $('.repair-add-box .PaymentPrice').val();

    if (($('#ProjectCategory').val() != 'Amendment') && (!totalPrice || parseInt(totalPrice) <= 0) && (!approximate || parseInt(approximate) <= 0)) {
        $('#AlertMessage').css('background-color', '#fe5339');
        $('#AlertMessage').html('<p>هزینه کل یا هزینه پیش بینی شده را مشخص کنید.</p>').fadeIn(500).delay(3000).fadeOut(500);
    } else if (($('#ProjectCategory').val() != 'Amendment') && (!payment || parseInt(payment) < 0)) {
        $('#AlertMessage').css('background-color', '#fe5339');
        $('#AlertMessage').html('<p>مبلغ دریافتی را مشخص کنید.</p>').fadeIn(500).delay(3000).fadeOut(500);
    } else {

        let send = true;

        if (send) {

            $('.OverHide').show();
            $('.Loading').show();

            $('.repair-add-box').hide()

            $.post({
                data: $('.repair-add-box form').serialize(),
                url: '/repair/add',
                success: function (result, status, xhr) {

                    if (result.status == 2) {
                        if (byContinue) {
                            location = '/edit/' + $('.repair-add-box .projectId').val() + '?id=' + result.id
                        }
                        else {
                            location.reload()
                        }
                    }
                    else if (result.status == 4) {
                        $('.repair-add-box').show()
                        $('#AlertMessage').css('background-color', '#fe5339');
                        $('#AlertMessage').html('<p>مبلغ درخواستی بیشتر از اعتبار مشتری می باشد</p>').fadeIn(500).delay(3000).fadeOut(500);
                        $('.OverHide').hide();
                        $('.Loading').hide();
                    }
                    else if (result.status == 6) {
                        $('.repair-add-box').show()
                        $('#AlertMessage').css('background-color', '#fe5339');
                        $('#AlertMessage').html('<p>مجموع مبلغ دریافتی پروژه نمیتواند منفی باشد</p>').fadeIn(500).delay(3000).fadeOut(500);
                        $('.OverHide').hide();
                        $('.Loading').hide();
                    }
                }
            });
        }
    }
}

function changeCategory() {
    if ($('#ProjectCategory').val() == 'Amendment') {
        $('#TotalPrice').parent().hide()
        $('#Approximate').parent().hide()
        $('#payment-box').hide()
    }
    else if ($('#ProjectCategory').val() == 'Edit') {
        $('#TotalPrice').parent().show()
        $('#Approximate').parent().show()
        $('#payment-box').show()
    }
}

$('.search-payment').change(function () {
    let li = $('.li-project');
    for (i = 0; i < li.length; i++) {
        if ($('.search-payment').val()) {
            let input = $(li[i]).find('.payment-name');
            let text = input.val();
            if (text.toUpperCase().indexOf($('.search-payment').val().toUpperCase()) > -1) {
                li[i].style.display = '';
            } else {
                li[i].style.display = 'none';
            }
        } else {
            li[i].style.display = '';
        }
    }
});

$("body").on("change", ".project-payment-type", function () {
    if ($(this).val() == "ToDesigner") {
        $(this).parent().parent().find('.project-payment-designer-id').show();
    }
    else {
        $(this).parent().parent().find('.project-payment-designer-id').hide();
    }
});

function RemoveMessage() {
    let remove = confirm("آیا از حذف این موارد اطمینان دارید؟");

    if (remove) {

        $('.OverHide').show()
        $('.Loading').show()

        let conversationId = []

        $('input[class="accepted"]:checked').each(function (index, value) {

            conversationId.push(value.getAttribute("data-id"))
        })

        $.post({
            data: { id: conversationId },
            url: '/conversation/remove',
            success: function (result, status, xhr) {

                for (var i = 0; i < conversationId.length; i++) {
                    document.getElementById('conversation-' + conversationId[i]).remove()
                }

                chatLightGallery.refresh()

                $('.OverHide').hide()
                $('.Loading').hide()

                $('.accepted-btn').hide()
                clearConversationBtn()
            }
        })
    }
}

function removeProject(projectId) {
    let remove = confirm("آیا از حذف پروژه اطمینان دارید؟");

    if (remove) {

        $('.OverHide').show()
        $('.Loading').show()

        $.post({
            data: { id: projectId },
            url: '/project/remove',
            success: function (result, status, xhr) {

                if (result) {

                    $('#AlertMessage').css('background-color', '#fe5339');
                    $('#AlertMessage').html('<p>' + result + '</p>').fadeIn(500).delay(3000).fadeOut(500);

                    $('.OverHide').hide()
                    $('.Loading').hide()
                }
                else {

                    location.href = '/project'
                }
            }
        })
    }
}

function selectAllMessage() {

    $('input[class="accepted"]').each(function (index, value) {
        if (($(value).parent().parent().hasClass('is-screen') == false) && ($(value).parent().parent().hasClass('is-render') == false)) {
            if ($('#select-all-message').is(':checked')) {
                $(value).prop('checked', true);
            } else {
                $(value).prop('checked', false);
            }
        }
    })

    showOrHideBtn()
}

function selectAllScreen() {

    $('input[class="accepted"]').each(function (index, value) {
        if ($(value).parent().parent().hasClass('is-screen')) {
            if ($('#select-all-screen').is(':checked')) {
                $(value).prop('checked', true);
            } else {
                $(value).prop('checked', false);
            }
        }
    })

    showOrHideBtn()
}

function selectAllRender() {
    $('input[class="accepted"]').each(function (index, value) {
        if ($(value).parent().parent().hasClass('is-render')) {
            if ($('#select-all-render').is(':checked')) {
                $(value).prop('checked', true);
            } else {
                $(value).prop('checked', false);
            }
        }
    })

    showOrHideBtn()
}

function accepted(status) {
    $('.OverHide').show()
    $('.Loading').show()

    let conversationId = []

    $('input[class="accepted"]:checked').each(function (index, value) {

        conversationId.push(value.getAttribute("data-id"))

        if (status) {
            $('.chat-status-' + value.getAttribute("data-id")).removeClass('not-accepted')
        } else {
            $('.chat-status-' + value.getAttribute("data-id")).addClass('not-accepted')
        }

        $(value).prop('checked', false);
    })

    $.post({
        data: { status, conversationId: conversationId },
        url: '/conversation/accepted',
        success: function (result, status, xhr) {

            $('.OverHide').hide()
            $('.Loading').hide()

            $('.accepted-btn').hide()
            clearConversationBtn()
        }
    })
}

function clearConversationBtn() {
    $('#select-all-message').prop('checked', false);
    $('#select-all-screen').prop('checked', false);
    $('#select-all-render').prop('checked', false);
}

$('input[class="accepted"]').change(function () {
    event.preventDefault();
    showOrHideBtn();
})

function showOrHideBtn() {
    if ($('input[class="accepted"]:checked').length) {
        $('.accepted-btn').show()
    } else {
        $('.accepted-btn').hide()
    }
}


function searchProject(page) {

    $('.OverHide').show();
    $('.Loading').show();

    var data = {
        page: page,
        title: $('.project-search-box .title').val(),
        projectCategory: $('.project-search-box .project-category').val(),
        designerId: $('.project-search-box .designer-id').val(),
        fromDate: $('.project-search-box .from-date').val(),
        toDate: $('.project-search-box .to-date').val(),
        fromPrice: $('.project-search-box .from-price').val(),
        toPrice: $('.project-search-box .to-price').val(),
        type: $('.project-search-box .type').val(),
    }

    $.post({
        data: data,
        url: "/project/search",
        success: function (result, status, xhr) {

            $('.OverHide').hide();
            $('.Loading').hide();

            $('.project-list-ajax').replaceWith(result)
        }
    });
}

$('body').on('click', '.btn-add-transaction', function () {

    $('.add-customer-transaction').hide()

    $('.Loading').show()

    let price = $('.add-customer-transaction .PaymentPrice').val()
    let paymentType = $('.add-customer-transaction .project-payment-type').val()
    let customerId = $('#customerId').val()
    let designerId = $('.project-payment-designer-id select').val()
    let description = $('.transaction-modal-description input').val()

    $.post({

        data: { price, paymentType, customerId, designerId, description },
        url: "/transaction/add",

        success: function (result, status, xhr) {

            //$.post({

            //    data: { customerId },
            //    url: "/transaction/getlist",

            //    success: function (result, status, xhr) {

            //        $('#customer-transaction-ajax').replaceWith(result)

            //        $('.OverHide').hide()
            //        $('.Loading').hide()
            //    }
            //});

            location.reload();
        }
    });
})

$('body').on('click', '.show-add-customer-transaction', function () {

    $('.OverHide').show()
    $('.add-customer-transaction').show()
})

$('body').on('click', '.hide-add-transaction', function () {

    $('.add-customer-transaction').hide()
    $('.OverHide').hide()
})



function removeTransaction(transactionId) {

    let remove = confirm("آیا از حذف پرداخت مورد نظر مطمئن هستید؟");

    if (remove) {

        $('.OverHide').show()
        $('.Loading').show()

        $.post({
            data: { id: transactionId },
            url: '/transaction/remove',
            success: function (result, status, xhr) {

                location.reload();
            }
        })
    }
}

function removeCustomer(id) {

    let remove = confirm('آیا از حذف کامل مشتری اطمینان دارید؟')

    if (remove) {

        $('.OverHide').show()
        $('.Loading').show()

        $.post({
            data: { id: id },
            url: '/customer/delete',
            success: function (result, status, xhr) {

                location.href = '/customer'
            },
            error: function (result, status, xhr) {

                alert(result.responseJSON.message)

                $('.OverHide').hide()
                $('.Loading').hide()
            },
        })
    }
}

if (document.getElementById('customer-chat-light-gallery')) {

    $('#customer-chat-light-gallery').scrollTop($('#customer-chat-light-gallery> div').height());
}


if (document.getElementById('customer-chat-light-gallery')) {
    chatLightGallery = lightGallery(document.getElementById('customer-chat-light-gallery'), {
        selector: '.customer-chat-light-gallery-selector',
        thumbnail: true,
        mobileSettings: {
            showCloseIcon: true,
            download: true,
        }
    });
}

$('body').on('click', '.customer-chat-done-status', function () {

    $('.OverHide').show();
    $('.Loading').show();

    let customerId = $(this).data("id");
    let This = $(this);

    $.post({
        data: { CustomerId: customerId },
        url: "/customer/toggle-done",
        success: function (result, status, xhr) {

            This.toggleClass('customer-chat-done-status-active');

            $('.OverHide').hide();
            $('.Loading').hide();
        }
    });
});

$('.customer-chat-upload > div').click(function () {
    $('.customer-chat-upload > input').click()
})

$('.customer-chat-upload > input').change(function (e) {


    $('.OverHide').show()
    $('.Loading').show()

    var file = e.target.files[0];
    var formData = new FormData();
    formData.append('attachment', file);
    formData.append('customerId', $('.customer-id').val());

    $.post({
        data: formData,
        url: '/customer/payment',
        contentType: false,
        processData: false,
        success: function (result, status, xhr) {

            location.reload();
        }
    })
})

$('.customer-transaction-title').click(function () {
    $(this).toggleClass('active');

    $('.add-customer-items').toggle();
})

function mediaFavorite(e, id) {

    e.stopPropagation()

    $('.OverHide').show();
    $('.Loading').show();

    $.post({
        data: { Id: id },
        url: "/conversation/favorite",
        success: function (result, status, xhr) {
            $('.media-favorite-' + id).toggleClass('media-favorite-active');

            $('.OverHide').hide();
            $('.Loading').hide();
        }
    });
}

function getPayment(id) {

    $('.OverHide').show();
    $('.Loading').show();

    $.post({
        data: { id: id },
        url: '/payment/details',
        success: function (result, status, xhr) {

            $('.Loading').hide();
            $('body').append(result);
        },
        error: function (result, status, xhr) {

            if (result.status == 401) {
                location.href = 'Designer/VerifyCode?url=' + location.pathname + location.search
            }
        }
    })
}


function updatePayment() {

    $('.payment-edit').hide();
    $('.Loading').show();

    $.post({
        data: {
            id: $('.payment-id').val(),
            price: $('.payment-price').val().replaceAll(',', ''),
            date: $('.payment-date').val(),
            type: $('.payment-type').find(':selected').val(),
            designerId: $('.payment-designerId').val(),
            description: $('.payment-description').val()
        },
        url: '/payment/edit',
        success: function (result, status, xhr) {



            console.log(result);

            location.reload();

            //if (result) {

            //    $('.Loading').hide();
            //    $('body').append(result);

            //    //setTimeout(() => {
            //    //    $('.Loading').hide();
            //    //    $('.mini-chat').show();
            //    //    $('.user-chats').scrollTop($('.user-chats > .chats').height());
            //    //}, 50);
            //} else {
            //    location.href = 'Designer/VerifyCode?url=' + location.pathname + location.search
            //}
        },
        error: function (result, status, xhr) {

            console.log(result)
            if (result.status == 401) {
                location.href = 'Designer/VerifyCode?url=' + location.pathname + location.search
            } else if (result.status == 422) {
                if (result.responseJSON == 4) {
                    $('.Loading').hide();
                    $('.payment-edit').show()
                    $('#AlertMessage').css('background-color', '#fe5339');
                    $('#AlertMessage').html('<p>مبلغ درخواستی بیشتر از اعتبار مشتری می باشد</p>').fadeIn(500).delay(3000).fadeOut(500);
                }
                else if (result.responseJSON == 6) {
                    $('.Loading').hide();
                    $('.payment-edit').show()
                    $('#AlertMessage').css('background-color', '#fe5339');
                    $('#AlertMessage').html('<p>مجموع مبلغ دریافتی پروژه نمیتواند منفی باشد</p>').fadeIn(500).delay(3000).fadeOut(500);
                }
            }
        }
    })
}

function closePaymentEdit() {

    $('.payment-edit').remove();
    $('.OverHide').hide();
}

$('.project-report-icon').click(function () {

    $('.OverHide').show();
    $('.Loading').show();

    if (document.getElementById('is-admin').value == 'true') {
        if ($(this).hasClass('project-report-icon-active')) {
            $(this).removeClass('project-report-icon-active');
            $('.project-report-badge').text($('.project-report-badge').text() - 1);
        }
    }




    let id = $(this).data('id');
    let title = $(this).data('title');

    $.post({
        data: { projectId: id },
        url: '/conversation/get',
        success: function (result, status, xhr) {

            $('.mini-chat-header').html(title);
            $('.mini-chat-body').html(result);

            setTimeout(() => {
                $('.Loading').hide();
                $('.mini-chat').show();
                $('.user-chats').scrollTop($('.user-chats > .chats').height());
            }, 50);
        }
    })
})