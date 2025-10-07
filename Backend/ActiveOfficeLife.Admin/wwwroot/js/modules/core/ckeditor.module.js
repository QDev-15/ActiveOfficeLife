// /wwwroot/js/ckeditor.module.js
import { UploadCKEditorPlugin } from './uploadCkEditorPlugin.module.js';

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
  const resize = cfg.resize || { maxWidth: 1600, maxHeight: 1600, quality: 0.9 }; // set false để tắt

  editor.plugins.get('FileRepository').createUploadAdapter = loader =>
      new UploadCKEditorPlugin(loader, { resize });
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
                'undo', 'redo', 'codeBlock', 'imageTextAlternative',
                'toggleImageCaption', '|', 'imageStyle:inline', 'imageStyle:block',
                'imageStyle:side', 'resizeImage'
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
                'imageStyle:side',
                'resizeImage'
            ],
            // Tùy chọn kích thước nhanh
            resizeUnit: '%',
            resizeOptions: [
                { name: 'resizeImage:original', label: '100%', value: null },
                { name: 'resizeImage:25', label: '25%', value: '25' },
                { name: 'resizeImage:50', label: '50%', value: '50' },
                { name: 'resizeImage:75', label: '75%', value: '75' }
            ],
            styles: [ 'inline', 'block', 'side' ]
        },
        table: { contentToolbar: [ 'tableColumn', 'tableRow', 'mergeTableCells' ] },
        aolUpload: {
            resize: true        // override resize or set false
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
