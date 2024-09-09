// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// sweetalerts
// credit: https://khalidabuhakmeh.com/confirmation-dialogs-with-htmx-and-sweetalert

document.body.addEventListener('htmx:confirm', function (evt) {
    if (evt.target.matches("[confirm-with-sweet-alert='true']")) {
        evt.preventDefault();
        swal({
            title: "Are you sure?",
            text: "Are you sure you are sure?",
            icon: "warning",
            buttons: true,
            dangerMode: true,
        }).then((confirmed) => {
            if (confirmed) {
                evt.detail.issueRequest();
            }
        });
    }
});
