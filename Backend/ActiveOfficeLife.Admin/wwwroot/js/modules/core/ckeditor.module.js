// /wwwroot/js/ckeditor.module.js

// ===== Helpers =====
function getCsrf() {
  const el = document.querySelector('input[name="__RequestVerificationToken"]');
  return el ? el.value : null;
}

// Resize ảnh client-side (tùy chọn)
async function resizeImageIfNeeded(file, { maxWidth = 1600, maxHeight = 1600, quality = 0.9 } = {}) {
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

// ===== Upload Adapter =====
class DriveUploadAdapter {
  constructor(loader, { endpoint, headers, resize }) {
    this.loader = loader;
    this.endpoint = endpoint;
    this.headers = headers;   // function or object
    this.resize = resize;     // {maxWidth, maxHeight, quality} | false
    this.abortController = null;
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
function debounce(fn, wait = 200) {
    let t;
    return (...args) => {
        clearTimeout(t);
        t = setTimeout(() => fn.apply(null, args), wait);
    };
}
// Plugin hook
function UploadAdapterPlugin(editor) {
  const cfg = editor.config.get('aolUpload') || {};
  const endpoint = cfg.endpoint || '/api/FTP/upload/googledrive';
  const headers = cfg.headers || (() => {
    const h = {};
    const csrf = getCsrf();
    if (csrf) h['RequestVerificationToken'] = csrf;
    // Nếu cần Bearer:
    // const token = localStorage.getItem('access_token');
    // if (token) h['Authorization'] = `Bearer ${token}`;
    return h;
  });
  const resize = cfg.resize || { maxWidth: 1600, maxHeight: 1600, quality: 0.9 }; // set false để tắt

  editor.plugins.get('FileRepository').createUploadAdapter = loader =>
    new DriveUploadAdapter(loader, { endpoint, headers, resize });
}

// ===== Public API =====
const _editors = new Map(); // selector -> editor instance

export async function initCK(selector, options = {}) {
    const el = typeof selector === 'string' ? document.querySelector(selector) : selector;
    if (!el) throw new Error('Không tìm thấy phần tử để init CKEditor');

    const editor = await ClassicEditor.create(el, {
        extraPlugins: [ UploadAdapterPlugin ],
        placeholder: options.placeholder || 'Viết nội dung tại đây...',
        toolbar: {
            items: options.toolbarItems || [
            'heading', '|',
            'bold', 'italic', 'underline', 'strikethrough', 'link', 'blockquote', '|',
            'bulletedList', 'numberedList', 'outdent', 'indent', '|',
            'alignment', 'removeFormat', '|',
            'imageUpload', 'insertTable', 'mediaEmbed', '|',
            'undo', 'redo', 'codeBlock'
            ],
            shouldNotGroupWhenFull: true
        },
        alignment: { options: [ 'left', 'center', 'right', 'justify' ] },
        image: {
            toolbar: [
            'imageTextAlternative',
            'toggleImageCaption',
            '|',
            'imageStyle:inline',
            'imageStyle:block',
            'imageStyle:side'
            ],
            styles: [ 'inline', 'block', 'side' ]
        },
        table: { contentToolbar: [ 'tableColumn', 'tableRow', 'mergeTableCells' ] },
        aolUpload: {
            endpoint: options.endpoint || '/api/FTP/upload/googledrive',
            headers: options.headers,     // fn or object
            resize: options.resize        // override resize or set false
        }
    });

    // Chiều cao tối thiểu
    editor.ui.view.editable.element.style.minHeight = (options.minHeight || 480) + 'px';

    _editors.set(selector, editor);
    // --- Live sync vào <textarea> khi người dùng gõ ---
    const liveSyncEnabled = (options.liveSync !== false);           // mặc định: true
    const liveSyncDebounce = options.liveSyncDebounce ?? 200;       // mặc định: 200ms
    if (liveSyncEnabled) {
        const liveSync = debounce(() => editor.updateSourceElement(), liveSyncDebounce);
        editor.model.document.on('change:data', liveSync);
        // lưu disposer để gỡ khi destroy
        editor._aolLiveSyncOff = () => editor.model.document.off('change:data', liveSync);
    }

    return editor;
}

// Đồng bộ nội dung về textarea trước khi submit form
export function updateSourceOnSubmit(formOrSelector) {
  const form = typeof formOrSelector === 'string'
    ? document.querySelector(formOrSelector)
    : formOrSelector;
  if (!form) return;
  form.addEventListener('submit', () => {
    _editors.forEach(ed => ed.updateSourceElement());
  });
}

// Lấy editor theo selector
export function getEditor(selector) { return _editors.get(selector) || null; }

// Hủy editor (nếu cần)
export async function destroyCK(selector) {
    const ed = _editors.get(selector);
    if (!ed) return;
    if (typeof ed._aolLiveSyncOff === 'function') {
        try { ed._aolLiveSyncOff(); } catch { }
    }
    await ed.destroy();
    _editors.delete(selector);
}

// (Tùy chọn) gắn global để gọi từ inline script
window.initck = initCK;
window.updateCkOnSubmit = updateSourceOnSubmit;
