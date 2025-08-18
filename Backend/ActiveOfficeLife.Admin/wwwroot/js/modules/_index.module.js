import { configInstance } from './core/config.module.js';
import { broadCastInstance } from './core/broadcast.module.js';
import { spinnerInstance } from './core/spinner.module.js';
import { messageInstance } from './core/messages.module.js';
import { categoryInstance } from './category.module.js';   
import { logInstance } from './log.module.js';

window.configInstance = configInstance;
window.spinnerInstance = spinnerInstance;
window.messageInstance = messageInstance;
window.categoryInstance = categoryInstance;
window.logInstance = logInstance
window.broadCastInstance = broadCastInstance;


document.addEventListener('DOMContentLoaded', () => {
    // Lấy fullName từ ConfigModule
    const fullName = configInstance.user.fullName || ConfigModule.user.username || 'Người dùng';

    // Tìm tất cả các phần tử có class .aol_fullname và thay nội dung
    document.querySelectorAll('.aol_fullname').forEach(el => {
        el.textContent = fullName;
    });
    document.addEventListener('shown.bs.modal', function (e) {
        if ($(e.target).find('.select2').length) {
            document.removeEventListener('focusin', bootstrap.Modal.prototype._handleFocusin);
        }
    });
});




