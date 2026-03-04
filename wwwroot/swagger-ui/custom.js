window.onload = function () {

    document.addEventListener("click", function (e) {

        let el = e.target.closest(".opblock-summary-path");

        if (el) {

            let path = el.innerText.trim();

            let base = window.location.origin;

            let fullUrl = base + path;

            window.location.href = fullUrl;

        }

    });

};