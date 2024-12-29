function submitSurveyFunc() {

    // ارسال فرم به صورت AJAX

    var submitBtn = document.getElementById('submitSurvey');

    if (submitBtn != null) {

        submitBtn.addEventListener('click', function () {

            // Gather form data
            const formData = {
                DesignerId: document.getElementById("designerId").value,
                CustomerId: document.getElementById("customerId").value,
                Description: document.getElementById("description").value,
                Vote: document.querySelector("input[name='Vote']:checked")?.value,
                DesignQualityVote: document.querySelector("input[name='DesignQualityVote']:checked")?.value,
                ComplianceOfTheOrderWithTheDesignVote: document.querySelector("input[name='ComplianceOfTheOrderWithTheDesignVote']:checked")?.value,
                DeliveryTimeVote: document.querySelector("input[name='DeliveryTimeVote']:checked")?.value,
                AppropriateApproachOfTheDesignerVote: document.querySelector("input[name='AppropriateApproachOfTheDesignerVote']:checked")?.value,
                PriceVote: document.querySelector("input[name='PriceVote']:checked")?.value,
                AppropriateTreatmentOfManagementAndOfficeWorkersVote: document.querySelector("input[name='AppropriateTreatmentOfManagementAndOfficeWorkersVote']:checked")?.value
            };
            debugger;
            fetch('/FeedbackForms/Create', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(formData)
            }).then(response => {
                if (response.ok) {
                    alert('نظرسنجی با موفقیت ثبت شد!');
                    document.getElementById('customModal').style.display = 'none';
                    document.getElementById('modalOverlay').style.display = 'none';
                } else {
                    return response.text().then(text => {
                        alert('خطایی رخ داده است: ' + text);
                    });
                }
            }).catch(error => {
                console.error('Error submitting feedback:', error);
                alert('خطایی رخ داده است. لطفاً دوباره تلاش کنید.');
            });

        });

    }
}

