function showMessageModal(type, message, title = null) {
    const modalEl = document.getElementById("globalMessageModal");
    const modalTitle = document.getElementById("globalMessageModalTitle");
    const modalBody = document.getElementById("globalMessageModalBody");
    const modalFooter = document.getElementById("globalMessageModalFooter");
    const modalHeader = document.getElementById("globalMessageModalHeader");

    const modal = bootstrap.Modal.getOrCreateInstance(modalEl);

    // Reset
    modalFooter.innerHTML = '';
    modalTitle.innerHTML = '';
    modalBody.innerHTML = '';
    modalHeader.className = 'modal-header text-white';

    const config = {
        info: {
            title: 'Thông báo',
            color: 'bg-info',
            icon: 'bi-info-circle-fill',
            btn: 'btn-info'
        },
        warning: {
            title: 'Cảnh báo',
            color: 'bg-warning',
            icon: 'bi-exclamation-triangle-fill',
            btn: 'btn-warning'
        },
        error: {
            title: 'Lỗi',
            color: 'bg-danger',
            icon: 'bi-x-circle-fill',
            btn: 'btn-danger'
        },
        confirm: {
            title: 'Xác nhận',
            color: 'bg-primary',
            icon: 'bi-question-circle-fill',
            btn: 'btn-primary'
        }
    };

    const conf = config[type] || config.info;

    // Set header
    modalHeader.classList.add(conf.color);
    modalTitle.innerHTML = `<i class="bi ${conf.icon} me-2"></i>${title || conf.title}`;

    // Set message
    if (typeof message === 'string') {
        modalBody.innerHTML = message;
    } else if (message instanceof HTMLElement) {
        modalBody.innerHTML = '';
        modalBody.appendChild(message);
    }

    return new Promise(resolve => {
        if (type === 'confirm') {
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

            resolve(); // Trả ra undefined cho các loại khác confirm
        }

        modal.show();
    });
}

// Shortcut functions
function showInfo(message, title = null) {
    return showMessageModal('info', message, title);
}

function showError(message, title = null) {
    return showMessageModal('error', message, title);
}

function showWarning(message, title = null) {
    return showMessageModal('warning', message, title);
}

function showConfirm(message, title = null) {
    return showMessageModal('confirm', message, title);
}
