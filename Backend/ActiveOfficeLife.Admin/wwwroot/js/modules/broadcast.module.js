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

    // Gỡ bỏ lắng nghe
    off(eventName, callback) {
        if (!this._listeners[eventName]) return;
        this._listeners[eventName] = this._listeners[eventName].filter(cb => cb !== callback);
    }

    // Phát sự kiện
    emit(eventName, data) {
        if (this._listeners[eventName]) {
            this._listeners[eventName].forEach(cb => cb(data));
        }
    }

    // Set token + emit event
    setToken(token) {
        this.token = token;
        this.emit('tokenChanged', token);

        if (!token) {
            this.user = null;
            this.emit('userChanged', null);
        }
    }

    // Set user + emit event
    setUser(user) {
        this.user = user;
        this.emit('userChanged', user);
    }
}

export const broadCastInstance = new BroadCastModule();
