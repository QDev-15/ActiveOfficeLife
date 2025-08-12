// spinner.module.js

class SpinnerModule {
    static globalSpinnerId = 'volt-body';

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

    static showFor(idElement) {
        if (!idElement) return;
        const targetElement = document.getElementById(idElement);
        targetElement.style.position = 'relative';

        const spinner = document.createElement('div');
        spinner.classList.add('local-spinner-overlay');
        spinner.style.cssText = `
            position: absolute;
            top: 0; left: 0;
            width: 100%;
            height: 100%;
            background: rgba(255, 255, 255, 0.7);
            display: flex;
            align-items: center;
            justify-content: center;
            z-index: 1000;
        `;
        spinner.innerHTML = `
            <div class="spinner-border text-secondary" role="status">
              <span class="visually-hidden">Loading...</span>
            </div>
        `;

        targetElement.appendChild(spinner);
    }

    static hideFor(idElement) {
        if (!idElement) return;
        const targetElement = document.getElementById(idElement);
        const overlay = targetElement.querySelector('.local-spinner-overlay');
        if (overlay) {
            overlay.remove();
        }
    }
}

export const spinnerInstance = SpinnerModule;


