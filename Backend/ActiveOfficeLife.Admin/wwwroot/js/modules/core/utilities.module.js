

class Utilities {
    constructor() {
        this.minReloadMs = 15 * 1000;
        this.lastLoadedAt = Date.now();

        this._tasks = [];
        this._attached = false;
        // --- Select2 auto wiring ---
        this._select2Wired = false;
        this._select2Observer = null;
        // init tags
        this.sep = /[,，；;]+/;
        this.tags = [];
        this.box = null;
        this.editor = null;
        this.hiddenOriginal = null;
        // end init tags
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
    // format number ex: input 2 => 02, input 10 => 10
    _pad(n) { return n < 10 ? '0' + n : '' + n; };
    _toInputDatetimeLocal(value) {
        if (!value) return '';
        const d = (value instanceof Date) ? value : new Date(value);
        const yyyy = d.getFullYear();
        const MM = this._pad(d.getMonth() + 1);
        const dd = this._pad(d.getDate());
        const hh = this._pad(d.getHours());
        const mm = this._pad(d.getMinutes());
        // datetime-local cần 'YYYY-MM-DDTHH:mm'
        return `${yyyy}-${MM}-${dd}T${hh}:${mm}`;
    };
    // clear invalid
    _clearInvalid(el) {
        if (!el) return;
        el.classList.remove('is-invalid');
    }
    // đặt trạng thái invalid + thông báo
    _markInvalid(el, msg) {
        if (!el) return;
        el.classList.add('is-invalid');
        // nếu ngay sau input có .invalid-feedback thì set text
        const fb = el.nextElementSibling;
        if (fb && fb.classList && fb.classList.contains('invalid-feedback')) {
            fb.textContent = msg || fb.textContent;
        }
    }
    // validate url
    _isValidUrl(u) {
        if (!u) return true; // cho phép rỗng, nếu muốn bắt buộc thì kiểm tra riêng
        try { new URL(u); return true; } catch { return false; }
    }
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
    generateSlug(s) { return this.slugify(s); };
    generateTag(s) { return this.slugify(s).replace(/-/, '_'); };
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


    // INIT TAGS by region
    #region
    render() {
        // xóa tất cả tag pill (trừ editor)
        [...this.box.querySelectorAll('.tag')].forEach(n => n.remove());
        this.tags.forEach((t, i) => {
            const pill = document.createElement('span');
            pill.className = 'tag';
            pill.innerHTML = `<span>${this.escapeHtml(t)}</span><button type="button" class="remove" aria-label="Remove">&times;</button>`;
            pill.querySelector('.remove').addEventListener('click', () => {
                this.tags.splice(i, 1);
                this.render();
            });
            this.box.insertBefore(pill, this.editor);
        });
        this.hiddenOriginal.value = this.tags.join(', ');
    }
    commitChunk(chunk) {
        const parts = chunk.split(this.sep).map(s => s.trim()).filter(Boolean);
        for (const p of parts) if (!this.tags.includes(p)) this.tags.push(p);
        this.render();
    }
    backspaceMaybeRemove(e) {
        if (this.editor.value === '' && tags.length && e.key === 'Backspace') {
            tags.pop(); this.render();
            e.preventDefault();
        }
    }
    escapeHtml(s) { return s.replace(/[&<>"']/g, c => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c])); }
    initTags(input) {
        const original = input;                     // input gốc (có asp-for)
        const name = original.name;
        const initial = (original.value || '').split(this.sep).map(s => s.trim()).filter(Boolean);

        // ẩn input gốc & tạo hidden để post
        original.type = 'hidden';
        this.hiddenOriginal = original; // dùng luôn input gốc làm hidden (giữ name/binding)

        // container hiển thị pill + ô nhập
        this.box = document.createElement('div');
        this.box.className = 'tags-input';
        this.editor = document.createElement('input');
        this.editor.type = 'text'; this.editor.placeholder = 'Nhập từ khóa, nhấn phẩy hoặc Enter';
        this.box.appendChild(this.editor);
        this.hiddenOriginal.insertAdjacentElement('afterend', this.box);

        this.tags = [];

        // sự kiện
        this.editor.addEventListener('keydown', e => {
            if (e.key === 'Enter' || e.key === ',') {
                e.preventDefault();
                if (this.editor.value.trim()) { this.commitChunk(this.editor.value); this.editor.value = ''; }
            } else if (e.key === 'Backspace') {
                this.backspaceMaybeRemove(e);
            }
        });
        this.editor.addEventListener('input', () => {
            if (this.sep.test(this.editor.value)) {
                this.commitChunk(this.editor.value);
                this.editor.value = '';
            }
        });
        this.editor.addEventListener('blur', () => {
            if (this.editor.value.trim()) { this.commitChunk(this.editor.value); this.editor.value = ''; }
        });
        this.box.addEventListener('click', () => this.editor.focus());

        // paste nhiều từ khóa
        this.editor.addEventListener('paste', e => {
            const text = (e.clipboardData || window.clipboardData).getData('text');
            if (this.sep.test(text)) {
                e.preventDefault();
                this.commitChunk(text);
            }
        });

        // khởi tạo từ giá trị ban đầu
        if (initial.length) { this.tags = Array.from(new Set(initial)); this.render(); }
    }
    #endregion


}
export const utilities = new Utilities();