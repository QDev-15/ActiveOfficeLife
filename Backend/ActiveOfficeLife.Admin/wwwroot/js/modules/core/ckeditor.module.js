// /wwwroot/js/ckeditor.module.js
import { UploadCKEditorPlugin } from './uploadCkEditorPlugin.module.js';

function debounce(fn, wait = 200) {
    let t;
    return (...args) => {
        clearTimeout(t);
        t = setTimeout(() => fn.apply(null, args), wait);
    };
}
function waitForClassic(timeout = 8000) {
    return new Promise((resolve, reject) => {
        const start = Date.now();
        (function check() {
            if (window.CKEDITOR && window.CKEDITOR.ClassicEditor) {
                return resolve(window.CKEDITOR.ClassicEditor);
            }
            if (Date.now() - start >= timeout) {
                return reject(new Error('CKEditor super-build chưa nạp xong.'));
            }
            setTimeout(check, 50);
        })();
    });
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


    // LẤY ClassicEditor từ super-build, không cần sử dụng CKEDITOR.ClassicEditor.create(....)
    const ClassicEditor = await waitForClassic();

    const editor = await ClassicEditor.create(el, {
        extraPlugins: [UploadAdapterPlugin],

        placeholder: options.placeholder || 'Viết nội dung tại đây...',

        toolbar: {
            items: options.toolbarItems || [
                'undo', 'redo',
                'heading',
                'bold', 'italic', 'underline', 'strikethrough',
                'superscript', 'subscript',
                'link', 'blockquote', 'code', 'codeBlock',
                'removeFormat',
                'alignment',
                'bulletedList', 'numberedList', 'outdent', 'indent',
                'horizontalLine',
                'imageUpload', 'insertImage', 'mediaEmbed',
                'insertTable', 'tableColumn', 'tableRow', 'mergeTableCells',
                // image styles / resize (có trong super-build, free)
                'imageTextAlternative', 'toggleImageCaption',
                'imageStyle:inline', 'imageStyle:block', 'imageStyle:side',
                'imageStyle:alignLeft', 'imageStyle:alignCenter', 'imageStyle:alignRight',
                'resizeImage',
                // html helpers (free trong super-build)
                'htmlEmbed', 'sourceEditing'
            ],
            shouldNotGroupWhenFull: true
        },

        alignment: { options: ['left', 'center', 'right', 'justify'] }, // căn đoạn văn

        image: {
            toolbar: [
                'imageTextAlternative', 'toggleImageCaption', '|',
                // style căn ảnh + layout
                'imageStyle:alignLeft', 'imageStyle:alignCenter', 'imageStyle:alignRight',
                'imageStyle:inline', 'imageStyle:block', 'imageStyle:side', '|',
                'resizeImage'
            ],
            styles: ['inline', 'block', 'side', 'alignLeft', 'alignCenter', 'alignRight'],
            // UI resize
            resizeUnit: '%',
            resizeOptions: [
                { name: 'resizeImage:original', label: '100%', value: null },
                { name: 'resizeImage:25', label: '25%', value: '25' },
                { name: 'resizeImage:50', label: '50%', value: '50' },
                { name: 'resizeImage:75', label: '75%', value: '75' }
            ]
        },

        table: { contentToolbar: ['tableColumn', 'tableRow', 'mergeTableCells'] },

        // Tắt các plugin cloud/collab để khỏi đòi CloudServices / channelId / CKBox
        removePlugins: [
            // CKBox / CKFinder / Cloud / EasyImage
            'CKBox', 'CKBoxToolbar', 'CKBoxImageEdit', 'CKBoxUploadAdapter', 'CKBoxUtils',
            'CKFinder', 'CKFinderUploadAdapter', 'CloudServices', 'EasyImage',

            // Realtime collaboration (đủ bộ, thường sót cái Client)
            'RealTimeCollaborativeClient',
            'RealTimeCollaborativeComments',
            'RealTimeCollaborativeTrackChanges',
            'RealTimeCollaborativeRevisionHistory',
            'PresenceList', 'Comments',
            'TrackChanges', 'TrackChangesData', 'RevisionHistory',

            // Premium yêu cầu license
            'MultiLevelList', 'PasteFromOfficeEnhanced', 'CaseChange',
            'ExportPdf', 'ExportWord', 'ImportWord', 'ImportPdf',
            'WProofreader', 'FormatPainter', 'AIAssistant',

            // Khác (nếu không dùng)
            'Pagination', 'SlashCommand', 'Template', 'DocumentOutline', 'TableOfContents'
        ],

        // Nén ảnh trước khi upload (adapter của bạn) — KHÁC với UI resize
        aolUpload: {
            resize: { maxWidth: 1600, maxHeight: 1600, quality: 0.9 } // hoặc false để tắt
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
