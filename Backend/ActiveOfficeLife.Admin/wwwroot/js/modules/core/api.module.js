// ApiModule.js
import { configInstance } from './config.module.js';

class ApiModule {
    request(method, endpoint, params = null, body = null) {
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

                    if (!result.isSuccess) {
                        reject({
                            status: res.status,
                            message: result?.data?.message || res.statusText,
                            result
                        });
                        return;
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

    // GET: url, payload (params)
    get(url, payload = {}) {
        return this.request('GET', url, payload);
    }

    // POST: url, body
    post(url, body = {}) {
        return this.request('POST', url, null, body);
    }

    // PUT: url, body
    put(url, body = {}) {
        return this.request('PUT', url, null, body);
    }

    // DELETE: url, payload (params)
    delete(url, payload = {}) {
        return this.request('DELETE', url, payload);
    }
}

export const apiInstance = new ApiModule();
