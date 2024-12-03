$('.multiple-items').slick({
    infinite: false,
    slidesToShow: 7,
    slidesToScroll: 7,
    rtl: true,
    speed: 500,
    accessibility: false,
    arrows: false
});

$('.project-calender-previous').click(function () {
    $(".multiple-items").slick('slickPrev');
});

$('.project-calender-next').click(function () {
    $(".multiple-items").slick('slickNext');
});

$('.multiple-items').slick('slickGoTo', 28, true);

$('body').on('click', '.open-description', function () {
    $('#description-id').val($(this).data('description-id'));
    $('.popup-textarea').val($(this).data('description'));
    $('.OverHide').show();
    $('.pop-up').show();
});

$(document).mouseup(function (e) {
    var container = $('.pop-up, .date-popup');
    if (container.is(":visible")) {
        if (!container.is(e.target) && container.has(e.target).length === 0) {
            container.hide();
            $('.OverHide').hide();
        }
    }
});

$('.search-name').keyup(function () {
    let li = $('.li-project');
    for (i = 0; i < li.length; i++) {
        let input = $(li[i]).find('input');
        let text = input.val();
        if (text.toUpperCase().indexOf($('.search-name').val().toUpperCase()) > -1) {
            li[i].style.display = '';
        } else {
            li[i].style.display = 'none';
        }
    }
});

var chatLightGallery;

function formatDate(date) {
    var d = new Date(date);
    var month = '' + (d.getMonth() + 1);
    var day = '' + d.getDate();
    var year = d.getFullYear();
    var hour = d.getHours();
    var minute = d.getMinutes();

    if (month.length < 2) {
        month = '0' + month;
    }

    if (day.length < 2) {
        day = '0' + day;
    }

    if (hour.length < 2) {
        hour = '0' + hour;
    }

    if (minute.length < 2) {
        minute = '0' + minute;
    }

    return [year, month, day, hour, minute].join('-');
}

function rightChat(conversation, id) {

    var myImage = document.getElementById("myImage").value
    let dateTime = formatDate(new Date())
    var meLastTime = document.getElementById(dateTime)

    if (meLastTime) {

        var eel;

        if (conversation.attachment) {
            if (conversation.type == 4) {
                eel = '<div id="conversation-' + conversation.id + '">'
                eel += '<div data-id="' + conversation.id + '" class="chat-message-file" data-url="' + conversation.attachment + '">'
                eel += '<div class="chat-message-file-icon" onclick="downloadFile(event, \'' + conversation.attachment + '\')">'
                eel += '<svg xmlns="http://www.w3.org/2000/svg" class="icon icon-tabler icon-tabler-download" width="22" height="22" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round">'
                eel += '<path stroke="none" d="M0 0h24v24H0z" fill="none" />'
                eel += '<path d="M4 17v2a2 2 0 0 0 2 2h12a2 2 0 0 0 2 -2v-2" />'
                eel += '<path d="M7 11l5 5l5 -5" />'
                eel += '<path d="M12 4l0 12" />'
                eel += '</svg>'

                eel += '<svg xmlns="http://www.w3.org/2000/svg" class="icon icon-tabler icon-tabler-file" width="22" height="22" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round">'
                eel += '<path stroke="none" d="M0 0h24v24H0z" fill="none" />'
                eel += '<path d="M14 3v4a1 1 0 0 0 1 1h4" />'
                eel += '<path d="M17 21h-10a2 2 0 0 1 -2 -2v-14a2 2 0 0 1 2 -2h7l5 5v11a2 2 0 0 1 -2 2z" />'
                eel += '</svg>'

                eel += '<span class="file-loader"></span>'
                eel += '</div>'
                eel += '<div class="chat-message-file-details">'
                eel += '<div>' + conversation.fileName + '</div>'
                eel += '<div>' + (conversation.fileSize / 1024 / 1024).toFixed(2) + ' Mb</div>'
                eel += '</div>'
                eel += '</div>'
                eel += '</div>'
            }
            else if (conversation.attachment.endsWith('.ogg')) {
                eel = '<div class="d-inline-flex" data-id="' + conversation.id + '" id="conversation-' + conversation.id + '"><audio class="audio-player" controls><source src="' + conversation.attachment + '" type="audio/ogg" /></audio></div>'
            } else {
                eel = '<div href="' + conversation.attachment + '" id="conversation-' + conversation.id + '" class="chat-image chat-light-gallery-selector"><img data-id="' + conversation.id +'" class="message-photo" src="' + conversation.attachment + '" /></div>'
            }
        } else {
            eel = '<div class="d-inline-flex" id="conversation-' + conversation.id + '">'
            eel += '<p class="message-text" data-id="' + conversation.id +'">' + conversation.message + '</p>'
            eel += '</div>'
        }

        eel += ' <div class="chat-dropdown" id="dropdown-' + conversation.id + '">'
        eel += '<div onclick="ReplyMessage(\'' + conversation.id + '\')">پاسخ</div>'
        if (true) {
            if (conversation.message != null) {
                eel += '<div onclick="EditMessage(\'' + conversation.id + '\')">ویرایش</div>'
            }
            eel += '<div onclick="RemoveMessageMy(\'' + conversation.id + '\')">حذف</div>'
        }
        eel += '</div>'


        $(meLastTime).before(eel)
    } else {

        //return;

        var el = '<div class="chat chat-right">'
        el += '  <div class="chat-body">'
        el += '<div class="chat-content">'
/*        el += '<div class="chat-content" onclick="DropDown(' + conversation.id + ')">'*/
        el += '<div class="chat-avatar"><img src="' + myImage + '" /></div>'
        el += '<div class="msg-group">'
        if (conversation.attachment) {
            if (conversation.type == 4) {
                el += '<div id="conversation-' + conversation.id + '">'
                el += '<div data-id="' + conversation.id + '" class="chat-message-file" data-url="' + conversation.attachment + '">'
                el += '<div class="chat-message-file-icon" onclick="downloadFile(event, \'' + conversation.attachment + '\')">'
                el += '<svg xmlns="http://www.w3.org/2000/svg" class="icon icon-tabler icon-tabler-download" width="22" height="22" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round">'
                el += '<path stroke="none" d="M0 0h24v24H0z" fill="none" />'
                el += '<path d="M4 17v2a2 2 0 0 0 2 2h12a2 2 0 0 0 2 -2v-2" />'
                el += '<path d="M7 11l5 5l5 -5" />'
                el += '<path d="M12 4l0 12" />'
                el += '</svg>'

                el += '<svg xmlns="http://www.w3.org/2000/svg" class="icon icon-tabler icon-tabler-file" width="22" height="22" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round">'
                el += '<path stroke="none" d="M0 0h24v24H0z" fill="none" />'
                el += '<path d="M14 3v4a1 1 0 0 0 1 1h4" />'
                el += '<path d="M17 21h-10a2 2 0 0 1 -2 -2v-14a2 2 0 0 1 2 -2h7l5 5v11a2 2 0 0 1 -2 2z" />'
                el += '</svg>'

                el += '<span class="file-loader"></span>'
                el += '</div>'
                el += '<div class="chat-message-file-details">'
                el += '<div>' + conversation.fileName + '</div>'
                el += '<div>' + (conversation.fileSize / 1024 / 1024).toFixed(2) + ' Mb</div>'
                el += '</div>'
                el += '</div>'
                el += '</div>'
            }
            else if (conversation.attachment.endsWith('.ogg')) {
                el += '<div data-id="' + conversation.id + '" class="chat-audio" id="conversation-' + conversation.id + '"><audio class="audio-player" controls><source src="' + conversation.attachment + '" type="audio/ogg" /></audio></div>'
            } else {
                el += '<div href="' + conversation.attachment + '" class="chat-image chat-light-gallery-selector" id="conversation-' + conversation.id + '"><img data-id="' + conversation.id +'" class="message-photo" src="' + conversation.attachment + '" /></div>'
            }
        } else {
            el += '<div class="d-inline-flex" id="conversation-' + conversation.id + '">'
            el += '<p class="message-text" data-id="' + conversation.id +'">' + conversation.message + '</p>'
            el += '</div>'
        }
        //console.log(conversation)
        el += ' <div class="chat-dropdown" id="dropdown-' + conversation.id +'">'
        el += '<div onclick="ReplyMessage(\'' + conversation.id +'\')">پاسخ</div>'
                                                    if (true)
                                                    {
                                                        if (conversation.message != null)
                                                        {
                                                            el += '<div onclick="EditMessage(\'' + conversation.id +'\')">ویرایش</div>'
                                                        }
                                                        el += '<div onclick="RemoveMessageMy(\'' + conversation.id +'\')">حذف</div>'
                                                    }
        el +='</div>'

        el += '<span id="' + dateTime + '" class="chat-time">' + dateTime.split('-')[3] + ':' + +dateTime.split('-')[4] + '</span>'



        //el += '</div>'

        el += '</div>'
        el += ' </div>'

        $('.end-chat').before(el);
    }

    if ($('.audio-player')) {
        Array.from(document.querySelectorAll('.audio-player')).map((p) => new Plyr(p));
    }

    chatLightGallery.refresh();

    $('.user-chats').scrollTop($('.user-chats > .chats').height());
}

function enterChat() {

    var projectId = document.getElementById("projectId").value

    const urlParams = new URLSearchParams(window.location.search);

    if (urlParams.get('id')) {
        projectId = urlParams.get('id')
    }

    var message = document.getElementById("messageInput").value;

    if (message.length == 0) {
        return
    }


    let uuid = crypto.randomUUID()

    let conversation = { message: message, id: uuid }
    rightChat(conversation)

    document.getElementById("messageInput").value = null

    var formData = new FormData()
    formData.append('ProjectId', projectId)
    formData.append('Message', message)

    $('.user-chats').scrollTop($('.user-chats > .chats').height());

    let url = '/conversation/add'

    $.post({
        data: formData,
        url: url,
        contentType: false,
        processData: false,
        success: function (result, status, xhr) {
            //document.getElementById("messageInput").value = null
            //rightChat(result)

            //document.getElementById('conversation-' + uuid).outerHTML = document.getElementById('conversation-' + uuid).outerHTML.replace(uuid, result.id)

            var html = $('#dropdown-' + uuid).parent().html();
            console.log(html);
            html = html.replaceAll(uuid, result.id);
            console.log(html);
            $('#dropdown-' + uuid).parent().html(html)
        }
    })
}

function EditMessage(conversationId) {
    $("#messageInput").val($('#conversation-' + conversationId + ' .message-text').text());
    $(".chat-app-form").attr("onsubmit", "SendEditMessage(" + conversationId + ")");
    $("#sendButton").attr("onclick", "SendEditMessage(" + conversationId + ")");
    $("#send-btn-text").text("ویرایش");
    $("#messageInput").focus();
}

function SendEditMessage(conversationId) {

    $(".chat-app-form").attr("onsubmit", "enterChat()");
    $("#sendButton").attr("onclick", "enterChat()");
    $("#send-btn-text").text("ارسال");

    var message = document.getElementById("messageInput").value;

    $('#conversation-' + conversationId + ' .message-text').text(message)

    document.getElementById("messageInput").value = null

    $.post({
        data: { id: conversationId, message: message },
        url: '/conversation/edit',
        success: function (result, status, xhr) {
            //document.getElementById("messageInput").value = null
            //rightChat(result)
        }
    })
}

function RemoveMessageMy(conversationId) {

    let remove = confirm("آیا از حذف این مورد اطمینان دارید؟");

    if (remove) {

        $('.OverHide').show()
        $('.Loading').show()

        $.post({
            data: { id: [conversationId] },
            url: '/conversation/remove',
            success: function (result, status, xhr) {

                $('.OverHide').hide()
                $('.Loading').hide()

                document.getElementById('conversation-' + conversationId).remove()
                document.getElementById('dropdown-' + conversationId).remove()
            }
        })
    }
}

//function DropDown(id, e) {
//    //e.preventDefault()
//    console.log(e)
//    $('.chat-dropdown').hide()
//    $('#dropdown-' + id).show()
//}


var recordButton = document.getElementById("recordButton");
var stopButton = document.getElementById("stopButton");

//add events to those 2 buttons

if (recordButton) {
    recordButton.addEventListener("click", startRecording);
}

if (stopButton) {
    stopButton.addEventListener("click", stopRecording);
}

function startRecording() {

    //start recording using the audio recording API
    audioRecorder.start()
        .then(() => { //on success
            $('#recordButton').hide();
            $('#stopButton').show();

        })
        .catch(error => { //on error
            alert('میکروفون یافت نشد')
        });
}

function stopRecording() {

    $('.OverHide').show()
    $('.Loading').show()

    audioRecorder.stop()
        .then(audioAsblob => {

            $('#recordButton').show();
            $('#stopButton').hide();

            //Play recorder audio
            sendVoice(audioAsblob);
        })
        .catch(error => {
            alert('خطا')
        });
}

function sendVoice(blob) {

    event.preventDefault();

    var projectId = document.getElementById("projectId").value

    const urlParams = new URLSearchParams(window.location.search);

    if (urlParams.get('id')) {
        projectId = urlParams.get('id')
    }

    var formData = new FormData();

    formData.append('ProjectId', projectId)
    formData.append('Attachment', blob)

    let url = '/conversation/add'

    $.post({
        xhr: function () {
            var xhr = new window.XMLHttpRequest();

            xhr.upload.addEventListener("progress", function (evt) {
                if (evt.lengthComputable) {
                    var percentComplete = evt.loaded / evt.total;
                    percentComplete = parseInt(percentComplete * 100);

                    //$('.upload-progress-bar').width(percentComplete+ '%')
                    //$('.upload-percent').html(percentComplete+ '%')
                    //$('.upload-size').html((fileSize / 1024 / 1024).toFixed(1) + 'MB')

                    if (percentComplete === 100) {

                    }

                }
            }, false);

            return xhr;
        },
        data: formData,
        url: url,
        contentType: false,
        processData: false,
        success: function (result, status, xhr) {
            rightChat(result)
            $('.OverHide').hide()
            $('.Loading').hide()
        }
    })
}

function selectPhoto2() {
    $('.OverHide').show()
    $('#upload-image-box').show()
}

if (document.getElementById('chat-light-gallery')) {
    var chatLightGallery = lightGallery(document.getElementById('chat-light-gallery'), {
        selector: '.chat-light-gallery-selector',
        thumbnail: true,
        //mobileSettings: {
        //    showCloseIcon: true,
        //    download: true,
        //}
    });
}

var audioRecorder = {
    /** Stores the recorded audio as Blob objects of audio data as the recording continues*/
    audioBlobs: [],/*of type Blob[]*/
    /** Stores the reference of the MediaRecorder instance that handles the MediaStream when recording starts*/
    mediaRecorder: null, /*of type MediaRecorder*/
    /** Stores the reference to the stream currently capturing the audio*/
    streamBeingCaptured: null, /*of type MediaStream*/
    /** Start recording the audio 
     * @returns {Promise} - returns a promise that resolves if audio recording successfully started
     */
    start: function () {
        //Feature Detection
        if (!(navigator.mediaDevices && navigator.mediaDevices.getUserMedia)) {
            //Feature is not supported in browser
            //return a custom error
            return Promise.reject(new Error('mediaDevices API or getUserMedia method is not supported in this browser.'));
        }

        else {
            //Feature is supported in browser

            //create an audio stream
            return navigator.mediaDevices.getUserMedia({ audio: true }/*of type MediaStreamConstraints*/)
                //returns a promise that resolves to the audio stream
                .then(stream /*of type MediaStream*/ => {

                    //save the reference of the stream to be able to stop it when necessary
                    audioRecorder.streamBeingCaptured = stream;

                    //create a media recorder instance by passing that stream into the MediaRecorder constructor
                    audioRecorder.mediaRecorder = new MediaRecorder(stream); /*the MediaRecorder interface of the MediaStream Recording
                    API provides functionality to easily record media*/

                    //clear previously saved audio Blobs, if any
                    audioRecorder.audioBlobs = [];

                    //add a dataavailable event listener in order to store the audio data Blobs when recording
                    audioRecorder.mediaRecorder.addEventListener("dataavailable", event => {
                        //store audio Blob object
                        audioRecorder.audioBlobs.push(event.data);
                    });

                    //start the recording by calling the start method on the media recorder
                    audioRecorder.mediaRecorder.start();
                });

            /* errors are not handled in the API because if its handled and the promise is chained, the .then after the catch will be executed*/
        }
    },
    /** Stop the started audio recording
     * @returns {Promise} - returns a promise that resolves to the audio as a blob file
     */
    stop: function () {
        //return a promise that would return the blob or URL of the recording
        return new Promise(resolve => {
            //save audio type to pass to set the Blob type
            let mimeType = audioRecorder.mediaRecorder.mimeType;

            //listen to the stop event in order to create & return a single Blob object
            audioRecorder.mediaRecorder.addEventListener("stop", () => {
                //create a single blob object, as we might have gathered a few Blob objects that needs to be joined as one
                let audioBlob = new Blob(audioRecorder.audioBlobs, { type: mimeType });

                //resolve promise with the single audio blob representing the recorded audio
                resolve(audioBlob);
            });
            audioRecorder.cancel();
        });
    },
    /** Cancel audio recording*/
    cancel: function () {
        //stop the recording feature
        audioRecorder.mediaRecorder.stop();

        //stop all the tracks on the active stream in order to stop the stream
        audioRecorder.stopStream();

        //reset API properties for next recording
        audioRecorder.resetRecordingProperties();
    },
    /** Stop all the tracks on the active stream in order to stop the stream and remove
     * the red flashing dot showing in the tab
     */
    stopStream: function () {
        //stopping the capturing request by stopping all the tracks on the active stream
        audioRecorder.streamBeingCaptured.getTracks() //get all tracks from the stream
            .forEach(track /*of type MediaStreamTrack*/ => track.stop()); //stop each one
    },
    /** Reset all the recording properties including the media recorder and stream being captured*/
    resetRecordingProperties: function () {
        audioRecorder.mediaRecorder = null;
        audioRecorder.streamBeingCaptured = null;

        /*No need to remove event listeners attached to mediaRecorder as
        If a DOM element which is removed is reference-free (no references pointing to it), the element itself is picked
        up by the garbage collector as well as any event handlers/listeners associated with it.
        getEventListeners(audioRecorder.mediaRecorder) will return an empty array of events.*/
    }
}

if (document.getElementsByClassName('file-box').length) {
    var html = document.getElementsByClassName('file-box')[0].outerHTML

    var number2 = 0

    function imgClick2() {
        $('#input-' + number2).click()
    }

    function inputChange(e) {

        var files = e.target.files

        Object.keys(files).forEach(index => {

            const file = files[index];
            let reader = new FileReader()
            reader.readAsDataURL(file)

            reader.onload = function () {

                $('#preview-' + number2).attr('src', reader.result)

                number2++

                let add = html.replace('preview-0', 'preview-' + number2).replace('input-0', 'input-' + number2)

                $('#befor-in').before(add)

            }
        })
    }

    function uploadImage() {

        let type = $('input[name=newAddress]:checked', '#upload-image-box').val()

        let message = $('#add-media-description').val()

        $('#upload-image-box').hide()

        $('.OverHide').show()
        $('.Loading').show()
        $('.loading-other').show()

        var formData = new FormData();

        let fileSize = 0

        document.getElementsByName('media').forEach((item, index) => {
            for (var i = 0; i < item.files.length; i++) {
                formData.append('Attachment', item.files[i])
                fileSize = fileSize + item.files[i].size
            }
        })

        var projectId = document.getElementById("projectId").value

        if (window.location.href.indexOf('?id=') != -1) {
            const urlParams = new URLSearchParams(window.location.search);
            projectId = urlParams.get('id');
        }

        formData.append('ProjectId', projectId)
        formData.append('Type', type)
        formData.append('Message', message)

        $.post({
            xhr: function () {
                var xhr = new window.XMLHttpRequest();

                xhr.upload.addEventListener("progress", function (evt) {
                    if (evt.lengthComputable) {
                        var percentComplete = evt.loaded / evt.total;
                        percentComplete = parseInt(percentComplete * 100);

                        $('.upload-progress-bar').width(percentComplete + '%')
                        $('.upload-percent').html(percentComplete + '%')
                        $('.upload-size').html((fileSize / 1024 / 1024).toFixed(1) + 'MB')

                        if (percentComplete === 100) {

                        }

                    }
                }, false);

                return xhr;
            },
            data: formData,
            url: '/conversation/add-image',
            contentType: false,
            processData: false,
            success: function (result, status, xhr) {

                if (message) {
                    rightChat({ message: message, id: crypto.randomUUID() })
                }

                if (result) {
                    result.forEach(item => {
                        rightChat(item)
                    })
                }

                $('#add-media-description').val('')

                while (document.getElementsByName('media').length >= 2) {
                    document.getElementsByName('media')[0].parentElement.remove();
                }

                $('#upload-image-box').hide()
                $('.OverHide').hide()
                $('.Loading').hide()
                $('.loading-other').hide()
            }
        })
    }

    function closeUpload() {
        $('#upload-image-box').hide()
        $('.OverHide').hide()
    }
}

// Project Media

function sendFile() {

    $('.OverHide').show()
    $('.Loading').show()
    $('.loading-other').show()

    var attchment = document.getElementById('fileInput').files

    var projectId = document.getElementById('projectId').value

    var formData = new FormData();

    formData.append('ProjectId', projectId)
    //formData.append('ConversationId', messageIdForReply)
    formData.append('Type', 'File')

    let fileSize = 0

    for (var i = 0; i < attchment.length; i++) {
        formData.append('Attachment', attchment[i])
        fileSize = fileSize + attchment[i].size
    }

    let url = '/conversation/add'

    $.post({
        xhr: function () {
            var xhr = new window.XMLHttpRequest();

            xhr.upload.addEventListener("progress", function (evt) {
                if (evt.lengthComputable) {
                    var percentComplete = evt.loaded / evt.total;
                    percentComplete = parseInt(percentComplete * 100);

                    $('.upload-progress-bar').width(percentComplete + '%')
                    $('.upload-percent').html(percentComplete + '%')
                    $('.upload-size').html((fileSize / 1024 / 1024).toFixed(1) + 'MB')

                    if (percentComplete === 100) {

                    }

                }
            }, false);

            return xhr;
        },
        data: formData,
        url: url,
        contentType: false,
        processData: false,
        success: function (result, status, xhr) {
            rightChat(result)
            $('.OverHide').hide()
            $('.Loading').hide()
            $('.loading-other').hide()
        }
    })
}

function mediaGallery() {
    lightGallery(document.getElementById('lightGallery'), {
        //animateThumb: false,
        //zoomFromOrigin: false,
        //allowMediaOverlap: true,
        //toggleThumb: true,
        selector: '.lightgallery-selector',
        thumbnail: true,
    });

    lightGallery(document.getElementById('lightGallery2'), {
        //animateThumb: false,
        //zoomFromOrigin: false,
        //allowMediaOverlap: true,
        selector: '.lightgallery-selector2',
        thumbnail: true,
    });

    $('.load-row-btn').click(function (e) {
        $(this).remove()
    })

    Array.from(document.getElementsByClassName('lightgallery-selector')).forEach(item => {
        item.getElementsByTagName('img')[0].addEventListener('load', hideImageLoading)
        item.getElementsByTagName('img')[0].addEventListener('error', hideImageLoading)
    })

    Array.from(document.getElementsByClassName('lightgallery-selector2')).forEach(item => {
        item.getElementsByTagName('img')[0].addEventListener('load', hideImageLoading)
        item.getElementsByTagName('img')[0].addEventListener('error', hideImageLoading)
    })
}

function LoadRow(className) {
    Array.from(document.getElementsByClassName(className)).forEach(item => {
        $(item.getElementsByTagName('img')[0]).attr('src', $(item).attr('href'))
        $(item.getElementsByTagName('img')[1]).show()
        $(item.getElementsByTagName('span')[0]).hide()
        $(item.getElementsByTagName('span')[1]).hide()
        $(item.getElementsByTagName('span')[2]).hide()
    })
}

function hideImageLoading() {
    this.parentElement.getElementsByTagName('img')[1].style.display = 'none'
    this.parentElement.getElementsByTagName('span')[0].style.display = 'block'
    this.parentElement.getElementsByTagName('span')[1].style.display = 'block'
    this.parentElement.getElementsByTagName('span')[2].style.display = 'block'
}

function createImage(options) {
    options = options || {};
    const img = (Image) ? new Image() : document.createElement("img");
    if (options.src) {
        img.src = options.src;
    }
    return img;
}

function convertToPng(imgBlob) {
    const imageUrl = window.URL.createObjectURL(imgBlob);
    const canvas = document.createElement("canvas");
    const ctx = canvas.getContext("2d");
    const imageEl = createImage({ src: imageUrl });
    imageEl.onload = (e) => {
        canvas.width = e.target.width;
        canvas.height = e.target.height;
        ctx.drawImage(e.target, 0, 0, e.target.width, e.target.height);
        canvas.toBlob(copyToClipboard, "image/png", 1);
    };
}

async function copyImage(e, idName) {

    e.stopPropagation()

    $('.OverHide').show()
    $('.Loading').show()

    let src = document.getElementById(idName).src
    const img = await fetch(src);
    const imgBlob = await img.blob();

    if (imgBlob.type == 'image/jpeg') {
        convertToPng(imgBlob);
    }
    else if (imgBlob.type == 'image/png') {
        copyToClipboard(imgBlob);
    }
    else {
        console.error("Format unsupported");
    }
}

async function copyToClipboard(pngBlob) {
    try {
        await navigator.clipboard.write([
            new ClipboardItem({
                [pngBlob.type]: pngBlob
            })
        ]);
        console.log("Image copied");
    } catch (error) {
        console.error(error);
    }

    $('.OverHide').hide()
    $('.Loading').hide()
}

if ($('.audio-player')) {
    Array.from(document.querySelectorAll('.audio-player')).map((p) => new Plyr(p));
}

$('.user-avatar-image').click(function () {

    $('.user-avatar-file').click();
});

$('.user-avatar-file').change(function (e) {

    $('.OverHide').show()
    $('.Loading').show()

    var file = e.target.files[0];
    var formData = new FormData();
    formData.append('image', file);

    $.post({
        data: formData,
        url: '/public/avatar',
        contentType: false,
        processData: false,
        success: function (result, status, xhr) {

            let reader = new FileReader();
            reader.readAsDataURL(file);
            reader.onload = function () {

                $('.user-avatar-image').attr('src', reader.result)
            }

            $('.OverHide').hide()
            $('.Loading').hide()
        }
    })
});

//$('.chat-content .msg-group').each(function(index) {

//    console.log($(this).html())
//})

$('.reply-up-message').click(function () {

    let id = $(this).data('id');
    document.getElementById('conversation-' + id).scrollIntoView();
    $('.user-chats').scrollTop($('.user-chats').scrollTop() - 10)
})

$('#Print').click(function () {

    if ($('#Print').is(':checked')) {
        $('#PrintPrice').parent().show();
    } else {
        $('#PrintPrice').parent().hide();
    }
})

$('#print-add-repair').click(function () {

    if ($('#print-add-repair').is(':checked')) {
        $('#PrintPrice').parent().show();
    } else {
        $('#PrintPrice').parent().hide();
    }
})

$('.Print-Repair').click(function () {

    if ($(this).is(':checked')) {

        $(this).parent().parent().find('.PrintPrice').parent().show();
    } else {
        $(this).parent().parent().find('.PrintPrice').parent().hide();
    }
})



//$('.OverHide').click(function () {
//    if ($('.mini-chat').css('display') == 'block') {
//        $('.mini-chat').hide();
//        $('.OverHide').hide();
//    }
//})

$('.mini-chat-close').click(function () {
    $('.mini-chat').hide();
    if ($('.project-report-popup').css('opacity') == 0) {
        $('.project-report-popup').css('opacity', 1);
    } else {
        $('.OverHide').hide();
    }
})

function downloadFile(e, src) {

    e.stopPropagation();

    //$('.OverHide').show();
    //$('.Loading').show();



    $($(e.srcElement).parent().find('svg')).hide();
    $($(e.srcElement).parent().find('.file-loader')[0]).css('display', 'block');

    var xhr = new XMLHttpRequest();
    xhr.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {

            saveData([this.response], src.split('/')[src.split('/').length - 1])

            //insertFileToDb({ url: src, blob: this.response });

            $($(e.srcElement).parent().find('.file-loader')[0]).hide();
            $($(e.srcElement).parent().find('svg')[1]).show();


            //$('.OverHide2').hide();
            //$('.loading-fb').hide();
        }
    }

    xhr.onprogress = (e) => {
        console.log(e.loaded)
        console.log(e.total)
    }

    xhr.open('GET', src);
    xhr.responseType = 'blob';
    xhr.send();
}

function saveData(data, fileName) {
    var a = document.createElement("a");
    document.body.appendChild(a);
    a.style = "display: none";
    //var json = JSON.stringify(data);
    //var type = data[0].type;
    //var blob = new Blob([json], { type: type });
    var url = window.URL.createObjectURL(data[0]);
    a.href = url;
    a.download = fileName;
    a.click();
    window.URL.revokeObjectURL(url);
}

var isReply;
var messageIdForReply;

function ReplyMessage (id) {

    if (isEdit) {
        document.getElementById("messageInput").value = null
        isEdit = false

        $('#recordButton').show();
        $('#selectPhoto').show();
        $('#selectFileButton').show();
    }

    isReply = true
    messageIdForReply = $(popOverEl).data('id')

    if (Number.isInteger(messageIdForReply) == false) {

        clearReply();
        return
    }

    $('.chat-reply-content label').text($(popOverEl).data('name'))

    $('.chat-reply-content p').text('صدا')

    if ($(popOverEl).find('.message-text').length !== 0) {
        $('.chat-reply-content p').text($(popOverEl).find('p.message-text').text())
    }

    if ($(popOverEl).find('.message-photo').length !== 0) {
        $('.chat-reply-content p').text('تصویر')
    }

    if ($(popOverEl).find('.chat-message-file').length !== 0) {
        $('.chat-reply-content p').text($($(popOverEl).find('.chat-message-file-details > div')[0]).text());
    }

    $('.chat-reply-box').show();

    if (!$('.user-chats').attr('style') || !$('.user-chats').attr('style').includes('264')) {
        $('.user-chats').attr('style', 'height: calc(100vh - 264px) !important');

        $('.user-chats').animate({ scrollTop: $('.user-chats').scrollTop() + 73 }, 'slow');
    }

    $('.popover-overhide').click();

    $('#messageInput').focus()
}

function clearReply() {
    messageIdForReply = null;
    $('.user-chats').attr('style', 'height: calc(100vh - 191px) !important');
    $('.chat-reply-box').hide();
    isReply = null;
    previousTouch = null

    if (isEdit) {
        document.getElementById("messageInput").value = null
    }

    isEdit = false

    $('#recordButton').show();
    $('#selectPhoto').show();
}

document.addEventListener('contextmenu', event => {

    var container = $('.chat-content .msg-group .message-text, .chat-content .msg-group .message-photo, .chat-content .msg-group .chat-audio, .chat-content .msg-group .chat-message-file');

    if (!container.is(event.target) && container.has(event.target).length === 0) {

    } else {
        event.preventDefault()
    }
});

$('body').on('mouseup', '.chat-content .msg-group .message-text, .chat-content .msg-group .message-photo, .chat-content .msg-group .chat-audio, .chat-content .msg-group .chat-message-file', function (event) {

    if (event.which == 3) {

        //$('.chat-dropdown').hide();
        $('#dropdown-' + $(this).data('id')).fadeIn(300).css('display', 'flex');
        event.preventDefault();
    }
});