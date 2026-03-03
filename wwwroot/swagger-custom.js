window.onload = function () {

    document.addEventListener("click", function (e) {

        const pathElement = e.target.closest(".opblock-summary-path");

        if (pathElement) {

            const path = pathElement.innerText.trim();
            const baseUrl = window.location.origin;

            // Open API directly
            window.open(baseUrl + path, "_blank");
        }
    });

};