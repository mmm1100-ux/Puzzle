
var isCtrl = false;

document.onkeyup = function (e) {
    if (e.keyCode == 17) isCtrl = false;
}

document.onkeydown = function (e) {

    if (e.keyCode == 17) isCtrl = true;
    if (e.keyCode == 83 && isCtrl == true) {

        let page = document.getElementsByClassName('print-background')

        for (var i = 0; i < page.length; i++) {

            domtoimage.toBlob(document.getElementById(page[i].id))
                .then(function (blob) {

                    var name = document.getElementById('print-name-for-jpg').value
                    window.saveAs(blob, name + ' - ' + (i + 1) + '.png');
                });
        }

        return false;
    }
}