import { ConfigModule } from './config.module.js';
import { SpinnerModule } from './spinner.module.js';
import { MessageModule } from './messages.module.js';
import { CategoryModule } from './category.module.js';
// import log module    
import { LogModule } from './log.module.js'; }

window.configInstance = ConfigModule;
configInstance.initUserCookie();
window.spinnerInstance = SpinnerModule;
window.messageInstance = MessageModule;
window.categoryInstance = new CategoryModule();
window.logInstance = new LogModule();