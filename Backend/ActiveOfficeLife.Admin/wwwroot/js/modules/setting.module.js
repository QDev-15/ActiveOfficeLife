import { apiInstance } from './core/api.module.js';
import { messageInstance } from './core/messages.module.js'
import { spinnerInstance } from './core/spinner.module.js';
import { utilities } from './core/utilities.module.js';
import { uploaderModule } from './core/uploader.module.js'
class SettingModule {
    constructor(
    ) {
        // Đặt endpoint tại đây cho dễ đổi
        this.ENDPOINTS = {
            GET: 'Setting/get',     // GET -> trả { success, data }
            PUT: '/Setting/update',          // PUT body full model.
            PATCH: '/Setting/patch',
            // THÊM 2 endpoint cho OAuth:
            GOOGLE_CONNECT_URL: (id) => `Setting/googledrive/connect?settingId=${encodeURIComponent(id)}`,
            GOOGLE_DISCONNECT: '/Setting/googledrive/disconnect',
            // NEW:
            UPLOAD_LOGO: '/FTP/upload/googledrive',
            RESOLVE_MEDIA_URL: (id) => `/FTP/public-url/${encodeURIComponent(id)}` // <- tùy backend
        };
        utilities.minReloadMs = 5000;

        this._unreg = utilities.registerReloadTask({
            path: "/setting",
            reload: () => this.getdata()
        });
        this.init();
    }

    init() {
        this.bindEvents();
        this.getdata();
    }
    dispose() {
        this._unreg?.();
    }
    // ---- LOAD ----
    getdata() {
        spinnerInstance.showFor("setting-form");
        apiInstance.get(this.ENDPOINTS.GET, null)
            .then(res => {
                if (!res) return;
                const s = res;
                this.bindForm(s);
                this.updatePreviewAndQuickView(s.logo, s);
                this.updateGoogleOauthUi(!!s.googleToken);   // <-- A) toggle UI theo token

                // bắn event để view ẩn nhãn "Đang tải..."
                document.dispatchEvent(new CustomEvent('setting:loaded'));
            })
            .catch(err => {
                console.error('Load setting failed:', err);
            }).finally(() => {
                spinnerInstance.hideFor("setting-form");
            });
    }

    bindForm(s) {
        const orEmpty = v => (v == null ? '' : v);
        const setVal = (sel, val) => {
            const el = document.querySelector(sel);
            if (el) el.value = orEmpty(val);
        };

        setVal('#setting-id', s.id);
        setVal('#setting-name', s.name);
        setVal('#setting-logo', s.logo);
        setVal('#setting-phone', s.phoneNumber);
        setVal('#setting-email', s.email);
        setVal('#setting-address', s.address);
        setVal('#setting-google-client-id', s.googleClientId);
        setVal('#setting-google-client-secret', s.googleClientSecretId);
        setVal('#setting-google-folder-id', s.googleFolderId);
        // KHÔNG còn googleToken input
    }


    // --- EVENTS ---
    bindEvents() {
        // các toggle khác (nếu có)
        document.querySelectorAll('.setting-toggle').forEach(t =>
            t.addEventListener('change', (e) => this.handleToggle(e))
        );

        // Save/Reset
        const btnSave = document.querySelector('#btn-save-setting');
        const btnReset = document.querySelector('#btn-reset-setting');
        if (btnSave) btnSave.addEventListener('click', () => this.save());
        if (btnReset) btnReset.addEventListener('click', () => this.getdata());

        // Preview logo
        const btnPreview = document.querySelector('#btn-preview-logo');
        if (btnPreview) btnPreview.addEventListener('click', () => {
            const logo = (document.querySelector('#setting-logo')?.value || '').trim();
            this.updatePreviewAndQuickView(logo);
        });
        ['#setting-name', '#setting-email', '#setting-phone', '#setting-address', '#setting-logo']
            .forEach(sel => {
                const el = document.querySelector(sel);
                if (el) el.addEventListener('input', () => {
                    const logo = document.querySelector('#setting-logo')?.value || '';
                    this.updatePreviewAndQuickView(logo);
                });
            });
        //// Tự reload khi tab trở lại visible / cửa sổ được focus
        //this._maybeReloadOnFocus = this._maybeReloadOnFocus?.bind(this) || ((e) => {
        //    const visible = document.visibilityState === 'visible';
        //    const enoughTime = (Date.now() - this.lastLoadedAt) > this.minReloadMs;

        //    if (!visible || !enoughTime) return;

        //    // Tránh giật khi người dùng đang gõ
        //    if (utilitiesInstance._isTyping()) {
        //        clearTimeout(this._focusTimer);
        //        this._focusTimer = setTimeout(() => this.getdata(), 1000);
        //    } else {
        //        this.getdata();
        //    }
        //});

        //window.addEventListener('focus', this._maybeReloadOnFocus);
        //document.addEventListener('visibilitychange', this._maybeReloadOnFocus);

        //// Hỗ trợ back-forward cache của trình duyệt (Safari/Firefox)
        //window.addEventListener('pageshow', (ev) => {
        //    if (ev.persisted) this._maybeReloadOnFocus();
        //});
        const btnUploadLogo = document.querySelector('#btn-upload-logo');
        if (btnUploadLogo) btnUploadLogo.addEventListener('click', () => this.handleUploadLogo());
    }

    // --- SAVE ---
    getPayloadFromForm() {
        const v = sel => (document.querySelector(sel)?.value ?? '').toString().trim();
        return {
            id: v('#setting-id'),
            name: v('#setting-name'),
            logo: v('#setting-logo'),
            phoneNumber: v('#setting-phone'),
            email: v('#setting-email'),
            address: v('#setting-address'),
            googleClientId: v('#setting-google-client-id'),
            googleClientSecretId: v('#setting-google-client-secret'),
            googleFolderId: v('#setting-google-folder-id'),
            // KHÔNG gửi googleToken từ client; backend sẽ set khi OAuth callback thành công
        };
    }
    async handleUploadLogo() {
        const fileInput = document.querySelector('#input-logo-file');
        const state = document.querySelector('#logo-upload-state');
        const settingId = (document.querySelector('#setting-id')?.value || '').trim();

        if (!fileInput || !fileInput.files || fileInput.files.length === 0) {
            alert('Chọn file trước khi upload.');
            return;
        }
        if (!settingId) {
            alert('Vui lòng lưu Setting (có Id) trước khi upload.');
            return;
        }

        const file = fileInput.files[0];
        // Tạo tab (optional) nếu cần popup blocker – không cần ở đây
        try {
            if (state) state.textContent = 'Đang upload...';

            const form = new FormData();
            form.append('settingId', settingId);
            form.append('File', file);  
            // Gọi upload
            //const media = await apiInstance.postForm(
            //    this.ENDPOINTS.UPLOAD_LOGO,
            //    form
            //);
            const media = await uploaderModule.uploadToGoogleDriver(form);

            // media = MediaModel: { fileId, filePath, ... }
            if (!media || !media.filePath) throw new Error('Upload không trả về filePath');

            // 1) Gán filePath vào trường logo (yêu cầu của bạn)
            const logoInput = document.querySelector('#setting-logo');
            if (logoInput) logoInput.value = media.filePath;

            // 2) Preview ngay bằng FilePath (URL public)
            const urlForPreview = media.filePath;
            this.updatePreviewAndQuickView(urlForPreview);

            if (state) state.textContent = 'Upload xong, đang lưu setting...';

            // 3) Gọi PUT lưu Setting (logo = fileId)
            await this.save();

            if (state) state.textContent = 'Đã lưu ✔. Đang tải lại...';

            // 4) Reload lại Setting để đồng bộ
            await this.getdata();

            if (state) state.textContent = 'Hoàn tất ✔';
            // Xoá file đã chọn để tránh upload nhầm lần sau
            fileInput.value = '';
        } catch (err) {
            console.error('Upload logo failed:', err);
            if (state) state.textContent = 'Lỗi upload.';
            messageInstance.error('Upload logo thất bại.');
        }
    }

    async save() {
        const btnSave = document.querySelector('#btn-save-setting');
        const btnReset = document.querySelector('#btn-reset-setting');
        const setState = (t) => {
            const s = document.querySelector('#save-state');
            if (s) s.textContent = t || '';
        };
        const toggleBusy = (b) => {
            if (btnSave) btnSave.disabled = b;
            if (btnReset) btnReset.disabled = b;
        };

        try {
            toggleBusy(true);
            setState('Đang lưu...');

            const payload = this.getPayloadFromForm();
            const { data: res } = await apiInstance.patch(this.ENDPOINTS.PATCH, payload);

            setState('Đã lưu ✔');
            this.updatePreviewAndQuickView(payload.logo, payload);
        } catch (err) {
            console.error('Save setting failed:', err);
            setState('Lỗi khi lưu. Vui lòng thử lại.');
            alert('Không thể lưu cài đặt.');
        } finally {
            toggleBusy(false);
        }
    }

    // --- GOOGLE OAUTH UI ---
    updateGoogleOauthUi(isConnected) {
        const btnConnect = document.querySelector('#btn-google-connect');
        const btnDisconnect = document.querySelector('#btn-google-disconnect');
        const badge = document.querySelector('#google-connect-status');

        if (isConnected) {
            if (btnConnect) btnConnect.style.display = 'none';
            if (btnDisconnect) btnDisconnect.style.display = '';
            if (badge) {
                badge.textContent = 'Đã kết nối';
                badge.className = 'badge text-bg-success';
            }
        } else {
            if (btnConnect) btnConnect.style.display = '';
            if (btnDisconnect) btnDisconnect.style.display = 'none';
            if (badge) {
                badge.textContent = 'Chưa kết nối';
                badge.className = 'badge text-bg-secondary';
            }
        }
    }

    // --- preview & quick view (giữ nguyên) ---
    updatePreviewAndQuickView(logoValue, data) {
        const setText = (sel, val) => {
            const el = document.querySelector(sel);
            if (el) el.textContent = (val == null ? '' : val);
        };

        // cập nhật text
        if (data) {
            setText('#quick-name', data.name);
            setText('#quick-email', data.email);
            setText('#quick-phone', data.phoneNumber);
            setText('#quick-address', data.address);
        } else {
            const v = sel => (document.querySelector(sel)?.value ?? '').toString();
            setText('#quick-name', v('#setting-name'));
            setText('#quick-email', v('#setting-email'));
            setText('#quick-phone', v('#setting-phone'));
            setText('#quick-address', v('#setting-address'));
        }

        // cập nhật ảnh
        const preview = document.querySelector('#logo-preview');
        const quick = document.querySelector('#quick-logo');

        const setImg = (url) => {
            if (preview) {
                preview.src = url || '';
                preview.style.display = url ? 'block' : 'none';
                preview.onerror = () => (preview.style.display = 'none');
            }
            if (quick) {
                quick.src = url || '';
                quick.style.display = url ? 'block' : 'none';
                quick.onerror = () => (quick.style.display = 'none');
            }
        };

        const val = (logoValue ?? '').trim();
        if (!val) { setImg(''); return; }

        // Nếu đã là URL tuyệt đối hoặc đường dẫn bắt đầu bằng "/"
        const looksLikeUrl = /^(https?:)?\/\//i.test(val) || val.startsWith('/');
        if (looksLikeUrl) {
            setImg(val);
            return;
        }

        // Ngược lại coi như là Google Drive FileId -> tự build URL xem ảnh
        const toDriveViewUrl = (id) =>
            `https://drive.google.com/uc?export=view&id=${encodeURIComponent(id)}`;

        setImg(toDriveViewUrl(val));
    }


    handleToggle(event) {
        const settingName = event.target.dataset.settingName;
        const isEnabled = event.target.checked;
        fetch('/api/settings/update', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ settingName, isEnabled })
        })
            .then(r => r.json())
            .then(data => {
                if (data.success) console.log(`Setting ${settingName} updated.`);
                else console.error(`Failed to update setting ${settingName}.`);
            })
            .catch(error => console.error('Error:', error));
    }
}
export const settingInstance = new SettingModule();


$(document).on('click', '#btn-google-connect', function () {
    try {
        const id = (document.querySelector('#setting-id')?.value || '').trim();
        if (!id) {
            messageInstance.info('Vui lòng lưu Setting trước khi kết nối Google.');
            return;
        }

        const authUrl = apiInstance.getUrl(settingInstance.ENDPOINTS.GOOGLE_CONNECT_URL(id));
        window.open(authUrl, '_blank', 'noopener');
        
    } catch (e) {
        console.error('Connect Google failed:', e);
        messageInstance.error('Không thể bắt đầu kết nối Google.');
    }
});
$(document).on('click', '#btn-google-disconnect', function () {
    apiInstance.get(settingInstance.ENDPOINTS.GOOGLE_DISCONNECT).then(() => {
        messageInstance.success("Ngắt kết nối đến google drive thành công.")
        settingInstance.getdata();
    })
});

