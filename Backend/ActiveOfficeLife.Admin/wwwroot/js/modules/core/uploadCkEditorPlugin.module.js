import { uploaderModule } from './uploader.module.js';
export class UploadCKEditorPlugin {
    constructor(loader, { resize }) {
        this.loader = loader;
        this.resize = resize;     // {maxWidth, maxHeight, quality} | false
        this.abortController = null;
    }

    async upload() {
        this.abortController = new AbortController();
        let file = await this.loader.file;

        // (Optional) Resize client-side
        if (this.resize) {
            try { file = await this.resizeImageIfNeeded(file, this.resize); } catch { /* ignore */ }
        }

        const form = new FormData();
        form.append('file', file, file.name);

        const media = await uploaderModule.uploadToGoogleDriver(form);

        
        if (!media || !media.filePath) throw new Error('Không tìm thấy URL ảnh trong phản hồi server.');

        return { default: media.filePath };
    }

    // Resize ảnh client-side (tùy chọn)
    async resizeImageIfNeeded(file, { maxWidth = 1600, maxHeight = 1600, quality = 0.9 } = {}) {
        // Chỉ resize các file ảnh
        if (!file || !file.type.startsWith('image/')) return file;

        const img = await new Promise((res, rej) => {
            const i = new Image();
            i.onload = () => res(i);
            i.onerror = rej;
            i.src = URL.createObjectURL(file);
        });

        let { width, height } = img;
        const ratio = Math.min(maxWidth / width, maxHeight / height, 1); // <=1 để không upscale
        if (ratio === 1) return file; // Không cần resize

        width = Math.round(width * ratio);
        height = Math.round(height * ratio);

        const canvas = document.createElement('canvas');
        canvas.width = width;
        canvas.height = height;
        const ctx = canvas.getContext('2d');
        ctx.drawImage(img, 0, 0, width, height);

        const mime = file.type || 'image/jpeg';
        const blob = await new Promise(res => canvas.toBlob(res, mime, quality));
        return new File([blob], file.name, { type: mime, lastModified: Date.now() });
    }

    abort() {
        if (this.abortController) this.abortController.abort();
    }
}
