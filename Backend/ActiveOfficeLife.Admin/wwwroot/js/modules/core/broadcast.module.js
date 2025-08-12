class BroadCastModule {
    constructor() {
        this._listeners = {}; // lưu sự kiện
    }

    // Đăng ký lắng nghe sự kiện
    on(eventName, callback) {
        if (!this._listeners[eventName]) {
            this._listeners[eventName] = [];
        }
        this._listeners[eventName].push(callback);
    }

    off(eventName) {
        if (this._listeners[eventName]) {
            delete this._listeners[eventName];
        }
    }
    clearAllListeners() {
        this._listeners = {};
    }
    // Phát sự kiện
    emit(eventName, data) {
        if (this._listeners[eventName]) {
            this._listeners[eventName].forEach(cb => cb(data));
        }
    }
}

export const broadCastInstance = new BroadCastModule();
