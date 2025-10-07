// ===== Upload Adapter =====
import { configInstance } from './config.module.js';
class UploaderModule {
    constructor() {
        this.endpoints = {
            uploadToGoogleDriver: '/FTP/upload/googledrive'
        }
    }
    request(endpoint, formData) {
        configInstance.checkLogined();
        return new Promise((resolve, reject) => {
            if (!configInstance.token) {
                window.location.href = '/login';
                return;
            }
            if (!endpoint.startsWith("/")) endpoint = "/" + endpoint;

            const url = new URL(configInstance.urlApi + endpoint);

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
    async uploadToGoogleDriver(formData) {
        return this.request(this.endpoints.uploadToGoogleDriver, formData);
    }
}

export const uploaderModule = new UploaderModule();