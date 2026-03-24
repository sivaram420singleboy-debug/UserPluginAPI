window.addEventListener("load", function () {

    setTimeout(function () {

        document.querySelectorAll('.opblock-summary').forEach(function (block) {

            block.addEventListener('click', function () {

                const methodElement = block.querySelector('.opblock-summary-method');
                const pathElement = block.querySelector('.opblock-summary-path');

                if (!methodElement || !pathElement) return;

                const method = methodElement.textContent.trim();
                const path = pathElement.textContent.trim();

                if (method === "GET") {
                    const baseUrl = window.location.origin;
                    const apiUrl = baseUrl + path;
                    window.location.href = apiUrl;
                }

            });

        });

    }, 1500);

});