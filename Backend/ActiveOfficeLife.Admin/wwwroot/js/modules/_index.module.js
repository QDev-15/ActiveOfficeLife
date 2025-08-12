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