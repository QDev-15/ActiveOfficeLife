import { configInstance } from './config.module.js';
class PingMudule {
    constructor() {
        this.stopped = false;
        this.__hbOpts = null;          // <-- nhớ options để resume
        // --- heartbeat state ---
        this.__hbRunning = false;
        this.__hbTimerId = null;
        this.__hbInFlight = false; // tránh chồng request
        this.counter = 1;
        // optional: tự dừng khi unload
        window.addEventListener('beforeunload', () => this.stopAuthHeartbeat(), { once: true });
    }

    /**
 * Ping 1 lần: kiểm tra kết nối + token còn hợp lệ?
 * Trả về { ok: boolean, status: number }
 *  - ok=true khi server trả 200 (hoặc 2xx)
 *  - ok=false khi 401/403 hoặc lỗi mạng
 */
    async ping(endpoint = 'ping') {
        const t0 = performance.now();
        // chuẩn hóa endpoint
        if (!endpoint.startsWith('/')) endpoint = '/' + endpoint;
        // mặc định coi như unauthorized nếu thiếu token
        if (!configInstance.token) {
            const latencyMs = Math.round(performance.now() - t0);
            const result = { ok: false, status: 401, latencyMs };
            this.writeLog(latencyMs, false);
            return result;
        }
        try {
            const url = new URL(configInstance.urlApi + endpoint);

            const res = await fetch(url.toString(), {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${configInstance.token}`
                },
                cache: 'no-store'
            });
            const latencyMs = Math.round(performance.now() - t0);
            const ok = res.ok && res.status !== 401 && res.status !== 403;
            const result = { ok, status: res.status, latencyMs };

            this.writeLog(latencyMs, true);

            return result;
        } catch {
            const latencyMs = Math.round(performance.now() - t0);
            const result = { ok: false, status: 0, latencyMs };
            this.writeLog(latencyMs, false);

            return result;
        }
    }
    writeLog(time, status) {
        //if (this.counter > 10) {
        //    this.stopAuthHeartbeat();
        //};
        //this.counter += 1;
        //return; // tạm ẩn log
        if (status) {
            console.debug(
                `[PING] ${new Date().toISOString()} | client=${window.location.host} -> ${window.apiUrl} times=${time}ms`
            );
        } else {
            console.debug(
                `[PING] ${new Date().toISOString()} | client=${window.location.host} -> ${window.apiUrl} times=${latencyMs}ms ----- faild`
            );
        }
    }
    /**
     * Bắt đầu heartbeat: mỗi intervalMs gọi ping 1 lần.
     * Nếu nhận 401/403 => dừng và chuyển về /login.
     */
    startAuthHeartbeat(options = {}) {
        const {
            endpoint = '/ping',
            intervalMs = 1000,
            redirectWhenUnauthorized = '/login',
            pauseWhenHidden = false
        } = options;

        if (this.__hbRunning) return; // tránh chạy trùng
        this.__hbRunning = true;
        this.__hbOpts = { endpoint, intervalMs, redirectWhenUnauthorized, pauseWhenHidden }; // <-- lưu options
        const tick = async () => {
            if (this.__hbInFlight) return;
            this.__hbInFlight = true;

            const { ok, status } = await this.ping(endpoint);
            this.__hbInFlight = false;

            if (!ok && (status === 401 || status === 403)) {
                console.log('Unauthorized, please login.')
                this.stopAuthHeartbeat();
                configInstance.logout();
            }
            // Các lỗi khác (0, 5xx) thì tiếp tục lặp — tuỳ bạn nếu muốn xử lý thêm
        };

        // gọi ngay 1 lần
        tick();
        this.__hbTimerId = setInterval(() => {
            if (pauseWhenHidden && document.hidden) return; // tiết kiệm khi tab ẩn
            tick();
        }, intervalMs);


        // 🔁 Auto-resume khi trang hiển thị lại (kể cả bfcache)
        if (!this.__hbAutoResumeBound) {
            this.__hbAutoResumeBound = true;
            window.addEventListener('pageshow', () => {
                // nếu đã stop do beforeunload/bfcache, start lại
                if (!this.__hbRunning) {
                    this.startAuthHeartbeat(this.__hbOpts || {});
                }
            });
        }
    }

    /**
     * Dừng heartbeat
     */
    stopAuthHeartbeat() {
        this.counter = 0;
        if (this.__hbTimerId) clearInterval(this.__hbTimerId);
        this.__hbTimerId = null;
        this.__hbRunning = false;
        this.__hbInFlight = false;
    }
}
export const pingInstance = new PingMudule();