import { configInstance } from './core/config.module.js';
import { broadCastInstance } from './core/broadcast.module.js';
import { spinnerInstance } from './core/spinner.module.js';
import { messageInstance } from './core/messages.module.js';
import { pingInstance } from './core/ping.module.js';
import { utilities } from './core/utilities.module.js';
import { categoryInstance } from './category.module.js';
import { settingInstance } from './setting.module.js';   
import { logInstance } from './log.module.js';
import { postInstance } from './post.module.js';
import { tagInstance } from './tag.module.js';
import { adInstance } from './ad.module.js';


window.configInstance = configInstance;
window.utilities = utilities;
window.pingInstance = pingInstance;
window.spinnerInstance = spinnerInstance;
window.messageInstance = messageInstance;
window.categoryInstance = categoryInstance;
window.settingInstance = settingInstance;
window.postInstance = postInstance;
window.logInstance = logInstance
window.broadCastInstance = broadCastInstance;
window.tagInstance = tagInstance;
window.adInstance = adInstance;
pingInstance.startAuthHeartbeat();



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




