window.onload = function () {

    document.addEventListener("click", function (e) {

        // Only GET button click
        const methodButton = e.target.closest(".opblock-summary-method");

        if (methodButton && methodButton.innerText.trim() === "GET") {

            const parent = methodButton.closest(".opblock");
            const pathElement = parent.querySelector(".opblock-summary-path");

            if (pathElement) {

                const path = pathElement.innerText.trim();
                const baseUrl = window.location.origin;

                // Redirect current page
                window.location.href = baseUrl + path;
            }
        }
    });

};