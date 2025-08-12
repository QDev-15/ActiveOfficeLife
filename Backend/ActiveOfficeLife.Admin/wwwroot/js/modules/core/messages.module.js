// messages.module.js
export const MessageType = {
    Info: 'info',
    Warning: 'warning',
    Error: 'error',
    Confirm: 'confirm',
};

class MessageModule {
    static destroy() {
        const modalEl = document.getElementById("globalMessageModal");
        const modalTitle = document.getElementById("globalMessageModalTitle");
        const modalBody = document.getElementById("globalMessageModalBody");
        const modalFooter = document.getElementById("globalMessageModalFooter");
        const modalHeader = document.getElementById("globalMessageModalHeader");

        if (!modalEl) return;

        // Xóa tất cả nội dung và class
        modalTitle.innerHTML = '';
        modalBody.innerHTML = '';
        modalFooter.innerHTML = '';
        modalHeader.className = 'modal-header';

        // Remove all buttons' onclick để tránh memory leak
        Array.from(modalFooter.querySelectorAll('button')).forEach(btn => {
            btn.onclick = null;
        });

        // Reset backdrop config nếu bạn gán lại sau này
        const modal = bootstrap.Modal.getInstance(modalEl);
        if (modal) {
            modal._config = {};
        }
    }

    static show(options) {
        this.destroy(); // 👈 Reset trước khi show mới

        let {
            type,
            title,
            message,
            fullscreen = false,
            clickOutsideToClose = true
        } = options;

        const modalEl = document.getElementById("globalMessageModal");
        const modalTitle = document.getElementById("globalMessageModalTitle");
        const modalBody = document.getElementById("globalMessageModalBody");
        const modalFooter = document.getElementById("globalMessageModalFooter");
        const modalHeader = document.getElementById("globalMessageModalHeader");

        const modal = bootstrap.Modal.getOrCreateInstance(modalEl);

        const config = {
            [MessageType.Info]: {
                title: 'Thông báo',
                color: 'bg-info',
                icon: 'bi-info-circle-fill',
                btn: 'btn-info'
            },
            [MessageType.Warning]: {
                title: 'Cảnh báo',
                color: 'bg-warning',
                icon: 'bi-exclamation-triangle-fill',
                btn: 'btn-warning'
            },
            [MessageType.Error]: {
                title: 'Lỗi',
                color: 'bg-danger',
                icon: 'bi-x-circle-fill',
                btn: 'btn-danger'
            },
            [MessageType.Confirm]: {
                title: 'Xác nhận',
                color: 'bg-primary',
                icon: 'bi-question-circle-fill',
                btn: 'btn-primary'
            }
        };

        const conf = config[type] || config[MessageType.Info];

        if (!fullscreen && clickOutsideToClose && type === MessageType.Confirm) {
            clickOutsideToClose = false; // Mặc định Confirm sẽ có clickOutsideToClose
        }
        modalHeader.classList.add(conf.color);
        modalTitle.innerHTML = `<i class="bi ${conf.icon} me-2"></i>${title || conf.title}`;

        if (typeof message === 'string') {
            modalBody.innerHTML = message;
        } else {
            modalBody.innerHTML = '';
            modalBody.appendChild(message);
        }

        if (fullscreen) {
            modalEl.classList.add('modal-fullscreen');
        } else {
            modalEl.classList.remove('modal-fullscreen');
        }

        modal._config = modal._config || {};
        modal._config.backdrop = clickOutsideToClose ? true : 'static';

        return new Promise(resolve => {
            if (type === MessageType.Confirm) {
                const yesBtn = document.createElement('button');
                yesBtn.className = `btn ${conf.btn}`;
                yesBtn.textContent = 'Có';
                yesBtn.onclick = () => {
                    modal.hide();
                    resolve(true);
                };

                const noBtn = document.createElement('button');
                noBtn.className = 'btn btn-secondary';
                noBtn.textContent = 'Không';
                noBtn.onclick = () => {
                    modal.hide();
                    resolve(false);
                };

                modalFooter.appendChild(noBtn);
                modalFooter.appendChild(yesBtn);
            } else {
                const okBtn = document.createElement('button');
                okBtn.className = `btn ${conf.btn}`;
                okBtn.textContent = 'Đóng';
                okBtn.onclick = () => modal.hide();
                modalFooter.appendChild(okBtn);

                resolve();
            }

            modal.show();
        });
    }

    static info(message, opts = {}) {
        return this.show({
            type: MessageType.Info, message, ...opts
        });
    }

    static warning(message, opts = {}) {
        return this.show({ type: MessageType.Warning, message, ...opts });
    }

    static error(message, opts = {}) {
        return this.show({ type: MessageType.Error, message, ...opts });
    }

    static confirm(message, opts = {}) {
        return this.show({ type: MessageType.Confirm, message, ...opts });
    }
}


export const messageInstance = MessageModule;
