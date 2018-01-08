window.addEventListener("load", async function () {
    var a = document.querySelector("a.PostLogoutRedirectUri");
    if (a) {
        await new Promise(resolve => setTimeout(resolve, 2000));
        window.location = a.href;
    }
});