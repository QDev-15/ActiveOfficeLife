// spinner.module.js

class SpinnerModule {
    static globalSpinnerId = 'volt-body';
    static mainContainer = 'main-container';
    static _resolveElement(target) {
        if (!target) return null;
        if (target instanceof HTMLElement) return target;
        if (typeof target === 'string') {
            const selector = target.startsWith('#') ? target : `#${target}`;
            return document.querySelector(selector);
        }
        return null;
    }
    // Đợi render xong (2 rAF để chắc chắn đã paint)
    static _waitForPaint() {
        return new Promise(res => requestAnimationFrame(() =>
            requestAnimationFrame(res)
        ));
    }

    static showGlobal() {
        // Nếu đã tồn tại thì không thêm nữa
        if (document.getElementById(this.globalSpinnerId)) return;

        const overlay = document.createElement('div');
        overlay.id = this.globalSpinnerId;
        overlay.style.cssText = `
            position: fixed;
            top: 0; left: 0;
            width: 100vw;
            height: 100vh;
            background: rgba(0,0,0,0.3);
            display: flex;
            align-items: center;
            justify-content: center;
            z-index: 9999;
        `;
        overlay.innerHTML = `
            <div class="spinner-border text-primary" role="status" style="width: 3rem; height: 3rem;">
              <span class="visually-hidden">Loading...</span>
            </div>
        `;
        document.body.appendChild(overlay);
    }

    static hideGlobal() {
        const spinner = document.getElementById(this.globalSpinnerId);
        if (spinner) {
            spinner.remove();
        }
    }

    // Hiển thị spinner trên 1 phần tử (hoặc selector)
    static showFor(target, opts = {}) {
        const el = this._resolveElement(target);
        if (!el) return null;

        // Tránh tạo trùng
        if (el.querySelector(':scope > .local-spinner-overlay')) return el.querySelector(':scope > .local-spinner-overlay');

        const {
            bg = 'rgba(255,255,255,0.7)',
            zIndex = 1000,
            message = '',               // ví dụ: 'Đang tải...'
            spinnerClass = 'text-secondary', // bootstrap color: text-primary|secondary|light|...
            rounded = true              // bo góc theo container
        } = opts;

        // Nhớ lại position gốc để khôi phục khi hide
        const cs = getComputedStyle(el);
        const shouldSetRelative = cs.position === 'static' || !cs.position;
        if (this.shouldSetSetRelative(el)) el.dataset._spinnerRestorePosition = '1';
        if (shouldSetRelative) el.style.position = 'relative';

        const overlay = document.createElement('div');
        overlay.className = 'local-spinner-overlay';
        overlay.style.cssText = `
          position: absolute;
          inset: 0;
          background: ${bg};
          display: flex;
          flex-direction: column;
          gap: 8px;
          align-items: center;
          justify-content: center;
          z-index: ${zIndex};
          ${rounded ? `border-radius: ${cs.borderRadius};` : ''}
        `;

            overlay.innerHTML = `
          <div class="spinner-border ${spinnerClass}" role="status" aria-live="polite" aria-busy="true">
            <span class="visually-hidden">Loading...</span>
          </div>
          ${message ? `<div class="small text-muted">${this.escapeHtml(message)}</div>` : ''}
        `;
        // Khóa cuộn container trong lúc hiện spinner
        if (!el.dataset._spinnerRestoreOverflow) {
          el.dataset._spinnerRestoreOverflow = cs.overflow || '';
          el.style.overflow = 'hidden';
        }
        el.appendChild(overlay);
        return overlay;
    }

    // Ẩn spinner của phần tử
    static hideFor(target) {
        const el = this._resolveElement(target);
        if (!el) return false;
        const overlay = el.querySelector(':scope > .local-spinner-overlay');
        if (overlay) overlay.remove();

        // Khôi phục position nếu là mình set
        if (el.dataset._spinnerRestorePosition === '1') {
            el.style.position = '';
            delete el.dataset._spinnerRestorePosition;
        }
        // Mở khóa cuộn
        if ('_spinnerRestoreOverflow' in el.dataset) {
          el.style.overflow = el.dataset._spinnerRestoreOverflow;
          delete el.dataset._spinnerRestoreOverflow;
        }
        return true;
    }

    static showForMainContainer(opts = {}) {
        return this.showFor(this.mainContainer, opts);
    }
    static hideForMainContainer() {
        return this.hideFor(this.mainContainer);
    }

    // ❗️Hàm async: chỉ resolve sau khi spinner đã PAINT xong
    static async showForAsync(target, opts = {}) {
        const overlay = this.showFor(target, opts);
        await this._waitForPaint();
        return overlay;
    }
    static async hideForAsync(target, opts = {}) {
        const overlay = this.hideFor(target);
        await this._waitForPaint();
        return overlay;
    }
    static async showForMainContainerAsync(opts = {}) {
        const overlay = this.showForMainContainer(opts);
        await this._waitForPaint();
        return overlay;
    }
    static async hideForMainContainerAsync() {
        const overlay = this.hideForMainContainer();
        await this._waitForPaint();
        return overlay;
    }

    // Tiện ích bao trọn quy trình: hiển thị → đợi paint → chạy action → ẩn
    static async withSpinner(target, action, opts = {}) {
        await this.showForAsync(target, opts);
        try {
            return await action();
        } finally {
            this.hideFor(target);
        }
    }

    // Helpers
    static shouldSetSetRelative(el) {
        const cs = getComputedStyle(el);
        return cs.position === 'static' || !cs.position;
    }
    static escapeHtml(s = '') {
        return s.replace(/[&<>"']/g, m => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[m]));
    }
}

export const spinnerInstance = SpinnerModule;


