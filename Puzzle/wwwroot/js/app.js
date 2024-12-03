

// Notification support

function showNotification(title, body) {
    let notificationOptions = {
        body: body,
        icon: '/images/icons/192.png'
    }
    let notif = new Notification(title, notificationOptions);

    notif.onclick = () => {
        console.log('Notification clicked');
    }
}

if (window.Notification) {
    if (Notification.permission === 'granted') {
    } else if (Notification.permission !== 'denied') {
        Notification.requestPermission(permission => {
            if (permission === 'granted') {
            }
        })
    }
}

try {
    messaging.getToken({ vapidKey: 'BB__EQoTMXaIa8xMn3e4yMuWOmWSxVM1PMwtAHk0J1r3VpZYLC9WdXZt-sy5XuPMsiMTshf5yMNi4xL4PuI_csY' }).then((currentToken) => {
        if (currentToken) {
            // Send the token to your server and update the UI if necessary
            // ...
            console.log(currentToken);


            $.post({
                data: { firbase: currentToken },
                url: '/my/add-fcmtoken',
                success: function (result, status, xhr) {


                }
            })


            return currentToken;
        } else {
            // Show permission request UI
            console.log('No registration token available. Request permission to generate one.');
            // ...
        }
    }).catch((err) => {
        console.log('An error occurred while retrieving token. ', err);
        // ...
    });
} catch (eee) {
    //alert(eee);
}

try {
    messaging.onMessage(payload => {
        console.log('Message received. ', payload);
        // ...
        showNotification(payload.notification.title, payload.notification.body);
    });
} catch (eee) {
    //alert(eee);
}


$("body").on("contextmenu", "img", function (e) {
    return false;
});

var chatLightGallery;

createChatLightGallery();

function createChatLightGallery() {
    chatLightGallery = null
    if (document.getElementById('chat-light-gallery')) {
        chatLightGallery = lightGallery(document.getElementById('chat-light-gallery'), {
            selector: '.chat-light-gallery-selector',
            thumbnail: true,
            mobileSettings: {
                showCloseIcon: true,
                download: true,
            }
        });

        lightDownloader(chatLightGallery);
    }
}

function lightDownloader(elem) {

    elem.el.addEventListener('lgAfterSlide', function (e) {

        if (location.pathname.includes('/my/projectdetails')) {
            let url = elem.galleryItems[e.detail.index].src;

            let ex = url.split('?')[0].split('/')[url.split('/').length - 1].split('.')[1]
            let name = guid(10) + '.' + ex;

            if (!getCookie(url)) {
                setCookie(url, 'true', 21);
                downloadImage(url, name);
            }
        }
    });
}


for (var iwe = 0; iwe < $('.project-details-lightgallery').length; iwe++) {

    var elId = $('.project-details-lightgallery')[iwe].id

    var projectDetailsLightgallery = lightGallery(document.getElementById(elId), {
        selector: '.lightgallery-selector',
        thumbnail: true,
        mobileSettings: {
            showCloseIcon: true,
            download: true,
        }
    });

    lightDownloader(projectDetailsLightgallery);
}

if (document.getElementById('video-gallery')) {

    lightGallery(document.getElementById('video-gallery'), {
        download: false,
        plugins: [lgVideo]
    });
}

if (document.getElementById('project-chat-box')) {

    $('.user-chats').scrollTop($('.user-chats > .chats').height());
}

function removeChatLightGallery() {

    if (chatLightGallery) {
        chatLightGallery.destroy();
    }
}



function enterChat() {
    try {
        var projectId = document.getElementById("projectId").value

        const urlParams = new URLSearchParams(window.location.search);

        if (urlParams.get('id')) {
            projectId = urlParams.get('id')
        }

        var message = document.getElementById("messageInput").value;

        if (message.length == 0) {
            return
        }

        var conversationId = messageIdForReply

        let uuid = 'uuid-' + (Math.random() + 1).toString(36).substring(7)

        let url = '/my/conversation/add'

        if (isEdit) {
            url = '/my/conversation/update'
            uuid = $(popOverEl).data('id')
        }

        let conversation = { message: message, id: uuid }
        rightChat(conversation)

        document.getElementById("messageInput").value = null

        var formData = new FormData()
        formData.append('ProjectId', projectId)
        formData.append('Message', message)
        formData.append('ConversationId', conversationId)
        formData.append('Id', uuid)



        $.post({
            data: formData,
            url: url,
            contentType: false,
            processData: false,
            success: function (result, status, xhr) {
                //document.getElementById("messageInput").value = null
                //rightChat(result)


                document.getElementById('conversation-' + uuid).outerHTML = document.getElementById('conversation-' + uuid).outerHTML.replace(uuid, result.id)
                $('#conversation-' + result.id).html($('#conversation-' + result.id).html().replace(uuid, result.id))
            }
        })
    } catch (e) {
        AddError(e)
    }
}

function guid(length) {
    let result = '';
    const characters = 'abcdefghijklmnopqrstuvwxyz0123456789';
    const charactersLength = characters.length;
    let counter = 0;
    while (counter < length) {
        result += characters.charAt(Math.floor(Math.random() * charactersLength));
        counter += 1;
    }
    return result;
}

function AddError(ex) {
    try {
        $.post({
            data: { message: ex },
            url: '/error/add',
            success: function (result, status, xhr) {
                //document.getElementById("messageInput").value = null
                //rightChat(result)
            }
        })
    } catch (e) {
        alert(e)
    }
}

function rightChat(conversation, id) {

    if (isEdit) {

        $('#conversation-' + conversation.id).find('.message-text').text(conversation.message)

        clearReply()

        return
    }

    var myName = document.getElementById("myName").value
    var el = '<div class="chat chat-right">'
    el += '<div class="chat-body" id="conversation-' + conversation.id + '">'
    el += '<div class="chat-content" data-name="' + myName + '" data-id="' + conversation.id + '">'



    if (isReply) {

        var replyElement = $('#conversation-' + messageIdForReply).find('.chat-content')
        var messageTextParent = 'صدا'
        var widthStyle = ''

        if (replyElement.find('.message-text').length !== 0) {
            messageTextParent = replyElement.find('p.message-text').text()
        }

        if (replyElement.find('.message-photo').length !== 0) {
            messageTextParent = 'تصویر'
        }

        if (replyElement.find('.message-photo').length !== 0) {
            messageTextParent = 'تصویر'
        }

        if (replyElement.find('.chat-message-file').length !== 0) {
            messageTextParent = $(replyElement.find('.chat-message-file-details > div')[0]).text();
        }

        if (conversation.attachment) {
            if (conversation.type = 4) {
                widthStyle = 'width: 220px; max-width: 100%;'
            }
            else if (conversation.attachment.endsWith('.ogg')) {
                widthStyle = 'width: 220px;'
            } else {
                widthStyle = 'width: 120px;'
            }
        }

        el += '<div class="reply-up-message" data-id="' + messageIdForReply + '" style="' + widthStyle + '">'
        el += '<label>' + replyElement.data('name') + '</label>'
        el += '<p>'
        el += '<span>' + messageTextParent + '</span></p>'
        el += '</div>'
    }




    if (conversation.attachment) {
        if (conversation.type == 4) {
            el += '<div class="chat-message-file" data-url="' + conversation.attachment + '">'
            el += '<div class="chat-message-file-icon" onclick="downloadFile(event, \'' + conversation.attachment + '\')">'
            el += '<svg xmlns="http://www.w3.org/2000/svg" class="icon icon-tabler icon-tabler-download" width="22" height="22" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round">'
            el += '<path stroke="none" d="M0 0h24v24H0z" fill="none"></path>'
            el += '<path d="M4 17v2a2 2 0 0 0 2 2h12a2 2 0 0 0 2 -2v-2"></path>'
            el += '<path d="M7 11l5 5l5 -5"></path>'
            el += '<path d="M12 4l0 12"></path>'
            el += '</svg>'
            el += '<svg xmlns="http://www.w3.org/2000/svg" class="icon icon-tabler icon-tabler-file" width="22" height="22" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round">'
            el += '<path stroke="none" d="M0 0h24v24H0z" fill="none"></path>'
            el += '<path d="M14 3v4a1 1 0 0 0 1 1h4"></path>'
            el += '<path d="M17 21h-10a2 2 0 0 1 -2 -2v-14a2 2 0 0 1 2 -2h7l5 5v11a2 2 0 0 1 -2 2z"></path>'
            el += '</svg>'
            el += '<span class="file-loader"></span>'
            el += '</div>'
            el += '<div class="chat-message-file-details">'
            el += '<div>' + conversation.fileName + '</div>'
            el += '<div>' + (conversation.fileSize / 1024 / 1024).toFixed(2) + ' Mb</div>'
            el += '</div>'
            el += '</div>'
        }
        else if (conversation.attachment.endsWith('.ogg')) {
            el += '<audio class="audio-player" controls><source src="' + conversation.attachment + '" type="audio/ogg" /></audio>'
        } else {
            el += '<div href="/picture' + conversation.attachment + '?width=1920&quality=100&format=jpeg" class="chat-light-gallery-selector">'
            el += '<img class="message-photo" src="/picture' + conversation.attachment + '?width=480&format=jpeg" />'
            el += '</div>'
        }
    } else {
        el += '<p class="message-text">' + conversation.message + '</p>'
    }

    el += '</div>'


    clearReply();
    //el += '<div class="chat-dropdown" id="dropdown-' + conversation.id + '">'
    //if (conversation.message) {
    //    el += '<label class="chat-dropdown-item" onclick="EditMessage(' + conversation.id + ')">ویرایش</label>'
    //}
    //el += '<label class="chat-dropdown-item" onclick="RemoveMessage(' + conversation.id + ')">حذف</label>'
    //el += '</div>'

    el += '</div>'
    el += ' </div>'

    $('.end-chat').before(el);

    chatLightGallery.refresh();

    new Plyr(document.querySelectorAll('.audio-player')[document.querySelectorAll('.audio-player').length - 1])

    $('.user-chats').scrollTop($('.user-chats > .chats').height());
}

function selectPhoto() {
    $('#photoInput').click()
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
    formData.append('ConversationId', messageIdForReply)

    let url = '/my/conversation/add'

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
            $('.loading-fb').hide()
        }
    })
}

function sendFile() {

    $('.OverHide2').show();
    $('.upload-progress').css('display', 'flex');

    var attchment = document.getElementById('fileInput').files

    var projectId = document.getElementById('projectId').value

    var formData = new FormData();

    formData.append('ProjectId', projectId)
    formData.append('ConversationId', messageIdForReply)
    formData.append('Type', 'File')

    let fileSize = 0

    for (var i = 0; i < attchment.length; i++) {
        formData.append('Attachment', attchment[i])
        //fileSize = fileSize + attchment[i].size
    }

    //let totalSize = (fileSize / 1024 / 1024).toFixed(1);

    let url = '/my/conversation/add'

    $.post({
        xhr: function () {
            var xhr = new window.XMLHttpRequest();

            xhr.upload.addEventListener("progress", function (evt) {
                if (evt.lengthComputable) {
                    var percentComplete = evt.loaded / evt.total;
                    percentComplete = parseInt(percentComplete * 100);

                    $('.upload-progress-bar > div').width(percentComplete + '%')

                    $('.upload-progress-size').html((evt.loaded / 1024 / 1024).toFixed(1) + ' مگابایت از ' + (evt.total / 1024 / 1024).toFixed(1) + ' مگابایت')

                    if (percentComplete === 100) {
                        $('.OverHide2').hide();
                        $('.upload-progress').hide();
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
            $('.loading-fb').hide()
        }
    })
}


function sendPhoto() {

    $('.OverHide2').show()
    $('.loading-fb').show()

    let successCount = 0

    var attchment = document.getElementById("photoInput").files

    for (var i = 0; i < attchment.length; i++) {

        AddError(attchment[i].name);

        if (attchment[i].name.toLowerCase().includes('.heic')) {
            heic2any({
                blob: attchment[i],
                toType: "image/jpeg",
                /*quality: 0.5, // cuts the quality and size by half*/
            }).then((conversionResult) => {

                //var url = URL.createObjectURL(conversionResult);
                //document.getElementById("chat-app-form").innerHTML = `<a target="_blank" href="${url}"><img src="${url}"></a>`;


                new Compressor(conversionResult, {
                    //quality: 1,
                    maxWidth: 1680,
                    maxHeight: 1680,
                    success(result) {

                        quality = 1

                        if (result.size > 2 * 1024 * 1024) {
                            quality = 0.6
                        } else if (result.size > 1 * 1024 * 1024) {
                            quality = 0.7
                        }
                        else if (result.size > 512 * 1024 * 1024) {
                            quality = 0.75
                        }
                        else if (result.size > 384 * 1024) {
                            quality = 0.8
                        }
                        else if (result.size > 256 * 1024) {
                            quality = 0.85
                        }

                        new Compressor(result, {
                            quality: quality < 1 ? quality : null,
                            maxWidth: 1680,
                            maxHeight: 1680,
                            success(result) {

                                var projectId = document.getElementById("projectId").value

                                const urlParams = new URLSearchParams(window.location.search);

                                if (urlParams.get('id')) {
                                    projectId = urlParams.get('id')
                                }

                                var formData = new FormData();

                                formData.append('Attachment', result, 'heic.jpg')
                                formData.append('ProjectId', projectId)
                                formData.append('ConversationId', messageIdForReply)

                                let url = '/my/conversation/add'

                                $.post({
                                    data: formData,
                                    url: url,
                                    contentType: false,
                                    processData: false,
                                    success: function (result, status, xhr) {

                                        rightChat(result)

                                        successCount++

                                        if (attchment.length == successCount) {

                                            $('.OverHide2').hide()
                                            $('.loading-fb').hide()
                                        }
                                    }
                                })
                            },
                            error(err) {
                                AddError(err);
                                alert('خطا در انجام عملیات!');
                            },
                        });
                    },
                    error(err) {
                        AddError(err);
                        alert('خطا در انجام عملیات!');
                    },
                });
                console.log('a')
            })
                .catch((e) => {
                    console.log('b')
                });
        }
        else {

            new Compressor(attchment[i], {
                //quality: 1,
                maxWidth: 1680,
                maxHeight: 1680,
                success(result) {

                    quality = 1

                    if (result.size > 2 * 1024 * 1024) {
                        quality = 0.6
                    } else if (result.size > 1 * 1024 * 1024) {
                        quality = 0.7
                    }
                    else if (result.size > 512 * 1024 * 1024) {
                        quality = 0.75
                    }
                    else if (result.size > 384 * 1024) {
                        quality = 0.8
                    }
                    else if (result.size > 256 * 1024) {
                        quality = 0.85
                    }

                    new Compressor(result, {
                        quality: quality < 1 ? quality : null,
                        maxWidth: 1680,
                        maxHeight: 1680,
                        success(result) {

                            var projectId = document.getElementById("projectId").value

                            const urlParams = new URLSearchParams(window.location.search);

                            if (urlParams.get('id')) {
                                projectId = urlParams.get('id')
                            }

                            var formData = new FormData();

                            formData.append('Attachment', result, result.name)
                            formData.append('ProjectId', projectId)
                            formData.append('ConversationId', messageIdForReply)

                            let url = '/my/conversation/add'

                            $.post({
                                data: formData,
                                url: url,
                                contentType: false,
                                processData: false,
                                success: function (result, status, xhr) {

                                    rightChat(result)

                                    successCount++

                                    if (attchment.length == successCount) {

                                        $('.OverHide2').hide()
                                        $('.loading-fb').hide()
                                    }
                                }
                            })
                        },
                        error(err) {
                            AddError(err);
                            alert('خطا در انجام عملیات!');
                        },
                    });
                },
                error(err) {
                    AddError(err);
                    alert('خطا در انجام عملیات!');
                },
            });
        }
    }
}

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
            $('.OverHide2').show()
            stopButton.style.display = 'flex';

        })
        .catch(error => { //on error
            alert('میکروفون یافت نشد')
        });
}

function stopRecording() {

    $('.OverHide').show()
    $('.loading-fb').show()

    audioRecorder.stop()
        .then(audioAsblob => {

            $('#recordButton').show();
            $('#stopButton').hide();
            $('.OverHide2').hide()

            //Play recorder audio
            sendVoice(audioAsblob);
        })
        .catch(error => {
            alert('خطا')
        });
}

function login() {

    let mobile = $('.login > div > input').val()
    if (!mobile) {
        alert('موبایل را وارد نمایید.')
        return;
    }

    if ((mobile.length < 10) || ((mobile.substr(0, 1) == "0" || mobile.substr(0, 1) == "۰") && mobile.length < 11)) {
        alert('موبایل وارد شده صحیح نمی‌باشد.')
        return;
    }

    if (!$('#login-role-checkbox').is(':checked')) {
        alert('تایید قوانین و مقرارت الزامی می‌باشد.')
        return;
    }

    $('.OverHide').show()
    $('.loading-fb').show()

    $.post({
        data: { Mobile: mobile },
        url: "/my/login",
        success: function (result, status, xhr) {

            $('.verify input').first().val(mobile)

            $('.login').hide()
            $('.verify').show()
            $('.verify input').last().focus()

            $('.OverHide').hide()
            $('.loading-fb').hide()
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            try {
                alert(XMLHttpRequest.responseJSON.message)
            } catch (e) {

            }

            $('.OverHide').hide()
            $('.loading-fb').hide()
        }
    });
}

function verify() {

    var code = $('.verify input').last().val()

    $.post({
        data: { Mobile: $('.login input').val(), Code: code },
        url: "/my/verify",
        success: function (result, status, xhr) {
            location.pathname = "/my/project"
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("کد وارد شده صحیح نمی باشد");
        }
    });
}

$('#verify-1').keyup(function (e) {
    if (e.keyCode == 8) {
        return
    }
    $('#verify-2').focus()
})

$('#verify-2').keyup(function (e) {
    if (e.keyCode == 8) {
        $('#verify-1').focus()
        return
    }
    $('#verify-3').focus()
})

$('#verify-3').keyup(function (e) {
    if (e.keyCode == 8) {
        $('#verify-2').focus()
        return
    }
    $('#verify-4').focus()
})

$('#verify-4').keyup(function (e) {
    if (e.keyCode == 8) {
        $('#verify-3').focus()
        return
    }
    verify()
})

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

let category;
let quality;
let addProjectStep = 1;

$('.category-title').click(function () {
    //$('.details-box').hide()

    //$('.select-box').hide()
    //$('.add-project-select-category').hide()
    $('.select-box').removeClass('active')
    ////$('.quality-box label').removeClass('active')

    category = null;
    quality = null;

    $(this).parent().addClass('active')

    //$(this).parent().children('.details-box').show()

    //if ($(this).parent().children('.details-box').children('.category-box').length) {
    //    $(this).parent().children('.details-box').children('.category-box').show()
    //    $(this).parent().children('.details-box').children('.quality-box').hide()
    //} else {
    //    $(this).parent().children('.details-box .quality-box').show()
    //}

    //if ($(this).parent().children('.details-box').children('.category-box').length || $(this).parent().children('.details-box').children('.quality-box').length) {
    //    $('.submit-btn').hide()
    //} else {
    //    $('.submit-btn').show()
    category = $('.select-box.active').data('id')
    //}
})

$('.category-box label').click(function () {
    $('.category-box label').removeClass('active')

    $(this).addClass('active')

    category = $(this).data('id')

    if ($(this).parent().parent().children('.quality-box').length) {
        $(this).parent().parent().children('.quality-box').show()
    } else {
        $('.submit-btn').show()
    }
})

$('.quality-box label').click(function () {
    $('.quality-box label').removeClass('active')

    $(this).addClass('active')

    quality = $(this).data('id')

    $('.submit-btn').show()
})

let redirectUrl;

$('.add-project-box .submit-btn').click(function () {

    if (addProjectStep == 1) {

        if ($('.select-box.active').length == 0) {
            alert('دسته بندی مورد نظر را انتخاب نمایید')
            return
        }

        $('.add-project-select-category').hide()
        $('.select-box .category-title').hide()

        $('.select-box.active').children('.details-box').show()

        if ($('.select-box.active').find('.category-box').length) {

            addProjectStep = 2
        }
        else if ($('.select-box.active').find('.quality-box').length) {

            addProjectStep = 3
            $('.quality-box').show()
        }
        else {

            addProjectStep = 4
            $('.add-project-title-box').show()
            $('.submit-btn-box label').text('تایید و ثبت')
        }

    } else if (addProjectStep == 2) {

        if ($('.add-project-type-item label.active').length == 0) {
            alert('نوع مورد نظر را انتخاب نمایید')
            return
        }

        if ($('.select-box.active').find('.quality-box').length) {

            addProjectStep = 3
            $('.category-box').hide()
            $('.quality-box').show()
        }
        else {

            addProjectStep = 4
            $('.category-box').hide()
            $('.add-project-title-box').show()
            $('.submit-btn-box label').text('تایید و ثبت')
            $('.add-project-box #title').focus()
        }

    } else if (addProjectStep == 3) {

        if ($('.add-project-quality-box label.active').length == 0) {
            alert('کیفیت مورد نظر را انتخاب نمایید')
            return
        }

        $('.quality-box').hide()
        $('.add-project-title-box').show()
        $('.submit-btn-box label').text('تایید و ثبت')
        $('.add-project-box #title').focus()

        addProjectStep++;

    } else if (addProjectStep == 4) {

        if (!$('.add-project-box #title').val()) {
            alert('عنوان پروژه را وارد نمایید')
            return
        }

        $('.OverHide').show()
        $('.loading-fb').show()

        $.post({
            data: { quality, category, title: $('#title').val() },
            url: '/my/project/add',
            success: function (result, status, xhr) {

                redirectUrl = '/my/conversation/' + result.id;

                $('.OverHide').hide()
                $('.loading-fb').hide()

                $('.OverHide2').show()
                $('.add-project-modal').show()
            }
        })
    }
})

$('.add-project-modal-footer button').click(function () {

    location.href = redirectUrl;

    $('.add-project-modal').hide();
    $('.loading-fb').show();
})

if (location.pathname == '/my/project') {
    if (getCookie('show-notfi-confirm') != 'true') {
        if (getCookie('notfi-confirm-status') != 'true') {

            $('.OverHide2').show()
            $('.notif-confirm-modal').show()
        }
    }
}

$('.notif-confirm-no').click(function () {
    setCookie('show-notfi-confirm', 'true', 365)
    $('.notif-confirm-modal').hide()

    $('.loading-fb').show()

    $.post({
        data: { status: false },
        url: '/my/pushsms/status',
        success: function (result, status, xhr) {

            $('.OverHide2').hide()
            $('.loading-fb').hide()
        }
    })

})

$('.notif-confirm-yes').click(function () {
    setCookie('show-notfi-confirm', 'true', 365)
    $('.notif-confirm-modal').hide()

    $('.loading-fb').show()

    $.post({
        data: { status: true },
        url: '/my/pushsms/status',
        success: function (result, status, xhr) {

            $('.OverHide2').hide()
            $('.loading-fb').hide()
        }
    })
})

function setCookie(name, value, days) {
    var expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}

function getCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

$('.add-repair-modal-type div').click(function () {
    $('.add-repair-modal-type div').removeClass('active')
    $(this).addClass('active')
})

$('.add-repair-modal-close').click(function () {

    $('.OverHide2').hide()
    $('.add-repair-modal').hide()

    $('.add-repair-modal-type div').removeClass('active')
})

var projectIdForRepair = null

$('.my-repair-add-btn').click(function () {

    $('.OverHide2').fadeIn(200)
    $('.add-repair-modal').fadeIn(300)

    projectIdForRepair = $(this).data('id')
})

$('.chat-bottom-done div').click(function () {

    $('.my-repair-add-btn-side a').click()
    $('.OverHide2').fadeIn(200)
})

$('.my-repair-add-btn-side a').click(function () {

    document.getElementsByClassName('project-details-side')[0].style.width = "0px";
    $('.add-repair-modal').delay(300).fadeIn(200)

    projectIdForRepair = $(this).parent().data('id')
})

$('.add-repair-modal-footer button').click(function () {

    if ($('.add-repair-modal-type div.active').length == 0) {

        alert('یک گزینه را انتخاب نمایید')
        return
    }

    $('.add-repair-modal').hide()

    $('.loading-fb').show()

    let projectCategory

    if ($('.add-repair-modal-type div.active').text() == 'تغییرات') {

        projectCategory = 9
    }
    else if ($('.add-repair-modal-type div.active').text() == 'اصلاحیه') {

        projectCategory = 10
    }

    $.post({
        data: { projectCategory: projectCategory, projectId: projectIdForRepair },
        url: '/my/project/repair/add',
        success: function (result, status, xhr) {

            redirectUrl = '/my/conversation/' + projectIdForRepair;

            $('.loading-fb').hide()

            let text = $('.add-repair-modal-type div.active').text() + ' شما با موفقیت ثبت شد'
            text += '<br />'
            text += 'لطفا توضیحات و اطلاعات تکمیلی را ارسال کنید'

            $('.add-project-modal-description p').html(text)

            $('.add-project-modal').show()
        }
    })
})

$('.project-details-side-close').click(function () {

    document.getElementsByClassName('project-details-side')[0].style.width = "0px";
    $('.OverHide2').hide();
})

$('.project-details-side-open svg').click(function () {
    $('.OverHide2').show();
    document.getElementsByClassName('project-details-side')[0].style.width = "270px";

})








var popOverEl

var forcePopover = false;

$('body').on('click', '.chat-content', function (e) {

    e.stopPropagation();

    $('.chat-content').popover('dispose')
    $('.OverHide2').hide();
    $('.OverHide2').removeClass('popover-overhide');


    if (forcePopover == false) {

        if ($(this).find('.plyr').length != 0) {
            return;
        }


        if ($(this).find('.chat-light-gallery-selector').length != 0) {
            return;
        }
    }

    forcePopover = false

    $('.popover-reply').show();
    $('.popover-copy').show();
    $('.popover-download').show();
    $('.popover-edit').show();
    $('.popover-delete').show();

    popOverEl = this;

    $('.OverHide2').show();
    $('.OverHide2').addClass('popover-overhide');
    $(this).css('z-index', '1100')

    $('.chat-content').popover('hide')

    if ($(this).parent().parent().hasClass('chat-right') == false) {

        $('.popover-edit').hide();
        $('.popover-delete').hide();
    }

    if ($(this).find('.message-text').length == 0) {

        $('.popover-copy').hide();
        $('.popover-edit').hide();

    }

    if ($(this).find('.message-photo').length == 0) {

        $('.popover-download').hide();

    }

    if ($(this).find('.chat-light-gallery-selector-pre').length != 0) {

        $('.popover-download').hide();
    }

    $(this).popover({
        container: '.user-chats',
        //placement: 'top',
        html: true,
        content: function () {
            var clone = $('#popover-content-html').clone(true).removeClass('hide');
            return clone;
        }
    })

    $(this).popover('show')

})






//#region Reply Drage To Right

let chatElementRight = 0
var chatElementDrag
var isDown = false
var chatLightGalleryDestroy = false
var isChatLeft = false
var isReply = false

var messageIdForReply

//$('body').on('mousedown', '.chat-right .chat-content', function () {

//    chatElementDrag = this;
//    isDown = true;
//    isChatLeft = false;
//});

//$('body').on('mousedown', '.chat-left .chat-content', function () {

//    chatElementDrag = this;
//    isDown = true;
//    isChatLeft = true;
//});

//$('body').on('touchstart', '.chat-right .chat-content', function () {

//    chatElementDrag = this;
//    isDown = true;
//    isChatLeft = false;
//    previousTouch = null
//});

//$('body').on('touchstart', '.chat-left .chat-content', function () {

//    chatElementDrag = this;
//    isDown = true;
//    isChatLeft = true;
//    previousTouch = null
//});




//document.addEventListener('mousemove', function (event) {
//    //event.preventDefault();
//    if (isDown) {
//        var deltaX = event.movementX;


//        if (isChatLeft) {

//            chatElementRight += deltaX

//            if (chatElementRight > 50) {
//                chatElementRight = 50
//            }

//            if (chatElementRight < 0) {
//                chatElementRight = 0
//            }
//        } else {

//            chatElementRight -= deltaX

//            if (chatElementRight > 0) {
//                chatElementRight = 0
//            }

//            if (chatElementRight < -50) {
//                chatElementRight = -50
//            }
//        }

//        if (chatElementRight > 25 || chatElementRight < -25) {
//            isReply = true
//        }

//        if (isChatLeft) {

//            chatElementDrag.style.left = chatElementRight + 'px';
//        } else {

//            chatElementDrag.style.right = chatElementRight + 'px';
//        }

//        if (chatLightGalleryDestroy == false) {
//            chatLightGalleryDestroy = true
//            removeChatLightGallery()
//        }
//    }
//}, true);

//var previousTouch;

//document.addEventListener('touchmove', function (event) {
//    //event.preventDefault();
//    if (isDown) {

//        const touch = event.touches[0];

//        if (previousTouch) {

//            var deltaX = touch.pageX - previousTouch.pageX;

//            if (isChatLeft) {

//                chatElementRight += deltaX

//                if (chatElementRight > 50) {
//                    chatElementRight = 50
//                }

//                if (chatElementRight < 0) {
//                    chatElementRight = 0
//                }
//            } else {

//                chatElementRight -= deltaX

//                if (chatElementRight > 0) {
//                    chatElementRight = 0
//                }

//                if (chatElementRight < -50) {
//                    chatElementRight = -50
//                }
//            }

//            if (chatElementRight > 25 || chatElementRight < -25) {
//                isReply = true
//            }

//            if (isChatLeft) {

//                chatElementDrag.style.left = chatElementRight + 'px';
//            } else {

//                chatElementDrag.style.right = chatElementRight + 'px';
//            }

//        }

//        if (chatLightGalleryDestroy == false) {
//            chatLightGalleryDestroy = true
//            removeChatLightGallery()
//        }

//        previousTouch = touch;
//    }
//}, true);

//document.addEventListener('mouseup', function (event) {

//    console.log('mouseup')

//    if (isDown) {
//        //event.preventDefault();

//        isDown = false;
//        chatElementRight = 0

//        if (isChatLeft) {

//            chatElementDrag.style.left = '0px';
//        } else {

//            chatElementDrag.style.right = '0px';
//        }

//        if (isReply) {

//            messageIdForReply = $(chatElementDrag).data('id')

//            if (Number.isInteger(messageIdForReply) == false) {

//                clearReply();
//                return
//            }

//            $('.chat-reply-content label').text($(chatElementDrag).data('name'))

//            $('.chat-reply-content p').text('صدا')

//            if ($(chatElementDrag).find('.message-text').length !== 0) {
//                $('.chat-reply-content p').text($(chatElementDrag).find('p.message-text').text())
//            }

//            if ($(chatElementDrag).find('.message-photo').length !== 0) {
//                $('.chat-reply-content p').text('تصویر')
//            }

//            $('.chat-reply-box').show();

//            if (!$('.user-chats').attr('style') || !$('.user-chats').attr('style').includes('254')) {
//                $('.user-chats').attr('style', 'height: calc(100vh - 254px) !important');

//                $('.user-chats').animate({ scrollTop: $('.user-chats').scrollTop() + 73 }, 'slow');
//            }

//        }

//        $('.popover-overhide').click();
//    }



//    setTimeout(() => {


//        if (chatLightGalleryDestroy == true) {
//            chatLightGalleryDestroy = false
//            createChatLightGallery()
//        }
//    }, 100)

//}, true);

//document.addEventListener('touchend', function (event) {



//    if (isDown) {
//        //event.preventDefault();

//        isDown = false;
//        chatElementRight = 0

//        if (isChatLeft) {

//            chatElementDrag.style.left = '0px';
//        } else {

//            chatElementDrag.style.right = '0px';
//        }

//        if (isReply) {

//            messageIdForReply = $(chatElementDrag).data('id')

//            if (Number.isInteger(messageIdForReply) == false) {

//                clearReply();
//                return
//            }

//            $('.chat-reply-content label').text($(chatElementDrag).data('name'))

//            $('.chat-reply-content p').text('صدا')

//            if ($(chatElementDrag).find('.message-text').length !== 0) {
//                $('.chat-reply-content p').text($(chatElementDrag).find('p.message-text').text())
//            }

//            if ($(chatElementDrag).find('.message-photo').length !== 0) {
//                $('.chat-reply-content p').text('تصویر')
//            }

//            $('.chat-reply-box').show();

//            if (!$('.user-chats').attr('style') || !$('.user-chats').attr('style').includes('254')) {
//                $('.user-chats').attr('style', 'height: calc(100vh - 254px) !important');

//                $('.user-chats').animate({ scrollTop: $('.user-chats').scrollTop() + 73 }, 'slow');
//            }

//        }
//    }



//    setTimeout(() => {
//        if (chatLightGalleryDestroy == true) {
//            chatLightGalleryDestroy = false
//            createChatLightGallery()
//        }
//    }, 100)

//}, true);
$('.chat-reply-icon svg').click(function () {
    clearReply()
})

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




//#endregion










$('body').on('click', '.reply-up-message', function (e) {

    e.stopPropagation();
    let id = $(this).data('id');
    document.getElementById('conversation-' + id).scrollIntoView();
    $('.user-chats').scrollTop($('.user-chats').scrollTop() - 10)
});

if ($('.audio-player')) {
    Array.from(document.querySelectorAll('.audio-player')).map((p) => new Plyr(p));
}



$('body').on('click', '.chat-body', function (e) {

    if ($(this).hasClass('chat-content') || $(this).parents('.chat-content').length) {

        forcePopover = false
    } else {

        forcePopover = true
    }

    $(this).find('.chat-content').click()
})


function copy(text) {
    if (text) {
        text = text.trim()
    }
    navigator.clipboard.writeText(text);
}

$('body').on('click', '.popover-copy', function () {

    copy($(popOverEl).children('.message-text').text());
    $('.popover-overhide').click();
});

$('body').on('click', '.popover-reply', function () {

    if (isEdit) {
        document.getElementById("messageInput").value = null
        isEdit = false

        $('#recordButton').show();
        $('#selectPhoto').show();
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
});

$('body').on('click', '.popover-delete', function () {

    $('.OverHide2').hide();
    $('.OverHide2').removeClass('popover-overhide');

    $('.chat-content').popover('hide');
    $('.chat-content').css('z-index', 'unset');
});

$('body').on('click', '.popover-overhide', function () {

    $('.OverHide2').hide();
    $('.OverHide2').removeClass('popover-overhide');

    $('.chat-content').popover('hide');
    $('.chat-content').css('z-index', 'unset');

    $('.chat-content').popover('dispose')
});

$('.remove-item-cancel').click(function () {

    $('.OverHide2').hide();
    $('.remove-item-modal').hide()
})

$('.remove-item-cofirm').click(function () {

    $('.remove-item-modal').hide()
    $('.loading-fb').show()

    $.post({
        data: { id: $(popOverEl).data('id') },
        url: '/my/conversation/remove',
        success: function (result, status, xhr) {

            $(popOverEl).remove()

            $('.OverHide2').hide()
            $('.loading-fb').hide()


            clearReply();
        }
    })
})

$('.popover-delete').click(function () {

    $('.OverHide2').delay(100).fadeIn(200);
    $('.remove-item-modal').fadeIn(300);
})

$('.popover-download').click(function () {

    downloadImage($(popOverEl).children('.chat-light-gallery-selector').attr('href'));
    $('.popover-overhide').click();
})

var isEdit = false

$('.popover-edit').click(function () {

    isEdit = true

    $('#recordButton').hide();
    $('#selectPhoto').hide();


    $('.chat-reply-content label').text('ویرایش پیام');
    $('.chat-reply-content p').text($(popOverEl).children('.message-text').text().trim());
    document.getElementById("messageInput").value = $(popOverEl).children('.message-text').text().trim();

    if (!$('.user-chats').attr('style') || !$('.user-chats').attr('style').includes('264')) {
        $('.user-chats').attr('style', 'height: calc(100vh - 264px) !important');

        $('.user-chats').animate({ scrollTop: $('.user-chats').scrollTop() + 73 }, 'slow');
    }

    $('.chat-reply-box').show();

    $('.popover-overhide').click();

    $('#messageInput').focus()
})

async function downloadImage(imageSrc, name) {

    const link = document.createElement('a')
    link.href = imageSrc

    link.download = name ? name : imageSrc.split('/')[imageSrc.split('/').length - 1]
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
}

$('.user-details-box .submit-btn-box').click(function () {



    var firstName = $('.user-details-box .firstname').val()

    if (isEmptyOrSpaces(firstName)) {

        alert('نام را وارد نمایید');
        return;
    }

    var lastName = $('.user-details-box .lastname').val()

    if (isEmptyOrSpaces(lastName)) {

        alert('نام خانوادگی را وارد نمایید');
        return;
    }

    var city = $('.user-details-box .city').val()

    $('.OverHide2').show()
    $('.loading-fb').show()

    $.post({
        data: { firstName, lastName, city },
        url: '/my/user/update',
        success: function (result, status, xhr) {

            location.pathname = "/my/project"
        }
    })
})

function isEmptyOrSpaces(str) {
    return str === null || str.match(/^ *$/) !== null;
}

//$('.project-media-download-all').click(async function () {

//    $('.OverHide2').show()
//    $('.loading-fb').show()

//    let iii = 0

//    var render = $('.lightgallery-selector')

//    for (var i = 0; i < render.length; i++) {

//        let href = render[i].href

//        setTimeout(() => {

//            let ex = href.split('?')[0].split('/')[href.split('/').length - 1].split('.')[1]

//            downloadImage(href, guid(10) + '.' + ex);

//        }, iii * 200);

//        iii++;
//    }

//    var screen = $('.lightgallery-selector2')

//    for (var i = 0; i < screen.length; i++) {

//        let href = screen[i].href

//        setTimeout(() => {

//            let ex = href.split('?')[0].split('/')[href.split('/').length - 1].split('.')[1]

//            console.log(ex)

//            downloadImage(href, guid(10) + '.' + ex);

//        }, iii * 200);

//        iii++;
//    }

//    setTimeout(() => {

//        $('.OverHide2').hide()
//        $('.loading-fb').hide()

//    }, iii * 200);
//})

$('body').on('click', '.project-item-box', function (event) {

    if (!$(event.target).closest('.project-item-btn-box').length) {
        location.pathname = '/my/conversation/' + $(this).data('projectid')
    }
})

$('.project-details-header label').click(function () {

    let img = $(this).parent().parent().find('.lightgallery-selector')

    let iii = 0

    //$('.OverHide2').show()
    //$('.loading-fb').show();

    for (var i = 0; i < img.length; i++) {

        let href = img[i].href

        setTimeout(() => {

            let ex = href.split('?')[0].split('/')[href.split('/').length - 1].split('.')[1]

            downloadImage(href, guid(10) + '.' + ex);




            $('.project-details-box').append('<div style="font-size:12px;">' + href + '</div>');
            $('.project-details-box').append('<div style="font-size:12px;">' + new Date() + '</div>');
            $('.project-details-box').append('<hr />');

        }, iii * 1000);

        iii++;
    }

    //downloadImage('/abc.zip');
})

$('.customer-chat-upload > div').click(function () {

    $('.customer-chat-upload > input').click()
})

$('.customer-chat-upload > input').change(function () {

    $('.OverHide2').show()
    $('.loading-fb').show()

    let successCount = 0



    var attchment = document.getElementsByClassName('customer-chat-upload')[0].getElementsByTagName('input')[0].files

    for (var i = 0; i < attchment.length; i++) {

        new Compressor(attchment[i], {
            //quality: 1,
            maxWidth: 1620,
            maxHeight: 1620,
            success(result) {

                quality = 1

                if (result.size > 2 * 1024 * 1024) {
                    quality = 0.6
                } else if (result.size > 1 * 1024 * 1024) {
                    quality = 0.7
                }
                else if (result.size > 512 * 1024 * 1024) {
                    quality = 0.75
                }
                else if (result.size > 384 * 1024) {
                    quality = 0.8
                }
                else if (result.size > 256 * 1024) {
                    quality = 0.85
                }

                new Compressor(result, {
                    quality: quality < 1 ? quality : null,
                    maxWidth: 1620,
                    maxHeight: 1620,
                    success(result) {

                        var formData = new FormData();

                        formData.append('Attachment', result, result.name)

                        let url = '/my/payment'

                        $.post({
                            data: formData,
                            url: url,
                            contentType: false,
                            processData: false,
                            success: function (result, status, xhr) {

                                //rightChat(result)

                                successCount++

                                if (attchment.length == successCount) {

                                    location.reload();
                                }
                            }
                        })
                    },
                    error(err) {
                        AddError(err);
                        alert('خطا در انجام عملیات!');
                    },
                });
            },
            error(err) {
                AddError(err);
                alert('خطا در انجام عملیات!');
            },
        });
    }
})

if (document.getElementById('customer-chat-light-gallery')) {

    $('.customer-chat').scrollTop($('#customer-chat-light-gallery').height());
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

if (location.pathname == '/my/project') {

    $(document).ready(function () {

        $.post({
            data: {},
            url: '/my/payment/badge',
            success: function (result, status, xhr) {

                if (result.badge) {
                    $('.app-nav-payment').addClass('has-message');
                }
            }
        })
    })
}

function clickLightGallery(id) {
    $('#project-details-lightgallery-' + id + ' > div > a > img')[0].click();
}

$('.app-gallery-search-filter > div').click(function () {

    $('.OverHide2').show();
    $('.loading-fb').show();

    setTimeout(() => {

        $('.app-gallery-search-filter > div').removeClass('active');
        $(this).addClass('active');

        let quality = $(this).data('quality');

        if (quality) {
            $('.app-gallery-search-project').hide();

            $('.app-gallery-search-project').each((index, item) => {
                if ($(item).data('quality') == quality) {
                    $(item).show();
                }
            })
        } else {
            $('.app-gallery-search-project').show();
        }

        $('.OverHide2').hide();
        $('.loading-fb').hide()

    }, 100)
})

let db;
const request = window.indexedDB.open('DecopuzzleDatabase', 1);

request.onerror = (event) => {
    console.log('onerror')
    console.log(event)
    // Do something with request.errorCode!
};


request.onupgradeneeded = (event) => {
    // Save the IDBDatabase interface

    console.log('onupgradeneeded')
    const db = event.target.result;

    // Create an objectStore for this database
    const objectStore = db.createObjectStore('ChatFile', { keyPath: 'url' });
};

let db2;
request.onsuccess = (event) => {
    console.log('onsuccess')
    console.log(event)

    db2 = event.target.result;



    $('.chat-message-file').each((index, item) => {
        hasInDb(item);
    });

    // Do something with request.result!

    //stores.forEach((store) => {
    //    const objectStore = event.target.result.createObjectStore(store.name, {
    //        keyPath: store.keyPath,
    //    });
    //    objectStore.createIndex(store.keyPath, store.keyPath, { unique: true });
    //});


    //var store = db.createObjectStore('ChatFile', { keyPath: 'url' });
    //var index = store.createIndex('url', 'url', { unique: true });

    //var tx = db.transaction('ChatFile', 'readwrite');
    //var store = tx.objectStore('ChatFile');
    //var index = store.index("NameIndex");




};


function insertFileToDb(file) {
    // Create a new transaction
    const txn = db2.transaction('ChatFile', 'readwrite');

    // Get the UserDetails object store
    const store = txn.objectStore('ChatFile');
    // Insert a new record
    let query = store.put(file);

    // Handle the success case
    query.onsuccess = function (event) {
        console.log(event);
    };

    // Handle the error case
    query.onerror = function (event) {
        console.log(event.target.errorCode);
    }

    // Close the database once the transaction completes
    //txn.oncomplete = function () {
    //    db2.close();
    //};
}

function downloadFile(e, src) {

    e.stopPropagation();



    const txn_variable = db2.transaction('ChatFile', 'readonly');
    const store_variable = txn_variable.objectStore('ChatFile');
    let request = store_variable.get(src);

    request.onerror = (event) => {
        console.log('onerror');
    };

    request.onsuccess = (event) => {
        if (request.result) {

            saveData([event.target.result.blob], src.split('/')[src.split('/').length - 1])

            //$('.OverHide2').hide();
            //$('.loading-fb').hide();

        } else {

            $($(e.srcElement).parent().find('svg')).hide();
            $($(e.srcElement).parent().find('.file-loader')[0]).css('display', 'block');

            var xhr = new XMLHttpRequest();
            xhr.onreadystatechange = function () {
                if (this.readyState == 4 && this.status == 200) {

                    saveData([this.response], src.split('/')[src.split('/').length - 1])

                    insertFileToDb({ url: src, blob: this.response });

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
    };
}

function hasInDb(item) {

    const txn_variable = db2.transaction('ChatFile', 'readonly');
    const store_variable = txn_variable.objectStore('ChatFile');
    let request = store_variable.get($(item).data('url'));

    request.onerror = (event) => {
        console.log('onerror');
    };

    request.onsuccess = (event) => {
        if (request.result) {
            $($(item).find('svg')[0]).hide();
            $($(item).find('svg')[1]).show();
        }
    };
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

