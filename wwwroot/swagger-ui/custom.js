window.onload = function () {

    const interval = setInterval(() => {

        const container = document.querySelector(".topbar");

        if (container && !document.getElementById("api-dashboard")) {

            const panel = document.createElement("div");
            panel.id = "api-dashboard";

            panel.style.padding = "10px";
            panel.style.background = "#111";
            panel.style.display = "flex";
            panel.style.gap = "10px";

            panel.innerHTML = `
                <button onclick="openApi('/api/Calculator/addoperation')" style="padding:6px 12px;">Calculator</button>
                <button onclick="openApi('/api/plugin/dll1/test')" style="padding:6px 12px;">DLL1</button>
                <button onclick="openApi('/api/Image/ConvertPNG')" style="padding:6px 12px;">Image</button>
                <button onclick="openApi('/api/User/getusers')" style="padding:6px 12px;">User</button>
            `;

            container.appendChild(panel);

            clearInterval(interval);
        }

    }, 1000);

};

function openApi(path) {

    const base = window.location.origin;

    const url = base + path;

    history.pushState({}, "", url);

    window.open(url, "_blank");

}