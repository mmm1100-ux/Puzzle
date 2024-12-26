console.log("helllllllllo");
function submitSurveyFunc() {

    // ارسال فرم به صورت AJAX

    var submitBtn = document.getElementById('submitSurvey');

    console.log(submitBtn);

    if (submitBtn != null) {

        submitBtn.addEventListener('click', function () {
            const form = document.getElementById('feedbackForm');
            const formData = new FormData(form);
            console.log(form);
            console.log(formData);

            fetch('/FeedbackForms/Create', {
                method: 'POST',
                body: formData
            }).then(response => {
                if (response.ok) {
                    alert('نظرسنجی با موفقیت ثبت شد!');
                    modal.style.display = 'none';
                    overlay.style.display = 'none';
                } else {
                    return response.text().then(text => {
                        alert('خطایی رخ داده است: ' + text);
                    });
                }
            })
                .catch(error => {
                    console.error('Error submitting feedback:', error);
                    alert('خطایی رخ داده است. لطفاً دوباره تلاش کنید.');
                });

        });

    }
}

