

class Utilities {
    constructor() {
        this.minReloadMs = 15 * 1000;
        this.lastLoadedAt = Date.now();

        this._tasks = [];
        this._attached = false;
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
    slugify(text) {
        if (!text) return '';
        return text
            .toString()
            .replace(/Đ/g, 'D')
            .replace(/đ/g, 'd')
            .normalize('NFD')
            .replace(/[\u0300-\u036f]/g, '')
            .toLowerCase()
            .trim()
            .replace(/[^a-z0-9\s-]/g, '')
            .replace(/\s+/g, '-')
            .replace(/-+/g, '-');
    }
    _isTyping() {
        const ae = document.activeElement;
        if (!ae) return false;
        const tag = ae.tagName;
        if (tag === 'INPUT' || tag === 'TEXTAREA') return true;
        // contenteditable?
        if (ae.isContentEditable) return true;
        return false;
    }
}
export const utilities = new Utilities();