import { apiInstance } from './core/api.module.js';
import { mvcInstance } from './core/mvc.module.js';
import { configInstance } from './core/config.module.js';
import { messageInstance } from './core/messages.module.js';
import { spinnerInstance } from './core/spinner.module.js';
import { broadCastInstance } from './core/broadcast.module.js';
import { dialogInstance } from './core/dialog.module.js';
import { utilities } from './core/utilities.module.js';
import { initCK } from './core/ckeditor.module.js';
class PostModule {

    constructor() {
        this.token = configInstance.token;
        this.currentPage = 1;
        this.pageSize = 10;
        this.sortField = 'name';
        this.sortDirection = 'asc';
        this.globalData = [];
        this.totalCount = 0;
        this.tableId = "articlesTable";
        this.postTableInstance = null;
        this.ENDPOINTS = {
            GET_ONE: (id) => `/post/get/${id}`,        // chỉnh cho đúng route API của bạn
            CAT_ALL: `/category/all?pageSize=10000`, // hoặc /Category/all
            TAG_ALL: `/tag/all?pageSize=10000`,
            CREATE: `/post/create`,
            UPDATE: `/Post/update`,
            PATCH: `/Post/patch`,
            DELETE: (id) => `/post/delete/${id}`,     // nếu API của bạn là /post/{id} thì sửa lại
        };
        this.id = null;
        this.form = null;
    }

    // init DataTable
    initTable() {
        this.postTableInstance = $(`#${this.tableId}`).DataTable({
            processing: true,
            serverSide: true,
            stateSave: true,
            responsive: true,
            searchDelay: 500, // searchDelay mặc định sau 0.5s mới search
            order: [[1, 'desc']],
            pagingType: "full_numbers", // để hiển thị First, Prev, Next, Last
            mark: true, // ✅ Bật highlight search
            language: {
                paginate: {
                    first: '<i class="bi bi-chevron-double-left"></i>',
                    previous: '<i class="bi bi-chevron-left"></i>',
                    next: '<i class="bi bi-chevron-right"></i>',
                    last: '<i class="bi bi-chevron-double-right"></i>'
                }
            },
            ajax: (d, callback) => {
                let params = {
                    status: $("#article_status").val(),
                    pageIndex: (d.start / d.length) + 1,
                    pageSize: d.length,
                    keySearch: d.search.value || "",
                    sortField: d.order.length > 0 ? d.columns[d.order[0].column].name : "name",
                    sortDirection: d.order.length > 0 ? d.order[0].dir : "desc"
                };

                apiInstance.get('/post/all', params)
                    .then(response => {
                        this.pageSize = response.pageSize;
                        this.pageIndex = response.pageIndex;
                        callback({
                            data: response.items || [],
                            recordsTotal: response.totalCount || 0,
                            recordsFiltered: response.totalCount || 0,
                        });
                    })
                    .catch(error => {
                        console.error("❌ Error loading categories:", error);
                        callback({
                            data: [],
                            recordsTotal: 0,
                            recordsFiltered: 0
                        });
                    });
            },
            columns: [
                {
                    data: null,
                    title: "STT",
                    className: "text-center",
                    width: "60px",
                    render: function (data, type, row, meta) {
                        return meta.row + 1 + meta.settings._iDisplayStart;
                    },
                    orderable: false // Không sort theo STT
                },
                {
                    data: "title",
                    name: "title",
                    title: "Article Name",
                    className: "fw-bold text-primary",
                    orderable: true
                },
                {
                    data: null,
                    name: "isFeaturedHome",
                    title: "Featured Home",
                    className: "text-center",
                    orderable: true,
                    render: function (data, type, row) {
                        if (row.isFeaturedHome) {
                            return '<span class="badge bg-success">On</span>';
                        } else {
                            return '<span class="badge bg-secondary">Off</span>';
                        }
                    }
                },
                {
                    data: null,
                    name: "isHot",
                    title: "Hot",
                    className: "text-center",
                    orderable: true,
                    render: function (data, type, row) {
                        if (row.isHot) {
                            return '<span class="badge bg-success">On</span>';
                        } else {
                            return '<span class="badge bg-secondary">Off</span>';
                        }
                    }
                },
                {
                    data: null,
                    name: "isCenterHighlight",
                    title: "Center Highlight",
                    className: "text-center",
                    orderable: true,
                    render: function (data, type, row) {
                        if (row.isCenterHighlight) {
                            return '<span class="badge bg-success">On</span>';
                        } else {
                            return '<span class="badge bg-secondary">Off</span>';
                        }
                    }
                },
                {
                    data: "displayOrder",
                    name: "displayOrder",
                    title: "Order",
                    className: "text-center",
                },
                {
                    data: null, // null để lấy toàn bộ row
                    name: "category",
                    title: "Category name",
                    className: "text-center",
                    orderable: true,
                    render: function (data, type, row) {
                        if (data.category && data.category.name) {
                            return data.category.name;
                        } else {
                            return '----';
                        }
                    }
                },
                {
                    data: "status",
                    name: "status",
                    title: "Status",
                    className: "text-center",
                    orderable: true,
                    render: (status) => {
                        const s = (status || '').toString().toLowerCase();
                        const map = {
                            'published': 'success',
                            'draft': 'secondary',
                            'pending': 'warning',
                            'closed': 'dark'
                        };
                        const color = map[s] || 'secondary';
                        const text = status || 'Unknown';
                        return `<span class="badge bg-${color}">${text}</span>`;
                    }
                },
                {
                    data: null, // null để lấy toàn bộ row
                    title: "Actions",
                    className: "text-center",
                    orderable: false,
                    render: function (data, type, row) {
                        return `<button class="btn btn-sm btn-warning btn-edit"
                                    onclick="postInstance.edit('${row.id}', '${row.title}')"
                                    data-id="${row.id}"
                                    data-name="${row.title}">Edit</button>
                                <button class="btn btn-sm btn-view"
                                    onclick="postInstance.view('${row.id}')"
                                    data-id="${row.id}"
                                    data-name="${row.title}">View</button>
                                <button class="btn btn-sm btn-danger btn-delete"
                                    onclick="postInstance.delete('${row.id}', '${row.title}')"
                                    data-id="${row.id}"
                                    data-name="${row.title}">Delete</button>
                                `;
                    }
                }
            ],
            pageLength: 10
        });

    }

    // ====== Life cycle ======
    initEditPage() {

        // lấy id từ dataset hoặc query
        const root = document.getElementById('post-edit-root');
        this.id = root?.dataset?.postId || new URLSearchParams(location.search).get('id');
        if (!this.id) {
            messageInstance.error('Không tìm thấy ID bài viết.');
            return;
        }
        this.loadData();

    }
    refreshData() {
        this.postTableInstance?.ajax?.reload(null, false);
    }

    edit(id) {
        // Điều hướng tới trang Edit. Nếu action Edit nhận query ?id=...
        window.location.href = `/Articles/Edit?id=${encodeURIComponent(id)}`;
        // Nếu bạn map theo route /Articles/Edit/{id} thì dùng:
        // window.location.href = `/Articles/Edit/${encodeURIComponent(id)}`;
    }
    async loadData() {
        // 1) Render form ngay lập tức (skeleton/empty) để user thấy UI trước
        try {
            await spinnerInstance.showForMainContainerAsync();

            const html = this.buildEmptyEditFormHtml(); // render form trước
            $('#content-artile-edit').html(html);

            // init validator sớm để user có thể nhập luôn
            this.form = configInstance.initValidatorForm("content-artile-edit");

            // wire basic events/counters/buttons (chưa cần data)
            this.wireInputs();

            // init editor sớm (để user có thể nhập content ngay)
            // tránh init lại nhiều lần
            await this.ensureEditorInitialized();

            // status text mặc định
            const statusElement = document.getElementById('Status');
            const statusText = document.getElementById('statusText');
            if (statusText && statusElement) statusText.textContent = this.statusDisplay(statusElement.value || '');

            // render buttons theo status mặc định
            this.renderButtons(statusElement?.value);

            utilities.lastLoadedAt = Date.now();

        } catch (err) {
            console.error('Lỗi render form trước:', err);
            messageInstance.error('Không thể render form');
        } finally {
            await spinnerInstance.hideForMainContainerAsync();
        }

        // 2) Chạy API nền: không await trực tiếp để không chặn UI
        //    Bind UI theo từng phần khi dữ liệu về
        const postPromise = apiInstance.get(this.ENDPOINTS.GET_ONE(this.id));
        const catPromise = apiInstance.get(this.ENDPOINTS.CAT_ALL);
        const tagPromise = apiInstance.get(this.ENDPOINTS.TAG_ALL);

        // Post
        postPromise
            .then(postRes => {
                const post = postRes || {};
                this.model = post;

                this.bindPostToForm(post);
                this.bindSeoToForm(post);

                // status text + buttons theo status thật
                const statusElement = document.getElementById('Status');
                const statusText = document.getElementById('statusText');
                if (statusText && statusElement) statusText.textContent = this.statusDisplay(statusElement.value || '');
                this.renderButtons(statusElement?.value);

                // set editor content nếu editor đã init
                this.setEditorContentSafely(post.content || '');

                utilities.lastLoadedAt = Date.now();
            })
            .catch(err => {
                console.error('Lỗi load post:', err);
                messageInstance.error('Không thể tải bài viết');
            });

        // Categories
        catPromise
            .then(catRes => {
                const categories = (catRes && (catRes.items || catRes)) || [];
                this.bindCategories(categories);
            })
            .catch(err => {
                console.error('Lỗi load categories:', err);
                messageInstance.error('Không thể tải danh mục');
            });

        // Tags
        tagPromise
            .then(tagRes => {
                const tags = (tagRes && (tagRes.items || tagRes)) || [];
                // nếu post chưa về thì vẫn render tags, checked sẽ update sau trong bindPostToForm()
                this.bindTags(tags);
            })
            .catch(err => {
                console.error('Lỗi load tags:', err);
                messageInstance.error('Không thể tải tags');
            });
    }

    /* ===========================
       Helpers: build empty form
    =========================== */
    buildEmptyEditFormHtml() {
        // giữ nguyên IDs để các method khác không hỏng
        // tạo placeholder cho category/tags để bind sau
        return `
    <form id="article-edit-form" novalidate>
      <input type="hidden" id="Id" value="">
      <input type="hidden" id="SeoMetadataId" value="">
      <input type="hidden" id="Status" value="">

      <div class="row g-3">
        <div class="col-12 col-lg-8">
          <label for="Title" class="form-label">Title</label>
          <input id="Title" name="Title" class="form-control" value="" />
        </div>

        <div class="col-12 col-lg-4">
          <label for="Slug" class="form-label">Slug</label>
          <div class="input-group">
            <input id="Slug" name="Slug" class="form-control" value="" data-user-edited="false" data-last-auto="" />
            <button type="button" class="btn btn-outline-secondary" onclick="postInstance.reSlug()">Auto</button>
          </div>
        </div>

        <div class="col-12 col-md-4">
          <label for="CategoryId" class="form-label">Category</label>
          <select id="CategoryId" name="CategoryId" class="form-select select2">
            <option value="">-- Select --</option>
          </select>
        </div>

        <div class="col-12">
          <label class="form-label">Tags</label>
          <div id="tagsContainer">
            <div class="text-muted small">Đang tải tags...</div>
          </div>
        </div>

        <div class="col-12 d-flex flex-wrap gap-5">
          <div class="form-check form-switch mb-3 col-auto">
            <input id="IsCenterHighlight" type="checkbox" class="form-check-input">
            <label class="form-check-label" for="IsCenterHighlight">Center Highlight</label>
          </div>
          <div class="form-check form-switch mb-3 col-auto">
            <input id="IsFeaturedHome" type="checkbox" class="form-check-input">
            <label class="form-check-label" for="IsFeaturedHome">Featured Home</label>
          </div>
          <div class="form-check form-switch mb-3 col-auto">
            <input id="IsHot" type="checkbox" class="form-check-input">
            <label class="form-check-label" for="IsHot">Hot</label>
          </div>
        </div>

        <div class="col-12 col-lg-4">
          <label for="DisplayOrder" class="form-label">Order</label>
          <input id="DisplayOrder" type="number" class="form-control" value="0">
        </div>

        <div class="col-12">
          <label for="Summary" class="form-label">Summary</label>
          <textarea id="Summary" name="Summary" class="form-control" rows="3"></textarea>
          <small id="summaryCounter" class="form-text text-end d-block">0</small>
        </div>

        <div class="col-12">
          <label for="Content" class="form-label">Content</label>
          <textarea id="Content" name="Content" class="form-control" rows="10"></textarea>
        </div>

        <hr/>

        <div class="border shadow p-3 rounded mb-3 bg-gray-200">
          <h6>SEO</h6>
          <div class="mb-3">
            <label for="SeoMetadata_MetaTitle" class="form-label">Meta Title</label>
            <input id="SeoMetadata_MetaTitle" class="form-control" value="">
            <small id="seoTitleCounter" class="form-text text-end d-block">0</small>
          </div>

          <div class="mb-3">
            <label for="SeoMetadata_MetaDescription" class="form-label">Meta Description</label>
            <textarea id="SeoMetadata_MetaDescription" class="form-control" rows="2"></textarea>
            <small id="seoDescCounter" class="form-text text-end d-block">0</small>
          </div>

          <div class="mb-3">
            <label for="SeoMetadata_MetaKeywords" class="form-label">Meta Keywords</label>
            <input id="SeoMetadata_MetaKeywords" class="form-control" value="">
          </div>
        </div>
      </div>
    </form>
  `;
    }

    /* ===========================
       Helpers: bind data to UI
    =========================== */
    bindPostToForm(post) {
        // Bảo vệ không overwrite những gì user đã gõ khi data về trễ
        const $title = $('#Title');
        const $slug = $('#Slug');

        const titleChangedByUser = ($title.data('dirty') === true);
        const slugChangedByUser = ($slug.attr('data-user-edited') === 'true');

        if (!titleChangedByUser) $title.val(post.title || '');

        // chỉ set slug nếu user chưa tự sửa
        if (!slugChangedByUser) {
            const auto = utilities.slugify(post.title || '') || '';
            $slug.val(post.slug || auto);
            $slug.attr('data-last-auto', utilities.escapeHtml(auto));
        }

        $('#Id').val(post.id || '');
        $('#Status').val(post.status || '');
        $('#DisplayOrder').val(post.displayOrder ?? 0);
        $('#Summary').val(post.summary || '');
        $('#summaryCounter').text((post.summary || '').length);

        // switches
        $('#IsCenterHighlight').prop('checked', !!post.isCenterHighlight);
        $('#IsFeaturedHome').prop('checked', !!post.isFeaturedHome);
        $('#IsHot').prop('checked', !!post.isHot);

        // category (options có thể chưa có -> lưu tạm value; khi categories bind sẽ set lại)
        $('#CategoryId').val(post.categoryId || '');

        // update checked tags nếu tags đã render rồi
        if (Array.isArray(post.tagIds)) {
            const tagIds = post.tagIds.map(String);
            $('#tagsContainer input[type="checkbox"]').each(function () {
                const v = String(this.value || '');
                this.checked = tagIds.includes(v);
            });
        }

        // trigger change cho các code khác đang nghe
        $('#CategoryId').trigger('change');
    }

    bindSeoToForm(post) {
        const seo = post.seoMetadata || {};
        const seoId = post.seoMetadataId || seo.id || '';

        $('#SeoMetadataId').val(seoId || '');
        $('#SeoMetadata_MetaTitle').val(seo.metaTitle || '');
        $('#seoTitleCounter').text((seo.metaTitle || '').length);

        $('#SeoMetadata_MetaDescription').val(seo.metaDescription || '');
        $('#seoDescCounter').text((seo.metaDescription || '').length);

        $('#SeoMetadata_MetaKeywords').val(seo.metaKeywords || '');
    }

    bindCategories(categories) {
        const post = this.model || {};
        const currentCategoryId = String($('#CategoryId').val() || post.categoryId || '');

        const options = (categories || []).map(c => {
            const id = String(c.id || '');
            const selected = (id && id === String(currentCategoryId)) ? 'selected' : '';
            return `<option value="${utilities.escapeHtml(id)}" ${selected}>${utilities.escapeHtml(c.name || '')}</option>`;
        }).join('\n');

        $('#CategoryId').html(`<option value="">-- Select --</option>\n${options}`);

        // re-apply selected
        $('#CategoryId').val(currentCategoryId).trigger('change');
    }

    bindTags(tags) {
        const post = this.model || {};
        const postTagIds = (post.tagIds || []).map(String);

        const tagsHtml = (tags || []).map(t => {
            const id = String(t.id || '');
            const checked = postTagIds.includes(id) ? 'checked' : '';
            return `
      <div class="form-check form-check-inline">
        <input class="form-check-input" type="checkbox"
          id="tag_${utilities.escapeHtml(id)}"
          value="${utilities.escapeHtml(id)}" ${checked}>
        <label class="form-check-label" for="tag_${utilities.escapeHtml(id)}">
          ${utilities.escapeHtml(t.name || '')}
        </label>
      </div>
    `;
        }).join('\n');

        $('#tagsContainer').html(tagsHtml || `<div class="text-muted small">Không có tags</div>`);
    }

    /* ===========================
       CKEditor init safe
    =========================== */
    /* ===========================
   CKEditor init safe (supports re-render DOM)
=========================== */
    async ensureEditorInitialized() {
        const selector = '#Content';
        const el = document.querySelector(selector);

        // Không có textarea thì thôi (form chưa render xong)
        if (!el) return null;

        // Nếu đang init dở cho đúng element hiện tại thì dùng lại
        if (this._editorInitPromise && this._editorHostEl === el) {
            return this._editorInitPromise;
        }

        // Nếu đã có editor nhưng DOM đã re-render (host element khác / mất)
        // thì destroy editor cũ trước khi init lại
        if (this._editor) {
            const attachedOk =
                this._editorHostEl === el &&
                (this._editor.sourceElement ? this._editor.sourceElement === el : true);

            if (!attachedOk) {
                try {
                    await this._editor.destroy();
                } catch (e) {
                    console.warn('CKEditor destroy failed (ignored):', e);
                } finally {
                    this._editor = null;
                    this._editorInitPromise = null;
                    this._editorHostEl = null;
                }
            }
        }

        // Init mới cho element hiện tại
        this._editorHostEl = el;
        this._editorInitPromise = initCK(selector)
            .then(editor => {
                this._editor = editor;
                // đảm bảo editor vẫn đúng element (phòng init xong nhưng DOM đổi tiếp)
                const currentEl = document.querySelector(selector);
                if (currentEl && currentEl !== this._editorHostEl) {
                    // DOM đổi trong lúc init -> destroy editor vừa tạo để tránh leak
                    try { editor.destroy(); } catch { }
                    this._editor = null;
                    this._editorInitPromise = null;
                    this._editorHostEl = null;
                    return null;
                }
                return editor;
            })
            .catch(err => {
                console.error('CKEditor init failed:', err);
                this._editor = null;
                this._editorInitPromise = null;
                this._editorHostEl = null;
                return null;
            });

        return this._editorInitPromise;
    }


    setEditorContentSafely(html) {
        // nếu editor đã init thì set vào editor, nếu chưa thì set vào textarea
        if (this._editor && typeof this._editor.setData === 'function') {
            // chỉ set nếu user chưa nhập gì (tránh overwrite)
            const current = this._editor.getData?.() || '';
            if (!current || current.trim().length === 0) this._editor.setData(html || '');
        } else {
            const $content = $('#Content');
            if ($content.val() && String($content.val()).trim().length > 0) return;
            $content.val(html || '');
        }
    }

    validate() {
        return this.form?.valid();
    }
    // ====== Render ======

    renderButtons(status) {
        // ẩn tất cả
        const hide = id => document.getElementById(id)?.classList.add('d-none');
        const show = id => document.getElementById(id)?.classList.remove('d-none');
        ['btnView', 'btnPublish', 'btnUnpublish', 'btnClose', 'btnRepublish'].forEach(hide);

        const s = (status || '').toString().toLowerCase();
        if (s === 'draft') {
            show('btnPublish');
        } else if (s === 'published') {
            show('btnView'); show('btnUnpublish'); show('btnClose');
        } else if (s === 'paused') {
            show('btnRepublish'); show('btnClose');
        } else if (s === 'closed') {
            show('btnRepublish');
        }
    }

    statusDisplay(s) {
        const map = {
            draft: 'Bản nháp',
            published: 'Đã xuất bản',
            paused: 'Tạm dừng',
            closed: 'Đóng',
        };
        return map[(s || '').toLowerCase()] || (s || 'Không rõ');
    }
    // validate Title
    blurTitle() {
        this.validate();
        this.setAutoSlug();
    }
    // validate Slug
    setAutoSlug = () => {
        const $title = document.getElementById('Title');
        const $slug = document.getElementById('Slug');
        const next = utilities.slugify($title.value);
        if (!next) return;
        const userEdited = $slug.dataset.userEdited === 'true';
        if (!userEdited || !$slug.value) {
            $slug.value = next;
            $slug.dataset.lastAuto = next;  // ghi nhớ bản auto gần nhất
        }
    };

    reSlug() {
        const $slug = document.getElementById('Slug');
        $slug.dataset.userEdited = 'false';
        this.setAutoSlug();
    }
    changeSlug() {
        const $slug = document.getElementById('Slug');
        const lastAuto = $slug.dataset.lastAuto || '';
        $slug.dataset.userEdited = String($slug.value !== lastAuto);
    }
    // end validate Slug
    wireInputs() {
        const $summary = document.getElementById('Summary');
        $summary?.addEventListener('input', () => this.updateSummaryCounter());

        ['SeoMetadata_MetaTitle', 'SeoMetadata_MetaDescription'].forEach(id => {
            document.getElementById(id)?.addEventListener('input', () => this.updateSeoCounters());
        });

        // Khi Title blur, nếu SeoMetadata_MetaTitle đang trống -> gợi ý
        document.getElementById('Title')?.addEventListener('blur', () => {
            const el = document.getElementById('SeoMetadata_MetaTitle');
            if (el && !el.value?.trim()) el.value = document.getElementById('Title').value || '';
            this.updateSeoCounters();
        });
        $('#Title').on('input', function () {
            $(this).data('dirty', true);
        });
        // run first, after reload
        this.updateSummaryCounter();
        this.updateSeoCounters();
    }
    updateSummaryCounter() {
        const $summary = document.getElementById('Summary');
        const $counter = document.getElementById('summaryCounter');
        if ($summary && $counter) $counter.textContent = ($summary.value || '').length;
    }
    updateSeoCounters() {
        const t = (document.getElementById('SeoMetadata_MetaTitle')?.value || '').length;
        const d = (document.getElementById('SeoMetadata_MetaDescription')?.value || '').length;
        const tEl = document.getElementById('seoTitleCounter');
        const dEl = document.getElementById('seoDescCounter');
        if (tEl) tEl.textContent = t;
        if (dEl) dEl.textContent = d;
    }
    // ====== CRUD via API ======
    gatherPayload() {
        // gom dữ liệu từ form → payload API
        const tags = Array.from(document.querySelectorAll('#tagsContainer input[type="checkbox"]:checked')).map(x => x.value);
        const seoId = document.getElementById('SeoMetadataId')?.value || null;
        const metaT = document.getElementById('SeoMetadata_MetaTitle')?.value?.trim() || null;
        const metaD = document.getElementById('SeoMetadata_MetaDescription')?.value?.trim() || null;
        const metaK = document.getElementById('SeoMetadata_MetaKeywords')?.value?.trim() || null;
        return {
            id: this.id,
            title: document.getElementById('Title').value?.trim(),
            slug: document.getElementById('Slug').value?.trim(),
            status: document.getElementById('Status').value?.trim(),
            summary: document.getElementById('Summary').value?.trim(),
            content: document.getElementById('Content').value?.trim(),
            isCenterHighlight: document.getElementById('IsCenterHighlight').checked,
            isFeaturedHome: document.getElementById('IsFeaturedHome').checked,
            isHot: document.getElementById('IsHot').checked,
            displayOrder: parseInt(document.getElementById('DisplayOrder').value) || 0,
            categoryId: $('#CategoryId').val() || null,
            tagIds: tags, // hoặc Tags: [{id},…] tùy API
            // ===== SEO =====
            seoMetadataId: seoId,  // để server biết record hiện có
            seoMetadata: {
                id: seoId,                     // có thể null nếu tạo mới
                metaTitle: metaT,
                metaDescription: metaD,
                metaKeywords: metaK
            }
        };

    }
    async add() {
        try {
            // Gửi body rỗng: backend sẽ set AuthorId = current user, Status = Draft
            const created = await apiInstance.post(this.ENDPOINTS.CREATE, {});
            const newId = created?.id || created?.Id; // tuỳ casing server trả về

            if (!newId) {
                messageInstance.error('Không lấy được ID bài viết vừa tạo.');
                return;
            }

            // Chuyển sang trang Edit
            this.edit(newId);
        } catch (e) {
            console.error('Create post failed:', e);
            messageInstance.error(e?.message || 'Tạo bài viết thất bại.');
        }
    }
    async save() {
        try {
            await spinnerInstance.showForMainContainerAsync();
            const body = this.gatherPayload();
            // Nếu API của bạn phân biệt UPDATE vs PATCH, chọn cái bạn muốn:
            const res = await apiInstance.put?.(this.ENDPOINTS.UPDATE, body)
                ?? await apiInstance.post(this.ENDPOINTS.UPDATE, body);
            await spinnerInstance.hideForMainContainerAsync();
            messageInstance.success('Đã lưu bài viết.');
            // reload lại để đồng bộ timestamp/status nếu server thay đổi
            //await this.loadData();
        } catch (e) {
            console.error(e);
            messageInstance.error(e?.message || 'Lưu thất bại.');
        }
    }

    async changeStatus(nextStatus) {
        try {
            await spinnerInstance.showForMainContainerAsync();
            const res = await apiInstance.patch(this.ENDPOINTS.PATCH, { id: this.id, status: nextStatus });
            messageInstance.success('Trạng thái đã cập nhật.');
            await this.loadData();
        } catch (e) {
            console.error(e);
            messageInstance.error(e?.message || 'Đổi trạng thái thất bại.');
        } finally {
            await spinnerInstance.hideForMainContainerAsync();
        }
    }

    async delete() {
        try {
            if (!confirm('Xóa bài viết này?')) return;
            await spinnerInstance.showForMainContainerAsync();
            await apiInstance.delete?.(this.ENDPOINTS.DELETE(this.id))
                ?? await apiInstance.post(this.ENDPOINTS.DELETE(this.id)); // tùy API
            messageInstance.success('Đã xóa.');
            this.back();
        } catch (e) {
            console.error(e);
            messageInstance.error(e?.message || 'Xóa thất bại.');
        } finally {
            await spinnerInstance.hideForMainContainerAsync();
        }
    }

    // ====== Nav helpers ======
    back() { location.href = '/Articles?s=' + document.getElementById('Status').value; }
    backToDetail(id) {

    }
    view(idPost) {
        const id = idPost ?? this.id;
        //window.open(`/Articles/View/${encodeURIComponent(id)}?preview=1`, '_blank');
        window.location.href = `/Articles/View/${encodeURIComponent(id)}?preview=1`;
    }



}
export const postInstance = new PostModule(); 
