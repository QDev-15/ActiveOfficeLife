

class Utilities {
    constructor() {
        this.minReloadMs = 15 * 1000;
        this.lastLoadedAt = Date.now();

        this._tasks = [];
        this._attached = false;
        // --- Select2 auto wiring ---
        this._select2Wired = false;
        this._select2Observer = null;
    }
    _trim(p) { return p.replace(/\/+$/, ""); }
    _match(path) { return this._trim(new URL(location.href).pathname).indexOf(this._trim(path)) >= 0 }
    _run = () => {
        if (document.visibilityState !== "visible") return;
        const now = Date.now();

        for (const t of this._tasks) {
            if (!this._match(t.path)) continue;
            if (now - t.getLastLoadedAt() <= t.getMinReloadMs()) continue;

            if (t.isTyping() === true) {
                clearTimeout(t._timer);
                t._timer = setTimeout(t.reload, 1000);
            } else {
                t.reload();
            }
        }
    };
    _attachOnce() {
        if (this._attached) return;
        window.addEventListener("focus", this._run);
        document.addEventListener("visibilitychange", this._run);
        window.addEventListener("pageshow", (e) => { if (e.persisted) this._run(); });
        this._attached = true;
    }
    /**
      * Đăng ký 1 task reload theo URL. Trả về hàm unregister.
      * Thiếu getter sẽ dùng mặc định từ Utilities (lastLoadedAt/minReloadMs/isTyping).
      */
    registerReloadTask({ path, reload, isTyping, getLastLoadedAt, getMinReloadMs }) {
        if (!path || !reload) throw new Error("path và reload là bắt buộc");
        this._attachOnce();

        const task = {
            path,
            reload,
            isTyping: isTyping || (() => this._isTyping()),
            getLastLoadedAt: getLastLoadedAt || (() => this.lastLoadedAt),
            getMinReloadMs: getMinReloadMs || (() => this.minReloadMs),
            _timer: null,
        };
        this._tasks.push(task);

        return () => {
            const i = this._tasks.indexOf(task);
            if (i > -1) this._tasks.splice(i, 1);
            clearTimeout(task._timer);
        };
    }

    triggerReloadCheck() { this._run(); }
    clearAllReloadTasks() {
        for (const t of this._tasks) clearTimeout(t._timer);
        this._tasks.length = 0;
    }
    slugify = (s) => (s || '')
        .normalize('NFD').replace(/[\u0300-\u036f]/g, '')  // bỏ dấu
        .replace(/đ/g, 'd').replace(/Đ/g, 'D')
        .toLowerCase()
        .replace(/[^a-z0-9]+/g, '-')     // ký tự lạ -> -
        .replace(/-{2,}/g, '-')           // gộp --
        .replace(/^-+|-+$/g, '')          // trim -
        .slice(0, 120);                   // giới hạn độ dài
    _isTyping() {
        const ae = document.activeElement;
        if (!ae) return false;
        const tag = ae.tagName;
        if (tag === 'INPUT' || tag === 'TEXTAREA') return true;
        // contenteditable?
        if (ae.isContentEditable) return true;
        return false;
    }

    /// INIT Select2
    // Khởi tạo Select2 cho tất cả <select class="select2"> trong root
    initSelect2(root = document) {
        const $root = $(root);
        $root.find('select.select2')
            .not('.select2-hidden-accessible') // tránh init trùng
            .each((_, el) => {
                const $sel = $(el);

                // Tìm modal bao (nếu có) để dropdown hiển thị đúng z-index
                const $modal = $sel.closest('.modal');

                // Placeholder: ưu tiên data-placeholder; nếu không có thì lấy từ option rỗng đầu tiên
                const $emptyOpt = $sel.find('option[value=""], option:not([value])').first();
                const placeholder = $sel.data('placeholder') ?? ($emptyOpt.length ? $emptyOpt.text().trim() : undefined);

                // allowClear nếu có option rỗng hoặc đặt data-allow-clear
                const allowClear = $sel.data('allow-clear') ?? !!$emptyOpt.length;

                // Luôn có ô search; có thể tuỳ chỉnh qua data-minimum-results-for-search
                const minSearch = $sel.data('minimum-results-for-search') ?? 0;

                $sel.select2({
                    placeholder,
                    allowClear,
                    width: '100%',
                    minimumResultsForSearch: minSearch,
                    dropdownParent: $modal.length ? $modal : $(document.body)
                });

                // Auto select nếu chỉ có đúng 1 option khác rỗng
                const nonEmpty = $sel.find('option').filter((i, o) => (o.value ?? '') !== '');
                if (nonEmpty.length === 1 && $emptyOpt.length && !$sel.val()) {
                    $sel.val(nonEmpty[0].value).trigger('change');
                }
            });
    }

    // Gắn auto apply: DOM ready, mở modal, và khi node mới được thêm vào DOM
    setupSelect2Auto() {
        if (this._select2Wired) return;
        this._select2Wired = true;

        // Lần đầu
        this.initSelect2(document);

        // Khi mở Bootstrap modal
        $(document).on('shown.bs.modal', (e) => this.initSelect2(e.target));

        // Khi DOM có phần tử mới (SPA, partial load, ajax, v.v.)
        this._select2Observer = new MutationObserver((muts) => {
            for (const m of muts) {
                for (const node of m.addedNodes ?? []) {
                    if (!(node instanceof Element)) continue;
                    if (node.matches?.('select.select2')) this.initSelect2(node);
                    $(node).find?.('select.select2').length && this.initSelect2(node);
                }
            }
        });
        this._select2Observer.observe(document.body, { childList: true, subtree: true });
    }

    // (tuỳ chọn) Dừng auto apply nếu cần
    teardownSelect2Auto() {
        this._select2Observer?.disconnect();
        this._select2Observer = null;
        this._select2Wired = false;
    }

}
export const utilities = new Utilities();