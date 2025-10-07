// ===== Upload Adapter =====
import { configInstance } from './config.module.js';
class DriveUploadAdapterModule {
    constructor(loader, { endpoint, headers, resize }) {
        this.loader = loader;
        this.endpoint = endpoint;
        this.headers = headers;   // function or object
        this.resize = resize;     // {maxWidth, maxHeight, quality} | false
        this.abortController = null;
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
    async upload() {
        this.abortController = new AbortController();
        let file = await this.loader.file;

        // (Optional) Resize client-side
        if (this.resize) {
            try { file = await resizeImageIfNeeded(file, this.resize); } catch { /* ignore */ }
        }

        const form = new FormData();
        form.append('file', file, file.name);

        const h = typeof this.headers === 'function' ? this.headers() : (this.headers || {});
        const resp = await fetch(this.endpoint, {
            method: 'POST',
            body: form,
            headers: h,
            signal: this.abortController.signal
        });

        if (!resp.ok) throw new Error(`Upload failed (${resp.status})`);

        const ctype = resp.headers.get('content-type') || '';
        let url;
        if (ctype.includes('application/json')) {
            const data = await resp.json();
            url = data.url || data.link || data.Location || data.href;
        } else {
            const text = (await resp.text() || '').trim();
            if (/^https?:\/\//i.test(text)) url = text;
        }
        if (!url) throw new Error('Không tìm thấy URL ảnh trong phản hồi server.');

        return { default: url };
    }

    abort() {
        if (this.abortController) this.abortController.abort();
    }
}

export const uploaderModule = new DriveUploadAdapterModule();