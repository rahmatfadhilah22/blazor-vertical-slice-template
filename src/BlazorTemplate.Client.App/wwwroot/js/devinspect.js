// Inspect mode untuk DevInspector (hanya dipanggil di build DEBUG).
//
// Blazor tidak menandai elemen DOM dengan komponen asalnya, jadi satu-satunya sumber
// kebenaran adalah atribut data-component yang dipasang manual di root komponen.
// Elemen tanpa penanda sendiri dicari ke atas sampai penanda terdekat — itulah komponen
// yang dirender di titik itu.
window.devInspect = (function () {
    var dotnet = null;
    var hovered = null;

    function describe(el) {
        var chain = [];
        for (var node = el; node && node !== document.documentElement; node = node.parentElement) {
            var name = node.dataset && node.dataset.component;
            if (name) chain.push(name);
        }
        return {
            component: chain.length ? chain[0] : null,
            chain: chain,
            tag: el.tagName.toLowerCase()
        };
    }

    function clearHover() {
        if (hovered) hovered.classList.remove('dev-inspect-hover');
        hovered = null;
    }

    function onMove(e) {
        if (e.target === hovered) return;
        clearHover();
        hovered = e.target;
        if (hovered && hovered.classList) hovered.classList.add('dev-inspect-hover');
        if (dotnet) dotnet.invokeMethodAsync('OnInspectHover', dotnet === null ? null : null);
    }

    // Dipisah supaya payload hover dikirim sekali per perubahan elemen, bukan per piksel.
    function onMoveInfo(e) {
        var info = describe(e.target);
        if (dotnet) dotnet.invokeMethodAsync('OnInspectHover', info.component, info.tag, info.chain);
    }

    function move(e) {
        if (e.target !== hovered) {
            clearHover();
            hovered = e.target;
            if (hovered && hovered.classList) hovered.classList.add('dev-inspect-hover');
        }
        var info = describe(e.target);
        if (dotnet) dotnet.invokeMethodAsync('OnInspectHover', info.component, info.tag, info.chain);
    }

    function click(e) {
        // Inspect mode menelan klik supaya tombol di halaman tidak ikut beraksi.
        e.preventDefault();
        e.stopPropagation();
        var info = describe(e.target);
        if (dotnet) dotnet.invokeMethodAsync('OnInspectPick', info.component, info.tag, info.chain);
    }

    function key(e) {
        if (e.key === 'Escape') stop();
    }

    function start(ref) {
        if (dotnet) return;
        dotnet = ref;
        document.body.classList.add('dev-inspect-on');
        document.addEventListener('mousemove', move, true);
        document.addEventListener('click', click, true);
        document.addEventListener('keydown', key, true);
    }

    function stop() {
        if (!dotnet) return;
        document.body.classList.remove('dev-inspect-on');
        document.removeEventListener('mousemove', move, true);
        document.removeEventListener('click', click, true);
        document.removeEventListener('keydown', key, true);
        clearHover();
        dotnet = null;
    }

    return { start: start, stop: stop };
})();
