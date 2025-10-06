// ApiModule.js
import { configInstance } from './config.module.js';

class ApiModule {
    constructor() {
        this.stopped = false;
    }
    request(method, endpoint, params = null, body = null, extraHeaders = {}) {
        configInstance.checkLogined();
        return new Promise((resolve, reject) => {
            if (!configInstance.token) {
                // Token null -> logout
                window.location.href = '/login';
                return;
            }
            if (!endpoint.startsWith("/")) {
                endpoint = "/" + endpoint;
            }
            let url = new URL(configInstance.urlApi + endpoint);

            if (params && typeof params === 'object') {
                Object.entries(params).forEach(([key, value]) => {
                    if (value !== null && value !== undefined) {
                        url.searchParams.append(key, value);
                    }
                });
            }

            const options = {
                method,
                headers: {
                    'Content-Type': 'application/json'
                }
            };

            // Thêm token vào header
            options.headers['Authorization'] = `Bearer ${configInstance.token}`;

            // Nếu có body
            if (body) {
                options.body = JSON.stringify(body);
            }

            fetch(url.toString(), options)
                .then(async res => {
                    const contentType = res.headers.get('content-type');
                    let result = null;

                    if (contentType && contentType.includes('application/json')) {
                        result = await res.json();
                    } else {
                        result = await res.text();
                    }

                    if (!res.ok) {
                        reject({
                            status: res.status,
                            message: result?.message || res.statusText,
                            result
                        });
                        return;
                    }

                    if (!result.success) {
                        if (res.ok && res.status == 200 && result) {
                            resolve(result);
                            return;
                        } else {
                            reject({
                                status: res.status,
                                message: result?.data?.message || res.statusText,
                                result
                            });
                            return;
                        }
                    }
                    resolve(result?.data);
                })
                .catch(err => {
                    reject({
                        status: 0,
                        message: err.message || 'Network Error',
                        data: err
                    });
                });
        });
    }
    /// <summary>
    /// POST form-data (upload file)
    postForm(endpoint, formData, params = null) {
        configInstance.checkLogined();
        return new Promise((resolve, reject) => {
            if (!configInstance.token) {
                window.location.href = '/login';
                return;
            }
            if (!endpoint.startsWith("/")) endpoint = "/" + endpoint;

            const url = new URL(configInstance.urlApi + endpoint);
            if (params && typeof params === 'object') {
                Object.entries(params).forEach(([k, v]) => {
                    if (v !== null && v !== undefined) url.searchParams.append(k, v);
                });
            }

            const options = {
                method: 'POST',
                // KHÔNG set 'Content-Type' để trình duyệt tự gắn boundary
                headers: {
                    'Authorization': `Bearer ${configInstance.token}`
                },
                body: formData
            };

            fetch(url.toString(), options)
                .then(async res => {
                    const ct = res.headers.get('content-type') || '';
                    let result = ct.includes('application/json') ? await res.json() : await res.text();

                    if (!res.ok) {
                        reject({
                            status: res.status,
                            message: (result && (result.message || result.error)) || res.statusText,
                            result
                        });
                        return;
                    }

                    // Nếu server dùng envelope {success, data} -> trả data
                    if (result && Object.prototype.hasOwnProperty.call(result, 'success')) {
                        if (result.success !== true) {
                            reject({
                                status: res.status,
                                message: (result?.data?.message || result?.message || 'Request failed'),
                                result
                            });
                            return;
                        }
                        resolve(result.data);
                        return;
                    }

                    // Nếu server trả thẳng MediaModel
                    resolve(result);
                })
                .catch(err => {
                    reject({
                        status: 0,
                        message: err?.message || 'Network Error',
                        data: err
                    });
                });
        });
    }
    getUrl(endpoint) {
        if (!endpoint.startsWith("/")) {
            endpoint = "/" + endpoint;
        }
        return configInstance.urlApi + endpoint;
    }
    // GET: url, payload (params)
    get(url, payload) {
        return this.request('GET', url, payload);
    }

    // POST: url, body
    post(url, body) {
        return this.request('POST', url, null, body);
    }

    // PUT: url, body
    put(url, body) {
        return this.request('PUT', url, null, body);
    }
    // Patch: url, body
    patch(url, body) {
        return this.request('PATCH', url, null, body);
    }

    // DELETE: url, payload (params)
    delete(url, payload) {
        return this.request('DELETE', url, payload);
    }
}

export const apiInstance = new ApiModule();
