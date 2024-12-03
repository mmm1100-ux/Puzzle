$('.favorite').click(function () {

    $('.OverHide').show();
    $('.Loading').show();

    let projectId = $(this).data("id");
    let This = $(this);

    $.post({
        data: { Id: projectId },
        url: "/architect/favorite",
        success: function (result, status, xhr) {
            This.toggleClass('favorite-active');

            $('.OverHide').hide();
            $('.Loading').hide();
        }
    });
});

$('.done-by-designer').click(function () {

    $('.OverHide').show();
    $('.Loading').show();

    let projectId = $(this).data("id");
    let This = $(this);

    $.post({
        data: { ProjectId: projectId },
        url: "/architect/done",
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

if (location.pathname.toLowerCase().startsWith('/architect/media')) {
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
        url: '/architect/Conversation/Read',
        success: function (result, status, xhr) {

            if ($('.project-report-popup').length == 0) {
                $('.OverHide').hide();
            }

            $('.Loading').hide()
        }
    });
}


$('.btn-project-chat').click(function () {
    $('#media-box').hide()
    $('#media-box').empty()
    $('#info').hide()
    $('#project-chat-box').show()
    $('.btn-project-media').css("color", "#a1a6bb")
    $('.btn-project-edit').css("color", "#a1a6bb")
    $(this).css("color", "#3a405b")
})

$('.btn-project-media').click(function () {
    $('.OverHide').show()
    $('.Loading').show()

    let projectId = document.getElementById("projectId").value

    $.post({
        data: {
            ProjectId: projectId
        },
        url: '/architect/get-media',
        success: function (result, status, xhr) {
            $('#media-box').html(result)

            $('#project-chat-box').hide()
            $('#media-box').show()
            $('.btn-project-chat').css("color", "#a1a6bb");
            $('.btn-project-media').css("color", "#3a405b");

            mediaGallery();

            $('.OverHide').hide();
            $('.Loading').hide();
            $('.loading-other').hide()
        }
    })
})

$('.project-report-modal button').click(function () {

    let parentEl = $(this).parent().parent().parent();
    let id = parentEl.data('id')
    let reason = parentEl.find("input[name='project-report-option']:checked").val()
    let description = parentEl.find('textarea').val()

    if (!reason) {
        $('<div class="warning-message"><p>یک گزینه را انتخاب نمایید</p></div>').insertAfter('#AlertMessage').fadeIn(500).delay(3000).fadeOut(500);
        return;
    }

    if ((reason == 8) && (!description)) {
        $('<div class="warning-message"><p>توضیحات را وارد نمایید</p></div>').insertAfter('#AlertMessage').fadeIn(500).delay(3000).fadeOut(500);
        return;
    }

    $.post({
        data: { projectId: id, reason: reason, description: description },
        url: '/architect/add-report',
        success: function (result, status, xhr) {

            var nextEl = parentEl.next();
            var prevEl = parentEl.prev();

            parentEl.remove();

            if (nextEl.hasClass('project-report-modal')) {

                nextEl.show();
                $('.project-report-number').text('پروژه ' + ($('.project-report-modal').index(nextEl) + 1) + ' از ' + $('.project-report-modal').length + ' مورد');

            } else if (prevEl.hasClass('project-report-modal')) {

                prevEl.show();
                $('.project-report-number').text('پروژه ' + ($('.project-report-modal').index(prevEl) + 1) + ' از ' + $('.project-report-modal').length + ' مورد');

            } else {

                location.reload();
            }
        }
    })
    //console.log($(this).parent().parent().parent().find("input[name='project-report-option']:checked").val())
})

$('.project-report-next').click(function () {

    if ($(this).parent().parent().next().hasClass('project-report-modal')) {
        $(this).parent().parent().hide();
        $(this).parent().parent().next().show();

        $('.project-report-number').text('پروژه ' + ($('.project-report-modal').index($(this).parent().parent()) + 2) + ' از ' + $('.project-report-modal').length + ' مورد');
    }
})

$('.project-report-back').click(function () {

    if ($(this).parent().parent().prev().hasClass('project-report-modal')) {
        $(this).parent().parent().hide();
        $(this).parent().parent().prev().show();

        $('.project-report-number').text('پروژه ' + $('.project-report-modal').index($(this).parent().parent()) + ' از ' + $('.project-report-modal').length + ' مورد');
    }
})

function getChat(id, title) {

    $('.project-report-popup').css('opacity', 0);
    $('.OverHide').show();
    $('.Loading').show();

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
}