window.onload = function () {

    function attachEvents() {

        const blocks = document.querySelectorAll(".opblock");

        blocks.forEach(block => {

            block.addEventListener("click", function () {

                const path = block.querySelector(".opblock-summary-path");

                if (path) {

                    let api = path.innerText.trim();

                    let base = window.location.origin;

                    let newUrl = base + api;

                    history.pushState({}, "", newUrl);

                }

            });

        });

    }

    setTimeout(attachEvents, 2000);

};