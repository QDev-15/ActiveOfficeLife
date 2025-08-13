import { configInstance } from "./config.module.js";
class MvcModule {
    request(method, url, payload = null) {
        configInstance.checkLogined();
        const options = {
            method: method,
            headers: { 'Content-Type': 'application/json' }
        };
        if (payload) options.body = JSON.stringify(payload);

        return fetch(url, options).then(res => res.text());
    }
    get(url, payload = null) {
        if (payload) {
            const query = new URLSearchParams(payload).toString();
            url += (url.includes('?') ? '&' : '?') + query;
        }
        return this.request('GET', url);
    }
    post(url, payload) {
        return this.request('POST', url, payload);
    }
    put(url, payload) {
        return this.request('PUT', url, payload);
    }
    delete(url, payload) {
        return this.request('DELETE', url, payload);
    }
};
export const mvcInstance = new MvcModule();